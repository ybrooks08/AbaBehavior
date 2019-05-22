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
      <v-subheader>Reports</v-subheader>
      <v-list-tile to="/reporting/ready-to-bill" active-class="blue lighten-4">
        <v-list-tile-action>
          <v-icon>fa-funnel-dollar</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Ready to bill</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/reporting/billing_guide">
        <v-list-tile-action>
          <v-icon>fa-file-invoice-dollar</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Billing guide</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/reporting/sessions_by_users">
        <v-list-tile-action>
          <v-icon>fa-user</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Billing by User</v-list-tile-title>
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