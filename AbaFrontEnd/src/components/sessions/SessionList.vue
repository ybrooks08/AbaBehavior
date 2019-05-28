<template>
  <v-card>
    <v-toolbar dark class="secondary">
      <v-toolbar-title>Sessions</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-text-field v-model="search" placeholder="Search" prepend-icon="fa-search" clearable hide-details single-line solo-inverted></v-text-field>
      <!-- <v-menu class="mr-0" bottom left :disabled="loading">
        <v-btn slot="activator" icon :disabled="loading">
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
          <v-tab key="allSessionsTab" v-if="showOpen">
            Opened
          </v-tab>
          <v-tab key="readyToReview" v-if="showOpen">
            Ready to review
          </v-tab>
          <v-tab key="lastWeekClosedSessions">
            LW reviewed and billed
          </v-tab>
        </v-tabs>
      </template>
    </v-toolbar>
    <!-- <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear> -->

    <v-tabs-items v-model="tabModel">
      <v-tab-item key="allSessionsTab" v-if="showOpen">
        <v-card flat>
          <v-card-text class="pa-0">
            <session-list-table :search="search" :items="sessions" :loading="loading"></session-list-table>
          </v-card-text>
        </v-card>
      </v-tab-item>
      <v-tab-item key="readyToReview" v-if="showOpen">
        <v-card flat>
          <v-card-text class="pa-0">
            <session-list-table :search="search" :items="sessionsReadyToReview" :loading="loading"></session-list-table>
          </v-card-text>
        </v-card>
      </v-tab-item>
      <v-tab-item key="lastWeekClosedSessions">
        <v-card flat>
          <v-card-text class="pa-0">
            <session-list-table :search="search" :items="sessionsClosed" :loading="loading"></session-list-table>
          </v-card-text>
        </v-card>
      </v-tab-item>
    </v-tabs-items>

  </v-card>
</template>

<script>
import userApi from "@/services/api/UserServices";
import sessionListTable from "@/components/sessions/SessionListTable";

export default {
  name: "SessionList",

  props: {
    days: {
      type: Number,
      required: false,
      default: 30
    },
    showOpen: {
      type: Boolean,
      required: false,
      default: true
    }
  },

  components: {
    sessionListTable
  },

  data() {
    return {
      loading: false,
      sessions: [],
      sessionsClosed: [],
      sessionsReadyToReview: [],
      showClosed: false,
      search: "",
      tabModel: null
    };
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
      this.loading = true;
      try {
        this.sessions = [];
        let sessionsLocal = [];
        const sessions = await userApi.getSessionList(this.days, this.showClosed);
        //let sessions2 = Object.freeze(sessions);
        sessions.forEach(e => {
          e.sessionStart = this.$moment(e.sessionStart).local();
          e.sessionEnd = this.$moment(e.sessionEnd).local();
          sessionsLocal.push(e);
        });
        this.sessions = Object.freeze(sessionsLocal);

        this.sessionsClosed = [];
        let sessionsClosedLocal = [];
        const sessionsClosed = await userApi.getSessionListClosedLw();
        //let sessionsClosed2 = Object.freeze(sessionsClosed);
        sessionsClosed.forEach(e => {
          e.sessionStart = this.$moment(e.sessionStart).local();
          e.sessionEnd = this.$moment(e.sessionEnd).local();
          sessionsClosedLocal.push(e);
        });
        this.sessionsClosed = Object.freeze(sessionsClosedLocal);

        this.sessionsReadyToReview = [];
        let sessionsReadyToReviewLocal = [];
        const sessionsReadyToReview = await userApi.getSessionReadyToReview();
        sessionsReadyToReview.forEach(e => {
          e.sessionStart = this.$moment(e.sessionStart).local();
          e.sessionEnd = this.$moment(e.sessionEnd).local();
          sessionsReadyToReviewLocal.push(e);
        });
        this.sessionsReadyToReview = Object.freeze(sessionsReadyToReviewLocal);
      } catch (error) {
        console.error(error);
        this.$toast.error(error);
      } finally {
        this.loading = false;
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