<template>
  <v-card flat>
    <!-- <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear> -->
    <v-card-text class="pa-0">
      <v-card-title class="pa-1">
        <div class="text-xs-center mx-auto">
          <h3 class="body-2 mb-0">{{clientProblem.problemBehavior.problemBehaviorDescription}}</h3>
          <v-icon v-if="loading" style="position: absolute; top:0; left:0">fa-cog fa-spin</v-icon>
          <!-- <v-sheet class="mx-auto" color="green lighten-5" elevation="1" max-width="calc(100% - 32px)"> -->
          <v-sparkline v-if="value" :smooth="16" :gradient="['#f72047', '#ffd200', '#1feaea']" :line-width="2" :value="value" auto-draw stroke-linecap="round" height="30"></v-sparkline>
          <!-- </v-sheet> -->
          <small class="text-truncate blue--text font-weight-medium">Current: {{sto.currentSto == null ? 'N/A' : `STO ${sto.currentSto.index} | ${sto.currentSto.quantity} in ${sto.currentSto.weeks} weeks`}}</small><br>
          <small class="text-truncate green--text font-weight-medium">Last Mastered: {{sto.lastMasteredSto == null ? 'N/A' : `STO ${sto.lastMasteredSto.index} | ${sto.lastMasteredSto.quantity} in ${sto.lastMasteredSto.weeks} weeks`}}</small>
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
    clientProblem: {
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
    this.loadClientProblems();
  },

  methods: {
    async loadClientProblems() {
      this.loading = true;
      try {
        const data = await sessionServicesApi.getClientBehaviorChartValuesWeek(this.clientProblem.clientId, this.clientProblem.problemId);
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