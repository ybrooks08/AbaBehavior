<template>
  <v-layout row wrap>
    <v-flex xs12 class="no-print">
      <v-card>
        <v-toolbar dark class="secondary" fluid dense>
          <v-toolbar-title>Sessions history</v-toolbar-title>
        </v-toolbar>
        <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
        <v-card-text class="pa-1">
          <v-form ref="form" autocomplete="off" v-model="validForm">
            <v-layout row wrap>
              <v-flex xs12>
                <date-picker-menu :isLarge="true" :isDark="false" :btnColor="'primary'" :disabled="loading" v-model="datePickerModel"/>
              </v-flex>
              <v-flex md12>
                <v-autocomplete box hid :disabled="loading" :items="clients" v-model="clientId" label="Client" prepend-icon="fa-user" item-text="clientName" item-value="clientId" :rules="[required]" required>
                  <template slot="item" slot-scope="{ item }">
                    <v-list-tile-avatar>
                      <img :style="!item.active ? 'opacity: 0.5': ''" :src="`images/${item.gender ? item.gender.toLowerCase() : 'nogender'}.png`">
                    </v-list-tile-avatar>
                    <v-list-tile-content>
                      <v-list-tile-title :class="{ 'grey--text text--lighten-1': !item.active }">{{item.clientName}}</v-list-tile-title>
                      <v-list-tile-sub-title :class="{ 'grey--text text--lighten-1': !item.active }">{{item.dob | moment("utc", "MM/DD/YYYY")}} | Code: {{item.clientCode || "N/A" }}</v-list-tile-sub-title>
                    </v-list-tile-content>
                  </template>
                </v-autocomplete>
              </v-flex>
            </v-layout>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer/>
          <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="viewReport">Generate</v-btn>
        </v-card-actions>
      </v-card>
    </v-flex>
    <v-flex xs12 v-if="sessions.length > 0">
      <v-card>
        <v-toolbar dense dark class="secondary">
          <v-toolbar-title>Sessions history</v-toolbar-title>
          <v-spacer/>
        </v-toolbar>
        <v-card-text class="pa-0">
          <table v-if="sessions.length > 0" class="v-datatable v-table theme--light condensed">
            <thead>
              <tr>
                <th class="text-xs-center py-0">SessionId</th>
                <th class="text-xs-left py-0 px-1">User / Rol</th>
                <th class="text-xs-left py-0 px-1">Date</th>
                <th class="text-xs-left py-0 px-1">Status</th>
                <th class="text-xs-left py-0 px-1 hidden-sm-and-down">Start / End</th>
                <th class="text-xs-left py-0 px-1 hidden-sm-and-down">Type</th>
                <th class="text-xs-left py-0 px-1 hidden-sm-and-down">Pos</th>
                <th class="text-xs-left py-0 px-1">Units</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="r in sessions" :key="('session'+r.sessionId)">
                <td class="pl-2 pr-1 text-xs-center">{{r.sessionId}}</td>
                <td class="px-1">
                  <strong>{{r.userFullname}}</strong>
                  <br>
                  {{r.rol}}
                </td>
                <td class="px-1">{{r.sessionStart | moment("MM/DD/YYYY")}}</td>
                <td>
                  <v-chip dark label :color="r.sessionStatusColor">{{r.sessionStatus}}</v-chip>
                </td>
                <td class="hidden-sm-and-down px-1 text-truncate">
                  <v-icon color="green" small>fa-sign-in-alt</v-icon>
                  {{r.sessionStart | moment("LT")}}
                  <br>
                  <v-icon color="red" small>fa-sign-out-alt</v-icon>
                  {{r.sessionEnd | moment("LT")}}
                </td>
                <td class="hidden-sm-and-down px-1">{{r.sessionType}}</td>
                <td class="hidden-sm-and-down px-1">{{r.pos}}</td>
                <td class="px-1">
                  <strong>
                    <v-icon small>fa-star</v-icon>
                    {{r.totalUnits.toLocaleString()}}
                  </strong>
                  <br>
                  <v-icon small>fa-clock</v-icon>
                  {{(r.totalUnits / 4).toLocaleString()}}
                </td>
                <td class="text-xs-left pr-3 pl-0 right text-truncate">
                  <v-tooltip top>
                    <v-btn slot="activator" icon class="mx-0" @click.stop="sessionPrint(r)">
                      <v-icon color="grey darken-2">fa-print</v-icon>
                    </v-btn>
                    <span>Print</span>
                  </v-tooltip>
                  <v-tooltip top>
                    <v-btn slot="activator" icon class="mx-0" @click.stop="sessionNotes(r)">
                      <v-icon color="grey darken-2">fa-notes-medical</v-icon>
                    </v-btn>
                    <span>View Notes</span>
                  </v-tooltip>
                  <v-tooltip top>
                    <v-btn slot="activator" icon class="mx-0" @click.stop="sessionData(r)">
                      <v-icon color="grey darken-2">fa-chart-line</v-icon>
                    </v-btn>
                    <span>View data</span>
                  </v-tooltip>
                </td>
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
      clients: [],
      clientId: null,
      sessions: []
    };
  },

  computed: {
    user() {
      return this.$store.getters.user;
    },
    isAdminOrManagement() {
      return this.user.rol2 === "admin" || this.user.rol2 === "management";
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
      } finally { this.loading = false; }
    },

    async viewReport() {
      try {
        this.loading = true;
        this.sessions = [];
        let sessions = await reportingApi.getSessionsHistory(this.datePickerModel.start, this.datePickerModel.end, this.clientId);

        if (sessions.length == 0) {
          this.$toast.info("No data");
          return;
        }

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

    sessionNotes(session) {
      this.$store.commit("SET_ACTIVE_DATE", session.sessionStart);
      this.$store.commit("SET_ACTIVE_CLIENT", session.clientId);
      this.$store.commit("SET_ACTIVE_SESSION", session.sessionId);
      let routeData = this.$router.resolve("/clients/session_notes");
      window.open(routeData.href, "_blank");
      //this.$router.push('/clients/session_notes');
    },

    sessionData(session) {
      this.$store.commit("SET_ACTIVE_DATE", session.sessionStart);
      this.$store.commit("SET_ACTIVE_CLIENT", session.clientId);
      this.$store.commit("SET_ACTIVE_SESSION", session.sessionId);
      let routeData = this.$router.resolve("/session/session_collect_data");
      window.open(routeData.href, "_blank");
      //this.$router.push('/session/session_collect_data');
    },

    sessionPrint(session) {
      this.$store.commit("SET_ACTIVE_DATE", session.sessionStart);
      this.$store.commit("SET_ACTIVE_CLIENT", session.clientId);
      this.$store.commit("SET_ACTIVE_SESSION", session.sessionId);
      let routeData = this.$router.resolve("/session/session_print");
      window.open(routeData.href, "_blank");
    }
  }

};
</script>
