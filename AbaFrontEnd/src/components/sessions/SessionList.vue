<template>
  <v-card>
    <v-toolbar dark class="secondary">
      <v-toolbar-title>Sessions</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-text-field v-model="search" placeholder="Search" prepend-icon="fa-search" clearable hide-details single-line solo-inverted></v-text-field>
      <!-- <v-menu class="mr-0" bottom left :disabled="loadingLastSessions">
        <v-btn slot="activator" icon :disabled="loadingLastSessions">
          <v-icon>fa-ellipsis-v</v-icon>
        </v-btn>
        <v-list>
          <v-list-tile @click="showClosed = !showClosed">
            <v-list-tile-action>
              <v-icon>{{showClosed ? 'fa-eye-slash':'fa-eye'}}</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>{{showClosed ? 'Hide closed':'Show closed'}}</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
        </v-list>
      </v-menu> -->

      <template v-slot:extension>
        <v-tabs v-model="tabModel" color="secondary">
          <v-tab key="allSessionsTab">
            Opened
          </v-tab>
          <v-tab key="lastWeekClosedSessions">
            Closed LW
          </v-tab>
        </v-tabs>
      </template>
    </v-toolbar>
    <v-progress-linear style="position: absolute;" v-show="loadingLastSessions" :indeterminate="true" class="ma-0"></v-progress-linear>

    <v-tabs-items v-model="tabModel">
      <v-tab-item key="allSessionsTab">
        <v-card flat>
          <v-card-text class="pa-0">
            <table v-if="sessions.length > 0" class="v-datatable v-table theme--light condensed">
              <thead>
                <tr>
                  <th class="text-xs-center py-0 hidden-md-and-down">SessionId</th>
                  <th class="text-xs-left py-0 pl-2 pr-1">Client / Code</th>
                  <th class="text-xs-left py-0 px-1">User / Rol</th>
                  <th class="text-xs-left py-0 px-1">Date</th>
                  <th class="text-xs-left py-0 px-1">Status</th>
                  <th class="text-xs-left py-0 px-1 hidden-sm-and-down">Start / End</th>
                  <th class="text-xs-left py-0 px-1 hidden-sm-and-down">Type</th>
                  <th class="text-xs-left py-0 px-1 hidden-sm-and-down">Pos</th>
                  <th class="text-xs-left py-0 px-1 hidden-xs-only">Units</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="r in filteredSessions" :key="('session'+r.sessionId)">
                  <td class="pl-2 pr-1 text-xs-center hidden-md-and-down">{{r.sessionId}}</td>
                  <td class="pl-2 pr-1">
                    <strong>{{r.clientFullname}}</strong>
                    <br>
                    {{r.clientCode}}
                  </td>
                  <td class="px-1">
                    <strong class="hidden-xs-only">{{r.userFullname}}</strong>
                    <br class="hidden-xs-only">
                    {{r.userRol}}
                  </td>
                  <td class="px-1">{{r.sessionStart | moment('MM/DD/YYYY')}}</td>
                  <td>
                    <v-chip class="hidden-xs-only" dark label :color="r.sessionStatusColor">{{r.sessionStatus}}</v-chip>
                    <v-avatar tile size="28" class="hidden-sm-and-up" :color="r.sessionStatusColor">
                      <span class="white--text headline">{{r.sessionStatus.charAt(0)}}</span>
                    </v-avatar>
                  </td>
                  <td class="hidden-sm-and-down px-1 text-truncate">
                    <v-icon color="green" small>fa-sign-in-alt</v-icon>
                    {{r.sessionStart | moment('LT')}}
                    <br>
                    <v-icon color="red" small>fa-sign-out-alt</v-icon>
                    {{r.sessionEnd | moment('LT')}}
                  </td>
                  <td class="hidden-sm-and-down px-1">{{r.sessionType}}</td>
                  <td class="hidden-sm-and-down px-1">{{r.pos}}</td>
                  <td class="px-1 hidden-xs-only">
                    <strong>
                      <v-icon small>fa-star</v-icon>
                      {{r.totalUnits.toLocaleString()}}
                    </strong>
                    <br>
                    <v-icon small>fa-clock</v-icon>
                    {{(r.totalUnits / 4).toLocaleString()}}
                  </td>
                  <td class="text-xs-left pr-3 pl-0 right text-no-wrap">
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
            <v-alert v-else type="info" :value="true">No sessions to display</v-alert>
          </v-card-text>
        </v-card>
      </v-tab-item>
      <v-tab-item key="lastWeekClosedSessions">
        <v-card flat>
          <v-card-text class="pa-0">
            <table v-if="sessionsClosed.length > 0" class="v-datatable v-table theme--light condensed">
              <thead>
                <tr>
                  <th class="text-xs-center py-0 hidden-md-and-down">SessionId</th>
                  <th class="text-xs-left py-0 pl-2 pr-1">Client / Code</th>
                  <th class="text-xs-left py-0 px-1">User / Rol</th>
                  <th class="text-xs-left py-0 px-1">Date</th>
                  <th class="text-xs-left py-0 px-1">Status</th>
                  <th class="text-xs-left py-0 px-1 hidden-sm-and-down">Start / End</th>
                  <th class="text-xs-left py-0 px-1 hidden-sm-and-down">Type</th>
                  <th class="text-xs-left py-0 px-1 hidden-sm-and-down">Pos</th>
                  <th class="text-xs-left py-0 px-1 hidden-xs-only">Units</th>
                  <th></th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="r in filteredSessionsClosed" :key="('sessionClosed'+r.sessionId)">
                  <td class="pl-2 pr-1 text-xs-center hidden-md-and-down">{{r.sessionId}}</td>
                  <td class="pl-2 pr-1">
                    <strong>{{r.clientFullname}}</strong>
                    <br>
                    {{r.clientCode}}
                  </td>
                  <td class="px-1">
                    <strong class="hidden-xs-only">{{r.userFullname}}</strong>
                    <br class="hidden-xs-only">
                    {{r.userRol}}
                  </td>
                  <td class="px-1">{{r.sessionStart | moment('MM/DD/YYYY')}}</td>
                  <td>
                    <v-chip class="hidden-xs-only" dark label :color="r.sessionStatusColor">{{r.sessionStatus}}</v-chip>
                    <v-avatar tile size="28" class="hidden-sm-and-up" :color="r.sessionStatusColor">
                      <span class="white--text headline">{{r.sessionStatus.charAt(0)}}</span>
                    </v-avatar>
                  </td>
                  <td class="hidden-sm-and-down px-1 text-truncate">
                    <v-icon color="green" small>fa-sign-in-alt</v-icon>
                    {{r.sessionStart | moment('LT')}}
                    <br>
                    <v-icon color="red" small>fa-sign-out-alt</v-icon>
                    {{r.sessionEnd | moment('LT')}}
                  </td>
                  <td class="hidden-sm-and-down px-1">{{r.sessionType}}</td>
                  <td class="hidden-sm-and-down px-1">{{r.pos}}</td>
                  <td class="px-1 hidden-xs-only">
                    <strong>
                      <v-icon small>fa-star</v-icon>
                      {{r.totalUnits.toLocaleString()}}
                    </strong>
                    <br>
                    <v-icon small>fa-clock</v-icon>
                    {{(r.totalUnits / 4).toLocaleString()}}
                  </td>
                  <td class="text-xs-left pr-3 pl-0 right text-no-wrap">
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
            <v-alert v-else type="info" :value="true">No sessions to display</v-alert>
          </v-card-text>
        </v-card>
      </v-tab-item>
    </v-tabs-items>

  </v-card>
</template>

<script>
import userApi from "@/services/api/UserServices";

export default {
  name: "SessionList",

  props: {
    days: {
      type: Number,
      required: false,
      default: 30
    }
  },

  data() {
    return {
      loadingLastSessions: false,
      sessions: [],
      sessionsClosed: [],
      showClosed: false,
      search: "",
      tabModel: null
    };
  },

  computed: {
    filteredSessions: function() {
      return this.sessions.filter(item => {
        let regex = new RegExp(this.search == null ? "" : this.search, "i");
        return (item.userFullname ? item.userFullname.match(regex) : true) || (item.clientFullname ? item.clientFullname.match(regex) : true) || (item.sessionStatus ? item.sessionStatus.match(regex) : true);
      });
    },
    filteredSessionsClosed: function() {
      return this.sessionsClosed.filter(item => {
        let regex = new RegExp(this.search == null ? "" : this.search, "i");
        return (item.userFullname ? item.userFullname.match(regex) : true) || (item.clientFullname ? item.clientFullname.match(regex) : true) || (item.sessionStatus ? item.sessionStatus.match(regex) : true);
      });
    }
  },

  watch: {
    showClosed() {
      this.getLastSessions();
    }
  },

  mounted() {
    this.getLastSessions();
  },

  methods: {
    async getLastSessions() {
      this.loadingLastSessions = true;
      try {
        this.sessions = [];
        const sessions = await userApi.getSessionList(this.days, this.showClosed);
        let sessions2 = Object.freeze(sessions);
        sessions2.forEach(e => {
          e.sessionStart = this.$moment(e.sessionStart).local();
          e.sessionEnd = this.$moment(e.sessionEnd).local();
          this.sessions.push(e);
        });

        this.sessionsClosed = [];
        const sessionsClosed = await userApi.getSessionListClosedLw();
        let sessionsClosed2 = Object.freeze(sessionsClosed);
        sessionsClosed2.forEach(e => {
          e.sessionStart = this.$moment(e.sessionStart).local();
          e.sessionEnd = this.$moment(e.sessionEnd).local();
          this.sessionsClosed.push(e);
        });
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loadingLastSessions = false;
      }
    },

    sessionNotes(session) {
      this.$store.commit("SET_ACTIVE_DATE", session.sessionStart);
      this.$store.commit("SET_ACTIVE_CLIENT", session.clientId);
      this.$store.commit("SET_ACTIVE_SESSION", session.sessionId);
      this.$router.push("/clients/session_notes");
    },

    sessionData(session) {
      this.$store.commit("SET_ACTIVE_DATE", session.sessionStart);
      this.$store.commit("SET_ACTIVE_CLIENT", session.clientId);
      this.$store.commit("SET_ACTIVE_SESSION", session.sessionId);
      this.$router.push("/session/session_collect_data");
    }
  }
};
</script>

<style scoped>
</style>