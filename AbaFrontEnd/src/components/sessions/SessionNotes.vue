<template>
  <v-container fluid grid-list-xs pa-0>
    <v-layout row wrap>
      <v-flex xs12 md12 lg12 xl10>
        <v-card class="elevation-12">
          <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
          <v-toolbar :color="!sessionDetailed ? 'secondary' : sessionDetailed.sessionStatusColor" dark tabs dense>
            <v-toolbar-title
              >Session notes for
              <strong class="yellow--text" v-if="sessionDetailed">{{ sessionDetailed.clientFullname }} <span class="white--text">at</span> ({{ sessionDetailed.pos }})</strong></v-toolbar-title
            >
            <v-spacer></v-spacer>
            <v-chip outline v-if="sessionDetailed" small disabled color="white" text-color="white">
              <v-icon left>fa-info-circle</v-icon>
              {{ sessionDetailed.sessionStatus }}
            </v-chip>
            <v-menu :disabled="loading" left bottom transition="slide-y-transition">
              <v-btn slot="activator" icon :disabled="loading">
                <v-icon small color="white">fa-ellipsis-v</v-icon>
              </v-btn>
              <v-list>
                <v-list-tile @click="editTime" v-if="!editDisabled && isAdmin">
                  <v-list-tile-action>
                    <v-icon medium>fa-clock</v-icon>
                  </v-list-tile-action>
                  <v-list-tile-content>
                    <v-list-tile-title>Edit time</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
                <v-divider v-if="!editDisabled && isAdmin"></v-divider>
                <v-list-tile to="/session/session_print">
                  <v-list-tile-action>
                    <v-icon medium>fa-print</v-icon>
                  </v-list-tile-action>
                  <v-list-tile-content>
                    <v-list-tile-title>Print</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
                <v-list-tile v-if="!editDisabled && (!sessionDetailed || !sessionDetailed.sign) && !isBilled" @click="send2Email">
                  <v-list-tile-action>
                    <v-icon medium>fa-signature</v-icon>
                  </v-list-tile-action>
                  <v-list-tile-content>
                    <v-list-tile-title>Send sign form to caregiver</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
                <v-list-tile v-else-if="!isBilled && !isReviewed" @click="deleteSign">
                  <v-list-tile-action>
                    <span class="fa-stack">
                      <v-icon medium>fa-signature fa-stack-2x</v-icon>
                      <v-icon medium color="red lighten-3">fa-times fa-stack-1x</v-icon>
                    </span>
                  </v-list-tile-action>
                  <v-list-tile-content>
                    <v-list-tile-title>Delete sign</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
                <v-list-tile v-if="isAdminOrLeadOrAssistant && (!sessionDetailed || sessionDetailed.sessionStatusCode !== 5) && !isBilled && !isReviewed" @click="markAsChecked">
                  <v-list-tile-action>
                    <v-icon medium>fa-check-circle</v-icon>
                  </v-list-tile-action>
                  <v-list-tile-content>
                    <v-list-tile-title>Mark as Checked</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
                <template v-if="!editDisabled && isAdminOrLeadOrAssistant && (!sessionDetailed || sessionDetailed.sessionStatusCode !== 2)">
                  <v-divider></v-divider>
                  <v-list-tile @click="rejectSession">
                    <v-list-tile-action>
                      <v-icon medium>fa-exclamation-circle</v-icon>
                    </v-list-tile-action>
                    <v-list-tile-content>
                      <v-list-tile-title>Reject session</v-list-tile-title>
                    </v-list-tile-content>
                  </v-list-tile>
                </template>
                <template v-if="!editDisabled && isAdminOrLeadOrAssistant && isAdminOrLeadOrAssistant && (!sessionDetailed || sessionDetailed.sessionStatusCode !== 5)">
                  <v-divider></v-divider>
                  <v-list-tile @click="deleteSession">
                    <v-list-tile-action>
                      <v-icon medium>fa-trash</v-icon>
                    </v-list-tile-action>
                    <v-list-tile-content>
                      <v-list-tile-title>Delete session</v-list-tile-title>
                    </v-list-tile-content>
                  </v-list-tile>
                </template>
                <template v-if="isAdminOrLeadOrAssistant && (!sessionDetailed || sessionDetailed.sessionStatusCode === 5)">
                  <v-divider></v-divider>
                  <v-list-tile @click="reopenSession">
                    <v-list-tile-action>
                      <v-icon medium>fa-lock-open</v-icon>
                    </v-list-tile-action>
                    <v-list-tile-content>
                      <v-list-tile-title>Reopen session</v-list-tile-title>
                    </v-list-tile-content>
                  </v-list-tile>
                </template>
                <template v-else-if="isAdmin && (!sessionDetailed || sessionDetailed.sessionStatusCode === 6)">
                  <v-divider></v-divider>
                  <v-list-tile @click="reopenSession">
                    <v-list-tile-action>
                      <v-icon medium>fa-lock-open</v-icon>
                    </v-list-tile-action>
                    <v-list-tile-content>
                      <v-list-tile-title>Reopen session</v-list-tile-title>
                    </v-list-tile-content>
                  </v-list-tile>
                </template>
                <v-list-tile @click="goToData">
                  <v-list-tile-action>
                    <v-icon medium>fa-chart-line</v-icon>
                  </v-list-tile-action>
                  <v-list-tile-content>
                    <v-list-tile-title>Data collection</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
                <v-list-tile v-if="isRbt && !editDisabled && sessionDetailed.sessionStatusCode !== 8" @click="markAsReady2Lead">
                  <v-list-tile-action>
                    <v-icon medium>fa-calendar-check</v-icon>
                  </v-list-tile-action>
                  <v-list-tile-content>
                    <v-list-tile-title>Ready to Analyst</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
                <template v-if="isChecked && isAdmin">
                  <v-divider></v-divider>
                  <v-list-tile @click="markAsReviewed">
                    <v-list-tile-action>
                      <v-icon medium>fa-search-dollar</v-icon>
                    </v-list-tile-action>
                    <v-list-tile-content>
                      <v-list-tile-title>Reviewed (ready to bill)</v-list-tile-title>
                    </v-list-tile-content>
                  </v-list-tile>
                </template>
              </v-list>
            </v-menu>

            <v-tabs slot="extension" :color="!sessionDetailed ? 'secondary' : sessionDetailed.sessionStatusColor" dark show-arrows v-model="tabModel">
              <v-tab key="details">Details</v-tab>
              <v-tab key="caregiver">Caregiver</v-tab>
              <template v-if="session.sessionType === 1">
                <v-tab key="risk">Risk Behavior</v-tab>
                <v-tab key="reinforcers">Reinforcers</v-tab>
                <v-tab key="progress">Progress</v-tab>
                <!--                <v-tab key="feedback">Feedback</v-tab>-->
                <v-tab v-if="user.rol2 !== 'tech'" key="summary">Summary</v-tab>
                <v-tab key="problems">Prob/Replac</v-tab>
              </template>
              <template v-else-if="session.sessionType === 2">
                <v-tab key="training">Caregiver training</v-tab>
              </template>
              <template v-if="session.sessionType === 3">
                <v-tab key="supervisionSession">Session</v-tab>
                <v-tab key="supervisionSummary">Summary</v-tab>
                <v-tab key="supervisionOversight">Oversight</v-tab>
                <v-tab key="supervisionExtra">Extra</v-tab>
              </template>
              <!-- <v-tab v-if="isAdmin" key="logs">Logs</v-tab> -->
            </v-tabs>
          </v-toolbar>
          <v-tabs-items v-model="tabModel">
            <v-tab-item key="details">
              <v-card flat>
                <v-card-text v-if="sessionDetailed" class="pa-2">
                  <v-container fluid grid-list-sm pa-0>
                    <v-layout row wrap>
                      <v-flex xs6>
                        <v-layout row wrap>
                          <v-flex class="body-2 text-xs-right" xs4>Date:</v-flex>
                          <v-flex xs8>{{ sessionDetailed.sessionStart | moment("ddd") }}, {{ sessionDetailed.sessionStart | moment("ll") }}</v-flex>
                          <v-flex class="body-2 text-xs-right" xs4>Time IN:</v-flex>
                          <v-flex xs8>
                            <v-icon color="green" small>fa-sign-in-alt</v-icon>
                            {{ sessionDetailed.sessionStart | moment("LT") }}
                          </v-flex>
                          <v-flex class="body-2 text-xs-right" xs4>Time OUT:</v-flex>
                          <v-flex xs8>
                            <v-icon color="red" small>fa-sign-out-alt</v-icon>
                            {{ sessionDetailed.sessionEnd | moment("LT") }}
                          </v-flex>
                          <v-flex class="body-2 text-xs-right" xs4>Units:</v-flex>
                          <v-flex xs8>
                            <v-icon small>fa-star</v-icon>
                            {{ sessionDetailed.totalUnits.toLocaleString() }}
                            <v-icon small>fa-clock</v-icon>
                            {{ (sessionDetailed.totalUnits / 4).toLocaleString() }}
                          </v-flex>
                          <v-flex class="body-2 text-xs-right" xs4>Drive time (hrs):</v-flex>
                          <v-flex xs8>
                            <div v-if="!driveTimeEditVisible">
                              <v-icon small>fa-car</v-icon>
                              {{ sessionDetailed.driveTime }}&nbsp;
                              <v-tooltip top>
                                <template #activator="data">
                                  <v-icon v-if="!editDisabled" color="primary" style="cursor: pointer;" small v-on="data.on" @click="editDriveTime">fa-pen-alt</v-icon>
                                </template>
                                <span>Edit Drive time in hours</span>
                              </v-tooltip>
                            </div>
                            <div v-else-if="!editDisabled">
                              <v-text-field
                                :disabled="loadEditDriveTime"
                                v-model="sessionDetailed.driveTime"
                                class="pa-0"
                                suffix="hrs"
                                single-line
                                hide-details
                                append-outer-icon="fa-paper-plane"
                                @click:append-outer="submitDriveTime"
                                placeholder="Edit drive time"
                                @keypress.enter.native="submitDriveTime"
                              ></v-text-field>
                            </div>
                          </v-flex>
                          <!-- <v-flex class="body-2 text-xs-right" xs4>Sign:</v-flex> -->
                          <v-flex xs8 :offset-xs4="!sessionDetailed || !sessionDetailed.sign || !sessionDetailed.sign.sign">
                            <v-chip v-if="!sessionDetailed || !sessionDetailed.sign || !sessionDetailed.sign.sign" label disabled color="orange" text-color="white">
                              <v-avatar>
                                <v-icon>fa-signature</v-icon>
                              </v-avatar>
                              UNSIGNED
                            </v-chip>
                            <v-img max-width="300" :contain="true" max-height="100" v-else :src="!sessionDetailed || sessionDetailed.sign.sign" />
                          </v-flex>
                        </v-layout>
                      </v-flex>
                      <v-flex xs6>
                        <v-layout row wrap>
                          <v-flex class="body-2 text-xs-right" xs4>Client:</v-flex>
                          <v-flex xs8>{{ sessionDetailed.clientFullname }} ({{ sessionDetailed.clientCode }})</v-flex>
                          <v-flex class="body-2 text-xs-right" xs4>Pos:</v-flex>
                          <v-flex xs8>
                            <div v-if="!posEditVisible">
                              <span class="text-no-wrap text-truncate">{{ sessionDetailed.pos }}</span> &nbsp;
                              <v-tooltip left>
                                <template #activator="data">
                                  <v-icon color="primary" style="cursor: pointer;" v-on="data.on" small @click="editPos">fa-pen-alt</v-icon>
                                </template>
                                <span>Edit POS</span>
                              </v-tooltip>
                            </div>
                            <v-select
                              v-if="posEditVisible"
                              :loading="loadingPosEdit"
                              :disabled="loading"
                              hide-details
                              single-line
                              class="pa-0 ma-0"
                              placeholder="Pos"
                              v-model="posToEdit"
                              :items="posEnum"
                              prepend-icon="fa-map-marker-alt"
                              @change="changeNewPos"
                            >
                              <template slot="selection" slot-scope="data">
                                <div class="input-group__selections__comma">
                                  {{ data.item.text }} &nbsp; <span class="grey--text text--darken-1">({{ data.item.value }})</span>
                                </div>
                              </template>
                              <template slot="item" slot-scope="data">
                                <template>
                                  <v-list-tile-avatar>
                                    <v-icon>fa-map-marker-alt</v-icon>
                                  </v-list-tile-avatar>
                                  <v-list-tile-content>
                                    <v-list-tile-title v-html="data.item.text"></v-list-tile-title>
                                    <v-list-tile-sub-title>Code: {{ data.item.value }}</v-list-tile-sub-title>
                                  </v-list-tile-content>
                                </template>
                              </template>
                            </v-select>
                          </v-flex>
                          <v-flex class="body-2 text-xs-right" xs4>Session type:</v-flex>
                          <v-flex xs8>{{ sessionDetailed.sessionType }}</v-flex>
                          <v-flex class="body-2 text-xs-right" xs4>Service:</v-flex>
                          <v-flex xs8>{{ sessionDetailed.hcpcs }} ({{ sessionDetailed.description }})</v-flex>
                        </v-layout>
                      </v-flex>
                      <v-flex xs12 v-if="sessionDetailed.sessionStatusCode == 2">
                        <v-alert :value="true" type="error">{{ session.sessionType !== 3 ? session.sessionNote.rejectNotes : session.sessionSupervisionNote.rejectNotes }}</v-alert>
                      </v-flex>
                    </v-layout>
                  </v-container>
                </v-card-text>
              </v-card>
            </v-tab-item>
            <v-tab-item key="caregiver">
              <v-card flat>
                <v-card-text class="pa-2">
                  <v-container fluid grid-list-sm pa-0>
                    <v-layout row wrap>
                      <v-flex xs12>
                        <v-select
                          v-if="session.sessionType !== 3"
                          box
                          hide-details
                          :disabled="loading || editDisabled"
                          label="Caregiver"
                          v-model="session.sessionNote.caregiverId"
                          :items="caregivers"
                          @change="setDirty"
                        ></v-select>
                        <v-select v-else box hide-details :disabled="loading || editDisabled" label="Caregiver" v-model="session.sessionSupervisionNote.caregiverId" :items="caregivers"></v-select>
                      </v-flex>
                      <v-flex xs12>
                        <v-textarea
                          v-if="session.sessionType !== 3"
                          box
                          hide-details
                          :disabled="loading || editDisabled"
                          label="Caregiver notes"
                          auto-grow
                          v-model="session.sessionNote.caregiverNote"
                          @change="setDirty"
                        ></v-textarea>
                        <v-textarea
                          v-else
                          box
                          hide-details
                          :disabled="loading || editDisabled"
                          label="Caregiver notes"
                          auto-grow
                          v-model="session.sessionSupervisionNote.caregiverNote"
                          @change="setDirty"
                        ></v-textarea>
                      </v-flex>
                    </v-layout>
                  </v-container>
                </v-card-text>
              </v-card>
            </v-tab-item>
            <template v-if="session.sessionType === 1">
              <v-tab-item key="risk">
                <v-card flat>
                  <v-card-text class="pa-2">
                    <v-container fluid grid-list-sm pa-0>
                      <v-layout row wrap>
                        <v-flex xs12>
                          <v-select
                            box
                            hide-details
                            :disabled="loading || editDisabled"
                            label="Risk Behavior"
                            v-model="session.sessionNote.riskBehavior"
                            :items="riskBehaviorCodes"
                            @change="setDirty"
                          ></v-select>
                        </v-flex>
                        <v-flex xs12 class="pt-3">
                          <v-switch
                            hide-details
                            color="primary"
                            :disabled="editDisabled"
                            label="Crisis involved"
                            v-model="session.sessionNote.riskBehaviorCrisisInvolved"
                            @change="setDirty"
                          ></v-switch>
                        </v-flex>
                        <v-flex xs12>
                          <v-textarea
                            box
                            hide-details
                            :disabled="loading || !session.sessionNote.riskBehaviorCrisisInvolved || editDisabled"
                            label="Explain"
                            auto-grow
                            v-model="session.sessionNote.riskBehaviorExplain"
                            @change="setDirty"
                          ></v-textarea>
                        </v-flex>
                      </v-layout>
                    </v-container>
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item key="reinforcers">
                <v-card flat>
                  <v-card-text class="pa-2">
                    <v-container fluid grid-list-sm pa-0>
                      <v-layout row wrap>
                        <v-flex xs12>
                          <v-textarea box auto-grow hide-details :disabled="loading || editDisabled" label="Edibles" v-model="session.sessionNote.reinforcersEdibles" @change="setDirty"></v-textarea>
                        </v-flex>
                        <v-flex xs12>
                          <v-textarea
                            box
                            auto-grow
                            hide-details
                            :disabled="loading || editDisabled"
                            label="Non-edibles"
                            v-model="session.sessionNote.reinforcersNonEdibles"
                            @change="setDirty"
                          ></v-textarea>
                        </v-flex>
                        <v-flex xs12>
                          <v-textarea box auto-grow hide-details :disabled="loading || editDisabled" label="Others" v-model="session.sessionNote.reinforcersOthers" @change="setDirty"></v-textarea>
                        </v-flex>
                        <v-flex xs12>
                          <v-textarea box auto-grow hide-details :disabled="loading || editDisabled" label="Result" v-model="session.sessionNote.reinforcersResult" @change="setDirty"></v-textarea>
                        </v-flex>
                      </v-layout>
                    </v-container>
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item key="progress">
                <v-card flat>
                  <v-card-text class="pa-2">
                    <v-container fluid grid-list-sm pa-0>
                      <v-layout row wrap>
                        <v-flex xs12>
                          <v-select
                            box
                            hide-details
                            :disabled="loading || editDisabled"
                            label="Participation level"
                            v-model="session.sessionNote.participationLevel"
                            :items="participationLevelCodes"
                            @change="setDirty"
                          ></v-select>
                        </v-flex>
                        <v-flex xs12>
                          <v-textarea
                            box
                            hide-details
                            :disabled="loading || editDisabled"
                            label="Progress notes"
                            auto-grow
                            v-model="session.sessionNote.progressNotes"
                            rows="15"
                            @change="checkMatching"
                          ></v-textarea>
                        </v-flex>
                        <v-flex xs12>
                          Matching percentaje:
                          <v-chip :class="{ pulse: matchingCantSave }" label small v-if="matching" :color="matchingCantSave ? 'red' : 'green'" text-color="white">
                            {{ (matching.percentaje * 100).toFixed(2) }}
                            <!-- <v-icon small right>percentaje</v-icon> -->
                          </v-chip>
                          <v-icon small v-if="loadingMatching">fa-cog fa-spin</v-icon>
                          <!-- <v-btn small flat icon color="green" @click="checkMatchingPercentaje" :disabled="loadingMatching || dirtyIndicator">
                            <v-icon small>fa-sync-alt</v-icon>
                          </v-btn> -->
                        </v-flex>
                        <v-flex xs12 v-if="matching">
                          <v-expansion-panel expand-icon="fa-caret-down">
                            <v-expansion-panel-content class="yellow lighten-4">
                              <template v-slot:header>
                                <div>Matching session: ID: {{ matching.sessionId }} Date: {{ matching.date | moment("utc", "MM/DD/YYYY") }}</div>
                              </template>
                              <v-card class="yellow lighten-5">
                                <v-card-text>
                                  {{ matching.progressNotes }}
                                </v-card-text>
                              </v-card>
                            </v-expansion-panel-content>
                          </v-expansion-panel>
                        </v-flex>
                      </v-layout>
                    </v-container>
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <!--              <v-tab-item key="feedback">-->
              <!--                <v-card flat>-->
              <!--                  <v-card-text class="pa-2">-->
              <!--                    <v-container fluid grid-list-sm pa-0>-->
              <!--                      <v-layout row wrap>-->
              <!--                        <v-flex xs12>-->
              <!--                          <v-switch hide-details color="primary" :disabled="editDisabled" label="Feedback provided to Caregiver" v-model="session.sessionNote.feedbackCaregiver"></v-switch>-->
              <!--                        </v-flex>-->
              <!--                        <v-flex xs12 class="pt-0">-->
              <!--                          <v-textarea box hide-details :disabled="loading || editDisabled || !session.sessionNote.feedbackCaregiver" label="Explain" auto-grow v-model="session.sessionNote.feedbackCaregiverExplain"></v-textarea>-->
              <!--                        </v-flex>-->
              <!--                        <v-flex xs12 class="pt-3">-->
              <!--                          <v-switch hide-details color="primary" :disabled="editDisabled" label="Feedback provided to other services providers" v-model="session.sessionNote.feedbackOtherServices"></v-switch>-->
              <!--                        </v-flex>-->
              <!--                        <v-flex xs12 class="pt-0">-->
              <!--                          <v-textarea box hide-details :disabled="loading || editDisabled || !session.sessionNote.feedbackOtherServices" label="Explain" auto-grow v-model="session.sessionNote.feedbackOtherServicesExplain"></v-textarea>-->
              <!--                        </v-flex>-->
              <!--                      </v-layout>-->
              <!--                    </v-container>-->
              <!--                  </v-card-text>-->
              <!--                </v-card>-->
              <!--              </v-tab-item>-->
              <v-tab-item v-if="user.rol2 !== 'tech'" key="summary">
                <v-card flat>
                  <v-card-text class="pa-2">
                    <v-container fluid grid-list-sm pa-0>
                      <v-layout row wrap>
                        <v-flex xs12>
                          <v-switch
                            hide-details
                            color="primary"
                            label="Direct observation/Data collection/Probing"
                            v-model="session.sessionNote.summaryDirectObservation"
                            @change="setDirty"
                          ></v-switch>
                        </v-flex>
                        <v-flex xs12>
                          <v-switch
                            hide-details
                            color="primary"
                            label="Observation of recipients's interaction with parent/caregiver/others"
                            v-model="session.sessionNote.summaryObservationFeedback"
                            @change="setDirty"
                          ></v-switch>
                        </v-flex>
                        <v-flex xs12>
                          <v-switch hide-details color="primary" label="Implemented reduction programs" v-model="session.sessionNote.summaryImplementedReduction" @change="setDirty"></v-switch>
                        </v-flex>
                        <v-flex xs12>
                          <v-switch hide-details color="primary" label="Implemented replacement programs" v-model="session.sessionNote.summaryImplementedReplacement" @change="setDirty"></v-switch>
                        </v-flex>
                        <v-flex xs12>
                          <v-switch hide-details color="primary" label="Generalization of treatment" v-model="session.sessionNote.summaryGeneralization" @change="setDirty"></v-switch>
                        </v-flex>
                        <v-flex xs12>
                          <v-switch
                            hide-details
                            color="primary"
                            label="Communication/Coordination of care with other services professionals"
                            v-model="session.sessionNote.summaryCommunication"
                            @change="setDirty"
                          ></v-switch>
                        </v-flex>
                        <v-flex xs12>
                          <v-textarea box hide-details :disabled="loading" label="Other" auto-grow v-model="session.sessionNote.summaryOther" @change="setDirty"></v-textarea>
                        </v-flex>
                      </v-layout>
                    </v-container>
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item key="problems">
                <v-card flat>
                  <v-card-text class="pa-2">
                    <v-container fluid grid-list-sm pa-0>
                      <v-layout row wrap>
                        <v-flex xs12>
                          <v-select
                            class="autoheigh"
                            box
                            hide-details
                            :disabled="loading"
                            label="Problem"
                            v-model="problemSelected"
                            :items="problemsUnique"
                            item-text="problemBehaviorDescription"
                            item-value="problemId"
                          ></v-select>
                        </v-flex>
                      </v-layout>
                      <template v-for="problem in session.sessionProblemNotes">
                        <v-layout v-if="problemSelected === problem.problemId" :key="problem.SessionProblemNoteId">
                          <v-flex xs6>
                            <v-textarea
                              box
                              hide-details
                              :disabled="loading || editDisabled"
                              label="During which activity bx. ocurred"
                              auto-grow
                              v-model="problem.duringWichActivities"
                              @change="setDirty"
                            ></v-textarea>
                            <br />
                            <span v-html="getIfBehaviorHasData(problemSelected)"></span>
                          </v-flex>
                          <v-flex xs6>
                            <v-textarea
                              box
                              hide-details
                              :disabled="loading || editDisabled"
                              label="Replacement bx. implemented interventions used"
                              auto-grow
                              v-model="problem.replacementInterventionsUsed"
                              @change="setDirty"
                            ></v-textarea>
                            <v-switch
                              :disabled="editDisabled"
                              :key="replacement.sessionProblemNoteReplacementId"
                              v-for="replacement in problem.sessionProblemNoteReplacements"
                              hide-details
                              color="primary"
                              :label="replacement.replacementProgram.replacementProgramDescription"
                              v-model="replacement.active"
                              @change="setDirty"
                            ></v-switch>
                          </v-flex>
                        </v-layout>
                      </template>
                    </v-container>
                  </v-card-text>
                </v-card>
              </v-tab-item>
            </template>
            <template v-else-if="session.sessionType === 2">
              <v-tab-item key="training">
                <v-card flat>
                  <v-card-text class="pa-2">
                    <v-container fluid grid-list-sm pa-0>
                      <v-layout row wrap>
                        <v-flex xs12>
                          <v-switch
                            hide-details
                            color="primary"
                            label="Observation of recipients's interaction with parent/caregiver/others"
                            v-model="session.sessionNote.caregiverTrainingObservationFeedback"
                            @change="setDirty"
                          ></v-switch>
                        </v-flex>
                        <v-flex xs12>
                          <v-switch hide-details color="primary" label="Parent/Caregiver training" v-model="session.sessionNote.caregiverTrainingParentCaregiverTraining" @change="setDirty"></v-switch>
                        </v-flex>
                        <v-flex xs12>
                          <v-switch hide-details color="primary" label="Competency check of caregiver" v-model="session.sessionNote.caregiverTrainingCompetencyCheck" @change="setDirty"></v-switch>
                        </v-flex>
                        <v-flex xs12>
                          <v-textarea box hide-details :disabled="loading" label="Other" auto-grow v-model="session.sessionNote.caregiverTrainingOther" @change="setDirty"></v-textarea>
                        </v-flex>
                        <v-flex xs12>
                          <v-textarea box hide-details :disabled="loading" label="Summary of training" auto-grow v-model="session.sessionNote.caregiverTrainingSummary" @change="setDirty"></v-textarea>
                        </v-flex>
                      </v-layout>
                    </v-container>
                  </v-card-text>
                </v-card>
              </v-tab-item>
            </template>
            <template v-if="session.sessionType === 3">
              <v-tab-item key="supervisionSession">
                <v-card flat>
                  <v-card-text class="pa-2">
                    <v-card flat>
                      <v-card-text class="pa-2">
                        <v-container fluid grid-list-sm pa-0>
                          <v-layout row wrap>
                            <v-flex xs12>
                              <v-switch hide-details color="primary" label="Direct meeting?" v-model="session.sessionSupervisionNote.isDirectSession" @change="setDirty"></v-switch>
                            </v-flex>
                            <v-subheader>Work with:</v-subheader>
                            <v-flex xs12 v-for="w in sessionSupervisionWorkWithCodes" :key="w.text">
                              <v-switch hide-details color="primary" :label="w.text" v-model="sessionSupervisionWorkWithArray" :value="w.value" @change="setDirty"></v-switch>
                            </v-flex>
                          </v-layout>
                        </v-container>
                      </v-card-text>
                    </v-card>
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item key="supervisionSummary">
                <v-card flat>
                  <v-card-text class="pa-2">
                    <v-card flat>
                      <v-card-text class="pa-2">
                        <v-container fluid grid-list-sm pa-0>
                          <v-layout row wrap>
                            <v-flex xs12>
                              <v-switch
                                hide-details
                                color="primary"
                                label="Observation and feedback regarding interaction of BCaBA"
                                v-model="session.sessionSupervisionNote.briefObservation"
                                @change="setDirty"
                              ></v-switch>
                            </v-flex>
                            <v-flex xs12>
                              <v-switch
                                hide-details
                                color="primary"
                                label="Replacement/Acquisition Programs Implementation monitoring"
                                v-model="session.sessionSupervisionNote.briefReplacement"
                                @change="setDirty"
                              ></v-switch>
                            </v-flex>
                            <v-flex xs12>
                              <v-switch hide-details color="primary" label="Generalization of Treatment" v-model="session.sessionSupervisionNote.briefGeneralization" @change="setDirty"></v-switch>
                            </v-flex>
                            <v-flex xs12>
                              <v-switch hide-details color="primary" label="BCaBA training and oversight" v-model="session.sessionSupervisionNote.briefBCaBaTraining" @change="setDirty"></v-switch>
                            </v-flex>
                            <v-flex xs12>
                              <v-switch
                                hide-details
                                color="primary"
                                label="In-Service Education Training (for session participants)"
                                v-model="session.sessionSupervisionNote.briefInService"
                                class="pb-3"
                                @change="setDirty"
                              ></v-switch>
                              <v-text-field
                                hide-details
                                :disabled="loading || !session.sessionSupervisionNote.briefInService"
                                label="Subject"
                                box
                                v-model="session.sessionSupervisionNote.briefInServiceSubject"
                                @change="setDirty"
                              ></v-text-field>
                            </v-flex>
                            <v-flex xs12>
                              <v-switch hide-details color="primary" label="Others" v-model="session.sessionSupervisionNote.briefOther" @change="setDirty"></v-switch>
                            </v-flex>
                            <v-flex xs12>
                              <v-textarea
                                box
                                hide-details
                                :disabled="loading || !session.sessionSupervisionNote.briefOther"
                                label="Other"
                                auto-grow
                                v-model="session.sessionSupervisionNote.briefOtherDescription"
                                @change="setDirty"
                              ></v-textarea>
                            </v-flex>
                          </v-layout>
                        </v-container>
                      </v-card-text>
                    </v-card>
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item key="supervisionOversight">
                <v-card flat>
                  <v-card-text class="pa-2">
                    <v-card flat>
                      <v-card-text class="pa-2">
                        <v-container fluid grid-list-sm pa-0>
                          <v-layout row wrap>
                            <v-flex xs9>
                              <v-switch
                                hide-details
                                color="primary"
                                label="Follow up upon recommendations from previous reassessment"
                                v-model="session.sessionSupervisionNote.oversightFollowUpBool"
                                @change="setDirty"
                              ></v-switch>
                            </v-flex>
                            <v-flex xs3>
                              <v-select
                                box
                                hide-details
                                :disabled="loading || !session.sessionSupervisionNote.oversightFollowUpBool"
                                label="Eval"
                                v-model="session.sessionSupervisionNote.oversightFollowUp"
                                :items="oversightSessionSupervisionEnum"
                                @change="setDirty"
                              ></v-select>
                            </v-flex>
                            <v-flex xs9>
                              <v-switch
                                hide-details
                                color="primary"
                                label="Designing, implementing and monitoring program for client"
                                v-model="session.sessionSupervisionNote.oversightDesigningBool"
                                @change="setDirty"
                              ></v-switch>
                            </v-flex>
                            <v-flex xs3>
                              <v-select
                                box
                                hide-details
                                :disabled="loading || !session.sessionSupervisionNote.oversightDesigningBool"
                                label="Eval"
                                v-model="session.sessionSupervisionNote.oversightDesigning"
                                :items="oversightSessionSupervisionEnum"
                                @change="setDirty"
                              ></v-select>
                            </v-flex>
                            <v-flex xs9>
                              <v-switch
                                hide-details
                                color="primary"
                                label="Contributing with Behavioral Assessment"
                                v-model="session.sessionSupervisionNote.oversightContributingBool"
                                @change="setDirty"
                              ></v-switch>
                            </v-flex>
                            <v-flex xs3>
                              <v-select
                                box
                                hide-details
                                :disabled="loading || !session.sessionSupervisionNote.oversightContributingBool"
                                label="Eval"
                                v-model="session.sessionSupervisionNote.oversightContributing"
                                :items="oversightSessionSupervisionEnum"
                                @change="setDirty"
                              ></v-select>
                            </v-flex>
                            <v-flex xs9>
                              <v-switch hide-details color="primary" label="Analyzing data" v-model="session.sessionSupervisionNote.oversightAnalyzingBool" @change="setDirty"></v-switch>
                            </v-flex>
                            <v-flex xs3>
                              <v-select
                                box
                                hide-details
                                :disabled="loading || !session.sessionSupervisionNote.oversightAnalyzingBool"
                                label="Eval"
                                v-model="session.sessionSupervisionNote.oversightAnalyzing"
                                :items="oversightSessionSupervisionEnum"
                                @change="setDirty"
                              ></v-select>
                            </v-flex>
                            <v-flex xs9>
                              <v-switch
                                hide-details
                                color="primary"
                                label="Goals progress evidenced in client performance"
                                v-model="session.sessionSupervisionNote.oversightGoalsBool"
                                @change="setDirty"
                              ></v-switch>
                            </v-flex>
                            <v-flex xs3>
                              <v-select
                                box
                                hide-details
                                :disabled="loading || !session.sessionSupervisionNote.oversightGoalsBool"
                                label="Eval"
                                v-model="session.sessionSupervisionNote.oversightGoals"
                                :items="oversightSessionSupervisionEnum"
                                @change="setDirty"
                              ></v-select>
                            </v-flex>
                            <v-flex xs9>
                              <v-switch
                                hide-details
                                color="primary"
                                label="Making decisions about progress"
                                v-model="session.sessionSupervisionNote.oversightMakingDecisionsBool"
                                @change="setDirty"
                              ></v-switch>
                            </v-flex>
                            <v-flex xs3>
                              <v-select
                                box
                                hide-details
                                :disabled="loading || !session.sessionSupervisionNote.oversightMakingDecisionsBool"
                                label="Eval"
                                v-model="session.sessionSupervisionNote.oversightMakingDecisions"
                                :items="oversightSessionSupervisionEnum"
                                @change="setDirty"
                              ></v-select>
                            </v-flex>
                            <v-flex xs9>
                              <v-switch
                                hide-details
                                color="primary"
                                label="Modeling technical, professional and ethical behavior"
                                v-model="session.sessionSupervisionNote.oversightModelingBool"
                                @change="setDirty"
                              ></v-switch>
                            </v-flex>
                            <v-flex xs3>
                              <v-select
                                box
                                hide-details
                                :disabled="loading || !session.sessionSupervisionNote.oversightModelingBool"
                                label="Eval"
                                v-model="session.sessionSupervisionNote.oversightModeling"
                                :items="oversightSessionSupervisionEnum"
                                @change="setDirty"
                              ></v-select>
                            </v-flex>
                            <v-flex xs9>
                              <v-switch
                                hide-details
                                color="primary"
                                label="Response to feedback from Lead Analyst"
                                v-model="session.sessionSupervisionNote.oversightResponseBool"
                                @change="setDirty"
                              ></v-switch>
                            </v-flex>
                            <v-flex xs3>
                              <v-select
                                box
                                hide-details
                                :disabled="loading || !session.sessionSupervisionNote.oversightResponseBool"
                                label="Eval"
                                v-model="session.sessionSupervisionNote.oversightResponse"
                                :items="oversightSessionSupervisionEnum"
                                @change="setDirty"
                              ></v-select>
                            </v-flex>
                            <v-flex xs9>
                              <v-switch hide-details color="primary" label="Overall Evaluation in session" v-model="session.sessionSupervisionNote.oversightOverallBool" @change="setDirty"></v-switch>
                            </v-flex>
                            <v-flex xs3>
                              <v-select
                                box
                                hide-details
                                :disabled="loading || !session.sessionSupervisionNote.oversightOverallBool"
                                label="Eval"
                                v-model="session.sessionSupervisionNote.oversightOverall"
                                :items="oversightSessionSupervisionEnum"
                                @change="setDirty"
                              ></v-select>
                            </v-flex>
                          </v-layout>
                        </v-container>
                      </v-card-text>
                    </v-card>
                  </v-card-text>
                </v-card>
              </v-tab-item>
              <v-tab-item key="supervisionExtra">
                <v-card flat>
                  <v-card-text class="pa-2">
                    <v-card flat>
                      <v-card-text class="pa-2">
                        <v-container fluid grid-list-sm pa-0>
                          <v-layout row wrap>
                            <v-textarea box :disabled="loading" label="Comments related to session" auto-grow v-model="session.sessionSupervisionNote.commentsRelated" @change="setDirty"></v-textarea>
                          </v-layout>
                          <v-layout row wrap>
                            <v-textarea box :disabled="loading" label="Recommendations" auto-grow v-model="session.sessionSupervisionNote.recommendations" @change="setDirty"></v-textarea>
                          </v-layout>
                          <v-layout row wrap>
                            <v-switch
                              hide-details
                              color="primary"
                              label="Validations: Previous agreement for times and place of next visit?"
                              v-model="session.sessionSupervisionNote.validation"
                              @change="setDirty"
                            ></v-switch>
                          </v-layout>
                          <v-layout row wrap>
                            <v-flex xs5>
                              <v-text-field
                                label="Next schedule date"
                                box
                                v-model="session.sessionSupervisionNote.nextScheduledDate"
                                mask="##/##/####"
                                return-masked-value
                                @change="setDirty"
                              ></v-text-field>
                            </v-flex>
                          </v-layout>
                        </v-container>
                      </v-card-text>
                    </v-card>
                  </v-card-text>
                </v-card>
              </v-tab-item>
            </template>
            <!-- <v-tab-item v-if="isAdmin" key="logs">
              <v-card flat>
                <v-card-text class="pa-2">
                  <v-container fluid grid-list-sm pa-0>
                    <v-layout row wrap>
                      <v-flex xs12>
                        <v-timeline align-top dense v-if="sessionLogs">
                          <v-timeline-item :color="t.iconColor" :icon="t.icon" fill-dot small v-for="t in sessionLogs" :key="t.sessionLogId">
                            <v-layout pt-3>
                              <v-flex xs3>
                                {{ t.entry | moment("MM/DD/YYYY") }}
                                <br />
                                {{ t.entry | moment("LT") }}
                              </v-flex>
                              <v-flex xs9 sm7>
                                <strong :class="t.iconColor + '--text'">{{ t.title }}</strong>
                                <div class="caption">{{ t.description }}</div>
                                <v-divider></v-divider>
                                <div class="caption">
                                  <small>by {{ t.user.firstname }} {{ t.user.lastname }}</small>
                                </div>
                              </v-flex>
                              <v-flex sm2 class="hidden-xs-only">
                                <span class="caption">
                                  <small>{{ t.entry | moment("from", "now") }}</small>
                                </span>
                              </v-flex>
                            </v-layout>
                          </v-timeline-item>
                        </v-timeline>
                      </v-flex>
                    </v-layout>
                  </v-container>
                </v-card-text>
              </v-card>
            </v-tab-item> -->
          </v-tabs-items>
          <v-card-actions>
            <v-chip v-if="dirtyIndicator" disabled color="orange" text-color="white">
              <v-avatar>
                <v-icon>fa-exclamation-circle</v-icon>
              </v-avatar>
              MODIFIED
            </v-chip>
            <v-chip v-if="matchingCantSave" disabled color="red" text-color="white">
              <v-avatar>
                <v-icon>fa-exclamation-circle</v-icon>
              </v-avatar>
              MATCHING ALERT
            </v-chip>
            <v-spacer></v-spacer>
            <v-btn :disabled="loading" @click="close" flat>{{ editDisabled ? "CLOSE" : "CANCEL" }}</v-btn>
            <v-btn v-if="!editDisabled" :disabled="loading || loadingMatching" :loading="loading" color="primary" @click="save(false)">Save</v-btn>
            <v-btn v-if="!editDisabled" :disabled="loading || loadingMatching" :loading="loading" color="success" @click="save">Save and return</v-btn>
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
    <edit-time ref="editTimeModal" :sessionId="activeSessionId" :model="editTimeModal" @cancel="editTimeModal = false" @onSubmit="onSubmitEditTime"></edit-time>
    <v-dialog width="100%" persistent transition="slide-y-transition" v-model="checkedModal" v-if="session.sessionType === 1">
      <v-card>
        <v-card-title class="warning white--text">
          <div class="title">Warning, please read carefully</div>
        </v-card-title>
        <v-container grid-list-md pa-1>
          <v-layout row wrap>
            <v-flex xs12>
              <v-card-text>
                Before continue, make sure you <strong class="blue--text">already saved all unsaved data</strong> and you <strong class="blue--text">reviewed all notes carefully.</strong>
                <v-divider />
                <v-container fluid grid-list-xs pa-0 v-if="sessionDetailed">
                  <v-layout row wrap>
                    <v-flex xs6>
                      <v-layout row wrap>
                        <v-flex pb-0 pt-1 class="body-2 text-xs-right" xs4>Date:</v-flex>
                        <v-flex pb-0 pt-1 xs8>{{ sessionDetailed.sessionStart | moment("ddd") }}, {{ sessionDetailed.sessionStart | moment("ll") }}</v-flex>
                        <v-flex py-0 class="body-2 text-xs-right" xs4>Time IN:</v-flex>
                        <v-flex py-0 xs8>
                          <v-icon color="green" small>fa-sign-in-alt</v-icon>
                          {{ sessionDetailed.sessionStart | moment("LT") }}
                        </v-flex>
                        <v-flex py-0 class="body-2 text-xs-right" xs4>Time OUT:</v-flex>
                        <v-flex py-0 xs8>
                          <v-icon color="red" small>fa-sign-out-alt</v-icon>
                          {{ sessionDetailed.sessionEnd | moment("LT") }}
                        </v-flex>
                        <v-flex py-0 class="body-2 text-xs-right" xs4>Units:</v-flex>
                        <v-flex py-0 xs8>
                          <v-icon small>fa-star</v-icon>
                          {{ sessionDetailed.totalUnits.toLocaleString() }}
                          <v-icon small>fa-clock</v-icon>
                          {{ (sessionDetailed.totalUnits / 4).toLocaleString() }}
                        </v-flex>
                        <v-flex pt-0 pb-1 class="body-2 text-xs-right" xs4>Pos:</v-flex>
                        <v-flex pt-0 pb-1 xs8
                          ><strong class="red--text pulse">{{ sessionDetailed.pos.toUpperCase() }}</strong></v-flex
                        >
                      </v-layout>
                    </v-flex>
                  </v-layout>
                </v-container>
                <v-divider />
                <v-alert v-if="!progressNoteEmpty" :value="true" color="error" icon="warning">
                  Your progress notes are empty. Sorry, you can't check this session until you fill the progress notes field.
                </v-alert>
                <v-alert v-if="checkedModalInconsistency" :value="true" color="black" outline>
                  There are inconsistencies between the selected place and the progress notes. Please check your note carefully and if everything is correct you can continue.
                  <v-divider />
                  <small class="text--black" v-html="checkedModalInconsistency"></small>
                </v-alert>
                <v-alert v-if="checkedModalProblems.length > 0" :value="true" color="black" outline>
                  There are problems with behaviors missing data.
                  <v-divider />
                  <small v-for="p in checkedModalProblems" :key="p"
                    >-Behavior <strong class="red--text">{{ p }}</strong> has data collected but missing data.<br
                  /></small>
                </v-alert>
                Do you want to change the status of this session to {{ checkedModalType }}?
              </v-card-text>
            </v-flex>
          </v-layout>

          <v-card-actions>
            <v-spacer />
            <v-btn flat @click="checkedModal = false">Cancel</v-btn>
            <v-btn color="primary" :disabled="!progressNoteEmpty || checkedModalProblems.length > 0" @click="onClickCheckedModal">SET AS {{ checkedModalType }}</v-btn>
          </v-card-actions>
        </v-container>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script>
import clientApi from "@/services/api/ClientServices";
import masterTableApi from "@/services/api/MasterTablesServices";
import sessionServicesApi from "@/services/api/SessionServices";
import editTime from "@/components/sessions/EditTime";

export default {
  data() {
    return {
      // activeSessionId: null,
      tabModel: 0,
      loadingSession: false,
      loadingCaregivers: false,
      loadingRiskBehaviorCodes: false,
      loadingParticipationLevelCodes: false,
      session: {
        sessionType: 0,
        sessionNote: {
          caregiverId: null,
          caregiverNote: null,
          riskBehavior: null,
          riskBehaviorCrisisInvolved: false,
          riskBehaviorExplain: null,
          reinforcersEdibles: null,
          reinforcersNonEdibles: null,
          reinforcersOthers: null,
          reinforcersResult: null,
          progressNotes: null,
          participationLevel: null,
          feedbackCaregiver: false,
          feedbackCaregiverExplain: null,
          feedbackOtherServices: false,
          feedbackOtherServicesExplain: null,
          summaryDirectObservation: false,
          summaryObservationFeedback: false,
          summaryImplementedReduction: false,
          summaryImplementedReplacement: false,
          summaryGeneralization: false,
          summaryCommunication: false,
          summaryOther: null,
          caregiverTrainingObservationFeedback: false,
          caregiverTrainingParentCaregiverTraining: false,
          caregiverTrainingCompetencyCheck: false,
          caregiverTrainingOther: null,
          caregiverTrainingSummary: null
        },
        sessionSupervisionNote: {
          workWith: 0
        }
      },
      sessionDetailed: null,
      problemSelected: null,
      caregivers: [],
      riskBehaviorCodes: [],
      participationLevelCodes: [],
      problemsUnique: [],
      loadingSessionMetrics: false,
      sessionSupervisionWorkWithCodes: [],
      sessionSupervisionWorkWithArray: [],
      oversightSessionSupervisionEnum: [],
      editTimeModal: false,
      sessionLogs: [],
      posEnum: [],
      posToEdit: null,
      posEditVisible: false,
      loadingPosEdit: false,
      driveTimeEditVisible: false,
      loadEditDriveTime: false,
      checkedModal: false,
      checkedModalType: null,
      checkedModalInconsistency: null,
      collectBehaviors: [],
      checkedModalProblems: [],
      dirtyIndicator: false,
      loadingMatching: false,
      matching: null
    };
  },

  components: {
    editTime
  },

  computed: {
    activeSessionId() {
      return this.$store.getters.activeSessionId;
    },
    activeClientId() {
      return this.$store.getters.activeClientId;
    },
    loading() {
      return this.loadingRiskBehaviorCodes || this.loadingCaregivers || this.loadingParticipationLevelCodes || this.loadingSession;
    },
    isBilled() {
      return !this.sessionDetailed || this.sessionDetailed.sessionStatusCode === 6;
    },
    isChecked() {
      return !this.sessionDetailed || this.sessionDetailed.sessionStatusCode === 5;
    },
    isReviewed() {
      return !this.sessionDetailed || this.sessionDetailed.sessionStatusCode === 7;
    },
    user() {
      return this.$store.getters.user;
    },
    isAdmin() {
      return this.user.rol2 === "admin";
    },
    isRbt() {
      return this.user.rol2 === "tech";
    },
    isAdminOrLead() {
      return this.user.rol2 === "admin" || this.user.rol2 === "analyst";
    },
    isAdminOrLeadOrAssistant() {
      return this.user.rol2 === "admin" || this.user.rol2 === "analyst" || this.user.rol2 === "assistant";
    },
    editDisabled() {
      return !this.sessionDetailed || (this.sessionDetailed.sessionStatusCode === 5 && !this.isAdmin) || this.sessionDetailed.sessionStatusCode === 6 || this.sessionDetailed.sessionStatusCode === 7;
    },
    isMobile() {
      return this.$vuetify.breakpoint.xs || this.$vuetify.breakpoint.sm;
    },
    progressNoteEmpty() {
      return this.session.sessionNote.progressNotes;
    },
    notAllowed() {
      return this.$store.getters.notAllowed;
    },
    matchingCantSave() {
      return !this.matching || this.matching.percentaje * 100 > 65;
    }
  },

  async mounted() {
    if (!this.activeSessionId) this.close();
    this.posEnum = await masterTableApi.getPosCodes();
    this.loadRiskBehaviorCodes();
    this.loadSessionSupervisionWorkWithCodes();
    this.loadParticipationLevelCodes();
    this.loadCaregivers();
    await this.loadSessionData();
    this.checkMatchingPercentaje();
  },

  beforeRouteLeave(to, from, next) {
    if (this.dirtyIndicator) {
      this.$confirm("There are unsaved changes, are you sure do you want to cancel all editings?").then(async res => {
        if (res) next();
        else next(false);
      });
    } else next();
  },

  methods: {
    async loadSessionData() {
      try {
        this.loadingSession = true;
        let sessionDetailed = await sessionServicesApi.getSessionDetailed(this.activeSessionId);
        let s1 = this.$moment(sessionDetailed.sessionStart).local();
        let s2 = this.$moment(sessionDetailed.sessionEnd).local();
        sessionDetailed.sessionStart = s1;
        sessionDetailed.sessionEnd = s2;
        this.sessionDetailed = sessionDetailed;
        this.sessionLogs = [];
        sessionDetailed.sessionLogs.forEach(s => {
          s.entry = this.$moment(s.entry).local();
          this.sessionLogs.push(s);
        });
        this.session = await sessionServicesApi.getSession(this.activeSessionId);
        if (this.session.sessionType === 3) {
          this.sessionSupervisionWorkWithCodes.forEach(c => {
            const a = (c.value & this.session.sessionSupervisionNote.workWith) != 0;
            this.sessionSupervisionWorkWithArray.push(a ? c.value : 0);
          });
          this.session.sessionSupervisionNote.nextScheduledDate =
            !this.session.sessionSupervisionNote.nextScheduledDate ||
            this.$moment(this.session.sessionSupervisionNote.nextScheduledDate)
              .utc()
              .format("MM/DD/YYYY");
        }
        this.problemsUnique = this.session.sessionProblemNotes.map(m => m.problemBehavior);
        this.collectBehaviors = await sessionServicesApi.getCollectBehaviors(this.activeSessionId);
        // console.log(this.collectBehaviors);
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loadingSession = false;
      }
    },

    async loadCaregivers() {
      try {
        this.loadingCaregivers = true;
        this.caregivers = await clientApi.getClientCaregivers(this.activeClientId);
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loadingCaregivers = false;
      }
    },

    async loadRiskBehaviorCodes() {
      try {
        this.loadingRiskBehaviorCodes = true;
        this.riskBehaviorCodes = await masterTableApi.getRiskBehaviorCodes();
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loadingRiskBehaviorCodes = false;
      }
    },

    async loadSessionSupervisionWorkWithCodes() {
      try {
        this.sessionSupervisionWorkWithCodes = await masterTableApi.getSessionSupervicionWorkWithCodes();
        this.oversightSessionSupervisionEnum = await masterTableApi.getOversightSessionSupervision();
      } catch (error) {
        this.$toast.error(error.message || error);
      }
    },

    async loadParticipationLevelCodes() {
      try {
        this.loadingParticipationLevelCodes = true;
        this.participationLevelCodes = await masterTableApi.getParticipationLevelCodes();
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loadingParticipationLevelCodes = false;
      }
    },

    close() {
      if (this.isAdmin) {
        this.$router.go(-1);
      }
      this.$router.push("/clients/sessions_details");
    },

    async save(exit = true) {
      if (this.session.sessionType === 1) {
        if (this.session.sessionNote.riskBehaviorCrisisInvolved && !this.session.sessionNote.riskBehaviorExplain) {
          this.$toast.error("You need to explain the crisis envolved");
          return;
        }
        if (!this.session.sessionNote.reinforcersResult) {
          this.$toast.error("You need to fill Reinforcers/Result field");
          return;
        }
        let ediblesCount = 0;
        if (!this.session.sessionNote.reinforcersEdibles) ediblesCount++;
        if (!this.session.sessionNote.reinforcersNonEdibles) ediblesCount++;
        if (!this.session.sessionNote.reinforcersOthers) ediblesCount++;
        if (ediblesCount > 1) {
          this.$toast.error("You need to fill at least 2 edibles fields");
          return;
        }
      }

      if (this.session.sessionType === 3) {
        let work = 0;
        this.sessionSupervisionWorkWithArray.forEach(c => {
          work |= c;
        });
        this.session.sessionSupervisionNote.workWith = work;
      }

      let hours = this.$moment.duration(this.$moment().diff(this.session.sessionStart)).asHours();
      if (hours > 48) {
        this.$confirm("This session was created more than 48 hours ago. If you edit it, you going to use a pass. Are you sure you want to edit this session?").then(async res => {
          if (!res) {
            return;
          } else {
            try {
              this.loadingSession = true;
              if (this.session.sessionNote) {
                delete this.session.sessionNote.caregiver;
              } else {
                delete this.session.sessionSupervisionNote.caregiver;
              }
              await sessionServicesApi.editSessionNotes(this.session);
              if (exit) this.close();
              this.$toast.success("Session saved successful");
              this.loadSessionData();
              this.dirtyIndicator = false;
              this.checkMatchingPercentaje();
            } catch (error) {
              this.$toast.error(error.message || error);
            } finally {
              this.loadingSession = false;
            }
          }
        });
      } else {
        try {
          this.loadingSession = true;
          if (this.session.sessionNote) {
            delete this.session.sessionNote.caregiver;
          } else {
            delete this.session.sessionSupervisionNote.caregiver;
          }
          await sessionServicesApi.editSessionNotes(this.session);
          if (exit) this.close();
          this.$toast.success("Session saved successful");
          this.loadSessionData();
          this.dirtyIndicator = false;
          this.checkMatchingPercentaje();
        } catch (error) {
          this.$toast.error(error.message || error);
        } finally {
          this.loadingSession = false;
        }
      }
    },

    async deleteSession() {
      this.$confirm("Do you want to delete this Session?").then(async res => {
        if (res) {
          try {
            if (!this.activeSessionId) return;
            await sessionServicesApi.deleteSession(this.activeSessionId);
            this.close();
          } catch (error) {
            this.$toast.error(error.message || error);
          }
        }
      });
    },

    rejectSession() {
      this.$prompt(null, { title: "Reject note", label: "Reason", textArea: true }).then(async desc => {
        if (desc) {
          if (!this.activeSessionId) return;
          await sessionServicesApi.rejectSession({
            sessionId: this.activeSessionId,
            RejectMessage: desc
          });
          this.close();
        }
      });
    },

    async send2Email() {
      let signPath = this.$router.resolve({
        name: "sign"
      }).href;
      let fullPath = `${window.location.origin}/${signPath}`;
      try {
        this.loadingSession = true;
        const response = await sessionServicesApi.sendUrlSign({ url: fullPath }, this.activeSessionId);
        this.loadSessionData();
        this.$toast.success("Email sent with code: " + response.statusCode);
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loadingSession = false;
      }
    },

    async deleteSign() {
      try {
        this.loadingSession = true;
        await sessionServicesApi.deleteSign(this.activeSessionId);
        this.loadSessionData();
        this.$toast.success("Sign deleted successful");
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loadingSession = false;
      }
    },

    async markAsChecked() {
      this.checkedModalType = "Checked";
      if (this.session.sessionType !== 1) {
        this.onClickCheckedModal();
        return;
      }
      //      const word = this.sessionDetailed.pos == "School" ? "Home" : "School";
      this.checkedModalInconsistency = await this.checkInconsistency();
      this.checkedModalProblems = await this.checkModelProblems();
      this.checkedModal = true;
    },

    async markAsReady2Lead() {
      if (this.matchingCantSave) {
        this.$toast.error("You cannot set Ready to Analyst because progress notes exceed allowed matching percent.");
        return;
      }
      this.$confirm("Are you sure you want to mark this session as Ready to Analyst(Lead)? <br><br><small class='red--text'>*Remember to save the changes first if you have not done so.</small>").then(
        async res => {
          if (res) {
            const model = {
              sessionId: this.activeSessionId,
              sessionStatus: 8 //checked
            };
            try {
              if (!this.sessionDetailed.sign || !this.sessionDetailed.sign.sign) {
                this.$toast.error("You can not check this session without a valid signature");
                return;
              }
              await sessionServicesApi.changeSessionStatus(model);
              this.close();
            } catch (error) {
              this.$toast.error(error);
            }
          }
        }
      );
    },

    async markAsReviewed() {
      this.checkedModalType = "Reviewed";
      if (this.session.sessionType !== 1) {
        this.onClickCheckedModal();
        return;
      }
      //      const word = this.sessionDetailed.pos == "School" ? "Home" : "School";
      this.checkedModalInconsistency = await this.checkInconsistency();
      this.checkedModalProblems = await this.checkModelProblems();
      this.checkedModal = true;
    },

    editTime() {
      //this.$refs.editTimeModal.sessionId = this.activeSessionId;
      this.$refs.editTimeModal.orgTimeStart = this.sessionDetailed.sessionStart;
      this.$refs.editTimeModal.orgTimeEnd = this.sessionDetailed.sessionEnd;
      this.$refs.editTimeModal.timeStart = this.sessionDetailed.sessionStart.format("HH:mm");
      this.$refs.editTimeModal.timeEnd = this.sessionDetailed.sessionEnd.format("HH:mm");
      this.editTimeModal = true;
    },

    onSubmitEditTime() {
      this.editTimeModal = false;
      this.loadSessionData();
    },

    async reopenSession() {
      this.$confirm("Are you sure you want to reopen this session?").then(async res => {
        if (res) {
          const model = {
            sessionId: this.activeSessionId,
            sessionStatus: 4 //reopen
          };
          try {
            await sessionServicesApi.changeSessionStatus(model);
            this.close();
          } catch (error) {
            this.$toast.error(error);
          }
        }
      });
    },

    editPos() {
      this.posToEdit = this.sessionDetailed.posCode;
      this.posEditVisible = true;
    },

    async changeNewPos(pos) {
      this.loadingPosEdit = true;
      const model = {
        id: this.activeSessionId,
        value: pos
      };
      try {
        await sessionServicesApi.editSessionPos(model);
      } catch (error) {
        this.$toast.error("Error changing the POS value. Error: " + error);
      } finally {
        this.loadSessionData();
        this.posEditVisible = false;
        this.loadingPosEdit = false;
      }
    },

    editDriveTime() {
      //this.posToEdit = this.sessionDetailed.posCode;
      this.driveTimeEditVisible = true;
    },

    async submitDriveTime() {
      this.loadEditDriveTime = true;
      const model = {
        id: this.activeSessionId,
        value: this.sessionDetailed.driveTime
      };
      try {
        await sessionServicesApi.editSessionDriveTime(model);
      } catch (error) {
        this.$toast.error("Error changing the driving time value. Error: " + error);
      } finally {
        this.loadSessionData();
        this.driveTimeEditVisible = false;
        this.loadEditDriveTime = false;
      }
    },

    goToData() {
      this.$router.push("/session/session_collect_data");
    },

    async checkInconsistency() {
      if (!this.progressNoteEmpty) return;
      const pos = this.sessionDetailed.pos;
      let notes = this.session.sessionNote.progressNotes.toLowerCase();
      const notAllowed = this.notAllowed.find(s => s.place.toLowerCase() === pos.toLowerCase());
      if (!notAllowed) return;
      const words = notAllowed.words.map(m => m.toLowerCase());
      if (words.some(s => notes.includes(s))) {
        words.forEach(w => {
          notes = notes.replace(new RegExp(w.toLowerCase(), "g"), `<strong class="red--text pulse">${w.toLowerCase()}</strong>`);
        });
      }
      return notes;
    },

    async checkModelProblems() {
      let errors = [];
      this.session.sessionProblemNotes.forEach(s => {
        const p = this.collectBehaviors.find(w => w.problemId == s.problemId);
        if (p && !(p.noData || p.total == 0)) {
          if (!s.duringWichActivities || !s.replacementInterventionsUsed) {
            errors.push(s.problemBehavior.problemBehaviorDescription);
          }
        }
      });
      return errors;
    },

    async onClickCheckedModal() {
      const model = {
        sessionId: this.activeSessionId,
        sessionStatus: this.checkedModalType == "Checked" ? 5 : 7 //checked or reviewed
      };
      try {
        if (!this.sessionDetailed.sign || !this.sessionDetailed.sign.sign) {
          this.$toast.error("You can not check this session without a valid signature");
          return;
        }
        await sessionServicesApi.changeSessionStatus(model);
        this.close();
      } catch (error) {
        this.$toast.error(error);
      }
    },

    getIfBehaviorHasData(problemId) {
      const collection = this.collectBehaviors.find(s => s.problemId === problemId);
      if (collection.noData || collection.total == 0) return "<label class='blue--text'>* No data collected in this behavior</label>";
      return "<label class='red--text'>* Data collected in this behavior, both fields are mandatory.</label>";
    },

    setDirty() {
      this.dirtyIndicator = true;
    },

    async checkMatchingPercentaje() {
      try {
        this.matching = null;
        this.loadingMatching = true;
        const res = await sessionServicesApi.checkMatchingPercentaje(this.activeSessionId);
        this.matching = res.percentaje == 0 ? null : res;
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loadingMatching = false;
      }
    },

    async checkMatching() {
      try {
        this.setDirty();
        this.matching = null;
        this.loadingMatching = true;
        const obj = {
          sessionId: this.activeSessionId,
          progressNotes: this.session.sessionNote.progressNotes
        };
        const res = await sessionServicesApi.checkMatchingPercentajeString(obj);
        this.matching = res.percentaje == 0 ? null : res;
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loadingMatching = false;
      }
    }
  }
};
</script>

<style>
.v-menu__content {
  height: auto !important;
  max-height: 800px !important;
}
</style>
