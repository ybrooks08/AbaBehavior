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
                <date-picker-menu :isLarge="true" :isDark="false" :btnColor="'primary'" :disabled="loading" v-model="datePickerModel" />
              </v-flex>
              <v-flex md12>
                <v-autocomplete box hide-details :disabled="loading" :items="clients" v-model="clientId" label="Client" prepend-icon="fa-user" item-text="clientName" item-value="clientId" :rules="[required]" required>
                  <template slot="item" slot-scope="{ item }">
                    <v-list-tile-avatar>
                      <img :style="!item.active ? 'opacity: 0.5' : ''" :src="`images/${item.gender ? item.gender.toLowerCase() : 'nogender'}.png`" />
                    </v-list-tile-avatar>
                    <v-list-tile-content>
                      <v-list-tile-title :class="{ 'grey--text text--lighten-1': !item.active }">{{ item.clientName }}</v-list-tile-title>
                      <v-list-tile-sub-title :class="{ 'grey--text text--lighten-1': !item.active }">{{ item.dob | moment("utc", "MM/DD/YYYY") }} | Code: {{ item.clientCode || "N/A" }}</v-list-tile-sub-title>
                    </v-list-tile-content>
                  </template>
                </v-autocomplete>
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
    <v-flex v-if="sessionsId.length > 0">
      <v-select box :disabled="loading" :items="users" v-model="userSelected" label="Filter by provider" class="no-print" />
      <div v-for="sessionId in sessionsId" :key="sessionId" class="pagebreak-before mb-2">
        <session-print :sessionId="sessionId"></session-print>
      </div>
    </v-flex>
  </v-layout>
</template>

<script>
import userApi from "@/services/api/UserServices";
import reportingApi from "@/services/api/ReportingServices";
import SessionPrint from "@/components/shared/SessionPrint";

export default {
  name: "PrintSessions",

  components: {
    SessionPrint
  },

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
      clients: [],
      clientId: null,
      sessions: [],
      users: [],
      userSelected: null
    };
  },

  computed: {
    user() {
      return this.$store.getters.user;
    },
    isAdminOrManagement() {
      return this.user.rol2 === "admin" || this.user.rol2 === "management";
    },
    sessionsId() {
      console.log(this.userSelected);
      let ids = this.userSelected === "All" ? this.sessions.map(s => s.sessionId) : this.sessions.filter(f => f.userFullname === this.userSelected).map(s => s.sessionId);
      return ids;
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
      } finally {
        this.loading = false;
      }
    },

    async viewReport() {
      try {
        this.loading = true;
        this.sessions = [];
        this.users = [];
        this.sessions = await reportingApi.getSessionsHistory(this.datePickerModel.start, this.datePickerModel.end, this.clientId);
        if (this.sessions.length === 0) {
          this.$toast.info("No data");
          return;
        }
        this.users = [...new Set(this.sessions.map(s => s.userFullname))];
        this.users.unshift("All");
        this.userSelected = "All";
        // sessions.forEach(e => {
        //   this.sessionsId.push(e.sessionId);
        // });
      } catch (error) {
        this.$toast.error(error.response.data || error.message);
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>

<style scoped></style>
