<template>
  <v-layout row wrap>
    <v-flex xs12 sm6 md4 lg3 d-flex class="no-print">
      <v-select v-model="selectCount" class="no-print" :items="items" box label="Select count" @change="loadData"></v-select>
    </v-flex>

    <table style="width: 100%">
      <template v-for="p in behArray">
        <tr :key="'p'+p" class="no-page-break">
          <!-- <v-subheader>{{p.problemBehavior.problemBehaviorDescription}}</v-subheader> -->
          <v-card>
            <client-progress-behavior :dateStart="dateStart" :dateEnd="dateEnd" :behaviorId="p"></client-progress-behavior>
          </v-card>
        </tr>
        <tr :key="'p1'+p">&nbsp;</tr>
      </template>
      <template v-for="p in repArray">
        <tr :key="'r'+p" class="no-page-break">
          <!-- <v-subheader>{{p.replacement.replacementProgramDescription}}</v-subheader> -->
          <client-progress-replacement :dateStart="dateStart" :dateEnd="dateEnd" :replacementId="p"></client-progress-replacement>
        </tr>
        <tr :key="'p1'+p">&nbsp;</tr>
      </template>
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
      behIds: [],
      clientReplacements: [],
      repIds: [],
      dateStart: null,
      dateEnd: null,
      selectCount: 3,
      items: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
    };
  },

  created() {
    this.dateStart = this.$route.query.dateStart;
    this.dateEnd = this.$route.query.dateEnd;
    this.loadData();
  },

  computed: {
    activeClientId() {
      return this.$store.getters.activeClientId;
    },
    behArray() {
      return this.parseToPages(this.behIds, this.selectCount);
    },
    repArray() {
      return this.parseToPages(this.repIds, this.selectCount);
    }
  },

  methods: {
    async loadData() {
      console.log("Changed");
      this.clientBehaviors = [];
      this.behIds = [];
      this.clientReplacements = [];
      this.repIds = [];
      this.loading = true;
      try {
        this.clientBehaviors = await sessionServicesApi.getClientBehaviors(this.activeClientId);
        this.behIds = this.clientBehaviors.map(w => w.problemId);
        this.clientReplacements = await sessionServicesApi.getClientReplacements(this.activeClientId);
        this.repIds = this.clientReplacements.map(w => w.replacementId);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    parseToPages(elements, pageSize = 3) {
      let result = [];
      while (elements.length) {
        result.push(elements.splice(0, pageSize).join(","));
      }
      return result;
    }
  }
};
</script>

<style scoped>
</style>
