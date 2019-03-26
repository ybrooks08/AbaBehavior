<template>
  <v-container fluid grid-list-xs pa-0>
    <v-layout row wrap>
      <v-flex xs12>
        <expiring-referrals/>
      </v-flex>
    </v-layout>
    <v-layout row wrap>
      <v-flex xs12>
        <expiring-assessments/>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import ExpiringReferrals from '@/components/clients/ExpiringReferrals';
import ExpiringAssessments from '@/components/clients/ExpiringAssessments';

export default {
  components: {
    ExpiringReferrals,
    ExpiringAssessments
  },
}
</script>
