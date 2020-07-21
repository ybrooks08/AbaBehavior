<template>
  <v-card>
    <v-toolbar dark class="secondary" fluid dense>
      <v-toolbar-title>Client Replacements Charts</v-toolbar-title>
    </v-toolbar>
    <v-card-text class="pa-0" :key="clientId">
      <v-tabs>
        <v-tab v-for="b in clientReplacements" :key="'replx' + b.clientReplacementId" ripple>
          <span v-if="b.replacement.replacementProgramDescription.length <= 30"> {{ b.replacement.replacementProgramDescription }}</span>
          <span v-else>{{ b.replacement.replacementProgramDescription.substr(0, 30) }}...</span>
        </v-tab>
        <v-tab-item v-for="b in clientReplacements" :key="'replx' + b.clientReplacementId" lazy>
          <replacement-monthly-chart :problemId="b.replacementId" :clientId="b.clientId" :clientReplacementId="b.clientReplacementId" />
        </v-tab-item>
      </v-tabs>
    </v-card-text>
  </v-card>
</template>

<script>
import clientApi from "@/services/api/ClientServices";
import ReplacementMonthlyChart from "@/components/shared/charts/ReplacementMonthlyChart";

export default {
  props: {
    clientId: {
      type: [Number, String],
      required: true
    }
  },

  components: {
    ReplacementMonthlyChart
  },

  data() {
    return {
      // clientProblems: []
      clientReplacements: []
    };
  },

  watch: {
    clientId() {
      // this.loadClientProblems();
      this.loadClientReplacements();
    }
  },

  mounted() {
    // this.loadClientProblems();
    this.loadClientReplacements();
  },

  methods: {
    // async loadClientProblems() {
    //   try {
    //     this.clientProblems = [];
    //     this.clientProblems = await clientApi.getClientProblems(this.clientId);
    //     console.log(this.clientProblems);
    //   } catch (error) {
    //     this.$toast.error(error);
    //   }
    // }

    async loadClientReplacements() {
      try {
        this.clientReplacements = [];
        const clientReplacements = await clientApi.getClientReplacements(this.clientId);
        this.clientReplacements = clientReplacements.filter(s => s.active === true);
        // console.log(this.clientReplacements);
      } catch (error) {
        this.$toast.error(error);
      }
    }
  }
};
</script>

<style></style>
