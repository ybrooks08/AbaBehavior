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
      <v-list-tile to="/users">
        <v-list-tile-action>
          <v-icon>fa-users</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Employees (users)</v-list-tile-title>
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
      <v-subheader>Master tables</v-subheader>
      <v-list-tile to="/diagnosis">
        <v-list-tile-action>
          <v-icon>fa-stethoscope</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Diagnosis</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/problems-behaviors">
        <v-list-tile-action>
          <v-icon>fa-frown</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Problems behaviors</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/replacement-program">
        <v-list-tile-action>
          <v-icon>fa-registered</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Replacement program</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/documents_setup">
        <v-list-tile-action>
          <v-icon>fa-certificate</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Documents</v-list-tile-title>
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
      <v-list-tile to="/reporting/rbt_ba_services_log" active-class="blue lighten-4">
        <v-list-tile-action>
          <v-icon>fa-clipboard-list</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Services Logs</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/reporting/sessions-history" active-class="blue lighten-4">
        <v-list-tile-action>
          <v-icon>fa-history</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Sessions history</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/reporting/client_progress" active-class="blue lighten-4">
        <v-list-tile-action>
          <v-icon>fa-chart-line</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Client progress</v-list-tile-title>
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
      <v-list-tile to="/competency_checks" active-class="blue lighten-4">
        <v-list-tile-action>
          <v-icon>fa-calendar-check</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Competency checks</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/reporting/monthly-notes" active-class="blue lighten-4">
        <v-list-tile-action>
          <v-icon>fa-calendar-alt</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Monthly notes</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/reporting/monthly-week-data" active-class="blue lighten-4">
        <v-list-tile-action>
          <v-icon>fa-table</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Monthly/Week data</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/reporting/caregiver-collections" active-class="blue lighten-4">
        <v-list-tile-action>
          <v-icon>fa-user-shield</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Caregiver data collections</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/reporting/print-sessions" active-class="blue lighten-4">
        <v-list-tile-action>
          <v-icon>fa-print</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Print Sessions</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/reporting/matching-report" active-class="blue lighten-4">
        <v-list-tile-action>
          <v-icon>fa-clone</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Matching report</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-subheader>Tools</v-subheader>
      <v-list-tile to="/reporting/adjust-session-analyst">
        <v-list-tile-action>
          <v-icon>fa-people-arrows</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Adjust sessions analysts</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/adjust-client-data-collect">
        <v-list-tile-action>
          <v-icon>fa-sliders-h</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Adjust client data collect</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/reporting/staff-client">
        <v-list-tile-action>
          <v-icon>fa-link</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Staff and Clients relationship</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
      <v-list-tile to="/system_logs">
        <v-list-tile-action>
          <v-icon>fa-list-alt</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>System Logs</v-list-tile-title>
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
