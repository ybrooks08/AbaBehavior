<template>
  <v-layout row wrap>
    <v-flex xs12>
      <v-card>
        <v-toolbar dark class="secondary" fluid dense>
          <v-toolbar-title>Sessions ready to bill</v-toolbar-title>
        </v-toolbar>
        <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
        <v-card-text class="pa-0">
          <v-expansion-panel v-if="sessions.length > 0">
            <v-expansion-panel-content v-for="client in clients" :key="'client'+client.clientId" expand-icon="fa-caret-down">
              <div slot="header">
                {{client.firstname}} {{client.lastname}}
                <v-chip small class="right ma-0 mr-4" color="primary" text-color="white">{{getSessionCount(client.clientId)}}</v-chip>
              </div>
              <table class="v-datatable v-table theme--light condensed">
                <thead>
                  <tr style="height: auto;">
                    <th class="text-xs-center py-0">Status</th>
                    <th class="text-xs-left py-0">Service</th>
                    <th class="text-xs-left py-0">Date</th>
                    <th class="text-xs-left py-0">User</th>
                    <th class="text-xs-left py-0">Type</th>
                    <th class="text-xs-left py-0">Start / End time</th>
                    <th class="text-xs-center py-0">Total Units / Hours</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="r in getSessions(client.clientId)" :key="('session'+r.sessionId)">
                    <td>
                      <v-chip label disabled :color="r.sessionStatusColor" text-color="white">{{r.sessionStatus}}</v-chip>
                    </td>
                    <td>
                      <v-chip disabled label text-color="white" :color="r.behaviorAnalisisCode.color"> {{r.behaviorAnalisisCode.hcpcs}}</v-chip>
                    </td>
                    <td>
                      {{r.sessionStart | moment("MM/DD/YYYY")}}
                    </td>
                    <td>
                      {{r.userFullname}}
                    </td>
                    <td>
                      {{r.sessionType}}
                    </td>
                    <td>
                      <v-icon small>fa-check-circle</v-icon>
                      {{r.sessionStart | moment("LT")}}
                      <br>
                      <v-icon small>fa-times-circle</v-icon>
                      {{r.sessionEnd | moment("LT")}}
                    </td>
                    <td class="text-xs-center">
                      <strong>
                        <v-icon small>fa-star</v-icon>
                        {{r.totalUnits.toLocaleString()}}
                      </strong>
                      <br>
                      <v-icon small>fa-clock</v-icon>
                      {{(r.totalUnits / 4).toLocaleString()}}
                    </td>
                  </tr>
                </tbody>
              </table>
            </v-expansion-panel-content>
          </v-expansion-panel>
          <v-alert v-else type="info" :value="true">
            No sessions ready to bill right now
          </v-alert>
        </v-card-text>
      </v-card>
    </v-flex>
  </v-layout>
</template>

<script>
import reportingApi from "@/services/api/ReportingServices";

export default {
  data() {
    return {
      loading: false,
      sessions: [],
      clients: []
    };
  },

  mounted() {
    this.$store.commit("SET_ACTIVE_CLIENT", 0);
    this.loadSessions();
  },

  methods: {
    async loadSessions() {
      this.clients = [];
      this.loading = true;
      try {
        let response = await reportingApi.getSessionsReady2Bill();
        response.forEach(e => {
          e.sessionStart = this.$moment(e.sessionStart).local();
          e.sessionEnd = this.$moment(e.sessionEnd).local();
          this.sessions.push(e);
        });
        const clients = [...new Set(this.sessions.map(o => JSON.stringify(o.client)))].map(s => JSON.parse(s)).sort((a, b) => a.firstname.localeCompare(b.firstname));
        this.clients = Object.freeze(clients);
      } catch (error) {
        console.log(error);
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    getSessionCount(clientId) {
      let c = this.sessions.filter(s => s.client.clientId === clientId);
      return c.length;
    },

    getSessions(clientId) {
      const c = this.sessions.filter(s => s.client.clientId === clientId).sort((a, b) => a.behaviorAnalisisCode.hcpcs.localeCompare(b.behaviorAnalisisCode.hcpcs));
      const all = Object.freeze(c);
      return all;
      //return c;
    }
  }
};
</script>

