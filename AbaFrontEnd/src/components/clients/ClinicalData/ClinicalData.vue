<template>
  <v-card>
    <v-toolbar dark dense fluid>
      <v-toolbar-title>Clinical data</v-toolbar-title>
    </v-toolbar>
    <v-card-text class="pa-1">
      <v-layout row wrap>
        <v-flex xs12>
          <problem-replacement-setup v-if="clinicalAutorized" :clientId="id" />
          <problem-replacement-rbt v-else :clientId="id" />
        </v-flex>
      </v-layout>
    </v-card-text>
  </v-card>
</template>

<script>
import ProblemReplacementSetup from "@/components/clients/ClinicalData/ProblemReplacementSetup";
import ProblemReplacementRbt from "@/components/clients/ClinicalData/ProblemReplacementRbt";

export default {
  props: {
    id: {
      type: [Number, String],
      required: true
    }
  },

  computed: {
    clinicalAutorized() {
      return this.$store.getters.user.rol2 === "admin" || this.$store.getters.user.rol2 === "analyst";
    }
  },

  components: {
    ProblemReplacementSetup,
    ProblemReplacementRbt
  },

  data() {
    return {
      loading: false
    };
  }
};
</script>

<style scoped>
.v-list__tile__action {
  min-width: 15px;
  padding-left: 1%;
}
</style>