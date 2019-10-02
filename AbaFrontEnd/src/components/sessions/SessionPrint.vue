<template>
  <session-print :sessionId="activeSessionId"></session-print>
  <!--<v-card>-->
  <!--  <v-card-text class="print-full-width print-font">-->
  <!--    <v-container fluid grid-list-sm pa-0>-->
  <!--      <v-layout row wrap>-->
  <!--        <v-flex xs6>-->
  <!--          <span class="headline">{{ sessionPrint.clientName }}</span>-->
  <!--        </v-flex>-->
  <!--        <v-flex xs6 class="text-xs-right">-->
  <!--          <span class="title grey&#45;&#45;text text&#45;&#45;darken-2">{{ sessionPrint.sessionStart | moment("LL") }}</span>-->
  <!--        </v-flex>-->
  <!--        <v-flex xs12>-->
  <!--          <v-divider></v-divider>-->
  <!--        </v-flex>-->
  <!--        <v-flex xs6>-->
  <!--          <v-layout row wrap>-->
  <!--            <v-flex class="font-weight-medium text-xs-right" xs4>Provider:</v-flex>-->
  <!--            <v-flex xs8>{{ sessionPrint.userFullname }} ({{ sessionPrint.userRol }})</v-flex>-->
  <!--            <v-flex class="font-weight-medium text-xs-right" xs4>Time IN:</v-flex>-->
  <!--            <v-flex xs8>-->
  <!--              <v-icon color="green" small>fa-sign-in-alt</v-icon>-->
  <!--              {{ sessionPrint.sessionStart | moment("LT") }}-->
  <!--            </v-flex>-->
  <!--            <v-flex class="font-weight-medium text-xs-right" xs4>Time OUT:</v-flex>-->
  <!--            <v-flex xs8>-->
  <!--              <v-icon color="red" small>fa-sign-out-alt</v-icon>-->
  <!--              {{ sessionPrint.sessionEnd | moment("LT") }}-->
  <!--            </v-flex>-->
  <!--            <v-flex class="font-weight-medium text-xs-right" xs4>Units:</v-flex>-->
  <!--            <v-flex xs8>-->
  <!--              <v-icon small>fa-star</v-icon>-->
  <!--              {{ sessionPrint.totalUnits.toLocaleString() }}-->
  <!--              <v-icon small>fa-clock</v-icon>-->
  <!--              {{ (sessionPrint.totalUnits / 4).toLocaleString() }}-->
  <!--            </v-flex>-->
  <!--            <v-flex class="font-weight-medium text-xs-right" xs4>Drive time:</v-flex>-->
  <!--            <v-flex xs8>-->
  <!--              <v-icon small>fa-car</v-icon>-->
  <!--              {{ sessionPrint.driveTime.toLocaleString() }}-->
  <!--            </v-flex>-->
  <!--          </v-layout>-->
  <!--        </v-flex>-->
  <!--        <v-flex xs6>-->
  <!--          <v-layout row wrap>-->
  <!--            <v-flex class="font-weight-medium text-xs-right" xs4>Code:</v-flex>-->
  <!--            <v-flex xs8>{{ sessionPrint.clientCode }}</v-flex>-->
  <!--            <v-flex class="font-weight-medium text-xs-right" xs4>Pos:</v-flex>-->
  <!--            <v-flex xs8>-->
  <!--              <span class="text-no-wrap text-truncate">{{ sessionPrint.pos }} ({{ sessionPrint.posNum }})</span>-->
  <!--            </v-flex>-->
  <!--            <v-flex class="font-weight-medium text-xs-right" xs4>Session type:</v-flex>-->
  <!--            <v-flex xs8>{{ sessionPrint.sessionType }}</v-flex>-->
  <!--            <v-flex class="font-weight-medium text-xs-right" xs4>Service:</v-flex>-->
  <!--            <v-flex xs8>{{ sessionPrint.service }} ({{ sessionPrint.serviceDescription }})</v-flex>-->
  <!--          </v-layout>-->
  <!--        </v-flex>-->

  <!--        <v-flex xs12>-->
  <!--          <v-card flat>-->
  <!--            <v-divider></v-divider>-->
  <!--            <table class="table-horizontal" width="100%">-->
  <!--              <tr class="no-page-break">-->
  <!--                <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">Caregiver:</td>-->
  <!--                <td class="pl-1">{{ sessionPrint.caregiver }} ({{ sessionPrint.caregiverType }})</td>-->
  <!--              </tr>-->
  <!--              <tr class="no-page-break">-->
  <!--                <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">Caregiver notes:</td>-->
  <!--                <td class="pl-1" v-html="breakLine(sessionPrint.caregiverNote)"></td>-->
  <!--              </tr>-->
  <!--              <template v-if="sessionPrint.sessionTypeNum === 2">-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%">-->
  <!--                    <v-icon small>{{ sessionPrint.caregiverTraining.caregiverTrainingObservationFeedback ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                  </td>-->
  <!--                  <td class="pl-1">-->
  <!--                    Observation of recipients's interaction with parent/caregiver/others-->
  <!--                  </td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%">-->
  <!--                    <v-icon small>{{ sessionPrint.caregiverTraining.caregiverTrainingParentCaregiverTraining ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                  </td>-->
  <!--                  <td class="pl-1">-->
  <!--                    Parent/Caregiver training-->
  <!--                  </td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%">-->
  <!--                    <v-icon small>{{ sessionPrint.caregiverTraining.caregiverTrainingCompetencyCheck ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                  </td>-->
  <!--                  <td class="pl-1">-->
  <!--                    Competency check of caregiver-->
  <!--                  </td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">Others:</td>-->
  <!--                  <td class="pl-1" v-html="breakLine(sessionPrint.caregiverTraining.caregiverTrainingOther)"></td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">Summary:</td>-->
  <!--                  <td class="pl-1" v-html="breakLine(sessionPrint.caregiverTraining.caregiverTrainingSummary)"></td>-->
  <!--                </tr>-->
  <!--              </template>-->
  <!--              <template v-if="sessionPrint.sessionTypeNum === 3">-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%">-->
  <!--                    <v-icon small>{{ sessionPrint.supervisionNote.isDirectSession ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                  </td>-->
  <!--                  <td class="pl-1">-->
  <!--                    Direct session-->
  <!--                  </td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">Work with:</td>-->
  <!--                  <td class="pl-1">-->
  <!--                    <span v-for="w in sessionSupervisionWorkWithCodes" :key="'w-' + w.text"> {{ w.text }} | </span>-->
  <!--                  </td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">-->
  <!--                    Summary:-->
  <!--                  </td>-->
  <!--                  <td class="pl-1">-->
  <!--                    <v-icon small>{{ sessionPrint.supervisionNote.briefObservation ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                    Observation and feedback regarding interaction of BCaBA<br/>-->
  <!--                    <v-icon small>{{ sessionPrint.supervisionNote.briefReplacement ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                    Replacement/Acquisition Programs Implementation monitoring<br/>-->
  <!--                    <v-icon small>{{ sessionPrint.supervisionNote.briefGeneralization ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                    Generalization of Treatment<br/>-->
  <!--                    <v-icon small>{{ sessionPrint.supervisionNote.briefBCaBaTraining ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                    BCaBA training and oversight<br/>-->
  <!--                    <v-icon small>{{ sessionPrint.supervisionNote.briefInService ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                    In-Service Education Training (for session participants)<br/>-->
  <!--                    <div style="padding-left: 20px;">Subject: {{ sessionPrint.supervisionNote.briefInServiceSubject || "N/A" }}</div>-->
  <!--                    <v-icon small>{{ sessionPrint.supervisionNote.briefOther ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                    Other<br/>-->
  <!--                    <div style="padding-left: 20px;" v-html="breakLine(sessionPrint.supervisionNote.briefOtherDescription)"></div>-->
  <!--                  </td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">-->
  <!--                    Oversight:-->
  <!--                  </td>-->
  <!--                  <td class="pl-1">-->
  <!--                    <v-layout row wrap>-->
  <!--                      <v-flex xs8>-->
  <!--                        <v-icon small>{{ sessionPrint.supervisionNote.oversightFollowUpBool ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                        Follow up upon recommendations from previous reassessment-->
  <!--                      </v-flex>-->
  <!--                      <v-flex xs4 text-xs-right>-->
  <!--                        <strong>{{ sessionPrint.supervisionNote.oversightFollowUp }}</strong>-->
  <!--                      </v-flex>-->
  <!--                    </v-layout>-->
  <!--                    <v-layout row wrap>-->
  <!--                      <v-flex xs8>-->
  <!--                        <v-icon small>{{ sessionPrint.supervisionNote.oversightDesigningBool ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                        Designing, implementing and monitoring program for client-->
  <!--                      </v-flex>-->
  <!--                      <v-flex xs4 text-xs-right>-->
  <!--                        <strong>{{ sessionPrint.supervisionNote.oversightDesigning }}</strong>-->
  <!--                      </v-flex>-->
  <!--                    </v-layout>-->
  <!--                    <v-layout row wrap>-->
  <!--                      <v-flex xs8>-->
  <!--                        <v-icon small>{{ sessionPrint.supervisionNote.oversightContributingBool ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                        Contributing with Behavioral Assessment-->
  <!--                      </v-flex>-->
  <!--                      <v-flex xs4 text-xs-right>-->
  <!--                        <strong>{{ sessionPrint.supervisionNote.oversightContributing }}</strong>-->
  <!--                      </v-flex>-->
  <!--                    </v-layout>-->
  <!--                    <v-layout row wrap>-->
  <!--                      <v-flex xs8>-->
  <!--                        <v-icon small>{{ sessionPrint.supervisionNote.oversightAnalyzingBool ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                        Analyzing data-->
  <!--                      </v-flex>-->
  <!--                      <v-flex xs4 text-xs-right>-->
  <!--                        <strong>{{ sessionPrint.supervisionNote.oversightAnalyzing }}</strong>-->
  <!--                      </v-flex>-->
  <!--                    </v-layout>-->
  <!--                    <v-layout row wrap>-->
  <!--                      <v-flex xs8>-->
  <!--                        <v-icon small>{{ sessionPrint.supervisionNote.oversightGoalsBool ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                        Goals progress evidenced in client performance-->
  <!--                      </v-flex>-->
  <!--                      <v-flex xs4 text-xs-right>-->
  <!--                        <strong>{{ sessionPrint.supervisionNote.oversightGoals }}</strong>-->
  <!--                      </v-flex>-->
  <!--                    </v-layout>-->
  <!--                    <v-layout row wrap>-->
  <!--                      <v-flex xs8>-->
  <!--                        <v-icon small>{{ sessionPrint.supervisionNote.oversightMakingDecisionsBool ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                        Making decisions about progress-->
  <!--                      </v-flex>-->
  <!--                      <v-flex xs4 text-xs-right>-->
  <!--                        <strong>{{ sessionPrint.supervisionNote.oversightMakingDecisions }}</strong>-->
  <!--                      </v-flex>-->
  <!--                    </v-layout>-->
  <!--                    <v-layout row wrap>-->
  <!--                      <v-flex xs8>-->
  <!--                        <v-icon small>{{ sessionPrint.supervisionNote.oversightModelingBool ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                        Modeling technical, professional and ethical behavior-->
  <!--                      </v-flex>-->
  <!--                      <v-flex xs4 text-xs-right>-->
  <!--                        <strong>{{ sessionPrint.supervisionNote.oversightModeling }}</strong>-->
  <!--                      </v-flex>-->
  <!--                    </v-layout>-->
  <!--                    <v-layout row wrap>-->
  <!--                      <v-flex xs8>-->
  <!--                        <v-icon small>{{ sessionPrint.supervisionNote.oversightResponseBool ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                        Response to feedback from Lead Analyst-->
  <!--                      </v-flex>-->
  <!--                      <v-flex xs4 text-xs-right>-->
  <!--                        <strong>{{ sessionPrint.supervisionNote.oversightResponse }}</strong>-->
  <!--                      </v-flex>-->
  <!--                    </v-layout>-->
  <!--                    <v-layout row wrap>-->
  <!--                      <v-flex xs8>-->
  <!--                        <v-icon small>{{ sessionPrint.supervisionNote.oversightOverallBool ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                        Overall Evaluation in session-->
  <!--                      </v-flex>-->
  <!--                      <v-flex xs4 text-xs-right>-->
  <!--                        <strong>{{ sessionPrint.supervisionNote.oversightOverall }}</strong>-->
  <!--                      </v-flex>-->
  <!--                    </v-layout>-->
  <!--                  </td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">-->
  <!--                    Comments related:-->
  <!--                  </td>-->
  <!--                  <td class="pl-1" v-html="breakLine(sessionPrint.supervisionNote.commentsRelated || 'N/A')"></td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">-->
  <!--                    Recommendations:-->
  <!--                  </td>-->
  <!--                  <td class="pl-1" v-html="breakLine(sessionPrint.supervisionNote.Recommendations || 'N/A')"></td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%">-->
  <!--                    <v-icon small>{{ sessionPrint.supervisionNote.validation ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                  </td>-->
  <!--                  <td class="pl-1">-->
  <!--                    Validations: Previous agreement for times and place of next visit?-->
  <!--                  </td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">Next schedule:</td>-->
  <!--                  <td class="pl-1">-->
  <!--                    <span v-if="sessionPrint.supervisionNote.nextScheduledDate">{{ sessionPrint.supervisionNote.nextScheduledDate | moment("utc", "LL") }}</span>-->
  <!--                    <span v-else>N/A</span>-->
  <!--                  </td>-->
  <!--                </tr>-->
  <!--              </template>-->
  <!--              <template v-if="sessionPrint.sessionTypeNum === 1">-->
  <!--                <tr class="no-page-break grey lighten-3">-->
  <!--                  <td class="font-weight-medium text-xs-right">RISK BEHAVIOR</td>-->
  <!--                  <td></td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">Risk behavior:</td>-->
  <!--                  <td class="pl-1">-->
  <!--                    {{ sessionPrint.sessionNote.riskBehavior }}-->
  <!--                  </td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">-->
  <!--                    <v-icon small>{{ sessionPrint.sessionNote.crisisInvolved ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                  </td>-->
  <!--                  <td class="pl-1">-->
  <!--                    Crisis involved<br/>-->
  <!--                    <div v-html="breakLine(sessionPrint.sessionNote.crisisInvolvedExplain)"></div>-->
  <!--                  </td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break grey lighten-3">-->
  <!--                  <td class="font-weight-medium text-xs-right">REINFORCERS</td>-->
  <!--                  <td></td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">-->
  <!--                    Edibles:-->
  <!--                  </td>-->
  <!--                  <td class="pl-1" v-html="breakLine(sessionPrint.sessionNote.reinforcersEdibles || 'N/A')"></td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">-->
  <!--                    Non-edibles:-->
  <!--                  </td>-->
  <!--                  <td class="pl-1" v-html="breakLine(sessionPrint.sessionNote.reinforcersNonEdibles || 'N/A')"></td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">-->
  <!--                    Others:-->
  <!--                  </td>-->
  <!--                  <td class="pl-1" v-html="breakLine(sessionPrint.sessionNote.reinforcersOthers || 'N/A')"></td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">-->
  <!--                    Result:-->
  <!--                  </td>-->
  <!--                  <td class="pl-1" v-html="breakLine(sessionPrint.sessionNote.reinforcersResult || 'N/A')"></td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break grey lighten-3">-->
  <!--                  <td class="font-weight-medium text-xs-right">PROGRESS</td>-->
  <!--                  <td></td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">Participation level:</td>-->
  <!--                  <td class="pl-1">-->
  <!--                    {{ sessionPrint.sessionNote.participationLevel }}-->
  <!--                  </td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">-->
  <!--                    Progress notes:-->
  <!--                  </td>-->
  <!--                  <td class="pl-1" v-html="breakLine(sessionPrint.sessionNote.progressNotes || 'N/A')"></td>-->
  <!--                </tr>-->
  <!--                <template v-if="sessionPrint.userRolShort != 'tech'">-->
  <!--                  <tr class="no-page-break grey lighten-3">-->
  <!--                    <td class="font-weight-medium text-xs-right">SUMMARY</td>-->
  <!--                    <td></td>-->
  <!--                  </tr>-->
  <!--                  <tr class="no-page-break">-->
  <!--                    <td class="font-weight-medium text-xs-right" width="20%">-->
  <!--                      <v-icon small>{{ sessionPrint.sessionNote.summaryDirectObservation ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                    </td>-->
  <!--                    <td class="pl-1">-->
  <!--                      Direct observation/Data collection/Probing-->
  <!--                    </td>-->
  <!--                  </tr>-->
  <!--                  <tr class="no-page-break">-->
  <!--                    <td class="font-weight-medium text-xs-right" width="20%">-->
  <!--                      <v-icon small>{{ sessionPrint.sessionNote.summaryObservationFeedback ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                    </td>-->
  <!--                    <td class="pl-1">-->
  <!--                      Observation of recipients's interaction with parent/caregiver/others-->
  <!--                    </td>-->
  <!--                  </tr>-->
  <!--                  <tr class="no-page-break">-->
  <!--                    <td class="font-weight-medium text-xs-right" width="20%">-->
  <!--                      <v-icon small>{{ sessionPrint.sessionNote.summaryImplementedReduction ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                    </td>-->
  <!--                    <td class="pl-1">-->
  <!--                      Implemented reduction programs-->
  <!--                    </td>-->
  <!--                  </tr>-->
  <!--                  <tr class="no-page-break">-->
  <!--                    <td class="font-weight-medium text-xs-right" width="20%">-->
  <!--                      <v-icon small>{{ sessionPrint.sessionNote.summaryImplementedReplacement ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                    </td>-->
  <!--                    <td class="pl-1">-->
  <!--                      Implemented replacement programs-->
  <!--                    </td>-->
  <!--                  </tr>-->
  <!--                  <tr class="no-page-break">-->
  <!--                    <td class="font-weight-medium text-xs-right" width="20%">-->
  <!--                      <v-icon small>{{ sessionPrint.sessionNote.summaryGeneralization ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                    </td>-->
  <!--                    <td class="pl-1">-->
  <!--                      Generalization of treatment-->
  <!--                    </td>-->
  <!--                  </tr>-->
  <!--                  <tr class="no-page-break">-->
  <!--                    <td class="font-weight-medium text-xs-right" width="20%">-->
  <!--                      <v-icon small>{{ sessionPrint.sessionNote.summaryCommunication ? "fa-check-circle" : "far fa-circle" }}</v-icon>-->
  <!--                    </td>-->
  <!--                    <td class="pl-1">-->
  <!--                      Communication/Coordination of care with other services professionals-->
  <!--                    </td>-->
  <!--                  </tr>-->
  <!--                  <tr class="no-page-break">-->
  <!--                    <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">-->
  <!--                      Others:-->
  <!--                    </td>-->
  <!--                    <td class="pl-1" v-html="breakLine(sessionPrint.sessionNote.summaryOther || 'N/A')"></td>-->
  <!--                  </tr>-->
  <!--                </template>-->
  <!--                <tr class="no-page-break grey lighten-3">-->
  <!--                  <td class="font-weight-medium text-xs-right">PROB/REPLAC</td>-->
  <!--                  <td></td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break" v-for="p in sessionPrint.sessionNote.problems" :key="p.problem">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">{{ p.problem }}:</td>-->
  <!--                  <td class="pl-1">-->
  <!--                    <table class="table-print">-->
  <!--                      <tr>-->
  <!--                        <th width="50%">During which activity ocurred</th>-->
  <!--                        <th width="50%">Replacements and intervention used</th>-->
  <!--                      </tr>-->
  <!--                      <tr>-->
  <!--                        <td width="50%" v-html="breakLine(p.duringWichActivities || 'N/A')"></td>-->
  <!--                        <td width="50%">-->
  <!--                          <div v-html="breakLine(p.replacementInterventionsUsed || 'N/A')"></div>-->
  <!--                          <v-divider class="pb-1"></v-divider>-->
  <!--                          <template v-for="r in p.sessionProblemReplacements">-->
  <!--                            <v-icon small :key="p + r">fa-check-circle</v-icon>-->
  <!--                            {{ r }}<br :key="'br' + p + r"/>-->
  <!--                          </template>-->
  <!--                        </td>-->
  <!--                      </tr>-->
  <!--                    </table>-->
  <!--                  </td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break grey lighten-3">-->
  <!--                  <td class="font-weight-medium text-xs-right">DATA COLLECTION</td>-->
  <!--                  <td></td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">-->
  <!--                    Behaviors:-->
  <!--                  </td>-->
  <!--                  <td class="pl-1">-->
  <!--                    <table class="table-print">-->
  <!--                      <tr v-for="b in collection.collectBehaviors" :key="b.sessionCollectBehaviorV2Id">-->
  <!--                        <td width="50%">{{ b.behavior.problemBehaviorDescription }}</td>-->
  <!--                        <td width="50%">-->
  <!--                          {{ getTotalBehaviors(b) }}-->
  <!--                        </td>-->
  <!--                      </tr>-->
  <!--                    </table>-->
  <!--                  </td>-->
  <!--                </tr>-->
  <!--                <tr class="no-page-break">-->
  <!--                  <td class="font-weight-medium text-xs-right" width="20%" style="vertical-align: top;">-->
  <!--                    Replacements:-->
  <!--                  </td>-->
  <!--                  <td class="pl-1">-->
  <!--                    <table class="table-print">-->
  <!--                      <tr v-for="b in collection.collectReplacements" :key="b.sessionCollectReplacementV2Id">-->
  <!--                        <td width="50%">{{ b.replacement.replacementProgramDescription }}</td>-->
  <!--                        <td width="50%">-->
  <!--                          {{ getTotalReplacement(b) }}-->
  <!--                        </td>-->
  <!--                      </tr>-->
  <!--                    </table>-->
  <!--                  </td>-->
  <!--                </tr>-->
  <!--              </template>-->
  <!--            </table>-->
  <!--          </v-card>-->
  <!--        </v-flex>-->
  <!--        <v-layout row wrap>-->
  <!--          <v-flex class="pt-5" text-xs-center xs6>-->
  <!--            <div v-if="!sessionPrint.userSign">-->
  <!--              <div style="min-height: 100px;" class="mb-2"></div>-->
  <!--              <v-divider></v-divider>-->
  <!--              {{ sessionPrint.userFullname }}<br/>-->
  <!--              <small>{{ sessionPrint.userLicense }}</small>-->
  <!--            </div>-->
  <!--            <div v-else>-->
  <!--              <div style="max-height: 50px; height: 50px;" class="mb-2">-->
  <!--                <img style="height:100%;" :src="sessionPrint.userSign.sign"/>-->
  <!--              </div>-->
  <!--              <v-divider></v-divider>-->
  <!--              {{ sessionPrint.userFullname }}<br/>-->
  <!--              <small>{{ sessionPrint.userLicense }}</small>-->
  <!--            </div>-->
  <!--          </v-flex>-->
  <!--          <v-flex class="pt-5" text-xs-center xs6 v-if="hasLeadSign">-->
  <!--            <div v-if="!sessionExtraInfo.lead.userSign">-->
  <!--              <div style="min-height: 100px;" class="mb-2"></div>-->
  <!--              <v-divider></v-divider>-->
  <!--              {{ sessionExtraInfo.lead.firstname }} {{ sessionExtraInfo.lead.lastname }}<br/>-->
  <!--              <small>{{ sessionExtraInfo.lead.licenseNo }}</small>-->
  <!--            </div>-->
  <!--            <div v-else>-->
  <!--              <div style="max-height: 50px; height: 50px;" class="mb-2">-->
  <!--                <img style="height:100%" :src="sessionExtraInfo.lead.userSign.sign"/>-->
  <!--              </div>-->
  <!--              <v-divider></v-divider>-->
  <!--              {{ sessionExtraInfo.lead.firstname }} {{ sessionExtraInfo.lead.lastname }}<br/>-->
  <!--              <small>{{ sessionExtraInfo.lead.licenseNo }}</small>-->
  <!--            </div>-->
  <!--          </v-flex>-->
  <!--        </v-layout>-->
  <!--      </v-layout>-->
  <!--    </v-container>-->
  <!--  </v-card-text>-->
  <!--</v-card>-->
</template>

<script>
//import sessionServicesApi from "@/services/api/SessionServices";
//import masterTableApi from "@/services/api/MasterTablesServices";
import SessionPrint from "@/components/shared/SessionPrint";

export default {
  components: {
    SessionPrint
  },

  computed: {
    activeSessionId() {
      return this.$store.getters.activeSessionId;
    }
    //    hasLeadSign() {
    //      return this.sessionPrint.userId !== this.sessionExtraInfo.lead.userId;
    //    }
  },

  data() {
    return {
      loadingSession: false
      //      sessionPrint: {
      //        clientName: null,
      //        sessionStart: this.$moment(),
      //        sessionEnd: this.$moment(),
      //        userFullname: null,
      //        userRol: null,
      //        totalUnits: 0,
      //        clientCode: null,
      //        pos: null,
      //        posNum: 0,
      //        sessionType: null,
      //        sessionTypeNum: 0,
      //        service: null,
      //        serviceDescription: null,
      //        driveTime: 0,
      //        caregiver: null,
      //        caregiverType: null,
      //        caregiverNote: null,
      //        caregiverTraining: {
      //          caregiverTrainingObservationFeedback: false,
      //          caregiverTrainingParentCaregiverTraining: false,
      //          caregiverTrainingCompetencyCheck: false,
      //          caregiverTrainingOther: null,
      //          CaregiverTrainingSummary: null
      //        },
      //        supervisionNote: {
      //          isDirectSession: false,
      //          workWith: 0,
      //          briefObservation: false,
      //          briefReplacement: false,
      //          briefGeneralization: false,
      //          briefBCaBaTraining: false,
      //          briefInService: false,
      //          briefInServiceSubject: null,
      //          briefOther: false,
      //          briefOtherDescription: null
      //        },
      //        sessionNote: {
      //          riskBehavior: null
      //        }
      //      },
      //      collection: {
      //        collectBehaviors: [],
      //        collectReplacements: []
      //      },
      //      sessionSupervisionWorkWithCodes: [],
      //      sessionExtraInfo: {
      //        lead: {
      //          userId: null
      //        }
      //      }
    };
  }

  //  mounted() {
  //    if (!this.activeSessionId) this.close();
  //    this.loadSessionSupervisionWorkWithCodes();
  //    this.loadSessionData();
  //  },

  //  methods: {
  //    async loadSessionData() {
  //      try {
  //        this.loadingSession = true;
  //
  //        let sessionPrint = await sessionServicesApi.getSessionForPrint(this.activeSessionId);
  //        let d1 = this.$moment(sessionPrint.sessionStart).local();
  //        let d2 = this.$moment(sessionPrint.sessionEnd).local();
  //        sessionPrint.sessionStart = d1;
  //        sessionPrint.sessionEnd = d2;
  //        this.sessionPrint = sessionPrint;
  //        if (this.sessionPrint.sessionTypeNum === 3) {
  //          this.sessionSupervisionWorkWithCodes.forEach(c => {
  //            const a = (c.value & this.sessionPrint.supervisionNote.workWith) != 0;
  //            c.active = a;
  //          });
  //          this.sessionSupervisionWorkWithCodes = this.sessionSupervisionWorkWithCodes.filter(w => w.active);
  //        }
  //        this.collection.collectBehaviors = await sessionServicesApi.getCollectBehaviors(this.activeSessionId);
  //        this.collection.collectReplacements = await sessionServicesApi.getCollectReplacements(this.activeSessionId);
  //        this.sessionExtraInfo = await sessionServicesApi.getSessionPrintExtraInfo(this.activeSessionId);
  //      } catch (error) {
  //        this.$toast.error(error.message || error);
  //      } finally {
  //        this.loadingSession = false;
  //      }
  //    },
  //
  //    async loadSessionSupervisionWorkWithCodes() {
  //      try {
  //        this.sessionSupervisionWorkWithCodes = await masterTableApi.getSessionSupervicionWorkWithCodes();
  //      } catch (error) {
  //        this.$toast.error(error.message || error);
  //      }
  //    },
  //
  //    getTotalBehaviors(b) {
  //      if (b.noData) return "No data collected";
  //      if (!b.behavior.isPercent) return b.total;
  //      let total = b.total;
  //      let completed = b.completed != null ? b.completed : 0;
  //      let percent = total == 0 ? 0 : (completed / total) * 100;
  //      return `${percent.toFixed(0)}%`;
  //    },
  //
  //    getTotalReplacement(c) {
  //      if (c.noData) return "No data collected";
  //      let total = c.total;
  //      let completed = c.completed;
  //      let percent = total == 0 ? 0 : (completed / total) * 100;
  //      return `${percent.toFixed(0)}%`;
  //    },
  //
  //    breakLine(s) {
  //      return !s ? null : s.replace(/\n/g, "<br/>");
  //    }
  //  }
};
</script>
