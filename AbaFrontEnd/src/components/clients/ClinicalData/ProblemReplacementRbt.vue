<template>
  <v-layout row wrap>
    <v-flex xs12 md6>
      <v-list dense subheader>
        <v-list-tile v-for="p in clientProblems" :key="p.clientProblemId">
          <v-list-tile-avatar>
            <v-icon>fa-frown</v-icon>
          </v-list-tile-avatar>
          <v-list-tile-content>
            <v-list-tile-title class="body-2">
              {{p.problemBehavior.problemBehaviorDescription}} &nbsp;&nbsp;&nbsp;

              <v-menu open-on-hover offset-y>
                <template v-slot:activator="{ on }">
                  <v-chip v-on="on" style="max-height: 18px; cursor: pointer;" class="pa-0 ma-0" label small color="blue-grey lighten-4">{{p.stOs.length}} STO</v-chip>
                </template>

                <v-list dense subheader>
                  <v-list-tile v-for="(i, index) in p.stOs" :key="i.clientProblemStoId">
                    <v-list-tile-avatar>
                      <v-avatar size="32" color="secondary">
                        <span class="white--text headline">{{index + 1}}</span>
                      </v-avatar>
                    </v-list-tile-avatar>
                    <v-list-tile-content>
                      <v-list-tile-title class="body-2">
                        Get
                        <span class="purple--text font-weight-black">{{i.quantity}} or less</span> in
                        <span class="purple--text font-weight-black">{{i.weeks}}</span> consecutive week(s)
                      </v-list-tile-title>
                      <!--                      <v-list-tile-sub-title>-->
                      <!--                        Status:-->
                      <!--                        <strong :class="i.status.toLowerCase() == 'failed' ? 'red&#45;&#45;text': i.status.toLowerCase() == 'success' ? 'green&#45;&#45;text':''">{{i.status}}</strong>-->
                      <!--                      </v-list-tile-sub-title>-->
                    </v-list-tile-content>
                  </v-list-tile>
                </v-list>
              </v-menu>

            </v-list-tile-title>
            <v-list-tile-sub-title>
              Baseline: {{p.baselineCount || "N/A"}}
              <span v-if="p.baselineFrom">From {{p.baselineFrom | moment("MM/DD/YYYY")}}</span>&nbsp;
              <span v-if="p.baselineTo">to {{p.baselineTo | moment("MM/DD/YYYY")}}</span>
            </v-list-tile-sub-title>
          </v-list-tile-content>
        </v-list-tile>
      </v-list>
    </v-flex>

    <v-flex xs12 md6>
      <v-list dense subheader>
        <v-list-tile v-for="p in clientReplacements" :key="p.clientReplacementId">
          <v-list-tile-avatar>
            <v-icon>fa-registered</v-icon>
          </v-list-tile-avatar>
          <v-list-tile-content>
            <v-list-tile-title class="body-2">
              {{p.replacement.replacementProgramDescription}} &nbsp;&nbsp;&nbsp;

              <v-menu open-on-hover offset-y>
                <template v-slot:activator="{ on }">
                  <v-chip v-on="on" style="max-height: 18px;" class="pa-0 ma-0" label small color="blue-grey lighten-4">{{p.stOs.length}} STO</v-chip>
                </template>

                <v-list dense subheader>
                  <v-list-tile v-for="(i, index) in p.stOs" :key="i.clientReplacementStoId">
                    <v-list-tile-avatar>
                      <v-avatar size="32" color="secondary">
                        <span class="white--text headline">{{index + 1}}</span>
                      </v-avatar>
                    </v-list-tile-avatar>
                    <v-list-tile-content>
                      <v-list-tile-title class="body-2">
                        Get
                        <span class="purple--text font-weight-black">{{i.percent}} or more</span> in
                        <span class="purple--text font-weight-black">{{i.weeks}}</span> consecutive week(s)
                      </v-list-tile-title>
                      <!--                      <v-list-tile-sub-title>-->
                      <!--                        Status:-->
                      <!--                        <strong :class="p.status.toLowerCase() == 'failed' ? 'red&#45;&#45;text': p.status.toLowerCase() == 'success' ? 'green&#45;&#45;text':''">{{p.status}}</strong>-->
                      <!--                        &nbsp;&nbsp;&nbsp;-->
                      <!--                        <small>{{p.weekStart | moment('MM/DD/YYYY')}} - {{p.weekEnd | moment('MM/DD/YYYY')}}</small>-->
                      <!--                      </v-list-tile-sub-title>-->
                    </v-list-tile-content>
                  </v-list-tile>
                </v-list>
              </v-menu>

            </v-list-tile-title>
            <v-list-tile-sub-title>
              Baseline: {{p.baselinePercent || "N/A"}}
              <span v-if="p.baselineFrom">From {{p.baselineFrom | moment("MM/DD/YYYY")}}</span>&nbsp;
              <span v-if="p.baselineTo">to {{p.baselineTo | moment("MM/DD/YYYY")}}</span>
            </v-list-tile-sub-title>
          </v-list-tile-content>
        </v-list-tile>
      </v-list>
    </v-flex>
  </v-layout>
</template>

<script>
import clientApi from "@/services/api/ClientServices";

export default {
  name: "ProblemReplacementRbt",

  props: {
    clientId: {
      type: [Number, String],
      required: true
    }
  },

  data() {
    return {
      clientProblems: [],
      clientReplacements: []
    };
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

<style scoped>
</style>
