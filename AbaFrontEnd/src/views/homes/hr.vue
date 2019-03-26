<template>
  <v-container fluid grid-list-xs pa-0>
    <v-layout row wrap>
      <v-flex xs12>
        <expiring-documents/>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import ExpiringDocuments from '@/components/users/ExpiringDocuments';

export default {
  components: {
    ExpiringDocuments
  },

}
</script>
