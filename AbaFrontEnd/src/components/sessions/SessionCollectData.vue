<template>
  <v-container pa-0 grid-list-md fluid>
    <v-layout row wrap>
      <v-flex xs12>
        <v-card>
          <v-toolbar dark class="secondary" fluid dense flat>
            <v-toolbar-title>Collect behavior data {{activeDate | moment("l")}}</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-toolbar-items>
              <v-btn flat @click="goToNotes">
                <v-icon left>fa-file-medical</v-icon>
                VIEW NOTES
              </v-btn>
            </v-toolbar-items>
          </v-toolbar>
          <v-progress-linear style="position: absolute;" v-show="loadingBehavior" :indeterminate="true" class="ma-0"></v-progress-linear>
          <v-card-text class="pa-1">
            <v-layout row wrap>
              <v-flex xs12 sm6 md4 lg3 v-for="b in collectBehaviors" :key="b.sessionCollectBehaviorId">
                <v-card flat hover :class="b.noData ? 'red lighten-4':'green lighten-5'">
                  <v-card-title primary-title class="pa-2 text-no-wrap text-truncate">
                    <div class="subheading">{{b.behavior.problemBehaviorDescription}}</div>
                  </v-card-title>
                  <v-divider></v-divider>
                  <v-card-text class="pl-2 pr-2 pt-2 pb-0">
                    <template v-if="!b.behavior.isPercent">
                      <v-text-field v-model="b.total" :disabled="b.noData" :prepend-inner-icon="b.noData ? 'fa-question-circle' : b.total == 0 ? 'fa-smile' : 'fa-frown'" box hide-actions hide-details label="Total" @change="updateBehaviorCollection(b)" :data-vv-name="'beh'+b.sessionCollectBehaviorV2Id" :rules="errors.collect('beh'+b.sessionCollectBehaviorV2Id)" v-validate="'numeric'"></v-text-field>
                    </template>
                    <template v-else>
                      <v-layout row wrap>
                        <v-flex xs6>
                          <v-text-field v-model="b.total" :disabled="b.noData" :prepend-inner-icon="b.noData ? 'fa-question-circle' : 'fa-walking'" box hide-actions hide-details label="Total" @change="updateBehaviorCollection(b)" :data-vv-name="'beh'+b.sessionCollectBehaviorV2Id" :rules="errors.collect('beh'+b.sessionCollectBehaviorV2Id)" v-validate="'numeric'"></v-text-field>
                        </v-flex>
                        <v-flex xs6>
                          <v-text-field v-model="b.completed" :disabled="b.noData" :prepend-inner-icon="b.noData ? 'fa-question-circle' : b.completed == 0 ? 'fa-smile' : 'fa-frown'" box hide-actions hide-details label="Occurred" @change="updateBehaviorCollection(b)" :data-vv-name="'rep2'+b.sessionCollectBehaviorV2Id" :rules="errors.collect('rep2'+b.sessionCollectBehaviorV2Id)" v-validate="'numeric'"></v-text-field>
                        </v-flex>
                      </v-layout>
                    </template>
                  </v-card-text>
                  <v-card-actions class="pa-0 pb-2 pr-2">
                    <v-checkbox class="mt-2 pl-2" color="red" v-model="b.noData" label="No data" hide-details single-line @change="updateBehaviorCollection(b)"></v-checkbox>
                    <template v-if="b.behavior.isPercent">
                      <v-spacer></v-spacer>
                      <v-chip small label :color="b.noData ? 'red':'green'" text-color="white" class="mt-2">{{b.noData ? "N/A" : b.total == 0 ? "0" : (parseInt(b.completed) / parseInt(b.total) * 100).toFixed(1)}}
                        %
                      </v-chip>
                    </template>
                  </v-card-actions>
                </v-card>
              </v-flex>
            </v-layout>
          </v-card-text>
        </v-card>
      </v-flex>

      <v-flex xs12>
        <v-card>
          <v-toolbar dark class="secondary" fluid dense flat>
            <v-toolbar-title>Collect Replacement data {{activeDate | moment("l")}}</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-toolbar-items>
              <v-btn flat @click="goToNotes">
                <v-icon left>fa-file-medical</v-icon>
                VIEW NOTES
              </v-btn>
            </v-toolbar-items>
          </v-toolbar>
          <v-progress-linear style="position: absolute;" v-show="loadingReplacement" :indeterminate="true" class="ma-0"></v-progress-linear>
          <v-card-text class="pa-1">
            <v-layout row wrap>
              <v-flex xs12 sm6 md4 lg3 v-for="b in collectReplacements" :key="b.sessionCollectReplacementV2Id">
                <v-card flat hover :class="b.noData ? 'red lighten-4':'green lighten-5'">
                  <v-card-title primary-title class="pa-2 text-no-wrap text-truncate">
                    <div class="subheading">{{b.replacement.replacementProgramDescription}}</div>
                  </v-card-title>
                  <v-divider></v-divider>
                  <v-card-text class="pl-2 pr-2 pt-2 pb-0">
                    <v-layout row wrap>
                      <v-flex xs6>
                        <v-text-field v-model="b.total" :disabled="b.noData" :prepend-inner-icon="b.noData ? 'fa-question-circle' : 'fa-walking'" box hide-actions hide-details label="Total trials" @change="updateReplacementCollection(b)" :data-vv-name="'rep'+b.sessionCollectReplacementV2Id" :rules="errors.collect('rep'+b.sessionCollectReplacementV2Id)" v-validate="'numeric'"></v-text-field>
                      </v-flex>
                      <v-flex xs6>
                        <v-text-field v-model="b.completed" :disabled="b.noData" :prepend-inner-icon="b.noData ? 'fa-question-circle' : 'fa-check-circle'" box hide-actions hide-details label="Completed" @change="updateReplacementCollection(b)" :data-vv-name="'rep2'+b.sessionCollectReplacementV2Id" :rules="errors.collect('rep2'+b.sessionCollectReplacementV2Id)" v-validate="'numeric'"></v-text-field>
                      </v-flex>
                    </v-layout>
                  </v-card-text>
                  <v-card-actions class="pa-0 pb-2 pr-2">
                    <v-checkbox class="mt-2 pl-2" color="red" v-model="b.noData" label="No data" hide-details single-line @change="updateReplacementCollection(b)"></v-checkbox>
                    <v-spacer></v-spacer>
                    <v-chip small label :color="b.noData ? 'red':'green'" text-color="white" class="mt-2">{{b.noData ? "N/A" : b.total == 0 ? "0" : (parseInt(b.completed) / parseInt(b.total) * 100).toFixed(1)}}
                      %
                    </v-chip>
                  </v-card-actions>
                </v-card>
              </v-flex>
            </v-layout>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>

  </v-container>
</template>

<script>
import sessionServicesApi from "@/services/api/SessionServices";

export default {
  computed: {
    activeSessionId() {
      return this.$store.getters.activeSessionId;
    },
    activeClientId() {
      return this.$store.getters.activeClientId;
    },
    activeDate() {
      return this.$store.getters.activeDate;
    },
    isMobile() {
      return this.$vuetify.breakpoint.xs || this.$vuetify.breakpoint.sm;
    },
    editDisabled() {
      return false; //!this.sessionDetailed || this.sessionDetailed.sessionStatusCode === 5 || this.sessionDetailed.sessionStatusCode === 6;
    }
  },

  data() {
    return {
      loadingBehavior: false,
      loadingReplacement: false,
      collectBehaviors: [],
      collectReplacements: []
    };
  },

  mounted() {
    if (!this.activeSessionId) this.close();
    this.loadCollectBehaviors();
    this.loadCollectReplacements();
  },

  methods: {
    async loadCollectBehaviors() {
      try {
        this.loadingBehavior = true;
        this.collectBehaviors = await sessionServicesApi.getCollectBehaviors(this.activeSessionId);
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loadingBehavior = false;
      }
    },

    async loadCollectReplacements() {
      try {
        this.loadingReplacement = true;
        this.collectReplacements = await sessionServicesApi.getCollectReplacements(this.activeSessionId);
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loadingReplacement = false;
      }
    },

    async updateBehaviorCollection(b) {
      try {
        this.loadingBehavior = true;
        if (b.noData) {
          b.total = 0;
          b.completed = 0;
        }
        if (!b.total) b.total = 0;
        if (!b.completed) b.completed = 0;
        if (parseInt(b.completed) > parseInt(b.total)) b.completed = b.total;
        await sessionServicesApi.saveSessionCollectBehavior(b);
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loadingBehavior = false;
      }
    },

    async updateReplacementCollection(b) {
      try {
        this.loadingReplacement = true;
        if (b.noData) {
          b.total = 0;
          b.completed = 0;
        }
        if (!b.total) b.total = 0;
        if (!b.completed) b.completed = 0;
        if (parseInt(b.completed) > parseInt(b.total)) b.completed = b.total;
        await sessionServicesApi.saveSessionCollectReplacement(b);
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loadingReplacement = false;
      }
    },

    goToNotes() {
      this.$router.push("/clients/session_notes");
    }
  }
};
</script>
