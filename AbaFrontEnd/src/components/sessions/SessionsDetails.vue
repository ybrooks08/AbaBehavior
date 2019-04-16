<template>
  <v-container pa-0 grid-list-md fluid>
    <v-layout row wrap>
      <v-flex xs12>
        <calendar-sessions></calendar-sessions>
      </v-flex>

      <v-flex xs12>
        <v-card>
          <v-toolbar dark class="secondary" fluid dense>
            <v-toolbar-title>Client progress</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-menu class="mr-0" bottom left :disabled="loading">
              <v-btn slot="activator" icon :disabled="loading">
                <v-icon>fa-ellipsis-v</v-icon>
              </v-btn>
              <v-list>
                <v-list-tile to="/clients/add_edit_chart_note">
                  <v-list-tile-action>
                    <v-icon medium>fa-sticky-note</v-icon>
                  </v-list-tile-action>
                  <v-list-tile-content>
                    <v-list-tile-title>New quick note</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
                <v-divider></v-divider>
                <v-list-tile to="/reporting/client_progress_print">
                  <v-list-tile-action>
                    <v-icon medium>fa-print</v-icon>
                  </v-list-tile-action>
                  <v-list-tile-content>
                    <v-list-tile-title>Print version</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
              </v-list>
              <v-divider></v-divider>
            </v-menu>
          </v-toolbar>
          <v-card-text class="pa-1">
            <v-subheader inset class="red--text">Problem behaviors</v-subheader>
            <client-progress-behavior :key="('client-problem' + activeClientId)"></client-progress-behavior>
            <v-divider></v-divider>
            <v-subheader inset class="blue--text">Replacements program</v-subheader>
            <client-progress-replacement :key="('client-replacement' + activeClientId)"></client-progress-replacement>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import ClientProgressBehavior from "@/components/sessions/ProgressBehavior";
import ClientProgressReplacement from "@/components/sessions/ProgressReplacement";
import CalendarSessions from "@/components/sessions/CalendarSessions";

export default {
  components: {
    ClientProgressBehavior,
    ClientProgressReplacement,
    CalendarSessions
  },

  data() {
    return {
      loading: false,
      whatShow: [{ value: 1, text: "Problem behaviors" }, { value: 2, text: "Replacements" }],
      whatShowItem: 1
    };
  },

  computed: {
    activeClientId() {
      return this.$store.getters.activeClientId;
    },
    activeDate() {
      return this.$store.getters.activeDate;
    },
    user() {
      return this.$store.getters.user;
    }
  }
};
</script>