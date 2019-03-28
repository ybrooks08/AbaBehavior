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
      <v-subheader>Master tables</v-subheader>
      <v-list-tile to="/documents_setup">
        <v-list-tile-action>
          <v-icon>fa-certificate</v-icon>
        </v-list-tile-action>
        <v-list-tile-content>
          <v-list-tile-title>Documents</v-list-tile-title>
        </v-list-tile-content>
      </v-list-tile>
    </v-list>
  </div>
</template>
