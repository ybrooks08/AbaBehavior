<template>
  <v-card flat>
    <v-card-text class="pa-0">
      <v-card-title class="pa-1">
        <div class="text-xs-center mx-auto">
          <h3 class="body-2 mb-0">{{clientReplacement.replacement.replacementProgramDescription}}</h3>
          <v-icon v-if="loading" style="position: absolute; top:0; left:0">fa-cog fa-spin</v-icon>
          <v-sparkline v-if="value" :smooth="16" :gradient="['#1feaea', '#ffd200', '#f72047']" :line-width="2" :value="value" auto-draw stroke-linecap="round" height="30"></v-sparkline>
          <small class="text-truncate blue--text font-weight-medium">Current: {{sto.currentSto == null ? 'N/A' : `STO ${sto.currentSto.index} | ${sto.currentSto.percent}% in ${sto.currentSto.weeks} weeks`}}</small><br>
          <small class="text-truncate green--text font-weight-medium">Last Mastered: {{sto.lastMasteredSto == null ? 'N/A' : `STO ${sto.lastMasteredSto.index} | ${sto.lastMasteredSto.percent} in ${sto.lastMasteredSto.weeks} weeks`}}</small>
        </div>
      </v-card-title>
      <v-divider></v-divider>
    </v-card-text>
  </v-card>
</template>

<script>
// import clientApi from "@/services/api/ClientServices";
import sessionServicesApi from "@/services/api/SessionServices";

export default {
  props: {
    clientReplacement: {
      type: Object,
      required: true
    }
  },

  data() {
    return {
      value: null,
      loading: false,
      sto: {
        currentSto: null,
        lastMasteredSto: null
      }
    };
  },

  mounted() {
    this.loadClientReplacements();
  },

  methods: {
    async loadClientReplacements() {
      try {
        this.loading = true;
        const data = await sessionServicesApi.getClientReplacementChartValuesWeek(this.clientReplacement.clientId, this.clientReplacement.replacementId);
        this.value = data.chartData;
        this.sto = data.stoStatus;
      } catch (error) {
        console.error(error);
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>

<style>
</style>