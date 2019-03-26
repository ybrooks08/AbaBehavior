<template>
  <v-container fluid grid-list-md pa-0>
    <v-layout row wrap>
      <v-flex xs12>
        <session-list></session-list>
      </v-flex>
    </v-layout>
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
    <v-layout row wrap>
      <v-flex xs12>
        <expiring-documents/>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import ExpiringReferrals from '@/components/clients/ExpiringReferrals';
import ExpiringAssessments from '@/components/clients/ExpiringAssessments';
import ExpiringDocuments from '@/components/users/ExpiringDocuments';
import sessionList from '@/components/sessions/SessionList';

export default {
  components: {
    ExpiringReferrals,
    ExpiringAssessments,
    ExpiringDocuments,
    sessionList,
  },
};
</script>
