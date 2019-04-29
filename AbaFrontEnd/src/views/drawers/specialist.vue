<template>
  <div>
    <v-container fluid grid-list-md pa-0></v-container>
    <v-list class="pa-0">
      <v-divider />
      <v-list-tile to="/home" active-class="blue lighten-4">
        <v-list-tile-action>
          <v-icon>fa-home</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Home</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-divider />
      <template v-if="activeClients.length > 0">
        <v-subheader>Assigned clients</v-subheader>
        <template v-for="client in activeClients">
          <v-list-tile :class="{'red lighten-5' : !client.active, 'blue lighten-4' : activeClientId === client.clientId}" :key="client.assignmentId" @click="setActiveClient(client)">
            <v-list-tile-action>
              <v-avatar size="32">
                <img :src="`images/${client.gender ? client.gender.toLowerCase() : 'nogender'}.png`">
              </v-avatar>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>{{client.clientName}}</v-list-tile-title>
              <v-list-tile-sub-title>{{client.clientCode}}</v-list-tile-sub-title>
            </v-list-tile-content>
            <v-list-tile-action>
              <v-btn icon ripple @click.stop="clientDetails(client)">
                <v-icon color="primary">fa-info-circle</v-icon>
              </v-btn>
            </v-list-tile-action>
          </v-list-tile>
          <v-divider :key="'d-'+client.assignmentId" />
        </template>
      </template>
      <template v-if="inactiveClients.length > 0">
        <v-subheader>Assigned clients (inactive)</v-subheader>
        <template v-for="client in inactiveClients">
          <v-list-tile :class="{'red lighten-5' : !client.active, 'blue lighten-4' : activeClientId === client.clientId}" :key="client.assignmentId" @click="setActiveClient(client)">
            <v-list-tile-action>
              <v-avatar size="32">
                <img :src="`images/${client.gender ? client.gender.toLowerCase() : 'nogender'}.png`">
              </v-avatar>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>{{client.clientName}}</v-list-tile-title>
              <v-list-tile-sub-title>{{client.clientCode}}</v-list-tile-sub-title>
            </v-list-tile-content>
            <v-list-tile-action v-if="client.active">
              <v-btn icon ripple @click.stop="clientDetails(client)">
                <v-icon color="primary">fa-info-circle</v-icon>
              </v-btn>
            </v-list-tile-action>
          </v-list-tile>
          <v-divider :key="'d-'+client.assignmentId" />
        </template>
      </template>

      <template v-if="disabledClients.length > 0 && false">
        <v-list-group no-action>
          <v-list-tile slot="activator">
            <v-list-tile-content>
              <v-list-tile-title>Inactive or discharged clients</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
          <template v-for="client in disabledClients">
            <v-list-tile :class="{'blue lighten-4' : activeClientId === client.clientId}" :key="client.assignmentId" @click="setActiveClient(client)" class="no-left-padding">
              <v-list-tile-action>
                <v-avatar size="32">
                  <img style="opacity: 0.5;" :src="`images/${client.gender ? client.gender.toLowerCase() : 'nogender'}.png`">
                </v-avatar>
              </v-list-tile-action>
              <v-list-tile-content>
                <v-list-tile-title class="grey--text text">{{client.clientName}}</v-list-tile-title>
                <v-list-tile-sub-title class="grey--text">{{client.clientCode}}</v-list-tile-sub-title>
              </v-list-tile-content>
              <v-list-tile-action v-if="client.active">
                <v-btn icon ripple @click.stop="clientDetails(client)">
                  <v-icon color="primary">fa-info-circle</v-icon>
                </v-btn>
              </v-list-tile-action>
            </v-list-tile>
            <v-divider :key="'d-'+client.assignmentId" />
          </template>
        </v-list-group>
      </template>

      <v-subheader>Reports</v-subheader>
      <v-list-tile to="/reporting/rbt_ba_services_log" active-class="blue lighten-4">
        <v-list-tile-action>
          <v-icon>fa-clipboard-list</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Billing report</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/reporting/sessions-history" active-class="blue lighten-4">
        <v-list-tile-action>
          <v-icon>fa-history</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Progress Notes</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/reporting/time-sheet" active-class="blue lighten-4">
        <v-list-tile-action>
          <v-icon>fa-calendar-week</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Time sheet</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile v-if="isLead" to="/reporting/monthly-notes" active-class="blue lighten-4">
        <v-list-tile-action>
          <v-icon>fa-calendar-alt</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Monthly notes</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-subheader>Documents and resources</v-subheader>
      <v-list-tile to="/video_tutorials">
        <v-list-tile-action>
          <v-icon>fa-file-video</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Video tutorials</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
    </v-list>
  </div>
</template>

<script>
import userApi from "@/services/api/UserServices";

export default {
  data() {
    return {
      loadingClients: false,
      clients: []
    };
  },

  computed: {
    activeClientId() {
      return this.$store.getters.activeClientId;
    },
    activeClients() {
      return this.clients.filter(s => s.active && s.clientActive);
    },
    inactiveClients() {
      return this.clients.filter(s => !s.active && s.clientActive);
    },
    disabledClients() {
      return this.clients.filter(s => !s.clientActive);
    },
    isLead() {
      return this.$store.getters.user.rol2 === "analyst";
    }
  },

  mounted() {
    this.loadUserClients();
  },

  methods: {
    async loadUserClients() {
      try {
        this.loadingClients = true;
        this.clients = await userApi.loadUserClients();
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loadingClients = false;
      }
    },

    async setActiveClientNoRoute(clientId) {
      this.$store.commit("SET_ACTIVE_CLIENT", clientId);
    },

    async setActiveClient(client) {
      if (!client.active) return;
      this.$store.commit("SET_ACTIVE_CLIENT", client.clientId);
      this.$router.push("/clients/sessions_details");
    },

    clientDetails(client) {
      if (!client.active) return;
      this.setActiveClientNoRoute(client.clientId);
      this.$router.push("/clients/client_details/" + client.clientId);
    }
  }
};
</script>

<style scoped>
.no-left-padding >>> a {
  padding-left: 16px !important;
}
</style>