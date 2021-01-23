<template>
  <v-layout row wrap>
    <v-flex xs12 class="no-print">
      <v-card>
        <v-toolbar dark class="secondary" fluid dense>
          <v-toolbar-title>Tellus Information</v-toolbar-title>
        </v-toolbar>
        <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
        <v-card-text class="pa-1">
          <form-wizard>
            <tab-content title="Date Selecting" :selected="true">
                <v-form ref="form" autocomplete="off" v-model="validForm">
            <v-layout row wrap>
              <v-flex xs12>
                <date-picker-menu :isLarge="true" :isDark="false" :btnColor="'primary'" :disabled="loading" v-model="datePickerModel" />
              </v-flex>
              <v-flex md12>                
              </v-flex>
            </v-layout>
          </v-form>
            </tab-content>
            <tab-content title="About your Company"> 
                <p>Can contains</p>
                <p>Multiple Elements</p>
            </tab-content>
            <tab-content title="Finishing Up">
                <p>Or an image .. or any thing</p>                  
            </tab-content>  
          </form-wizard>
          
        </v-card-text>
        <v-card-actions>
          <small class="pl-4 grey--text">* Tellus credentials used are defined in settings section.</small>
          <v-spacer />
          <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="getTellusStepOne">Generate</v-btn>
        </v-card-actions>
      </v-card>
    </v-flex>
    <v-flex xs12 v-if="sessions.length > 0">
      <v-card>
        <v-toolbar dense dark class="secondary no-print">
          <v-toolbar-title>Sessions by User</v-toolbar-title>
          <v-spacer />
          <v-btn dark icon @click="print">
            <v-icon>fa-print</v-icon>
          </v-btn>
        </v-toolbar>
        <v-card-text class="pa-0 print-full-width">
          <table v-if="sessions.length > 0" class="v-datatable v-table theme--light print-font-small">
            <thead>
              <tr>
                <th class="text-xs-left py-0 px-1">Client/Code</th>
                <th class="text-xs-left py-0 px-1">Date</th>
                <th class="text-xs-left py-0 px-1">Start / End</th>
                <th class="text-xs-left py-0 px-1">Type</th>
                <th class="text-xs-left py-0 px-1">Pos</th>
                <th class="text-xs-left py-0 px-1">Units</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="r in sessions" :key="('session'+r.sessionId)">
                <td class="px-1 text-truncate">
                  <strong>{{ r.clientFullname }}</strong>
                  <br />
                  {{ r.code }}
                </td>
                <td class="px-1">{{ r.sessionStart | moment("MM/DD/YYYY") }}</td>
                <td class="hidden-sm-and-down px-1 text-truncate">
                  <v-icon color="green" small>fa-sign-in-alt</v-icon>
                  {{ r.sessionStart | moment("LT") }}
                  <br />
                  <v-icon color="red" small>fa-sign-out-alt</v-icon>
                  {{ r.sessionEnd | moment("LT") }}
                </td>
                <td class="hidden-sm-and-down px-1">{{ r.sessionType }}</td>
                <td class="hidden-sm-and-down px-1">{{ r.pos }}</td>
                <td class="px-1">
                  <strong>
                    <v-icon small>fa-star</v-icon>
                    {{ r.totalUnits.toLocaleString() }}
                  </strong>
                  <br />
                  <v-icon small>fa-clock</v-icon>
                  {{ (r.totalUnits / 4).toLocaleString() }}
                </td>
              </tr>
            </tbody>
            <tfoot>
              <tr class="grey lighten-2">
                <td colspan="5">Total</td>
                <td class="px-1">
                  <strong>
                    <v-icon small>fa-star</v-icon>
                    {{ totalUnits.toLocaleString() }}
                  </strong>
                  <br />
                  <v-icon small>fa-clock</v-icon>
                  {{ (totalUnits / 4).toLocaleString() }}
                </td>
              </tr>
            </tfoot>
          </table>
        </v-card-text>
      </v-card>
    </v-flex>
  </v-layout>
</template>

<script>
import userApi from "@/services/api/UserServices";
// import clientApi from '@/services/api/ClientServices';
import tellusApi from "@/services/api/TellusServices";
import {FormWizard, TabContent} from 'vue-step-wizard'
import 'vue-step-wizard/dist/vue-step-wizard.css'

export default {
  data() {
    return {
      loading: false,
      required: (value) => !!value || "This field is required.",
      validForm: false,
      datePickerModel: {
        start: this.$moment().subtract(1, "month").startOf("month").format("YYYY-MM-DDTHH:mm"),
        end: this.$moment().subtract(1, "month").endOf("month").format("YYYY-MM-DDTHH:mm")
      },
      users: [],
      sessions: [],
      /*user: null,
      pass: null*/
    };
  },

  components: {
  FormWizard,
  TabContent
  },

  computed: {
    totalUnits() {
      return this.sessions.map((a) => a.totalUnits).reduce((a, b) => a + b);
    }
  },

  mounted() {
    this.loadUsers();
  },

  methods: {
    dateSelected(range) {
      this.serviceLog.from = range.from;
      this.serviceLog.to = range.to;
    },

    async loadUsers() {
      this.users = [];
      this.loading = true;
      try {
        this.users = await userApi.getUsersCanCreateSessions();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async getTellusStepOne() {
      try {
        this.loading = true;
        this.sessions = [];
        let sessions = await tellusApi.GetTellusStepOne(this.datePickerModel.start, this.datePickerModel.end/*, this.user, this.pass*/);

        if (sessions.length == 0) {
          this.$toast.info("No data");
          return;
        }

        sessions.forEach((e) => {
          e.sessionStart = this.$moment(e.sessionStart).local();
          e.sessionEnd = this.$moment(e.sessionEnd).local();
          this.sessions.push(e);
        });
      } catch (error) {
        //this.$toast.error(error.response.data || error.message);
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    print() {
      window.print();
    }
  }
};
</script>
