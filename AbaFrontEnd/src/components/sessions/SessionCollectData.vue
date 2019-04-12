<template>
  <v-container pa-0 grid-list-md fluid>
    <v-layout row wrap>
      <v-flex xs12>
        <v-card>
          <v-toolbar dark class="secondary" fluid dense>
            <v-toolbar-title>Collect behavior data {{activeDate | moment('l')}}</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-toolbar-items>
              <v-btn flat @click="goToNotes">
                <v-icon left>fa-file-medical</v-icon>
                VIEW NOTES
              </v-btn>
            </v-toolbar-items>
          </v-toolbar>
          <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
          <v-card-text class="pa-1">
            <v-layout row wrap>
              <v-flex xs12 sm12 md6 lg4 v-for="(b, index) in clientBehaviors" :key="b.clientProblemId">
                <v-card color="blue lighten-4">
                  <v-card-title primary-title class="pa-2 text-no-wrap text-truncate">
                    <div>
                      <div class="headline">{{b.problemBehavior.problemBehaviorDescription}}</div>
                      <div>Total: {{getTotalBehaviors(b.problemBehavior.problemId)}}</div>
                    </div>
                  </v-card-title>
                  <v-card-actions>
                    <v-btn color="secondary" @click="addNewBehaviorFormShow(b)">NEW ENTRY</v-btn>
                    <v-spacer></v-spacer>
                    <v-btn fab dark small color="success" @click="addNewBehaviorQuickEntry(b)">
                      <v-icon dark>fa-bolt</v-icon>
                    </v-btn>
                    <!--<v-spacer></v-spacer>-->
                    <v-btn icon @click="showBehaviorDetailsEvent(index)">
                      <v-icon small>{{ showBehaviorDetails[index] ? 'fa-chevron-down' : 'fa-chevron-up' }}</v-icon>
                    </v-btn>
                  </v-card-actions>
                  <v-slide-y-transition>
                    <v-card-text class="px-0" v-show="showBehaviorDetails[index]">
                      <v-divider></v-divider>
                      <v-timeline align-top dense>
                        <v-timeline-item color="green" small v-for="t in getTimelineBehavior(b.problemBehavior.problemId)" :key="'t'+t.sessionCollectBehaviorId">
                          <v-layout pt-3>
                            <v-flex xs3>
                              <strong>{{t.entry | moment('LT')}}</strong>
                            </v-flex>
                            <v-flex xs7>
                              <strong>Duration: {{t.duration ? `${t.duration} secs` : 'N/A'}}</strong>
                              <div class="caption">{{t.notes}}</div>
                            </v-flex>
                            <v-flex xs2 class="pl-0">
                              <v-btn class="py-0 my-0" small flat dark icon @click="deleteBehaviorEntry(t)">
                                <v-icon small color="black">fa-trash</v-icon>
                              </v-btn>
                            </v-flex>
                          </v-layout>
                        </v-timeline-item>
                      </v-timeline>
                    </v-card-text>
                  </v-slide-y-transition>
                </v-card>
              </v-flex>
            </v-layout>
          </v-card-text>
        </v-card>
      </v-flex>
      <v-flex xs12>
        <v-card>
          <v-toolbar dark class="secondary" fluid dense>
            <v-toolbar-title>Collect replacement data {{activeDate | moment('l')}}</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-toolbar-items>
              <v-btn flat @click="goToNotes">
                <v-icon left>fa-file-medical</v-icon>
                VIEW NOTES
              </v-btn>
            </v-toolbar-items>

          </v-toolbar>
          <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
          <v-card-text class="pa-1">
            <v-layout row wrap>
              <v-flex xs12 sm12 md6 lg4 v-for="(b, index) in clientReplacements" :key="b.clientReplacementId">
                <v-card color="purple lighten-4">
                  <v-card-title primary-title class="pa-2 text-no-wrap text-truncate">
                    <div>
                      <div class="headline">{{b.replacement.replacementProgramDescription}}</div>
                      <div>{{getTotalReplacement(b.replacement.replacementId)}}</div>
                    </div>
                  </v-card-title>
                  <v-card-actions>
                    <v-btn color="secondary" @click="addNewReplacementFormShow(b)">NEW TRIAL</v-btn>
                    <v-spacer></v-spacer>
                    <v-btn fab dark small color="success" @click="addNewReplacementQuickEntry(b, true)">
                      <v-icon>fa-thumbs-up</v-icon>
                    </v-btn>
                    <v-btn fab dark small color="error" @click="addNewReplacementQuickEntry(b, false)">
                      <v-icon>fa-thumbs-down</v-icon>
                    </v-btn>
                    <!--<v-spacer></v-spacer>-->
                    <v-btn icon @click="showReplacementDetailsEvent(index)">
                      <v-icon small>{{ showReplacementDetails[index] ? 'fa-chevron-down' : 'fa-chevron-up' }}</v-icon>
                    </v-btn>
                  </v-card-actions>
                  <v-slide-y-transition>
                    <v-card-text class="px-0" v-show="showReplacementDetails[index]">
                      <v-divider></v-divider>
                      <v-timeline align-top dense>
                        <v-timeline-item :color="t.completed ? 'green':'red'" small v-for="t in getTimelineReplacement(b.replacement.replacementId)" :key="'r'+t.sessionCollectReplacementId">
                          <v-layout pt-3>
                            <v-flex xs3>
                              <strong>{{t.entry | moment('LT')}}</strong>
                            </v-flex>
                            <v-flex xs7>
                              <strong>
                                Completed:
                                <span :class="t.completed ? 'green--text':'red--text'">{{t.completed ? 'YES' : 'NO'}}</span>
                              </strong>
                              <div class="caption">{{t.notes}}</div>
                            </v-flex>
                            <v-flex xs2 class="pl-0">
                              <v-btn class="py-0 my-0" small flat dark icon @click="deleteReplacementEntry(t)">
                                <v-icon small color="black">fa-trash</v-icon>
                              </v-btn>
                            </v-flex>
                          </v-layout>
                        </v-timeline-item>
                      </v-timeline>
                    </v-card-text>
                  </v-slide-y-transition>
                </v-card>
              </v-flex>
            </v-layout>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>

    <v-dialog persistent width="600" v-model="openNewBehaviorDialog">
      <v-card class="grey lighten-3">
        <v-toolbar dark dense fluid>
          <v-toolbar-title>
            Add new
            <span class="yellow--text">{{formNewBehaviorTitle}}</span>
            event
          </v-toolbar-title>
        </v-toolbar>
        <v-card-text>
          <v-form ref="newBehaviorForm" autocomplete="off" v-model="validNewBehaviorForm">
            <v-container :grid-list-xl="!isMobile" :grid-list-xs="isMobile" pa-0>
              <v-layout row wrap>
                <v-flex xs12 md6>
                  <v-checkbox class="mt-0" color="primary" label="No collect time" v-model="addNewBehavior.noTime"></v-checkbox>
                  <v-time-picker class="hidden-sm-and-down" :color="addNewBehavior.noTime ? 'grey lighten-2':''" full-width v-model="addNewBehavior.entry"></v-time-picker>
                  <v-text-field class="hidden-md-and-up" hide-details box label="Time" :disabled="addNewBehavior.noTime" v-model="addNewBehavior.entry" type="time"></v-text-field>
                </v-flex>
                <v-flex xs12 md6>
                  <div :class="{'pa-2' : !isMobile}">
                    <v-subheader class="pl-0">Duration (secs):</v-subheader>
                    <v-slider v-model="addNewBehavior.duration" thumb-label prepend-icon="fa-stopwatch" min="0" max="60" :disabled="loading" class="mt-0"></v-slider>
                  </div>
                  <v-textarea box hide-details :disabled="loading" label="Notes" auto-grow v-model="addNewBehavior.notes"></v-textarea>
                </v-flex>
              </v-layout>
            </v-container>
          </v-form>
        </v-card-text>

        <v-card-actions>
          <v-spacer/>
          <v-btn :disabled="loading" flat @click="cancelNewBehavior">Cancel</v-btn>
          <v-btn :disabled="loading || !validNewBehaviorForm" :loading="loading" color="primary" @click="addNewBehaviorEvent">Add</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog persistent width="600" v-model="openNewReplacementDialog">
      <v-card class="grey lighten-3">
        <v-toolbar dark dense fluid>
          <v-toolbar-title>
            Add new
            <span class="yellow--text">{{formNewReplacementTitle}}</span>
            trial
          </v-toolbar-title>
        </v-toolbar>
        <v-card-text>
          <v-form ref="newReplacementForm" autocomplete="off" v-model="validNewReplacementForm">
            <v-container :grid-list-xl="!isMobile" :grid-list-xs="isMobile" pa-0>
              <v-layout row wrap>
                <v-flex xs12 md6>
                  <v-checkbox class="mt-0" color="primary" label="No collect time" v-model="addNewReplacement.noTime"></v-checkbox>
                  <v-time-picker class="hidden-sm-and-down" :color="addNewReplacement.noTime ? 'grey lighten-2':''" full-width v-model="addNewReplacement.entry"></v-time-picker>
                  <v-text-field class="hidden-md-and-up" hide-details box label="Time" :disabled="addNewReplacement.noTime" v-model="addNewReplacement.entry" type="time"></v-text-field>
                </v-flex>
                <v-flex xs12 md6>
                  <v-switch label="Was completed?" v-model="addNewReplacement.completed"></v-switch>
                  <v-textarea box hide-details :disabled="loading" label="Notes" auto-grow v-model="addNewReplacement.notes"></v-textarea>
                </v-flex>
              </v-layout>
            </v-container>
          </v-form>
        </v-card-text>

        <v-card-actions>
          <v-spacer/>
          <v-btn :disabled="loading" flat @click="cancelNewReplacement">Cancel</v-btn>
          <v-btn :disabled="loading || !validNewReplacementForm" :loading="loading" color="primary" @click="addNewReplacementEvent">Add</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script>
  import sessionServicesApi from '@/services/api/SessionServices';

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
    },

    data() {
      return {
        loading: false,
        validNewBehaviorForm: false,
        openNewBehaviorDialog: false,
        clientBehaviors: [],
        collectBehaviors: {
          dataDetails: [],
          dataSummary: [],
        },
        formNewBehaviorTitle: null,
        formNewBehaviorId: null,
        showBehaviorDetails: [],
        addNewBehavior: {
          noTime: false,
          entry: null,
          duration: 0,
          notes: null,
        },

        validNewReplacementForm: false,
        openNewReplacementDialog: false,
        formNewReplacementTitle: null,
        clientReplacements: [],
        showReplacementDetails: [],
        collectReplacements: {
          dataDetails: [],
          dataSummary: [],
        },
        addNewReplacement: {
          noTime: false,
          entry: null,
          completed: false,
          notes: null,
        },
      };
    },

    mounted() {
      if (!this.activeSessionId) this.close();
      this.loadGlobalData();
      this.loadCollectBehaviors();
      this.loadCollectReplacements();
    },

    methods: {
      async loadGlobalData() {
        try {
          this.clientBehaviors = await sessionServicesApi.getClientBehaviors(this.activeClientId);
          this.clientReplacements = await sessionServicesApi.getClientReplacements(this.activeClientId);
          this.clientBehaviors.forEach(() => { this.showBehaviorDetails.push(false); });
          this.clientReplacements.forEach(() => { this.showBehaviorDetails.push(false); });
        } catch (error) {
          this.$toast.error(error.message || error);
        }
      },

      async loadCollectBehaviors() {
        try {
          this.collectBehaviors = await sessionServicesApi.getCollectBehaviors(this.activeSessionId);
        } catch (error) {
          this.$toast.error(error.message || error);
        }
      },

      async loadCollectReplacements() {
        try {
          this.collectReplacements = await sessionServicesApi.getCollectReplacements(this.activeSessionId);
        } catch (error) {
          this.$toast.error(error.message || error);
        }
      },

      addNewBehaviorFormShow(b) {
        this.formNewBehaviorTitle = b.problemBehavior.problemBehaviorDescription;
        this.formNewBehaviorId = b.problemBehavior.problemId;
        this.addNewBehavior.duration = 0;
        this.addNewBehavior.entry = this.$moment().format('HH:mm');
        this.openNewBehaviorDialog = true;
      },

      addNewBehaviorQuickEntry(b) {
        this.formNewBehaviorId = b.problemBehavior.problemId;
        this.addNewBehavior.duration = 0;
        this.addNewBehavior.noTime = true;
        this.addNewBehaviorEvent();
      },

      cancelNewBehavior() {
        this.openNewBehaviorDialog = false;
        this.$refs.newBehaviorForm.reset();
      },

      async addNewBehaviorEvent() {
        try {
          this.loading = true;
          let s = this.$moment(`${this.activeDate.format('MM/DD/YYYY')} ${this.addNewBehavior.noTime ? '00:00:00' : this.addNewBehavior.entry}`);
          let data = {
            sessionId: this.activeSessionId,
            clientId: this.activeClientId,
            problemId: this.formNewBehaviorId,
            notes: this.addNewBehavior.notes,
            duration: this.addNewBehavior.duration,
            entry: s,
          };
          await sessionServicesApi.saveSessionCollectBehavior(data);
          await this.loadCollectBehaviors();
          this.cancelNewBehavior();
        } catch (error) {
          this.$toast.error(error.message || error);
        } finally {
          this.loading = false;
        }
      },

      getTotalBehaviors(problemId) {
        let c = this.collectBehaviors.dataSummary.find(s => s.problemId === problemId);
        return c ? c.count : 0;
      },

      showBehaviorDetailsEvent(i) {
        this.$set(this.showBehaviorDetails, i, !this.showBehaviorDetails[i]);
      },

      getTimelineBehavior(problemId) {
        let arr = this.collectBehaviors.dataDetails.filter(p => p.problemId == problemId);
        return arr;
      },

      deleteBehaviorEntry(t) {
        this.$confirm('Do you want to delete this entry?')
            .then(async res => {
              if (res) {
                this.loading = true;
                try {
                  await sessionServicesApi.deleteSessionCollectBehavior(t.sessionCollectBehaviorId);
                  this.loadCollectBehaviors();
                } catch (error) {
                  this.$toast.error(error);
                } finally {
                  this.loading = false;
                }
              }
            });
      },

      //replacements
      addNewReplacementFormShow(b) {
        this.formNewReplacementTitle = b.replacement.replacementProgramDescription;
        this.formNewReplacementId = b.replacement.replacementId;
        this.addNewReplacement.completed = false;
        this.addNewReplacement.entry = this.$moment().format('HH:mm');
        this.openNewReplacementDialog = true;
      },

      addNewReplacementQuickEntry(b, complete) {
        this.formNewReplacementTitle = b.replacement.replacementProgramDescription;
        this.formNewReplacementId = b.replacement.replacementId;
        this.addNewReplacement.completed = complete;
        this.addNewReplacement.noTime = true;
        this.addNewReplacementEvent();
      },

      cancelNewReplacement() {
        this.openNewReplacementDialog = false;
        this.$refs.newReplacementForm.reset();
      },

      async addNewReplacementEvent() {
        try {
          this.loading = true;
          let s = this.$moment(`${this.activeDate.format('MM/DD/YYYY')} ${this.addNewReplacement.noTime ? '00:00' : this.addNewReplacement.entry}`);
          let data = {
            sessionId: this.activeSessionId,
            clientId: this.activeClientId,
            replacementId: this.formNewReplacementId,
            notes: this.addNewReplacement.notes,
            completed: this.addNewReplacement.completed,
            entry: s,
          };
          await sessionServicesApi.saveSessionCollectReplacement(data);
          await this.loadCollectReplacements();
          this.cancelNewReplacement();
        } catch (error) {
          this.$toast.error(error.message || error);
        } finally {
          this.loading = false;
        }
      },

      getTotalReplacement(replacementId) {
        let c = this.collectReplacements.dataSummary.find(s => s.replacementId === replacementId);
        let total = c ? c.count : 0;
        let completed = c ? c.completed : 0;
        let percent = total == 0 ? 0 : completed / total * 100;
        return `Trials: ${total} | Completed: ${completed} | Percent: ${percent.toFixed(0)}%`;
      },

      showReplacementDetailsEvent(i) {
        this.$set(this.showReplacementDetails, i, !this.showReplacementDetails[i]);
      },

      getTimelineReplacement(replacementId) {
        let arr = this.collectReplacements.dataDetails.filter(p => p.replacementId == replacementId);
        return arr;
      },

      deleteReplacementEntry(t) {
        this.$confirm('Do you want to delete this trial?')
            .then(async res => {
              if (res) {
                this.loading = true;
                try {
                  await sessionServicesApi.deleteSessionCollectReplacement(t.sessionCollectReplacementId);
                  this.loadCollectReplacements();
                } catch (error) {
                  this.$toast.error(error);
                } finally {
                  this.loading = false;
                }
              }
            });
      },

      goToNotes() {
        this.$router.push('/clients/session_notes');
      },

    },
  };
</script>
