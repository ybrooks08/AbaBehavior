<template>
  <v-layout row wrap>
    <v-flex xs12 class="no-print">
      <v-card>
        <v-toolbar dark class="secondary" fluid dense>
          <v-toolbar-title>Adjust problems and replacements data collect for clients</v-toolbar-title>
        </v-toolbar>
        <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
        <v-card-text>
          <v-form ref="form" autocomplete="off" v-model="validForm">
            <v-layout row wrap>
              <!-- <v-flex xs12>
                <date-picker-menu :isLarge="true" :isDark="false" :btnColor="'primary'" :disabled="loading" v-model="datePickerModel" />
              </v-flex> -->
              <v-flex md12>
                <v-autocomplete box hid :disabled="loading" :items="clients" v-model="clientId" label="Client" prepend-icon="fa-user" item-text="clientName" item-value="clientId" :rules="[required]" required @change="clientSelect">
                  <template slot="item" slot-scope="{ item }">
                    <v-list-tile-avatar>
                      <img :style="!item.active ? 'opacity: 0.5': ''" :src="`images/${item.gender ? item.gender.toLowerCase() : 'nogender'}.png`">
                    </v-list-tile-avatar>
                    <v-list-tile-content>
                      <v-list-tile-title :class="{ 'grey--text text--lighten-1': !item.active }">{{item.clientName}}</v-list-tile-title>
                      <v-list-tile-sub-title :class="{ 'grey--text text--lighten-1': !item.active }">{{item.dob | moment('utc', 'MM/DD/YYYY')}} | Code: {{item.clientCode || 'N/A' }}</v-list-tile-sub-title>
                    </v-list-tile-content>
                  </template>
                </v-autocomplete>
              </v-flex>
              <template v-if="clientId">
                <v-flex xs12>
                  <v-layout row wrap>
                    <v-flex xs6>
                      <v-sheet class="pa-2">
                        <h3 class="subheading">Behaviors</h3>
                        <v-divider></v-divider>
                        <v-checkbox class="ma-0" v-for="i in clientBehaviors" :key="'p'+i.clientProblemId" v-model="problemsArray" :label="i.problemBehavior.problemBehaviorDescription" :value="i.problemId" hide-details multiple color="primary" :append-icon="!i.active ? 'fa-lg fa-exclamation-circle':''"></v-checkbox>
                      </v-sheet>
                    </v-flex>
                    <v-flex xs6>
                      <v-sheet class="pa-2">
                        <h3 class="subheading">Replacements</h3>
                        <v-divider></v-divider>
                        <v-checkbox class="ma-0" v-for="i in clientReplacements" :key="'r'+i.clientReplacementId" v-model="replacementsArray" :label="i.replacement.replacementProgramDescription" :value="i.replacementId" hide-details multiple color="primary" :append-icon="!i.active ? 'fa-lg fa-exclamation-circle':''"></v-checkbox>
                      </v-sheet>
                    </v-flex>
                  </v-layout>
                </v-flex>
                <v-flex xs12>
                  <v-radio-group v-model="action" label="What do you want to do?" class="mt-0 pb-3" hide-details :rules="[required]">
                    <v-radio label=" Add selected items to data collection" value="add" color="success">
                    </v-radio>
                    <v-radio label="Remove selected items from data collection" value="del" color="error"></v-radio>
                  </v-radio-group>
                </v-flex>
                <v-layout row fill-height align-center>
                  <v-flex>
                    <label class="v-label theme--light">For all session from:</label>
                    <v-text-field v-model="from" box label="start" prepend-inner-icon="fa-calendar-alt" return-masked-value mask="##/##/####" data-vv-name="from" :rules="errors.collect('from')" v-validate="'required|date_format:MM/dd/yyyy'" required data-vv-validate-on="blur"></v-text-field>
                  </v-flex>
                  <v-flex>
                    <label class="v-label theme--light">to:</label>
                    <v-text-field v-model="to" box label="end" prepend-inner-icon="fa-calendar-alt" return-masked-value mask="##/##/####" data-vv-name="to" :rules="errors.collect('to')" v-validate="'required|date_format:MM/dd/yyyy'" required data-vv-validate-on="blur"></v-text-field>
                  </v-flex>
                </v-layout>
              </template>
            </v-layout>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="loading || !validForm || !hasDataSelected || !hasDate" :loading="loading" color="primary" @click="adjust">Adjust</v-btn>
        </v-card-actions>
      </v-card>
    </v-flex>
  </v-layout>
</template>

<script>
import userApi from "@/services/api/UserServices";
import clientApi from "@/services/api/ClientServices";
import sessionServicesApi from "@/services/api/SessionServices";

export default {
  data() {
    return {
      loading: false,
      required: value => !!value || "This field is required.",
      validForm: false,
      datePickerModel: {
        start: this.$moment()
          .subtract(1, "month")
          .startOf("month")
          .format("YYYY-MM-DDTHH:mm"),
        end: this.$moment()
          .subtract(1, "month")
          .endOf("month")
          .format("YYYY-MM-DDTHH:mm")
      },
      clients: [],
      clientId: null,
      clientBehaviors: [],
      clientReplacements: [],
      problemsArray: [],
      replacementsArray: [],
      action: null,
      from: null,
      to: null
    };
  },

  computed: {
    user() {
      return this.$store.getters.user;
    },
    hasDataSelected() {
      return this.problemsArray.length > 0 || this.replacementsArray.length > 0;
    },
    hasDate() {
      return this.from && this.to;
    }
  },

  mounted() {
    this.$store.commit("SET_ACTIVE_CLIENT", null);
    this.loadUserClients();
  },

  methods: {
    dateSelected(range) {
      this.serviceLog.from = range.from;
      this.serviceLog.to = range.to;
    },

    async loadUserClients() {
      this.clients = [];
      this.loading = true;
      try {
        this.clients = await userApi.loadUserClients();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async clientSelect(c) {
      this.loading = true;
      try {
        this.problemsArray = [];
        this.action = null;
        this.clientBehaviors = await sessionServicesApi.getClientBehaviors(c, false);
        this.clientReplacements = await sessionServicesApi.getClientReplacements(c, false);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async adjust() {
      this.$validator.validateAll().then(res => {
        if (!res) {
          return;
        }
      });

      this.$confirm("Are you sure you want to adjust sessions?").then(async res => {
        if (res) {
          try {
            this.loading = true;
            const adjustClientDataCollectModel = {
              clientId: this.clientId,
              problems: this.problemsArray,
              replacements: this.replacementsArray,
              action: this.action,
              from: this.from,
              to: this.to
            };
            const count = await clientApi.adjustClientDataCollect(adjustClientDataCollectModel);
            this.$toast.info(count + " sessions affected.");
            this.problemsArray = [];
            this.replacementsArray = [];
            this.action = null;
            this.from = null;
            this.to = null;
          } catch (error) {
            this.$toast.error(error.message || error);
          } finally {
            this.loading = false;
          }
        }
      });
    }
  }
};
</script>