<template>
  <v-container fluid grid-list-md pa-0>
    <v-layout row wrap>
      <v-flex xs12>
        <session-list :showOpen="false"></session-list>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import SessionList from "@/components/sessions/SessionList";

export default {
  components: {
    SessionList
  }
};
</script>