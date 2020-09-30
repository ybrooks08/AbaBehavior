<template>
  <v-layout row wrap>
    <v-flex xs12 class="no-print">
      <v-card>
        <v-toolbar dark class="secondary" fluid dense>
          <v-toolbar-title>Service Log</v-toolbar-title>
        </v-toolbar>
        <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
        <v-card-text class="pa-1">
          <v-form ref="form" autocomplete="off" v-model="validForm">
            <v-layout row wrap>
              <v-flex xs12>
                <date-picker-menu :isLarge="true" :isDark="false" :btnColor="'primary'" :disabled="loading" v-model="datePickerModel" />
              </v-flex>
              <v-flex xs12 sm6 v-if="isAdminOrBillingOrManagement">
                <select-user v-model="userId" @change="changedUser()"></select-user>
              </v-flex>
              <v-flex xs12 sm6>
                <v-select
                  box
                  hid
                  :disabled="loading"
                  :items="clients"
                  v-model="clientId"
                  label="Client"
                  prepend-icon="fa-user"
                  item-text="clientName"
                  item-value="clientId"
                  :rules="[required]"
                  required
                >
                  <template slot="item" slot-scope="{ item }">
                    <v-list-tile-avatar>
                      <img :style="!item.active ? 'opacity: 0.5' : ''" :src="`images/${item.gender ? item.gender.toLowerCase() : 'nogender'}.png`" />
                    </v-list-tile-avatar>
                    <v-list-tile-content>
                      <v-list-tile-title :class="{ 'grey--text text--lighten-1': !item.active }">{{ item.clientName }}</v-list-tile-title>
                      <v-list-tile-sub-title :class="{ 'grey--text text--lighten-1': !item.active }"
                        >{{ item.dob | moment("utc", "MM/DD/YYYY") }} | Code: {{ item.clientCode || "N/A" }}
                      </v-list-tile-sub-title>
                    </v-list-tile-content>
                  </template>
                </v-select>
              </v-flex>
            </v-layout>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <small class="pl-4 grey--text">* Only checked, reviewed and billed sessions will be reported</small>
          <v-spacer />
          <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="viewReport">Generate</v-btn>
        </v-card-actions>
      </v-card>
    </v-flex>
    <v-flex xs12 v-if="report">
      <v-card>
        <v-toolbar dense dark class="secondary no-print">
          <v-toolbar-title>Service Log</v-toolbar-title>
          <v-spacer />
          <v-btn dark icon @click="print">
            <v-icon>fa-print</v-icon>
          </v-btn>
          <v-btn dark icon @click="report = null">
            <v-icon>fa-times-circle</v-icon>
          </v-btn>
        </v-toolbar>
        <v-card-text class="print-full-width">
          <div>
            <v-layout row wrap>
              <v-flex xs2>
                <img style="object-fit: contain;" src="images/logo.jpg" width="100" />
              </v-flex>
              <v-flex xs10>
                <h4>SERVICE LOG</h4>
                <v-divider></v-divider>
                <span>
                  FROM:
                  <strong>{{ datePickerModel.start | moment("MM/DD/YYYY") }}</strong> TO:
                  <strong>{{ datePickerModel.end | moment("MM/DD/YYYY") }}</strong>
                </span>
              </v-flex>
            </v-layout>

            <table class="table-print">
              <tr>
                <td>
                  <small>Provider Name:</small>
                  <br />
                  <span>{{ report.user.firstname }} {{ report.user.lastname }}</span>
                </td>
                <td>
                  <small>Recipient's Name:</small>
                  <br />
                  <span>{{ report.client.firstname }} {{ report.client.lastname }}</span>
                </td>
                <td>
                  <small>Physician(s) Name:</small>
                  <br />
                  <template v-for="r in report.client.referrals">
                    <span :key="r.npi">{{ r.referralFullname }} | NPI: {{ r.npi }}</span>
                    <br :key="'break' + r.npi" />
                  </template>
                </td>
              </tr>
              <tr>
                <td>
                  <small>Provider Number:</small>
                  <br />
                  <span>{{ report.user.mpi }}</span>
                </td>
                <td>
                  <small>Code:</small>
                  <br />
                  <span>{{ report.client.code }}</span>
                </td>
                <td rowspan="2">
                  <small>PA Number(s):</small>
                  <br />
                  <span>
                    <label v-for="a in report.client.assessments" :key="a.paNumber">
                      <div v-if="report.client.assessments.length > 1">|</div>
                      {{ a.paNumber }}
                    </label>
                  </span>
                </td>
              </tr>
              <tr>
                <td>
                  <small>License Number & NPI:</small>
                  <br />
                  <span>{{ report.user.licenseNo }}</span
                  >&nbsp;|&nbsp;
                  <span>NPI: {{ report.user.npi }}</span>
                </td>
                <td>
                  <small>Medicaid:</small>
                  <br />
                  <span>{{ report.client.memberId }}</span>
                </td>
              </tr>
            </table>
          </div>
          <table class="report-container table-print mt-2">
            <thead class="report-header">
              <tr>
                <th>DATE</th>
                <th>TIME IN</th>
                <th>TIME OUT</th>
                <th>UNITS</th>
                <th>HOURS</th>
                <th>LOC</th>
                <th class="text-xs-left">CAREGIVER</th>
                <th>CAREGIVER SIGN</th>
                <th>PROVIDER SIGN</th>
              </tr>
            </thead>

            <tbody class="report-content">
              <tr v-for="s in report.sessions" :key="'session' + s.sessionId">
                <td class="text-xs-center" style="vertical-align: middle;">{{ s.sessionStart | moment("MM/DD/YYYY") }}</td>
                <td class="text-xs-center" style="vertical-align: middle;">{{ s.sessionStart | moment("LT") }}</td>
                <td class="text-xs-center" style="vertical-align: middle;">{{ s.sessionEnd | moment("LT") }}</td>
                <td class="text-xs-center" style="vertical-align: middle;">{{ s.totalUnits.toLocaleString() }}</td>
                <td class="text-xs-center" style="vertical-align: middle;">{{ (s.totalUnits / 4).toLocaleString() }}</td>
                <td class="text-xs-center" style="vertical-align: middle;">{{ s.posCode }}</td>
                <td class="text-xs-left" style="vertical-align: middle;">{{ s.sessionTypeCode != 3 ? s.caregiverFullname : s.caregiverFullnameSupervision }}</td>
                <td class="text-xs-center" style="vertical-align: middle;">
                  <span v-if="!s.sign"></span>
                  <img v-else style="object-fit: contain; max-height: 20px;" :src="!s.sign || s.sign.sign" />
                </td>
                <td class="text-xs-center" style="vertical-align: middle;">
                  <span v-if="!report.user.userSign"></span>
                  <img v-else style="object-fit: contain; max-height: 20px;" :src="report.user.userSign.sign" />
                </td>
              </tr>

              <tr class="grey lighten-2">
                <td colspan="3">Total</td>
                <td class="px-1 text-xs-center">
                  {{ totalUnits.toLocaleString() }}
                </td>
                <td class="px-1 text-xs-center">
                  {{ (totalUnits / 4).toLocaleString() }}
                </td>
                <td colspan="4"></td>
              </tr>

              <tr>
                <td style="height: 50px !important;" colspan="10">
                  <small>Signature:</small>
                  <br />
                  <span v-if="!report.user.userSign"></span>
                  <img v-else class="ml-2" style="object-fit: contain; max-height: 30px;" :src="report.user.userSign.sign" />
                  <br />
                  <strong>{{ report.user.firstname }} {{ report.user.lastname }}, {{ report.user.rol.rolName }}</strong>
                  <br />
                  <span>{{ report.user.licenseNo }}</span>
                </td>
                <!-- <td style="height: 50px !important;" colspan="2">
                    <small>Date:</small>
                    <br>
                    <span>{{new Date() | moment('MMMM, Do YYYY')}}</span>
                  </td> -->
              </tr>
            </tbody>
          </table>
          <div></div>
        </v-card-text>
      </v-card>
    </v-flex>
  </v-layout>
</template>

<script>
import userApi from "@/services/api/UserServices";
// import clientApi from '@/services/api/ClientServices';
import reportingApi from "@/services/api/ReportingServices";

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
      userId: null,
      clients: [],
      clientId: null,
      report: null,
      sessions: []
    };
  },

  components: {
    SelectUser: () => import(/* webpackChunkName: "SelectUser" */ "@/components/shared/SelectUser")
  },

  computed: {
    user() {
      return this.$store.getters.user;
    },
    isAdminOrBillingOrManagement() {
      return this.user.rol2 === "admin" || this.user.rol2 === "billing" || this.user.rol2 === "management";
    },
    totalUnits() {
      return this.report.sessions.length > 0 ? this.report.sessions.map((a) => a.totalUnits).reduce((a, b) => a + b) : 0;
    }
  },

  mounted() {
    this.$store.commit("SET_ACTIVE_CLIENT", null);
    if (!this.isAdminOrBillingOrManagement) this.loadUserClients();
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
        this.clients = await userApi.loadUserClients(this.isAdminOrBillingOrManagement ? this.userId : -1);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async viewReport() {
      try {
        this.loading = true;
        this.sessions = [];
        this.report = null;
        this.report = await reportingApi.getServiceLog(this.datePickerModel.start, this.datePickerModel.end, this.clientId, this.isAdminOrBillingOrManagement ? this.userId : -1);
        this.report.sessions.forEach((e) => {
          e.sessionStart = this.$moment(e.sessionStart).local();
          e.sessionEnd = this.$moment(e.sessionEnd).local();
          this.sessions.push(e);
        });
      } catch (error) {
        this.$toast.error(error.response.data || error.message);
      } finally {
        this.loading = false;
      }
    },

    async changedUser() {
      await this.loadUserClients();
    },

    print() {
      window.print();
    }
  }
};
</script>
