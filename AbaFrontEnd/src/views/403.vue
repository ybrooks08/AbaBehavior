<template>
  <v-container fluid>
    <v-alert error value="true">
      <strong>Unauthorized</strong> You are not authorize to view this page
    </v-alert>
    <v-btn flat @click="$router.push('/')">Go to Dashboard</v-btn>
  </v-container>
</template>
