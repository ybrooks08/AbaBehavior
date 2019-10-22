<template>
  <v-card @dblclick="getCursor">
    <v-toolbar dark class="secondary" fluid dense>
      <v-toolbar-title>Calendar week</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-toolbar-items>
        <v-btn flat @click="$store.commit('SET_ACTIVE_DATE', $moment())">go today</v-btn>
      </v-toolbar-items>
      <v-toolbar-title class="body-2 grey--text"
        >Active date:
        <v-chip label color="info">{{ selectedDate | moment("L") }}</v-chip>
      </v-toolbar-title>
      <v-menu class="mr-0" bottom left :disabled="loading">
        <v-btn slot="activator" icon :disabled="loading">
          <v-icon>fa-ellipsis-v</v-icon>
        </v-btn>
        <v-list>
          <v-list-tile to="/clients/new_session">
            <v-list-tile-action>
              <v-icon medium>fa-file-medical</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>New Session (BA Service)</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
          <v-list-tile v-if="user.rol2 === 'analyst'" to="/clients/new_session/3">
            <v-list-tile-action>
              <v-icon medium>fa-file-alt</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>New Session (Supervision to BCaBA)</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
          <v-list-tile v-if="user.rol2 !== 'tech'" to="/clients/new_training">
            <v-list-tile-action>
              <v-icon medium>fa-user-shield</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>New Caregiver training</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
          <v-list-tile v-if="user.rol2 === 'analyst'" to="/clients/caregiver_data_collection">
            <v-list-tile-action>
              <v-icon medium>fa-chart-bar</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>Caregiver data collection</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
          <v-list-tile v-if="user.rol2 === 'analyst' || user.rol2 === 'assistant'" to="/clients/edit_monthly_note">
            <v-list-tile-action>
              <v-icon medium>fa-calendar-alt</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>Edit monthly notes</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
          <v-divider v-if="user.rol2 !== 'tech'"></v-divider>
          <v-list-tile v-if="user.rol2 !== 'tech'" to="/competency_checks">
            <v-list-tile-action>
              <v-icon medium>fa-user-check</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>Competency checks</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
        </v-list>
        <v-divider></v-divider>
      </v-menu>
    </v-toolbar>
    <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
    <vue-cal
      style="min-height: 460px;"
      default-view="week"
      :time-from="8 * 60"
      :time-to="22 * 60"
      :disable-views="['years', 'year', 'day']"
      :events="events"
      events-on-month-view="short"
      hide-view-selector
      show-all-day-events="true"
      startWeekOnSunday
      :selected-date="selectedDate"
      @cell-click="cellClick"
      @cell-dblclick="cellDbClick"
      @view-change="viewChange"
    >
      <template v-slot:event-renderer="{ event, view }">
        <v-menu offset-y>
          <template v-slot:activator="{ on }">
            <v-card v-on="on" flat class="white--text pa-1 text-xs-left" :color="event.color" style="height: 100%; margin-left: 1px !important; margin-bottom: 1px !important; cursor: pointer;">
              <div class="text-truncate condensed">
                <v-icon color="white" small>{{ event.sessionType === "ba_service" ? "fa-file-medical" : event.sessionType === "supervision_bcaba" ? "fa-file-alt" : "fa-user-shield" }}</v-icon
                >&nbsp;
                <span class="caption condensed">{{ view == "month" ? event.userFullname : event.title }}</span>
                <span v-if="view != 'month' && !event.allDay" class="right caption"> <v-icon small color="yellow">fa-star</v-icon>{{ event.totalUnits }} </span>
              </div>
              <template v-if="!event.allDay && view != 'month'">
                <div class="vuecal__event-content text-truncate condensed">
                  <small class="caption">{{ event.userFullname }}</small>
                </div>
                <template v-if="event.totalUnits > 4">
                  <small class="vuecal__event-time text-truncate condensed" v-if="view.id != 'month'">
                    <v-icon small color="white">fa-clock</v-icon>&nbsp;<span>{{ event.start | moment("LT") }}</span> - <span>{{ event.end | moment("LT") }}</span> </small
                  ><br />
                  <div class="text-truncate condensed">
                    <v-icon small color="white">fa-flag</v-icon>&nbsp;<small class="caption">{{ event.sessionStatus }}</small>
                  </div>
                </template>
              </template>
            </v-card>
          </template>
          <v-list dense>
            <template v-if="!event.allDay">
              <v-list-tile @click="sessionDetails(event)">
                <v-list-tile-action>
                  <v-icon small>fa-file-medical</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                  <v-list-tile-title>Notes</v-list-tile-title>
                </v-list-tile-content>
              </v-list-tile>
              <v-list-tile @click="sessionData(event)">
                <v-list-tile-action>
                  <v-icon small>fa-chart-line</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                  <v-list-tile-title>Data</v-list-tile-title>
                </v-list-tile-content>
              </v-list-tile>
              <v-list-tile @click="sessionPrint(event)">
                <v-list-tile-action>
                  <v-icon small>fa-print</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                  <v-list-tile-title>Print</v-list-tile-title>
                </v-list-tile-content>
              </v-list-tile>
            </template>
            <template v-else>
              <v-list-tile @click="openCaregiverCollect(event)">
                <v-list-tile-action>
                  <v-icon small>fa-user-shield</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                  <v-list-tile-title>Caregiver collection</v-list-tile-title>
                </v-list-tile-content>
              </v-list-tile>
            </template>
          </v-list>
        </v-menu>
      </template>
      <template v-slot:no-event
        >No sessions</template
      >
      <!-- <template v-slot:today-button>
        <v-btn icon @click="$store.commit('SET_ACTIVE_DATE', $moment())">
          <v-icon>my_location</v-icon>
        </v-btn>
      </template> -->
    </vue-cal>

    <v-menu v-model="showMenu" :position-x="x" :position-y="y" absolute offset-y>
      <v-list dense>
        <v-list-tile to="/clients/new_session">
          <v-list-tile-action>
            <v-icon medium>fa-file-medical</v-icon>
          </v-list-tile-action>
          <v-list-tile-content>
            <v-list-tile-title>New Session</v-list-tile-title>
          </v-list-tile-content>
        </v-list-tile>
      </v-list>
    </v-menu>
  </v-card>
</template>

<script>
import sessionServicesApi from "@/services/api/SessionServices";
import VueCal from "vue-cal";
import "vue-cal/dist/vuecal.css";

export default {
  components: { VueCal },

  props: {
    clientId: {
      type: [Number, String],
      required: true
    },
    selectedDate: {
      type: [Object, String],
      required: true
    }
  },

  computed: {
    user() {
      return this.$store.getters.user;
    }
  },

  data() {
    return {
      loading: false,
      events: [],
      x: 0,
      y: 0,
      showMenu: false,
      view: null
    };
  },

  watch: {
    clientId() {
      this.loadSessions();
    }
  },

  mounted() {
    this.loadSessions();
  },

  methods: {
    async loadSessions() {
      this.events = [];
      this.loading = true;
      const events = await sessionServicesApi.getSessionsCalendar3(this.clientId);
      events.sessions.forEach(event => {
        const sessionStart = this.$moment(event.sessionStart)
          .local()
          .format("YYYY-MM-DD HH:mm");
        const sessionEnd = this.$moment(event.sessionEnd)
          .local()
          .format("YYYY-MM-DD HH:mm");
        event.start = sessionStart;
        event.end = sessionEnd;
        this.events.push(event);
      });
      events.caregivers.forEach(event => {
        event.start = event.date;
        event.end = event.date;
        event.allDay = true;
        event.color = "grey darken-1";
        event.userFullname = event.title;
        this.events.push(event);
      });
      this.loading = false;
    },

    cellClick(e) {
      if (e) this.$store.commit("SET_ACTIVE_DATE", this.$moment(e.date || e));
    },

    cellDbClick() {
      if (this.view != "month") this.showMenu = true;
    },

    viewChange(e) {
      this.view = e.view;
    },

    sessionDetails(session) {
      if (this.user.rol2 === "tech" && parseInt(this.user.id) !== session.user.userId) return;
      if (this.user.rol2 === "assistant" && session.user.rol.rolShortName === "analyst") return;
      this.$store.commit("SET_ACTIVE_SESSION", session.sessionId);
      this.$store.commit("SET_ACTIVE_DATE", this.$moment(session.sessionStart));
      this.$router.push("/clients/session_notes");
    },

    sessionData(session) {
      if (this.user.rol2 === "tech" && parseInt(this.user.id) !== session.user.userId) return;
      if (this.user.rol2 === "assistant" && session.user.rol.rolShortName === "analyst") return;
      this.$store.commit("SET_ACTIVE_SESSION", session.sessionId);
      this.$store.commit("SET_ACTIVE_DATE", this.$moment(session.sessionStart));
      this.$router.push("/session/session_collect_data");
    },

    sessionPrint(session) {
      if (this.user.rol2 === "tech" && parseInt(this.user.id) !== session.user.userId) return;
      if (this.user.rol2 === "assistant" && session.user.rol.rolShortName === "analyst") return;
      this.$store.commit("SET_ACTIVE_SESSION", session.sessionId);
      this.$store.commit("SET_ACTIVE_DATE", this.$moment(session.sessionStart));
      this.$router.push("/session/session_print");
    },

    openCaregiverCollect(event) {
      if (this.user.rol2 !== "analyst") return;
      this.$store.commit("SET_ACTIVE_DATE", this.$moment(event.start));
      this.$router.push("/clients/caregiver_data_collection");
    },

    getCursor(e) {
      this.x = e.clientX;
      this.y = e.clientY;
    }
  }
};
</script>