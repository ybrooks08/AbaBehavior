<template>
  <v-dialog width="600" persistent transition="slide-y-transition" v-model="model">
    <v-card>
      <v-card-title class="teal white--text">
        <div class="title">{{ data.assessmentId == 0 ? "Add new authorization" : "Edit authorization" }}</div>
      </v-card-title>
      <v-container grid-list-md pa-1>
        <v-layout row wrap>
          <v-flex xs12>
            <v-card-text>
              <v-form ref="form" autocomplete="off" v-model="validForm">
                <v-layout row wrap>
                  <v-flex md12>
                    <v-select
                      box
                      :disabled="loading"
                      :items="behaviorAnalysisCodes"
                      v-model="data.behaviorAnalysisCodeId"
                      label="Assessment"
                      prepend-icon="fa-briefcase-medical"
                      item-text="description"
                      item-value="behaviorAnalysisCodeId"
                      :rules="[required]"
                      required
                    >
                      <template slot="selection" slot-scope="data">
                        <div class="input-group__selections__comma">
                          {{ data.item.description }} &nbsp;
                          <span class="grey--text text--darken-1">({{ data.item.hcpcs }})</span>
                        </div>
                      </template>
                      <template slot="item" slot-scope="data">
                        <template>
                          <v-list-tile-avatar>
                            <v-icon>fa-briefcase-medical</v-icon>
                          </v-list-tile-avatar>
                          <v-list-tile-content>
                            <v-list-tile-title v-html="data.item.description"></v-list-tile-title>
                            <v-list-tile-sub-title v-html="data.item.hcpcs"></v-list-tile-sub-title>
                          </v-list-tile-content>
                        </template>
                      </template>
                    </v-select>
                  </v-flex>
                  <v-layout row wrap>
                    <v-flex xs6 md4>
                      <v-text-field box :disabled="loading" label="Total units" v-model="data.totalUnits" prepend-icon="fa-star" mask="#####" :rules="[required]" required></v-text-field>
                    </v-flex>
                    <!-- <v-flex xs4 md3 class="text-xs-center pt-2">
                      <v-chip color="indigo" text-color="white">
                        <v-avatar>
                          <v-icon>fa-clock</v-icon>
                        </v-avatar>
                        {{ minutes }} mins
                      </v-chip>
                    </v-flex> -->
                    <v-flex xs6 md4>
                      <v-text-field box :disabled="loading" label="Total by Week" v-model="data.totalUnitsWeek" prepend-icon="fa-calendar-week" mask="#####" clearable></v-text-field>
                    </v-flex>
                    <v-flex xs12 md4>
                      <v-text-field box :disabled="loading" label="PA Number" v-model="data.paNumber" prepend-icon="fa-key" :rules="[required]" require></v-text-field>
                    </v-flex>
                  </v-layout>
                  <v-layout row wrap>
                    <v-flex sm12 md6>
                      <v-text-field
                        box
                        :disabled="loading"
                        label="Start date"
                        v-model="data.startDate"
                        prepend-icon="fa-calendar-plus"
                        required
                        return-masked-value
                        mask="##/##/####"
                        data-vv-name="startdate"
                        :rules="errors.collect('startdate')"
                        v-validate="'required|date_format:MM/dd/yyyy'"
                      ></v-text-field>
                    </v-flex>
                    <v-flex sm12 md6>
                      <v-text-field
                        box
                        :disabled="loading"
                        label="End date"
                        v-model="data.endDate"
                        prepend-icon="fa-calendar-minus"
                        required
                        return-masked-value
                        mask="##/##/####"
                        data-vv-name="enddate"
                        :rules="errors.collect('enddate')"
                        v-validate="'required|date_format:MM/dd/yyyy'"
                      ></v-text-field>
                    </v-flex>
                  </v-layout>
                </v-layout>
              </v-form>
            </v-card-text>
          </v-flex>
        </v-layout>

        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="loading" flat @click="cancel">Cancel</v-btn>
          <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="saveChanges">{{ data.assessmentId == 0 ? "Add authorization" : "Edit authorization" }}</v-btn>
        </v-card-actions>
      </v-container>
    </v-card>
  </v-dialog>
</template>

<script>
import masterTableApi from "@/services/api/MasterTablesServices";
import clientApi from "@/services/api/ClientServices";

export default {
  props: {
    model: {
      type: Boolean,
      required: true,
      default: false
    },
    clientId: null
  },

  data() {
    return {
      loading: false,
      required: value => !!value || "This field is required.",
      validForm: false,
      behaviorAnalysisCodes: [],
      data: {
        clientId: this.clientId,
        assessmentId: 0,
        behaviorAnalysisCodeId: null,
        totalUnits: 0,
        paNumber: null,
        startDate: null,
        endDate: null,
        totalUnitsWeek: null
      }
    };
  },

  computed: {
    minutes() {
      return this.data.totalUnits ? (this.data.totalUnits * 15).toLocaleString() : "N/A";
    }
  },

  async mounted() {
    this.behaviorAnalysisCodes = await masterTableApi.getBehaviorAnalysisCodes();
  },

  methods: {
    async saveChanges() {
      this.loading = true;
      try {
        await clientApi.addAssessment(this.data);
        this.$emit("onSubmit");
        this.$refs.form.reset();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    cancel() {
      this.$emit("cancel");
      this.$refs.form.reset();
    }
  }
};
</script>

<style scoped></style>
