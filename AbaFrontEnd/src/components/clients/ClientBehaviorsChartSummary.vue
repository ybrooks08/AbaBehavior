<template>
  <v-card>
    <v-toolbar dark class="secondary" fluid dense>
      <v-toolbar-title>Client Behaviors Charts</v-toolbar-title>
    </v-toolbar>
    <v-card-text class="pa-0" :key="clientId">
      <v-tabs>
        <v-tab v-for="b in clientProblems" :key="'beh' + b.clientProblemId" ripple>
          <span v-if="b.problemBehavior.problemBehaviorDescription.length <= 20"> {{ b.problemBehavior.problemBehaviorDescription }}</span>
          <span v-else>{{ b.problemBehavior.problemBehaviorDescription.substr(0, 20) }}...</span>
          <!-- {{ b.problemBehavior.problemBehaviorDescription.substr(0, 20) }} -->
        </v-tab>
        <v-tab-item v-for="b in clientProblems" :key="'beh' + b.clientProblemId">
          <behavior-monthly-chart :problemId="b.problemId" :clientId="b.clientId" :clientProblemId="b.clientProblemId" />
        </v-tab-item>
      </v-tabs>
    </v-card-text>
  </v-card>
</template>

<script>
import clientApi from "@/services/api/ClientServices";
import BehaviorMonthlyChart from "@/components/shared/charts/BehaviorMonthlyChart";

export default {
  props: {
    clientId: {
      type: [Number, String],
      required: true
    }
  },

  components: {
    BehaviorMonthlyChart
  },

  data() {
    return {
      clientProblems: []
      // clientReplacements: []
    };
  },

  watch: {
    clientId() {
      this.loadClientProblems();
      // this.loadClientReplacements();
    }
  },

  mounted() {
    this.loadClientProblems();
    // this.loadClientReplacements();
  },

  methods: {
    async loadClientProblems() {
      try {
        this.clientProblems = [];
        this.clientProblems = await clientApi.getClientProblems(this.clientId);
        // console.log(this.clientProblems);
      } catch (error) {
        this.$toast.error(error);
      }
    }

    // async loadClientReplacements() {
    //   try {
    //     this.clientReplacements = await clientApi.getClientReplacements(this.clientId);
    //   } catch (error) {
    //     this.$toast.error(error);
    //   }
    // }
  }
};
</script>

<style></style>
