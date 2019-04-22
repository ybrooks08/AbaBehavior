<template>
  <v-layout row wrap>
    <table style="width: 100%">
      <tr v-for="p in clientBehaviors" :key="'p'+p.clientProblemId" class="no-page-break">
        <v-subheader>{{p.problemBehavior.problemBehaviorDescription}}</v-subheader>
        <client-progress-behavior :behaviorId="p.problemId"></client-progress-behavior>
      </tr>
      <tr v-for="p in clientReplacements" :key="'r'+p.clientReplacementId" class="no-page-break">
        <v-subheader>{{p.replacement.replacementProgramDescription}}</v-subheader>
        <client-progress-replacement :replacementId="p.replacementId"></client-progress-replacement>
      </tr>
    </table>
  </v-layout>
</template>

<script>
import sessionServicesApi from "@/services/api/SessionServices";
import ClientProgressBehavior from "@/components/sessions/ProgressBehavior";
import ClientProgressReplacement from "@/components/sessions/ProgressReplacement";

export default {
  name: "ClientProgressPrintVersion",

  components: {
    ClientProgressBehavior,
    ClientProgressReplacement
  },

  data() {
    return {
      clientBehaviors: [],
      clientReplacements: []
    };
  },

  created() {
    this.loadData();
  },

  computed: {
    activeClientId() {
      return this.$store.getters.activeClientId;
    }
  },

  methods: {
    async loadData() {
      this.clientBehaviors = [];
      this.loading = true;
      try {
        this.clientBehaviors = await sessionServicesApi.getClientBehaviors(this.activeClientId);
        this.clientReplacements = await sessionServicesApi.getClientReplacements(this.activeClientId);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>

<style scoped>
</style>
