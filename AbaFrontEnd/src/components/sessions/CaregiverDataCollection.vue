<template>
  <v-container grid-list-xs pa-0>
    <v-layout row>
      <v-flex>
        <v-card class="elevation-12">
          <v-toolbar dense dark color="secondary">
            <v-toolbar-title>Caregiver data collection</v-toolbar-title>
          </v-toolbar>
          <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
          <v-card-text>
            <v-form ref="form" v-model="validForm">
              <v-layout row wrap>
                <v-flex xs12 md6>
                  <v-text-field ref="formDateSelect" box required label="Collection date" v-model="dateSelected" return-masked-value prepend-icon="fa-calendar" mask="##/##/####" data-vv-name="dateSelected" :rules="errors.collect('dateSelected')"
                                v-validate="'required|date_format:MM/dd/yyyy'" @change="dateChanged" @blur="dateBlur"/>
                </v-flex>
                <v-flex xs12 md6>
                  <v-select box :disabled="loading" label="Caregiver" v-model="mainData.caregiverId" :items="caregivers" :rules="[required]"></v-select>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex xs12>
                  <v-subheader>Behaviors</v-subheader>
                  <table class="v-datatable v-table theme--light">
                    <tbody>
                      <tr v-for="(p, index) in mainData.caregiverDataCollectionProblems" :key="'beh'+index">
                        <td class="text-xs-left" style="width: 50%">{{getProblemName(p.problemId)}}</td>
                        <td class="text-xs-right px-1">
                          <v-text-field append-icon="fa-flag fa-sm" prepend-inner-icon="fa-frown" box hide-actions hide-details label="Count" v-model="p.count" clearable :data-vv-name="'count'+index" :rules="errors.collect('count'+index)"
                                        v-validate="'numeric'"></v-text-field>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex xs12>
                  <v-subheader>Replacements</v-subheader>
                  <table class="v-datatable v-table theme--light">
                    <tbody>
                      <tr v-for="(p, index) in mainData.caregiverDataCollectionReplacements" :key="'beh'+index">
                        <td class="text-xs-left" style="width: 50%">{{getReplacementName(p.replacementId)}}</td>
                        <td class="text-xs-right px-1">
                          <v-text-field prepend-inner-icon="fa-sign-language" box hide-actions hide-details label="Trials" v-model="p.totalTrial" clearable :data-vv-name="'repTotal'+index" :rules="errors.collect('repTotal'+index)"
                                        v-validate="'numeric'"></v-text-field>
                        </td>
                        <td class="text-xs-right px-1">
                          <v-text-field prepend-inner-icon="fa-thumbs-up" box hide-actions hide-details label="Completed" v-model="p.totalCompleted" clearable :data-vv-name="'repComp'+index" :rules="errors.collect('repComp'+index)"
                                        v-validate="'numeric|max_value:'+p.totalTrial"></v-text-field>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </v-flex>
              </v-layout>
            </v-form>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn :disabled="loading" flat @click="close">Cancel</v-btn>
            <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="submit">Save</v-btn>
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import clientApi from "@/services/api/ClientServices";
import sessionServicesApi from "@/services/api/SessionServices";

export default {
  data() {
    return {
      loading: false,
      validForm: false,
      required: value => !!value || "This field is required.",
      dateSelected: null,
      caregivers: [],
      clientBehaviors: [],
      clientReplacements: [],
      mainData: {
        caregiverId: null,
        caregiverDataCollectionProblems: []
      }
    };
  },

  computed: {
    activeClientId() {
      return this.$store.getters.activeClientId;
    },
    activeDate() {
      return this.$store.getters.activeDate;
    }
  },

  mounted() {
    this.dateSelected = this.$moment(this.activeDate).format("MM/DD/YYYY");
    this.loadData();
  },

  methods: {
    async loadData() {
      try {
        if (!this.dateSelected) {
          this.$toast.warning("You must select a valid date before continue.");
          this.$refs.formDateSelect.focus();
          return;
        }
        this.loading = true;
        this.caregivers = await clientApi.getClientCaregivers(this.activeClientId);
        const dateStr = this.$moment(this.dateSelected, "MM/DD/YYYY").format("YYYY-MM-DD");
        this.clientBehaviors = await sessionServicesApi.getClientBehaviors(this.activeClientId, false);
        this.clientReplacements = await sessionServicesApi.getClientReplacements(this.activeClientId, false);
        this.mainData = await sessionServicesApi.getCaregiverCollectionData(dateStr, this.activeClientId);
      } catch (error) {
        console.error(error);
        this.$toast.error(error.message || error);
      } finally {
        this.loading = false;
      }
    },

    dateChanged() {
      this.loadData();
    },

    dateBlur() {
      if (!this.dateSelected) {
        this.$toast.warning("You must select a valid date before continue.");
        this.$refs.formDateSelect.focus();
        return;
      }
    },

    getProblemName(problemId) {
      return this.clientBehaviors.find(s => s.problemId === problemId).problemBehavior.problemBehaviorDescription;
    },

    getReplacementName(replacementId) {
      return this.clientReplacements.find(s => s.replacementId === replacementId).replacement.replacementProgramDescription;
    },

    close() {
      this.$router.push("/clients/sessions_details");
    },

    async submit() {
      try {
        this.loading = true;
        if (!this.mainData.caregiverId) throw Error("You must select a caregiver.");
        await sessionServicesApi.saveCaregiverCollectionData(this.mainData);
        this.close();
      } catch (error) {
        console.error(error);
        this.$toast.error(error.message || error);
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>
