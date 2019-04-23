<template>
  <v-container fluid grid-list-xs pa-0>
    <v-layout row wrap>
      <v-flex xs12>
        <v-card height="500">
          <v-progress-linear style="position: absolute;" v-show="loadingBasicInfo" :indeterminate="true" class="ma-0"></v-progress-linear>
          <v-tabs dark v-model="activeTabBasic" show-arrows>
            <v-tab key="basic-info">Basic Info</v-tab>
            <v-tab key="notes">Notes</v-tab>
            <v-tab key="caregivers" v-if="clientManagementAutorized">Caregivers</v-tab>
            <v-tab key="referral" v-if="clientManagementAutorized">Referrals</v-tab>
            <v-tab key="assessment" v-if="clientManagementAutorized">Authorizations</v-tab>
            <v-tab key="assignment" v-if="clientManagementAutorized">Staff</v-tab>
            <v-tab key="diagnosis" v-if="clientManagementAutorized">Diagnosis</v-tab>
            <v-spacer />
            <v-btn v-show="(activeTabBasic == 0 || activeTabBasic == 1) && clientManagementAutorized" dark flat :to="`/clients/add_edit/${id}`">
              <v-icon left>fa-edit</v-icon>
              EDIT
            </v-btn>

            <v-menu :close-on-content-click="false" v-model="addEditCaregiverMenu" bottom left min-width="600">
              <v-btn v-show="activeTabBasic==2" dark flat @click="addNewCaregiver" slot="activator">
                <v-icon left>fa-user-shield</v-icon>
                ADD
              </v-btn>
              <v-card>
                <v-card-text class="pa-2">
                  <v-form ref="form" autocomplete="off" v-model="caregiverMenuFormValid">
                    <v-container grid-list-md pa-0>
                      <v-layout row wrap>
                        <v-flex sm8>
                          <v-text-field box :disabled="loadingCaregiverForm" label="Fullname" v-model="caregiverForm.caregiverFullname" required prepend-icon="fa-tag" :rules="[required]"></v-text-field>
                        </v-flex>
                        <v-flex sm4>
                          <v-select box :disabled="loadingCaregiverForm" label="Relationship" v-model="caregiverType" required :items="caregiversTypes" item-text="description" item-value="caregiverTypeId" prepend-icon="fa-heart" :rules="[required]"></v-select>
                        </v-flex>
                      </v-layout>
                      <v-layout row wrap>
                        <v-flex sm8>
                          <v-text-field box :disabled="loadingCaregiverForm" label="Email" v-model="caregiverForm.email" type="email" prepend-icon="fa-envelope" data-vv-name="email" :rules="errors.collect('email')" v-validate="'email'"></v-text-field>
                        </v-flex>
                        <v-flex sm4>
                          <v-text-field box :disabled="loadingCaregiverForm" label="Phone" v-model="caregiverForm.phone" prepend-icon="fa-phone" type="phone" mask="phone"></v-text-field>
                        </v-flex>
                      </v-layout>
                    </v-container>
                  </v-form>
                </v-card-text>
                <v-card-actions>
                  <v-spacer></v-spacer>
                  <v-btn :disabled="loadingCaregiverForm" flat @click="close">Cancel</v-btn>
                  <v-btn :disabled="loadingCaregiverForm || !caregiverMenuFormValid" :loading="loadingCaregiverForm" color="primary" @click="submitCaregiverForm">Submit</v-btn>
                </v-card-actions>
              </v-card>
            </v-menu>

            <v-btn v-show="activeTabBasic == 3" dark flat @click="addReferral">
              <v-icon left>fa-user-md</v-icon>
              ADD
            </v-btn>
            <v-btn v-show="activeTabBasic == 4" dark flat @click="addAssessment">
              <v-icon left>fa-briefcase-medical</v-icon>
              ADD
            </v-btn>
            <v-btn v-show="activeTabBasic == 5" dark flat @click="addAssignment">
              <v-icon left>fa-user-tie</v-icon>
              ADD
            </v-btn>
            <v-btn v-show="activeTabBasic == 6" dark flat @click="addDiagnosis">
              <v-icon left>fa-stethoscope</v-icon>
              ADD
            </v-btn>
            <v-tab-item key="basic-info">
              <v-card flat>
                <v-card-title class="pb-0">
                  <h3 class="headline">{{client.firstname}} {{client.lastname}}</h3>
                </v-card-title>
                <v-card-text>
                  <v-container fluid grid-list-md pa-0>
                    <v-layout row wrap>
                      <v-flex xs12 sm6>
                        <v-layout row wrap>
                          <v-flex class="body-2" xs4>Medical record:</v-flex>
                          <v-flex xs8>{{client.code || "N/A"}}</v-flex>
                          <v-flex class="body-2" xs4>Nickname:</v-flex>
                          <v-flex xs8>{{client.nickname || "N/A"}}</v-flex>
                          <v-flex class="body-2" xs4>Dob:</v-flex>
                          <v-flex xs8>{{client.dob || new Date() | moment("utc","MMM Do, YYYY")}}</v-flex>
                          <v-flex class="body-2" xs4>Phone:</v-flex>
                          <v-flex xs8>{{client.phone | phone}}</v-flex>
                          <v-flex class="body-2" xs4>Email:</v-flex>
                          <v-flex xs8>{{client.email || "N/A"}}</v-flex>
                          <v-flex class="body-2" xs4>Address:</v-flex>
                          <v-flex xs8>{{client.address || "N/A"}}</v-flex>
                          <v-flex class="body-2" xs4>Apt:</v-flex>
                          <v-flex xs8>{{client.apt || "N/A"}}</v-flex>
                          <v-flex class="body-2" xs4>City:</v-flex>
                          <v-flex xs8>{{client.city || "N/A"}}</v-flex>
                          <v-flex class="body-2" xs4>State/Zip:</v-flex>
                          <v-flex xs8>{{client.state}} {{client.zipcode}}</v-flex>
                          <v-flex class="body-2" xs4>Gender:</v-flex>
                          <v-flex xs8>{{client.gender || "N/A"}}</v-flex>
                          <v-flex class="body-2" xs4>Race:</v-flex>
                          <v-flex xs8>{{client.race || "N/A"}}</v-flex>
                        </v-layout>
                      </v-flex>
                      <v-flex xs12 sm6 class="hidden-xs-only">
                        <v-layout row wrap>
                          <v-flex class="body-2" xs4>Language:</v-flex>
                          <v-flex xs8>{{client.primaryLanguage}}</v-flex>
                          <v-flex class="body-2" xs4>Emerg contact:</v-flex>
                          <v-flex xs8>{{client.emergencyContact || "N/A"}}</v-flex>
                          <v-flex class="body-2" xs4>Emerg email:</v-flex>
                          <v-flex xs8>{{client.emergencyEmail || "N/A"}}</v-flex>
                          <v-flex class="body-2" xs4>Emerg phone:</v-flex>
                          <v-flex xs8>{{client.emergencyPhone | phone}}</v-flex>
                          <v-flex class="body-2" xs4>SS:</v-flex>
                          <v-flex xs8>{{client.socialSecurity | social-security}}</v-flex>
                          <v-flex class="body-2" xs4>Insurance:</v-flex>
                          <v-flex xs8>{{client.insurance || "N/A"}}</v-flex>
                          <v-flex class="body-2" xs4>Medicaid ID:</v-flex>
                          <v-flex xs8>{{client.memberNo || "N/A"}}</v-flex>
                          <v-flex class="body-2" xs4>MMA Plan:</v-flex>
                          <v-flex xs8>{{client.mmaPlan || "N/A"}}</v-flex>
                          <v-flex class="body-2" xs4>MMA Id No:</v-flex>
                          <v-flex xs8>{{client.mmaIdNo || "N/A"}}</v-flex>
                          <v-flex class="body-2" xs4>Created:</v-flex>
                          <v-flex xs8>{{client.created || new Date() | moment("MMM Do, YYYY")}}</v-flex>
                          <v-flex class="body-2" xs4>Last edit:</v-flex>
                          <v-flex xs8>{{client.modified || new Date() | moment("from", "now")}}</v-flex>
                          <v-flex class="body-2" xs4>Active:</v-flex>
                          <v-flex xs8>{{client.active ? "YES" : "NO"}}</v-flex>
                        </v-layout>
                      </v-flex>
                    </v-layout>
                  </v-container>
                </v-card-text>
              </v-card>
            </v-tab-item>
            <v-tab-item key="notes">
              <v-layout wrap row>
                <v-flex xs12>
                  <v-card flat>
                    <v-card-text>
                      <span v-if="client.notes">{{client.notes}}</span>
                      <v-alert v-else value="true" type="warning">
                        <span class="body-2">NO NOTES</span>
                      </v-alert>
                    </v-card-text>
                  </v-card>
                </v-flex>
              </v-layout>
            </v-tab-item>
            <v-tab-item key="caregivers">
              <v-layout wrap row>
                <v-flex xs12>
                  <v-card flat class="scroll-y" height="440">
                    <v-card-text class="pa-1">
                      <table v-if="!client.caregivers || client.caregivers.length !== 0" class="v-datatable v-table theme--light">
                        <tbody>
                          <tr v-for="item in client.caregivers" :key="item.caregiverId">
                            <td class="text-xs-left px-1" style="width: 60px;">
                              <v-avatar>
                                <v-icon medium>fa-user-shield</v-icon>
                              </v-avatar>
                            </td>
                            <td class="text-xs-left px-1" style="width: 220px;">
                              <strong class="body-2">{{item.caregiverFullname}}</strong>
                              <br>
                              <span>
                                <v-icon small>fa-phone</v-icon>
                                {{item.phone | phone}}
                              </span>
                            </td>
                            <td class="text-xs-left px-1 hidden-xs-only">
                              <v-chip label color="indigo" text-color="white">{{item.caregiverType.description}}</v-chip>
                            </td>
                            <td class="text-xs-right px-1 text-no-wrap">
                              <v-btn icon @click="editCaregiver(item)" class="ma-0">
                                <v-icon color="grey">fa-edit</v-icon>
                              </v-btn>
                              <v-btn icon @click="deleteCaregiver(item)" class="ma-0">
                                <v-icon color="grey">fa-trash</v-icon>
                              </v-btn>
                            </td>
                          </tr>
                        </tbody>
                      </table>
                      <v-alert v-else value="true" type="error">
                        <span class="body-2">NO CAREGIVERS</span>
                      </v-alert>
                    </v-card-text>
                  </v-card>
                </v-flex>
              </v-layout>
            </v-tab-item>
            <v-tab-item key="referral">
              <v-card flat class="scroll-y" height="440">
                <v-card-text class="pa-1">
                  <table v-if="!client.referrals || client.referrals.length !== 0" class="v-datatable v-table theme--light">
                    <tbody>
                      <tr v-for="item in client.referrals" :key="item.referralId" :class="{'red lighten-5 grey--text': !item.active}">
                        <td class="text-xs-left px-1" style="width: 60px;">
                          <v-avatar>
                            <v-icon large>fa-user-md</v-icon>
                          </v-avatar>
                        </td>
                        <td class="text-xs-left px-1" style="width: 220px;">
                          <strong class="body-2">{{item.referralFullname}}</strong>
                          <br>
                          <span>{{item.provider}}</span>
                        </td>
                        <td class="text-xs-left px-1 hidden-xs-only">
                          <v-chip small label color="indigo" text-color="white">{{item.specialty}}</v-chip>
                        </td>
                        <td class="text-xs-left px-1 hidden-xs-only">
                          <strong class="body-2">Expires: {{item.dateExpires | moment("utc", "MMM Do, YYYY")}}</strong>
                          <br>
                          <span>{{item.dateExpires | moment("utc", "from", "now")}}</span>
                        </td>
                        <td class="text-xs-center px-0">
                          <v-switch hide-details color="primary" v-model="item.active" @change="changeReferralActive(item)"></v-switch>
                        </td>
                        <td class="text-xs-right px-1 text-no-wrap">
                          <v-btn icon @click="editReferral(item)" class="ma-0">
                            <v-icon color="grey">fa-edit</v-icon>
                          </v-btn>
                          <v-btn icon @click="deleteReferral(item)" class="ma-0">
                            <v-icon color="grey">fa-trash</v-icon>
                          </v-btn>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                  <v-alert v-else value="true" type="error">
                    <span class="body-2">NO REFERRALS</span>
                    <br>
                  </v-alert>
                </v-card-text>
              </v-card>
            </v-tab-item>
            <v-tab-item key="assessment">
              <v-card flat class="scroll-y" height="440">
                <v-card-text class="pa-1">
                  <table v-if="assessments.length !== 0" class="v-datatable v-table theme--light">
                    <tbody>
                      <tr v-for="item in assessments" :key="item.assessmentId" :class="{'red lighten-5 grey--text': !$moment().startOf('day').isBetween($moment(item.startDate), $moment(item.endDate), null, '[]')}">
                        <td class="text-xs-left px-1 hidden-xs-only" style="width: 60px;">
                          <v-avatar>
                            <v-icon large>fa-briefcase-medical</v-icon>
                          </v-avatar>
                        </td>
                        <td class="text-xs-left px-1">
                          <strong class="body-2 hidden-xs-only">{{item.behaviorAnalysisCode.description}}</strong>
                          <br class="hidden-xs-only">
                          <span>{{item.behaviorAnalysisCode.hcpcs}}</span>
                        </td>
                        <td class="text-xs-left px-1 hidden-xs-only">
                          <strong class="body-2">
                            <v-icon small>fa-unlock</v-icon>
                            {{item.paNumber}}
                          </strong>
                        </td>
                        <td class="text-xs-left px-1">
                          <strong class="body-2">
                            <v-icon small>fa-star</v-icon>
                            {{item.totalUnits.toLocaleString()}}
                          </strong>
                        </td>
                        <td class="text-xs-left px-1 hidden-xs-only">
                          <strong class="body-2">
                            <v-icon small>fa-clock</v-icon>
                            {{(item.totalUnits / 4).toLocaleString()}} hrs
                          </strong>
                          <br>
                          <span>{{(item.totalUnits * 15).toLocaleString()}} mins</span>
                        </td>
                        <td class="text-xs-left px-1">
                          <strong class="green--text">
                            <v-icon small>fa-calendar-plus</v-icon>
                            {{item.startDate | moment("utc","MM/DD/YYYY")}}
                          </strong>
                          <br>
                          <strong class="red--text">
                            <v-icon small>fa-calendar-minus</v-icon>
                            {{item.endDate | moment("utc","MM/DD/YYYY")}}
                          </strong>
                        </td>
                        <td class="text-xs-right px-1 text-no-wrap">
                          <v-btn icon @click="editAssessment(item)" class="ma-0">
                            <v-icon color="grey">fa-pen-alt</v-icon>
                          </v-btn>
                          <v-btn icon @click="deleteAssessment(item)" class="ma-0">
                            <v-icon color="grey">fa-trash</v-icon>
                          </v-btn>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                  <v-alert v-else value="true" type="error">
                    <span class="body-2">NO AUTHORIZATION</span>
                    <br>
                  </v-alert>
                </v-card-text>
              </v-card>
            </v-tab-item>
            <v-tab-item key="assignment">
              <v-card flat class="scroll-y" height="440">
                <v-card-text class="pa-1">
                  <table v-if="assignments.length !== 0" class="v-datatable v-table theme--light">
                    <tbody>
                      <tr v-for="item in assignments" :key="item.assignmentId" :class="{'red lighten-5 grey--text': !item.active}">
                        <td class="text-xs-left px-1" style="width: 60px;">
                          <v-avatar>
                            <v-icon large>fa-user-tie</v-icon>
                          </v-avatar>
                        </td>
                        <td class="text-xs-left px-1">
                          <strong class="body-2">{{item.user.firstname}} {{item.user.lastname}}</strong>
                          <br>
                          <span>
                            <v-icon small>fa-phone</v-icon>
                            {{item.user.phone | phone}}
                          </span>
                        </td>
                        <td class="text-xs-left px-1 hidden-xs-only">
                          <v-chip small label :color="item.active ? 'indigo':'grey'" text-color="white">{{item.user.rol.rolName}}</v-chip>
                        </td>
                        <td class="text-xs-right px-1">
                          <v-switch hide-details color="primary" v-model="item.active" @change="changeAssignmentActive(item)"></v-switch>
                        </td>
                        <td class="text-xs-right px-1">
                          <v-btn icon @click="deleteAssignment(item)" class="ma-0">
                            <v-icon color="grey">fa-trash</v-icon>
                          </v-btn>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                  <v-alert v-else value="true" type="error">
                    <span class="body-2">NO ASSIGNMENTS</span>
                    <br>
                  </v-alert>
                </v-card-text>
              </v-card>
            </v-tab-item>
            <v-tab-item key="diagnosis">
              <v-card flat class="scroll-y" height="440">
                <v-card-text class="pa-1">
                  <table v-if="!client.clientDiagnostics || client.clientDiagnostics.length !== 0" class="v-datatable v-table theme--light">
                    <tbody>
                      <tr v-for="item in client.clientDiagnostics" :key="item.clientDiagnosisId">
                        <td class="text-xs-left px-1" style="width: 60px;">
                          <v-avatar>
                            <v-icon large>fa-stethoscope</v-icon>
                          </v-avatar>
                        </td>
                        <td class="text-xs-left px-1">
                          <strong class="body-2">{{item.diagnosis.description}}</strong>
                          <br>
                          <span>{{item.diagnosis.code}}</span>
                        </td>
                        <td class="text-xs-right px-1">
                          <v-btn icon @click="deleteDiagnosis(item)" class="ma-0">
                            <v-icon color="grey">fa-trash</v-icon>
                          </v-btn>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                  <v-alert v-else value="true" type="error">
                    <span class="body-2">NO DIAGNOSIS</span>
                    <br>
                  </v-alert>
                </v-card-text>
              </v-card>
            </v-tab-item>
          </v-tabs>
        </v-card>
      </v-flex>
      <v-flex xs12>
        <clinical-data :id="id" />
      </v-flex>
    </v-layout>

    <add-edit-referral-dialog :model="referralDialog" :data="referralData" @cancel="referralDialog = false" @onSubmit="onSubmitReferral" />
    <add-assignment-dialog :model="assignmentDialog" :clientId="id" @cancel="assignmentDialog = false" @onSubmit="onSubmitAssignment" />
    <add-assessment-dialog ref="assessmentDiag" :model="assessmentDialog" :clientId="id" @cancel="assessmentDialog = false" @onSubmit="onSubmitAssessment" />
  </v-container>
</template>

<script>
import clientApi from "@/services/api/ClientServices";
import addEditReferralDialog from "@/components/clients/AddEditReferral";
import addAssignmentDialog from "@/components/clients/AddAssignment";
import addAssessmentDialog from "@/components/clients/AddAssessment";
import ClinicalData from "@/components/clients/ClinicalData/ClinicalData";

export default {
  props: {
    id: {
      type: [Number, String],
      required: true
    }
  },

  components: {
    addEditReferralDialog,
    addAssignmentDialog,
    addAssessmentDialog,
    ClinicalData
  },

  computed: {
    user() {
      return this.$store.getters.user;
    },
    clientManagementAutorized() {
      return this.user.rol === "admin" || this.user.rol === "client management";
    }
  },

  data() {
    return {
      loadingBasicInfo: false,
      loadingExtra: false,
      loadingCaregiverForm: false,
      required: value => !!value || "This field is required.",
      addEditCaregiverMenu: false,
      client: {},
      activeTab: 0,
      activeTabBasic: 0,
      caregiverForm: {
        caregiverId: 0,
        caregiverFullname: null,
        phone: null,
        email: null
      },
      caregiverMenuFormValid: false,
      caregiversTypes: [],
      caregiverType: 0,
      referralData: {
        referralId: 0,
        clientId: 0
      },
      referralDialog: false,
      assignmentData: {
        assignmentId: 0,
        clientId: 0
      },
      assignmentDialog: false,
      assignments: [],
      assessmentDialog: false,
      assessments: []
    };
  },

  mounted() {
    this.loadBasicInfo();
    this.loadAssignments();
    this.loadAssessments();
  },

  methods: {
    async loadBasicInfo() {
      this.loadingBasicInfo = true;
      this.loadingExtra = true;
      try {
        this.client = await clientApi.getClient(this.id);
        this.caregiversTypes = await clientApi.getCaregiversTypes();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loadingBasicInfo = false;
        this.loadingExtra = false;
      }
    },

    async loadAssessments() {
      this.loadingBasicInfo = true;
      try {
        this.assessments = await clientApi.getAssessments(this.id);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loadingBasicInfo = false;
      }
    },

    async loadAssignments() {
      this.loadingBasicInfo = true;
      try {
        this.assignments = await clientApi.getAssignments(this.id);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loadingBasicInfo = false;
      }
    },

    close() {
      this.$refs.form.reset();
      this.addEditCaregiverMenu = false;
    },

    addNewCaregiver() {
      this.caregiverForm.caregiverId = 0;
      this.caregiverForm.caregiverFullname = null;
      this.caregiverForm.phone = null;
      this.caregiverForm.email = null;
      this.caregiverType = 0;
      //this.addEditCaregiverMenu = true;
    },

    editCaregiver(caregiver) {
      this.caregiverForm.caregiverId = caregiver.caregiverId;
      this.caregiverForm.caregiverFullname = caregiver.caregiverFullname;
      this.caregiverForm.phone = caregiver.phone;
      this.caregiverForm.email = caregiver.email;
      this.caregiverType = caregiver.caregiverTypeId;
      this.addEditCaregiverMenu = true;
    },

    async deleteCaregiver(caregiver) {
      this.$confirm("Do you want to delete selected caregiver?").then(async res => {
        if (res) {
          this.loadingExtra = true;
          try {
            await clientApi.deleteCaregiver(caregiver);
            this.client.caregivers = this.client.caregivers.filter(s => s.caregiverId !== caregiver.caregiverId);
          } catch (error) {
            this.$toast.error(error);
          } finally {
            this.loadingExtra = false;
          }
        }
      });
    },

    async submitCaregiverForm() {
      this.loadingCaregiverForm = true;
      try {
        this.caregiverForm.caregiverTypeId = this.caregiverType;
        this.caregiverForm.clientId = this.id;
        let id = await clientApi.addEditCaregiver(this.caregiverForm);
        if (this.caregiverForm.caregiverId == 0) {
          this.client.caregivers.push({
            caregiverId: id,
            clientId: this.caregiverForm.clientId,
            caregiverFullname: this.caregiverForm.caregiverFullname,
            phone: this.caregiverForm.phone,
            email: this.caregiverForm.email,
            caregiverTypeId: this.caregiverForm.caregiverTypeId,
            caregiverType: {
              description: this.caregiversTypes.find(f => f.caregiverTypeId === this.caregiverForm.caregiverTypeId).description
            }
          });
        } else {
          let caregiver = this.client.caregivers.find(f => f.caregiverId == id);
          caregiver.caregiverFullname = this.caregiverForm.caregiverFullname;
          caregiver.phone = this.caregiverForm.phone;
          caregiver.email = this.caregiverForm.email;
          caregiver.caregiverTypeId = this.caregiverForm.caregiverTypeId;
          caregiver.caregiverType = {
            description: this.caregiversTypes.find(f => f.caregiverTypeId === this.caregiverForm.caregiverTypeId).description
          };
        }
        this.close();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loadingCaregiverForm = false;
      }
    },

    addReferral() {
      this.referralData = {
        clientId: this.id,
        referralId: 0
      };
      this.referralDialog = true;
    },

    editReferral(referral) {
      this.referralData = {
        ...referral,
        dateReferral: this.$moment(referral.dateReferral)
          .utc()
          .format("MM/DD/YYYY"),
        dateExpires: this.$moment(referral.dateExpires)
          .utc()
          .format("MM/DD/YYYY")
      };
      this.referralDialog = true;
    },

    onSubmitReferral(referral) {
      this.referralDialog = false;
      let referralItem = this.client.referrals.find(f => f.referralId === referral.referralId);
      if (!referralItem) {
        this.client.referrals.push(referral);
      } else {
        Object.assign(referralItem, referral);
      }
    },

    deleteReferral(referral) {
      this.$confirm("Do you want to delete selected referral?").then(async res => {
        if (res) {
          this.loadingBasicInfo = true;
          try {
            await clientApi.deleteReferral(referral);
            this.client.referrals = this.client.referrals.filter(r => r.referralId != referral.referralId);
          } catch (error) {
            this.$toast.error(error);
          } finally {
            this.loadingBasicInfo = false;
          }
        }
      });
    },

    async changeReferralActive(referral) {
      const newStatus = {
        status: referral.active,
        referralId: referral.referralId
      };
      try {
        await clientApi.changeReferralStatus(newStatus);
      } catch (error) {
        this.$toast.error(error);
      }
    },

    addAssessment() {
      this.$refs.assessmentDiag.data.clientId = this.id;
      this.$refs.assessmentDiag.data.assessmentId = 0;
      this.assessmentDialog = true;
    },

    editAssessment(a) {
      this.$refs.assessmentDiag.data.assessmentId = a.assessmentId;
      this.$refs.assessmentDiag.data.totalUnits = a.totalUnits;
      this.$refs.assessmentDiag.data.paNumber = a.paNumber;
      this.$refs.assessmentDiag.data.clientId = a.clientId;
      this.$refs.assessmentDiag.data.behaviorAnalysisCodeId = a.behaviorAnalysisCodeId;
      this.$refs.assessmentDiag.data.startDate = this.$moment(a.startDate)
        .utc()
        .format("MM/DD/YYYY");
      this.$refs.assessmentDiag.data.endDate = this.$moment(a.endDate)
        .utc()
        .format("MM/DD/YYYY");
      this.assessmentDialog = true;
      console.log(a);
    },

    addAssignment() {
      this.assignmentDialog = true;
    },

    onSubmitAssessment() {
      this.assessmentDialog = false;
      this.loadAssessments();
    },

    deleteAssessment(assessment) {
      this.$confirm("Do you want to delete selected authorization?").then(async res => {
        if (res) {
          this.loadingBasicInfo = true;
          try {
            await clientApi.deleteAssessment(assessment);
            this.loadAssessments();
          } catch (error) {
            this.$toast.error(error);
          } finally {
            this.loadingBasicInfo = false;
          }
        }
      });
    },

    onSubmitAssignment() {
      this.assignmentDialog = false;
      this.loadAssignments();
    },

    deleteAssignment(assignment) {
      this.$confirm("Do you want to delete selected assignment?").then(async res => {
        if (res) {
          this.loadingBasicInfo = true;
          try {
            await clientApi.deleteAssignment(assignment);
            this.loadAssignments();
          } catch (error) {
            this.$toast.error(error);
          } finally {
            this.loadingBasicInfo = false;
          }
        }
      });
    },

    async changeAssignmentActive(assignment) {
      const newStatus = {
        status: assignment.active,
        userId: assignment.assignmentId
      };
      this.loadingBasicInfo = true;
      try {
        await clientApi.changeAssignmentStatus(newStatus);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loadingBasicInfo = false;
      }
    },

    addDiagnosis() {
      this.$prompt(null, { title: "Add new diagnosis", label: "Diagnosis code" }).then(async text => {
        if (text) {
          let model = {
            clientId: this.id,
            code: text
          };
          try {
            this.loadingBasicInfo = true;
            let diag = await clientApi.addClientDiagnosis(model);
            this.client.clientDiagnostics.push(diag.data);
          } catch (error) {
            this.$toast.error(error);
          } finally {
            this.loadingBasicInfo = false;
          }
        }
      });
    },

    deleteDiagnosis(clientDiagnosis) {
      this.$confirm("Do you want to delete selected diagnosis?").then(async res => {
        if (res) {
          this.loadingBasicInfo = true;
          try {
            await clientApi.deleteClientDiagnosis(clientDiagnosis);
            this.client.clientDiagnostics = this.client.clientDiagnostics.filter(c => c.clientDiagnosisId !== clientDiagnosis.clientDiagnosisId);
          } catch (error) {
            this.$toast.error(error);
          } finally {
            this.loadingBasicInfo = false;
          }
        }
      });
    }
  }
};
</script>
