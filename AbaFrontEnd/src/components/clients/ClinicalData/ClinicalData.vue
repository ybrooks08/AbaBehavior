<template>
  <v-card>
    <v-toolbar dark dense fluid>
      <v-toolbar-title>Clinical data</v-toolbar-title>
    </v-toolbar>
    <v-card-text class="pa-1">
      <v-layout row wrap>
        <v-flex xs12>
          <problem-replacement-setup :clientId="id"/>
        </v-flex>
      </v-layout>
    </v-card-text>
  </v-card>
</template>

<script>
import ProblemReplacementSetup from '@/components/clients/ClinicalData/ProblemReplacementSetup';

export default {
  props: {
    id: {
      type: [Number, String],
      required: true,
    },
  },

  components: {
    ProblemReplacementSetup,
  },

  data() {
    return {
      loading: false,
    };
  },

};
</script>

<style scoped>
.v-list__tile__action {
  min-width: 15px;
  padding-left: 1%;
}
</style>