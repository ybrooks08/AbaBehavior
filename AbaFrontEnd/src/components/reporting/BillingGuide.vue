<template>
  <v-layout row wrap>
    <v-flex xs12 class="no-print">
      <v-card width="500">
        <v-toolbar dense dark class="secondary">
          <v-toolbar-title>Billing Guide parameters</v-toolbar-title>
        </v-toolbar>
        <v-card-text>
          <v-form autocomplete="off" v-model="validForm">
            <v-layout row wrap>
              <v-flex xs12>
                <date-picker-menu v-model="datePickerModel" :isLarge="true" :isDark="false" :btnColor="'primary'" :disabled="loading"/>
                <!-- <date-picker-menu :isLarge=true :isDark=false :btnColor="'primary'" :pickerStartInit="from" :pickerEndInit="to" :initialValue="'Last month'" @dateSelected="dateSelected" :disabled="loading" /> -->
              </v-flex>
              <v-flex xs12>
                <v-autocomplete box hide-details :disabled="loading" :items="clients" item-value="clientId" item-text="fullname" v-model="selectedClientId" label="Client" prepend-icon="fa-user" :rules="[required]" required>
                  <template slot="item" slot-scope="{ item }">
                    <v-list-tile-avatar>
                      <img :style="!item.active ? 'opacity: 0.5': ''" :src="`images/${item.gender ? item.gender.toLowerCase() : 'nogender'}.png`">
                    </v-list-tile-avatar>
                    <v-list-tile-content>
                      <v-list-tile-title :class="{ 'grey--text text--lighten-1': !item.active }">{{item.firstname}} {{item.lastname}}</v-list-tile-title>
                      <v-list-tile-sub-title :class="{ 'grey--text text--lighten-1': !item.active }">{{item.dob | moment("utc", "MM/DD/YYYY")}} | Code: {{item.code || "N/A" }}</v-list-tile-sub-title>
                    </v-list-tile-content>
                  </template>
                </v-autocomplete>
              </v-flex>
              <v-flex md12>
                <v-select box hide-details :disabled="loading" :items="behaviorAnalysisCodes" v-model="behaviorAnalysisCodeId" label="Service" prepend-icon="fa-briefcase-medical" item-text="description" item-value="behaviorAnalysisCodeId" :rules="[required]" required>
                  <template slot="selection" slot-scope="data">
                    <div class="input-group__selections__comma">
                      {{ data.item.description }} &nbsp; <span class="grey--text text--darken-1">({{data.item.hcpcs}})</span>
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
            </v-layout>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <small class="pl-4 grey--text">* Only reviewed and billed sessions will be reported</small>
          <v-spacer/>
          <!-- <v-btn :loading="loading" :disabled="loading || !validForm">Clear</v-btn> -->
          <v-btn color="primary" :loading="loading" :disabled="loading || !validForm" @click="View">View</v-btn>
        </v-card-actions>
      </v-card>
    </v-flex>
    <v-flex v-if="report !== null" xs12>
      <v-card>
        <v-toolbar dense dark class="secondary no-print">
          <v-toolbar-title>Billing guide overview</v-toolbar-title>
          <v-spacer/>
          <v-btn dark icon @click="print">
            <v-icon>fa-print</v-icon>
          </v-btn>
          <v-btn dark icon @click="report = null">
            <v-icon>fa-times-circle</v-icon>
          </v-btn>
        </v-toolbar>
        <v-card-text class="print-full-width">
          <v-subheader>Client</v-subheader>
          <v-layout row>
            <v-flex xs6>
              <v-layout row wrap>
                <v-flex class="body-2 text-xs-right" xs4>Recipient ID:</v-flex>
                <v-flex xs8>{{report.client.memberId || "N/A"}}</v-flex>
                <v-flex class="body-2 text-xs-right" xs4>Last Name:</v-flex>
                <v-flex xs8>{{report.client.lastname || "N/A"}}</v-flex>
                <v-flex class="body-2 text-xs-right" xs4>First Name:</v-flex>
                <v-flex xs8>{{report.client.firstname || "N/A"}}</v-flex>
              </v-layout>
            </v-flex>
            <v-flex xs6>
              <v-layout row wrap>
                <v-flex class="body-2 text-xs-right" xs4>Dob:</v-flex>
                <v-flex xs8>{{report.client.dob || new Date() | moment("utc", "MM/DD/YYYY")}}</v-flex>
                <v-flex class="body-2 text-xs-right" xs4>Patient account:</v-flex>
                <v-flex xs8>{{report.client.code}}</v-flex>
              </v-layout>
            </v-flex>
          </v-layout>
          <v-subheader>Diagnosis</v-subheader>
          <table class="v-datatable v-table theme--light print-font-small">
            <thead>
              <tr style="height: auto;">
                <th class="text-xs-left py-0">Code</th>
                <th class="text-xs-left py-0">Description</th>
              </tr>
            </thead>
            <tbody>
              <tr style="height: auto;" v-for="r in report.client.diagnosis" :key="('diag'+r.code)">
                <td style="height: auto;">{{r.code}}</td>
                <td style="height: auto;">{{r.description}}</td>
              </tr>
            </tbody>
          </table>
          <v-subheader>Active Referrals</v-subheader>
          <table class="v-datatable v-table theme--light print-font-small">
            <thead>
              <tr style="height: auto;">
                <th class="text-xs-left py-0">Fullname / Specialty</th>
                <th class="text-xs-left py-0">License / Provider</th>
                <th class="text-xs-left py-0">NPI</th>
                <th class="text-xs-left py-0">Full Address</th>
                <th class="text-xs-left py-0">Date Referral / Expires</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="r in report.client.referrals" :key="('referral'+r.referralId)">
                <td>
                  <strong>{{r.referralFullname}}</strong>
                  <br>
                  {{r.specialty}}
                </td>
                <td>
                  <strong>{{r.license}}</strong>
                  <br>
                  {{r.provider}}
                </td>
                <td>
                  <strong>{{r.npi}}</strong>
                </td>
                <td>
                  <strong>{{r.fullAddress}}</strong>
                </td>
                <td>
                  <v-icon small>fa-check-circle</v-icon>
                  {{r.dateReferral | moment("utc", "MM/DD/YYYY")}}
                  <br>
                  <v-icon small>fa-times-circle</v-icon>
                  {{r.dateExpires | moment("utc", "MM/DD/YYYY")}}
                </td>
              </tr>
            </tbody>
          </table>
          <v-subheader>Active Authorizations</v-subheader>
          <table class="v-datatable v-table theme--light print-font-small">
            <thead>
              <tr style="height: auto;">
                <th class="text-xs-left py-0">Auth</th>
                <th></th>
                <th class="text-xs-left py-0">PA Number</th>
                <th class="text-xs-left py-0">Total Units / Hours</th>
                <th class="text-xs-left py-0">Start / End date</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="r in report.client.assessments" :key="('ass'+r.assessmentId)">
                <td>
                  <strong>{{r.behaviorAnalysisCode.hcpcs}}</strong>
                </td>
                <td>
                  <template v-if="r.behaviorAnalysisCode.hcpcs == 'H0031' || r.behaviorAnalysisCode.hcpcs == 'H0032'">
                    <v-btn small v-if="r.status === 0" color="primary" @click="markAssessmentAsBilled(r)">Mark as billed</v-btn>
                    <v-chip v-else label disabled color="green" text-color="white">Billed</v-chip>
                  </template>
                </td>
                <td>
                  <strong>{{r.paNumber}}</strong>
                </td>
                <td>
                  <strong>
                    <v-icon small>fa-star</v-icon>
                    {{r.totalUnits.toLocaleString()}}
                  </strong>
                  <br>
                  <v-icon small>fa-clock</v-icon>
                  {{(r.totalUnits / 4).toLocaleString()}}
                </td>
                <td>
                  <v-icon small>fa-check-circle</v-icon>
                  {{r.startDate | moment("utc", "MM/DD/YYYY")}}
                  <br>
                  <v-icon small>fa-times-circle</v-icon>
                  {{r.endDate | moment("utc", "MM/DD/YYYY")}}
                </td>
              </tr>
            </tbody>
          </table>
          <v-subheader>Active Users</v-subheader>
          <table class="v-datatable v-table theme--light print-font-small">
            <thead>
              <tr style="height: auto;">
                <th class="text-xs-left py-0">Fullname / Lic No.</th>
                <th class="text-xs-left py-0">Provider Id</th>
                <th class="text-xs-left py-0">NPI</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="r in report.client.assignments" :key="('user'+r.userId)">
                <td>
                  <strong>{{r.firstname}} {{r.lastname}}</strong>
                  <br>
                  {{r.licenseNo}}
                </td>
                <td>
                  <strong>{{r.mpi}}</strong>
                </td>
                <td>
                  <strong>{{r.npi}}</strong>
                </td>
              </tr>
            </tbody>
          </table>
          <v-subheader>Sessions</v-subheader>
          <table class="v-datatable v-table theme--light print-font-small condensed">
            <thead>
              <tr style="height: auto;">
                <th class="text-xs-left py-0">Billing Status</th>
                <th class="text-xs-left py-0">Date</th>
                <th class="text-xs-left py-0">Service User</th>
                <th class="text-xs-left py-0">Service</th>
                <th class="text-xs-left py-0">Start / End time</th>
                <th class="text-xs-center py-0">Place</th>
                <th class="text-xs-right py-0">Units</th>
                <th class="text-xs-right py-0">Hours</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(r, i) in sessions" :key="('session'+r.sessionId)">
                <td>
                  <v-btn small v-if="r.sessionStatus !== 'Billed'" :loading="btnLoading[i]" color="primary" @click="markBilled(r, i)">Mark as billed</v-btn>
                  <v-chip v-else label disabled color="green" text-color="white">Billed</v-chip>
                </td>
                <td>
                  <strong>{{r.sessionStart | moment("MM/DD/YYYY")}}</strong>
                </td>
                <td>
                  <strong>{{r.userFullname}}</strong>
                </td>
                <td>
                  <strong>{{r.sessionType}}</strong>
                </td>
                <td>
                  <v-icon small>fa-check-circle</v-icon>
                  {{r.sessionStart | moment("LT")}}
                  <br>
                  <v-icon small>fa-times-circle</v-icon>
                  {{r.sessionEnd | moment("LT")}}
                </td>
                <td>
                  <strong>{{r.pos}}</strong>
                </td>
                <td class="text-xs-right">
                  <strong>
                    <v-icon small>fa-star</v-icon>
                    {{r.totalUnits.toLocaleString()}}
                  </strong>
                </td>
                <td class="text-xs-right">
                  <strong>
                    <v-icon small>fa-clock</v-icon>
                    {{(r.totalUnits / 4).toLocaleString()}}
                  </strong>
                </td>
              </tr>
            </tbody>
            <tfoot>
              <tr class="grey lighten-2">
                <td colspan="6">Total</td>
                <td class="text-xs-right">
                  <strong>
                    <v-icon small>fa-star</v-icon>
                    {{totalUnits.toLocaleString()}}
                  </strong>
                </td>
                <td class="text-xs-right">
                  <strong>
                    <v-icon small>fa-clock</v-icon>
                    {{(totalUnits / 4).toLocaleString()}}
                  </strong>
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
import clientApi from "@/services/api/ClientServices";
import masterTableApi from "@/services/api/MasterTablesServices";
import reportingApi from "@/services/api/ReportingServices";
import sessionServicesApi from "@/services/api/SessionServices";

export default {
  data() {
    return {
      loading: false,
      validForm: false,
      report: null,
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
      required: value => !!value || "This field is required.",
      clients: [],
      selectedClientId: null,
      behaviorAnalysisCodes: [],
      behaviorAnalysisCodeId: null,
      sessions: [],
      btnLoading: []
    };
  },

  computed: {
    totalUnits() {
      const totalUnits = this.sessions.length == 0 ? 0 : this.sessions.map(a => a.totalUnits).reduce((a, b) => a + b);
      return totalUnits;
    }
  },

  async mounted() {
    this.clients = [];
    this.loading = true;
    try {
      this.clients = await clientApi.getClients();
      this.clients.forEach(s => {
        s.fullname = `${s.firstname} ${s.lastname}`;
      });
      this.behaviorAnalysisCodes = await masterTableApi.getBehaviorAnalysisCodes(true);
    } catch (error) {
      this.$toast.error(error);
    } finally {
      this.loading = false;
    }
  },

  methods: {
    async View() {
      try {
        this.sessions = [];
        this.loading = true;
        this.report = null;
        this.report = await reportingApi.getBillingGuide(this.datePickerModel.start, this.datePickerModel.end, this.selectedClientId, this.behaviorAnalysisCodeId);
        this.report.sessions.forEach(e => {
          e.sessionStart = this.$moment(e.sessionStart).local();
          e.sessionEnd = this.$moment(e.sessionEnd).local();
          this.sessions.push(e);
        });
        console.log(this.report.client.assessments);
      } catch (error) {
        this.$toast.error(error.response.data || error.message);
      } finally {
        this.loading = false;
      }
    },

    async markBilled(session, index) {
      const model = {
        sessionId: session.sessionId,
        sessionStatus: 6 //billed
      };
      this.$set(this.btnLoading, index, true);
      try {
        await sessionServicesApi.changeSessionStatus(model);
        session.sessionStatus = "Billed";
      } catch (error) {
        console.error(error);
        this.$toast.error(error);
      } finally {
        this.$set(this.btnLoading, index, false);
      }
    },

    async markAssessmentAsBilled(assessment) {
      try {
        await sessionServicesApi.markAssessmentAsBilled(assessment.assessmentId);
        assessment.status = 1;
      } catch (error) {
        console.error(error);
        this.$toast.error(error);
      }
    },

    print() {
      window.print();
    }
  }
};
</script>
