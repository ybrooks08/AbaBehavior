<template>
  <div>
    <v-list dense>
      <v-list-tile to="/home">
        <v-list-tile-action>
          <v-icon>fa-home</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Home</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/clients">
        <v-list-tile-action>
          <v-icon>fa-user-friends</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Clients</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-divider />
      <v-list-tile to="/diagnosis">
        <v-list-tile-action>
          <v-icon>fa-stethoscope</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Diagnosis</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-subheader>Reports</v-subheader>
      <v-list-tile to="/reporting/rbt_ba_services_log" active-class="blue lighten-4">
        <v-list-tile-action>
          <v-icon>fa-clipboard-list</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Services Logs</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
    </v-list>
  </div>
</template>
