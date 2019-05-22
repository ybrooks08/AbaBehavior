<template>
  <v-container grid-list-xs pa-0>
    <v-layout row wrap>
      <v-flex xs12 class="no-print">
        <v-card>
          <v-toolbar dark class="secondary" fluid dense>
            <v-toolbar-title>Time sheet</v-toolbar-title>
          </v-toolbar>
          <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
          <v-card-text>
            <v-layout row wrap>
              <v-flex xs12 sm6 md5 lg4 xl3>
                <select-week v-model="dateRange"></select-week>
              </v-flex>
              <v-flex xs12 sm6 md7 lg8 xl9 v-if="isAdminOrBilling">
                <select-user v-model="userId"></select-user>
              </v-flex>
            </v-layout>
          </v-card-text>
          <v-card-actions>
            <small class="pl-4 grey--text">* Only reviewed and billed sessions will be reported</small>
            <v-spacer />
            <v-btn :disabled="loading" :loading="loading" color="primary" @click="viewReport">Generate</v-btn>
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
    <v-layout row wrap>
      <v-flex v-if="report.length > 0" xs12>
        <v-card>
          <v-toolbar dense dark class="secondary no-print">
            <v-toolbar-title>Time sheet report</v-toolbar-title>
            <v-spacer />
            <v-btn dark icon @click="print">
              <v-icon>fa-print</v-icon>
            </v-btn>
            <v-btn dark icon @click="report = null">
              <v-icon>fa-times-circle</v-icon>
            </v-btn>
          </v-toolbar>
          <v-layout row wrap>
            <v-flex xs2>
              <img style="object-fit: contain;" src="images/logo.jpg" width="100">
            </v-flex>
            <v-flex xs10>
              <h3 class="pt-3">EMPLOYEE TIME SHEET</h3>
              <h4 class="pr-3 right">EMPLOYEE: {{userData.firstname.toUpperCase()}} {{userData.lastname.toUpperCase()}}</h4><br>
              <h5 class="pr-3 right">{{userData.rol.rolName}}</h5>
              <v-divider></v-divider>
              <span>
                FROM:
                <strong>{{dateRange.start | moment("MM/DD/YYYY")}}</strong> TO:
                <strong>{{dateRange.end | moment("MM/DD/YYYY")}}</strong>
              </span>
            </v-flex>
          </v-layout>
          <v-card-text class="print-full-width">
            <table class="v-datatable v-table theme--light print-font-small">
              <thead>
                <tr style="height: auto;">
                  <th class="text-xs-left py-0">Date</th>
                  <th class="text-xs-left py-0">IN</th>
                  <th class="text-xs-left py-0">OUT</th>
                  <th class="text-xs-left py-0">Client</th>
                  <th class="text-xs-right py-0">Hours/Drive</th>
                  <th class="text-xs-right py-0">Reg</th>
                  <th class="text-xs-right py-0">Drv</th>
                  <th class="text-xs-right py-0">Extra</th>
                  <th class="text-xs-right py-0">ExtDrv</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="r in report" :key="('session'+r.sessionId)">
                  <td class="px-1">
                    <v-icon class="no-print" small>fa-calendar</v-icon>
                    {{r.date | moment('MM/DD/YYYY')}}
                  </td>
                  <td class="px-1">
                    <v-icon class="no-print" small>fa-clock</v-icon>
                    {{r.sessionIn | moment('LT')}}
                  </td>
                  <td class="px-1">
                    <v-icon class="no-print" small>fa-clock</v-icon>
                    {{r.sessionOut | moment('LT')}}
                  </td>
                  <td class="px-1 text-xs-left">{{r.client}}</td>
                  <td class="px-1 text-xs-right">{{r.sessionHours}}/{{r.sessionDriveTime}}</td>
                  <td class="px-1 text-xs-right">{{r.regularHours}}</td>
                  <td class="px-1 text-xs-right">{{r.regularDrive}}</td>
                  <td class="px-1 text-xs-right">{{r.extraHours}}</td>
                  <td class="px-1 text-xs-right">{{r.extraDrive}}</td>
                </tr>
              </tbody>
              <tfoot>
                <tr class="font-weight-medium">
                  <td class="px-1 text-xs-left" colspan="5">Totals</td>
                  <td class="px-1 text-xs-right">{{totalRegular}}</td>
                  <td class="px-1 text-xs-right">{{totalRegularDrive}}</td>
                  <td class="px-1 text-xs-right">{{totalExtraHours}}</td>
                  <td class="px-1 text-xs-right">{{totalExtraDrive}}</td>
                </tr>
                <tr class="font-weight-medium">
                  <td class="px-1 text-xs-left" colspan="5">Pay rate</td>
                  <td class="px-1 text-xs-right">${{userData.payRate.toFixed(2)}}</td>
                  <td class="px-1 text-xs-right">${{userData.driveTimePayRate.toFixed(2)}}</td>
                  <td class="px-1 text-xs-right">${{payExtraRate}}</td>
                  <td class="px-1 text-xs-right">${{payDriveTimeExtraRate}}</td>
                </tr>
                <tr class="font-weight-medium">
                  <td class="px-1 text-xs-left" colspan="5">Subtotal</td>
                  <td class="px-1 text-xs-right">${{(totalRegular * userData.payRate).toFixed(2)}}</td>
                  <td class="px-1 text-xs-right">${{(totalRegularDrive * userData.driveTimePayRate).toFixed(2)}}</td>
                  <td class="px-1 text-xs-right">${{(totalExtraHours * payExtraRate).toFixed(2)}}</td>
                  <td class="px-1 text-xs-right">${{(totalExtraDrive * payDriveTimeExtraRate).toFixed(2)}}</td>
                </tr>
                <tr class="font-weight-black black white--text">
                  <td class="px-1 text-xs-left" colspan="5">TOTAL</td>
                  <td class="px-1 text-xs-center" colspan="4">${{(totalRegular * userData.payRate + totalRegularDrive * userData.driveTimePayRate + totalExtraHours * payExtraRate + totalExtraDrive * payDriveTimeExtraRate).toFixed(2)}}</td>
                </tr>
              </tfoot>
            </table>
            <v-divider></v-divider>
            <p class="pt-4">I hereby certify that all time recorded above is true and correct. I also certify that I have not failed to record any time worked</p>
            <v-layout text-xs-center>
              <v-flex xs12 sm6 md4>
                <div v-if="!userData.userSign">
                  <div style="min-height: 100px;" class="mb-2"></div>
                  <v-divider></v-divider>
                  {{userData.firstname}} {{userData.lastname}}
                </div>
                <div v-else>
                  <div style="height: 100px;" class="mb-2">
                    <img style="height:100%" :src="userData.userSign.sign">
                  </div>
                  <v-divider></v-divider>
                  {{userData.firstname}} {{userData.lastname}}
                </div>
              </v-flex>
            </v-layout>

          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import reportingApi from "@/services/api/ReportingServices";

export default {
  components: {
    SelectWeek: () => import(/* webpackChunkName: "SelectWeek" */ "@/components/shared/SelectWeek"),
    SelectUser: () => import(/* webpackChunkName: "SelectUser" */ "@/components/shared/SelectUser")
  },

  data() {
    return {
      dateRange: { start: null, end: null },
      loading: false,
      userId: parseInt(this.$store.getters.user.id),
      report: [],
      userData: {
        payRate: 0,
        driveTimePayRate: 0
      }
    };
  },

  computed: {
    user() {
      return this.$store.getters.user;
    },
    isAdminOrBilling() {
      return this.user.rol2 === "admin" || this.user.rol2 === "billing";
    },
    totalRegular() {
      return this.report.reduce((a, b) => +a + +b.regularHours, 0);
    },
    totalRegularDrive() {
      return this.report.reduce((a, b) => +a + +b.regularDrive, 0);
    },
    totalExtraHours() {
      return this.report.reduce((a, b) => +a + +b.extraHours, 0);
    },
    totalExtraDrive() {
      return this.report.reduce((a, b) => +a + +b.extraDrive, 0);
    },
    payExtraRate() {
      return (this.userData.payRate + this.userData.payRate / 2).toFixed(2);
    },
    payDriveTimeExtraRate() {
      return (this.userData.driveTimePayRate + this.userData.driveTimePayRate / 2).toFixed(2);
    }
  },

  methods: {
    async viewReport() {
      try {
        this.loading = true;
        this.report = [];
        this.userData = {};
        const report = await reportingApi.getTimeSheet(this.dateRange.start, this.dateRange.end, this.userId);
        const rows = report.rows;
        rows.forEach(e => {
          e.date = this.$moment(e.date).local();
          e.sessionIn = this.$moment(e.sessionIn).local();
          e.sessionOut = this.$moment(e.sessionOut).local();
          this.report.push(e);
        });
        this.userData = report.user;
      } catch (error) {
        console.error(error);
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

