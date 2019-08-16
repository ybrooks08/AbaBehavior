<template>
  <v-card>
    <v-toolbar dark class="secondary" fluid dense>
      <v-toolbar-title>Client clinical summary</v-toolbar-title>
    </v-toolbar>
    <v-card-text class="pa-0">
      <v-tabs dark slider-color="yellow">
        <v-tab key="clinicalBehaviors" ripple>
          Behaviors
        </v-tab>
        <v-tab key="clinicalReplacements" ripple>
          Replacements
        </v-tab>
        <v-tab-item key="clinicalBehaviors">
          <v-layout row wrap>
            <v-flex xs12 class="pa-0">
              <v-alert type="warning" :value="clientProblems.length == 0">
                This client do not have any active behavior
              </v-alert>
            </v-flex>
            <v-flex xs4 v-for="p in clientProblems" :key="'p'+p.clientProblemId">
              <clinical-data-summary-behavior-stats :clientProblem="p"></clinical-data-summary-behavior-stats>
            </v-flex>
          </v-layout>
        </v-tab-item>
        <v-tab-item key="clinicalReplacements">
          <v-layout row wrap>
            <v-flex xs12 class="pa-0">
              <v-alert type="warning" :value="clientProblems.length == 0">
                This client do not have any active replacement
              </v-alert>
            </v-flex>
            <v-flex xs4 v-for="p in clientReplacements" :key="'r'+p.clientReplacementId">
              <clinical-data-summary-replacement-stats :clientReplacement="p"></clinical-data-summary-replacement-stats>
            </v-flex>
          </v-layout>
        </v-tab-item>
      </v-tabs>

    </v-card-text>
  </v-card>
</template>

<script>
import clientApi from "@/services/api/ClientServices";
import ClinicalDataSummaryBehaviorStats from "@/components/clients/ClinicalData/ClinicalDataSummaryBehaviorStats";
import ClinicalDataSummaryReplacementStats from "@/components/clients/ClinicalData/ClinicalDataSummaryReplacementStats";

export default {
  props: {
    clientId: {
      type: [Number, String],
      required: true
    }
  },

  components: {
    ClinicalDataSummaryBehaviorStats,
    ClinicalDataSummaryReplacementStats
  },

  data() {
    return {
      clientProblems: [],
      clientReplacements: []
    };
  },

  watch: {
    clientId() {
      this.loadClientProblems();
      this.loadClientReplacements();
    }
  },

  mounted() {
    this.loadClientProblems();
    this.loadClientReplacements();
  },

  methods: {
    async loadClientProblems() {
      try {
        this.clientProblems = await clientApi.getClientProblems(this.clientId);
      } catch (error) {
        this.$toast.error(error);
      }
    },

    async loadClientReplacements() {
      try {
        this.clientReplacements = await clientApi.getClientReplacements(this.clientId);
      } catch (error) {
        this.$toast.error(error);
      }
    }
  }
};
</script>

<style>
</style>