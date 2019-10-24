<template>
  <v-card @contextmenu="getCursor">
    <v-toolbar dark class="secondary" fluid dense>
      <v-toolbar-title>Calendar week</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-toolbar-title class="body-2 grey--text">Active date:</v-toolbar-title>
      <v-menu>
        <v-btn flat slot="activator" :disabled="loading">{{ today | moment("MM/DD/YYYY") }}</v-btn>
        <v-date-picker v-model="today" @input="changeDatePicker"></v-date-picker>
      </v-menu>
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
              <v-list-tile-title>New Session (Training to BCaBA)</v-list-tile-title>
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

    <v-card-text>
      <v-sheet>
        <v-calendar ref="calendar" v-model="start" :start="start" :end="end" type="week" color="primary" :now="today" :value="today" first-interval="8" interval-count="15" @contextmenu:time="newMenu" @click:date="clickDate">
          <template v-slot:dayHeader="{ date }">
            <template v-for="event in eventsMapHeader[date]">
              <!-- all day events don't have time -->
              <div :key="event.caregiverDataCollectionId" class="purple text-xs-center" style="cursor: pointer;" @click="openCaregiverCollect(event)">
                <v-icon color="white" class="hidden-sm-and-down">fa-sm fa-user-shield</v-icon>&nbsp;
                <small class="condensed white--text text-truncate text-no-wrap">{{ event.title }}</small>
              </div>
            </template>
          </template>
          <template slot="dayBody" slot-scope="{ date, timeToY, minutesToPixels }">
            <template v-for="event in eventsMap[date]">
              <v-menu :key="'menu' + event.sessionId" v-model="event.open" full-width>
                <div slot="activator" :key="event.sessionId" :style="{ top: timeToY(event.time) + minutesToPixels(event.timeMinutes) + 'px', height: minutesToPixels(event.duration) + 'px' }" class="my-event with-time" :class="event.sessionStatusColor">
                  <v-card-text class="pa-1 white--text">
                    <v-icon color="white">{{ event.sessionType === "ba_service" ? "fa-file-medical" : event.sessionType === "training_bcaba" ? "fa-file-alt" : "fa-user-shield" }}</v-icon>
                    <span class="body-2">&nbsp;{{ event.title }}</span>
                    <v-divider class="ma-1"></v-divider>
                    <template v-if="event.sessionType === 'ba_service'">
                      <v-icon class="white--text" small>fa-star</v-icon>
                      {{ event.totalUnits }}&nbsp;
                    </template>
                    <v-icon class="white--text" small>fa-map-marker-alt</v-icon>
                    <span class="text-truncate mr-5">{{ event.pos }}</span>
                  </v-card-text>
                </div>
                <v-card color="grey lighten-4" min-width="350px" flat full-width>
                  <v-toolbar dense :color="event.sessionStatusColor" dark flat>
                    <v-icon>{{ event.sessionType === "ba_service" ? "fa-file-medical" : event.sessionType === "training_bcaba" ? "fa-file-alt" : "fa-user-shield" }}</v-icon>
                    <v-toolbar-title>{{ event.title }} - {{ event.rolShortName }}</v-toolbar-title>
                  </v-toolbar>
                  <v-card-text>
                    <v-icon small>fa-calendar</v-icon>&nbsp;
                    <span>{{ event.date | moment("LL") }}</span>
                    <br />
                    <v-icon color="green" small>fa-sign-in-alt</v-icon>&nbsp;
                    <span>{{ event.timeStartFormat }}</span>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <v-icon color="red" small>fa-sign-out-alt</v-icon>&nbsp;
                    <span>{{ event.timeEndFormat }}</span>
                    <br />
                    <v-icon small>{{ event.sessionType === "ba_service" ? "fa-file-medical" : event.sessionType === "training_bcaba" ? "fa-file-alt" : "fa-user-shield" }}</v-icon
                    >&nbsp;
                    <span>{{ event.sessionTypeFormated }}</span>
                    <br />
                    <v-icon small>fa-star</v-icon>&nbsp;
                    <span>{{ event.totalUnits }}</span>
                    <br />
                    <v-icon small>fa-clock</v-icon>&nbsp;
                    <span>{{ (event.totalUnits / 4).toLocaleString() }}</span>
                    <br />
                    <v-icon small>fa-map-marker-alt</v-icon>&nbsp;
                    <span>{{ event.pos }}</span>
                    <br />
                    <v-icon :color="event.sessionStatusColor" small>fa-flag</v-icon>&nbsp;
                    <span>
                      <v-chip label small :color="event.sessionStatusColor" text-color="white">{{ event.sessionStatus }}</v-chip>
                    </span>
                    <br />
                  </v-card-text>
                  <v-card-actions class="grey lighten-2">
                    <v-btn color="secondary" @click="sessionDetails(event)">Notes</v-btn>
                    <v-btn v-if="event.sessionType === 'ba_service'" color="secondary" @click="sessionData(event)">Data</v-btn>
                  </v-card-actions>
                </v-card>
              </v-menu>
            </template>
          </template>
        </v-calendar>
      </v-sheet>
    </v-card-text>
    <v-card-actions>
      <v-btn color=" primary" @click="prevWeek">
        <v-icon dark left>fa-chevron-circle-left</v-icon>
        Prev week
      </v-btn>
      <v-spacer></v-spacer>
      <v-btn color="primary" @click="nextWeek()">
        Next week
        <v-icon dark right>fa-chevron-circle-right</v-icon>
      </v-btn>
    </v-card-actions>

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

export default {
  name: "CalendarSessions",

  computed: {
    activeClientId() {
      return this.$store.getters.activeClientId;
    },
    user() {
      return this.$store.getters.user;
    },
    eventsMap() {
      const map = {};
      this.events.forEach(e => (map[e.date] = map[e.date] || []).push(e));
      return map;
    },
    eventsMapHeader() {
      const map = {};
      this.caregiversCollectionDataEvents.forEach(e => (map[e.date] = map[e.date] || []).push(e));
      return map;
    }
  },

  watch: {
    activeClientId() {
      this.loadSessions();
    }
  },

  data() {
    return {
      loading: false,
      showMenu: false,
      x: 0,
      y: 0,
      today: this.$moment().format("YYYY-MM-DD hh:mm"),
      start: this.$moment()
        .startOf("isoWeek")
        .format("YYYY-MM-DD"),
      end: this.$moment()
        .endOf("isoWeek")
        .format("YYYY-MM-DDTHH:mm"),
      events: [],
      caregiversCollectionDataEvents: [],
      activeDate: {}
    };
  },

  mounted() {
    const activeDate = this.$store.getters.activeDate;
    this.today = activeDate.format("YYYY-MM-DD hh:mm");
    this.start = this.$moment(this.today)
      .startOf("isoWeek")
      .format("YYYY-MM-DD");
    this.end = this.$moment(this.today)
      .endOf("isoWeek")
      .format("YYYY-MM-DD");
    this.loadSessions();
    //this.$refs.calendar.scrollToTime('08:00');
  },

  methods: {
    async loadSessions() {
      this.events = [];
      this.loading = true;
      const events = await sessionServicesApi.getSessionsCalendar2(this.activeClientId);
      events.forEach(event => {
        const sessionStart = this.$moment(event.sessionStart).local();
        const sessionEnd = this.$moment(event.sessionEnd).local();
        event.sessionStart = sessionStart;
        event.sessionEnd = sessionEnd;
        event.duration = sessionEnd.diff(sessionStart, "minutes");
        event.date = sessionStart.format("YYYY-MM-DD");
        event.time = sessionStart.format("HH");
        event.timeMinutes = sessionStart.format("mm");
        event.timeStartFormat = sessionStart.format("LT");
        event.timeEndFormat = sessionEnd.format("LT");
        this.events.push(event);
      });
      this.$refs.calendar.scrollToTime("08:00");
      this.caregiversCollectionDataEvents = await sessionServicesApi.getCaregiverCollectionDataForCalendar(this.activeClientId);
      this.loading = false;
    },

    getCursor(e) {
      //e.preventDefault();
      this.x = e.clientX;
      this.y = e.clientY;
    },

    newMenu(date) {
      this.activeDate = new Date(`${date.date} ${date.time}`);
      this.$store.commit("SET_ACTIVE_DATE", this.$moment(this.activeDate));
      this.$nextTick(() => {
        this.showMenu = true;
      });
    },

    sessionDetails(session) {
      if (this.user.rol2 === "tech" && parseInt(this.user.id) !== session.user.userId) return;
      if (this.user.rol2 === "assistant" && session.user.rol.rolShortName === "analyst") return;
      this.$store.commit("SET_ACTIVE_SESSION", session.sessionId);
      this.$store.commit("SET_ACTIVE_DATE", this.$moment(session.date));
      this.$router.push("/clients/session_notes");
    },

    sessionData(session) {
      if (this.user.rol2 === "tech" && parseInt(this.user.id) !== session.user.userId) return;
      if (this.user.rol2 === "assistant" && session.user.rol.rolShortName === "analyst") return;
      this.$store.commit("SET_ACTIVE_SESSION", session.sessionId);
      this.$store.commit("SET_ACTIVE_DATE", this.$moment(session.date));
      this.$router.push("/session/session_collect_data");
    },

    nextWeek() {
      this.$refs.calendar.next();
      this.today = this.$moment(this.today)
        .add(7, "days")
        .format("YYYY-MM-DD");
      this.$store.commit("SET_ACTIVE_DATE", this.$moment(this.today));
    },

    prevWeek() {
      this.$refs.calendar.prev();
      this.today = this.$moment(this.today)
        .subtract(7, "days")
        .format("YYYY-MM-DD");
      this.$store.commit("SET_ACTIVE_DATE", this.$moment(this.today));
    },

    clickDate(e) {
      this.today = e.date;
      this.$store.commit("SET_ACTIVE_DATE", this.$moment(e.date));
    },

    changeDatePicker(e) {
      this.start = this.$moment(e)
        .startOf("isoWeek")
        .format("YYYY-MM-DD");
      this.end = this.$moment(e)
        .endOf("isoWeek")
        .format("YYYY-MM-DDTHH:mm");
      this.$store.commit("SET_ACTIVE_DATE", this.$moment(e));
    },

    openCaregiverCollect(event) {
      if (this.user.rol2 !== "analyst") return;
      this.$store.commit("SET_ACTIVE_DATE", this.$moment(event.date));
      this.$router.push("/clients/caregiver_data_collection");
    }
  }
};
</script>

<style scoped>
.my-event {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;

  font-size: 12px;
  padding: 3px;
  cursor: pointer;
  margin-bottom: 1px;
  left: 4px;
  margin-right: 8px;
  position: relative;
}

.with-time {
  position: absolute;
  left: 0;
  right: 0;
  margin-right: 0;
}
</style>
