<template>
  <v-layout row wrap>
    <v-flex xs12 class="no-print">
      <v-card>
        <v-toolbar dark class="secondary" fluid dense>
          <v-toolbar-title>Matching Report</v-toolbar-title>
        </v-toolbar>
        <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
        <v-card-text class="pa-1">
          <v-form ref="form" autocomplete="off" v-model="validForm">
            <v-layout row wrap>
              <v-flex xs12>
                <date-picker-menu :isLarge="true" :isDark="false" :btnColor="'primary'" :disabled="loading" v-model="datePickerModel" />
              </v-flex>
              <v-flex xs12 sm6>
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
                  @change="clear"
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
          <v-spacer />
          <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="viewReport">Generate</v-btn>
        </v-card-actions>
      </v-card>
    </v-flex>
    <v-flex xs12 v-if="sessions.length > 0">
      <v-card>
        <v-toolbar dense dark class="secondary no-print">
          <v-toolbar-title>Sessions</v-toolbar-title>
          <v-spacer />
          <v-btn dark icon @click="report = null">
            <v-icon>fa-times-circle</v-icon>
          </v-btn>
        </v-toolbar>
        <v-card-text class="print-full-width pa-0">
          <table class="report-container table-print">
            <thead class="report-header">
              <tr>
                <th>ID</th>
                <th>DATE</th>
                <!-- <th>TIME IN</th>
                <th>TIME OUT</th> -->
                <th>UNITS</th>
                <th>STATUS</th>
                <th>MOST MATCHES</th>
                <th>PCT</th>
                <th>ID</th>
                <th>Date</th>
              </tr>
            </thead>

            <tbody class="report-content">
              <tr v-for="s in sessions" :key="'session' + s.sessionId" :set="(match = getMatch(s.sessionId))">
                <!-- <tr v-for="s in sessions" :key="'session' + s.sessionId"> -->
                <td class="text-xs-center" style="vertical-align: middle;">
                  <v-btn color="success" small class="mx-0" @click.stop="sessionNotes(s)">
                    {{ s.sessionId }}
                  </v-btn>
                </td>
                <td class="text-xs-center" style="vertical-align: middle;">{{ s.sessionStart | moment("MM/DD/YYYY") }}</td>
                <!-- <td class="text-xs-center" style="vertical-align: middle;">{{ s.sessionStart | moment("LT") }}</td>
                <td class="text-xs-center" style="vertical-align: middle;">{{ s.sessionEnd | moment("LT") }}</td> -->
                <td class="text-xs-center" style="vertical-align: middle;">{{ s.totalUnits.toLocaleString() }}</td>
                <td class="text-xs-center" style="vertical-align: middle;">
                  <v-chip small dark label :color="s.sessionStatusColor">{{ s.sessionStatus }}</v-chip>
                </td>
                <td>
                  <v-progress-linear v-if="match" background-color="success" color="error" :ref="'prog' + s.sessionId"></v-progress-linear>
                </td>
                <td class="text-xs-center" style="vertical-align: middle;" :ref="'percNo' + s.sessionId"><v-icon small>fa-cog fa-spin</v-icon></td>
                <td class="text-xs-center" style="vertical-align: middle;" :ref="'perc' + s.sessionId"></td>
                <td class="text-xs-center" style="vertical-align: middle;" :ref="'percDate' + s.sessionId"></td>
              </tr>
            </tbody>
          </table>
        </v-card-text>
      </v-card>
    </v-flex>
  </v-layout>
</template>

<script>
import userApi from "@/services/api/UserServices";
import reportingApi from "@/services/api/ReportingServices";
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
      userId: null,
      clients: [],
      clientId: null,
      sessions: [],
      values: []
    };
  },

  components: {
    SelectUser: () => import(/* webpackChunkName: "SelectUser" */ "@/components/shared/SelectUser")
  },

  computed: {
    user() {
      return this.$store.getters.user;
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
      this.clear();
      this.loading = true;
      try {
        this.clients = await userApi.loadUserClients(this.userId);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async viewReport() {
      try {
        this.loading = true;
        this.clear();
        let sessions = await reportingApi.getSessionsForMatching(this.datePickerModel.start, this.datePickerModel.end, this.clientId, this.userId);
        sessions.forEach(e => {
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
      this.clear();
      await this.loadUserClients();
    },

    print() {
      window.print();
    },

    async getMatch(id) {
      const res = await sessionServicesApi.checkMatchingPercentaje(id);
      this.$refs["perc" + id][0].innerHTML = res.sessionId;
      this.$refs["percNo" + id][0].innerHTML = (res.percentaje * 100).toFixed(2) + "%";
      this.$refs["prog" + id][0].value = res.percentaje * 100;
      this.$refs["percDate" + id][0].innerHTML = this.$moment(res.date).format("MM/DD/YYYY");
    },

    getMatch2(id) {
      sessionServicesApi.checkMatchingPercentaje(id).then(function(response) {
        console.log(response);
      });
    },

    sessionNotes(session) {
      this.$store.commit("SET_ACTIVE_DATE", session.sessionStart);
      this.$store.commit("SET_ACTIVE_CLIENT", session.clientId);
      this.$store.commit("SET_ACTIVE_SESSION", session.sessionId);
      let routeData = this.$router.resolve("/clients/session_notes");
      window.open(routeData.href, "_blank");
    },

    clear() {
      this.sessions = [];
    }
  }
};
</script>
