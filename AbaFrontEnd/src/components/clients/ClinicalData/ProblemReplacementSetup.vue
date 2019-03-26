<template>
  <v-card flat>
    <v-tabs dark>
      <v-tab key="problems">Problems</v-tab>
      <v-tab key="replacement">Replacements</v-tab>
      <v-tab-item key="problems">
        <v-card flat class="grey lighten-3">
          <v-card-text class="pa-1">
            <v-list dense subheader>
              <v-list-tile :disabled="loading" v-for="p in clientProblems" :key="p.clientProblemId">
                <v-list-tile-avatar>
                  <v-icon>fa-frown</v-icon>
                </v-list-tile-avatar>
                <v-list-tile-content>
                  <v-list-tile-title class="body-2">
                    {{p.problemBehavior.problemBehaviorDescription}} &nbsp;&nbsp;&nbsp;
                    <v-chip style="max-height: 18px;" class="pa-0 ma-0" label small color="blue-grey lighten-4">{{p.stOs.length}} STO</v-chip>
                  </v-list-tile-title>
                  <v-list-tile-sub-title>
                    Baseline: {{p.baselineCount || 'N/A'}},
                    <span v-if="p.baselineFrom">From {{p.baselineFrom | moment('utc','MM/DD/YYYY')}}</span>&nbsp;
                    <span v-if="p.baselineTo">to {{p.baselineTo | moment('utc','MM/DD/YYYY')}}</span>
                  </v-list-tile-sub-title>
                </v-list-tile-content>
                <v-list-tile-action>
                  <v-tooltip top>
                    <template #activator="data">
                      <v-btn icon :disabled="problemFormShow" @click.stop="updateClientProblem(p)" v-on="data.on">
                        <v-icon small color="grey">fa-pen</v-icon>
                      </v-btn>
                    </template>
                    <span>Edit problem behavior</span>
                  </v-tooltip>
                </v-list-tile-action>
                <v-list-tile-action>
                  <v-tooltip top>
                    <template #activator="data">
                      <v-btn icon :disabled="problemFormShow" @click.stop="showProblemSto(p)" v-on="data.on">
                        <v-icon small color="grey">fa-medal</v-icon>
                      </v-btn>
                    </template>
                    <span>Setup STOs</span>
                  </v-tooltip>
                </v-list-tile-action>
                <v-list-tile-action>
                  <v-tooltip top>
                    <template #activator="data">
                      <v-btn icon :disabled="problemFormShow" @click.stop="deleteProblem(p)" v-on="data.on">
                        <v-icon small color="grey">fa-trash</v-icon>
                      </v-btn>
                    </template>
                    <span>Delete problem behavior</span>
                  </v-tooltip>
                </v-list-tile-action>
              </v-list-tile>
            </v-list>

            <v-btn v-if="!problemFormShow" :disabled="loading" block flat @click="newProblem">Click here to add new Problem behavior</v-btn>
            <div v-show="problemFormShow" class="pt-2">
              <v-form ref="problemFormRef" autocomplete="off" v-model="problemFormValid">
                <v-layout row wrap>
                  <v-flex md4>
                    <v-autocomplete solo hide-details v-model="clientProblem.problemId" :items="problemsMaster" item-value="problemId" item-text="problemBehaviorDescription" placeholder="Select problem" :rules="[required]" required></v-autocomplete>
                  </v-flex>
                  <v-flex md2>
                    <v-text-field solo hide-details v-model="clientProblem.baselineCount" placeholder="Base" clearable></v-text-field>
                  </v-flex>
                  <v-flex md3>
                    <v-text-field solo hide-details v-model="clientProblem.baselineFrom" placeholder="from" return-masked-value required mask="##/##/####" data-vv-name="periodEnd" :rules="errors.collect('periodEnd')" v-validate="'date_format:MM/DD/YYYY'" clearable></v-text-field>
                  </v-flex>
                  <v-flex md3>
                    <v-text-field solo hide-details v-model="clientProblem.baselineTo" placeholder="from" return-masked-value required mask="##/##/####" data-vv-name="periodEnd" :rules="errors.collect('periodEnd')" v-validate="'date_format:MM/DD/YYYY'" clearable></v-text-field>
                  </v-flex>
                </v-layout>
                <div class="text-xs-right">
                  <v-btn flat @click="cancelProblemForm">Cancel</v-btn>
                  <v-btn :disabled="!problemFormValid" color="primary" @click="saveProblem">Save</v-btn>
                </div>
              </v-form>
            </div>
          </v-card-text>
        </v-card>
      </v-tab-item>

      <v-tab-item key="replacements">
        <v-card flat class="grey lighten-3">
          <v-card-text class="pa-1">
            <v-list dense subheader>
              <v-list-tile :disabled="loading" v-for="p in clientReplacements" :key="p.clientReplacementId">
                <v-list-tile-avatar>
                  <v-icon>fa-registered</v-icon>
                </v-list-tile-avatar>
                <v-list-tile-content>
                  <v-list-tile-title class="body-2">
                    {{p.replacement.replacementProgramDescription}} &nbsp;&nbsp;&nbsp;
                    <!--<v-chip style="max-height: 18px;" class="pa-0 ma-0" label small color="blue-grey lighten-4">{{p.stOs.length}} STO</v-chip>-->
                  </v-list-tile-title>
                  <v-list-tile-sub-title>
                    Baseline: {{p.baselinePercent || 'N/A'}},
                    <span v-if="p.baselineFrom">From {{p.baselineFrom | moment('utc','MM/DD/YYYY')}}</span>&nbsp;
                    <span v-if="p.baselineTo">to {{p.baselineTo | moment('utc','MM/DD/YYYY')}}</span>
                  </v-list-tile-sub-title>
                </v-list-tile-content>
                <v-list-tile-action>
                  <v-tooltip top>
                    <template #activator="data">
                      <v-btn v-on="data.on" icon :disabled="replacementFormShow" @click.stop="updateClientReplacement(p)">
                        <v-icon small color="grey">fa-pen</v-icon>
                      </v-btn>
                    </template>
                    <span>Edit replacement</span>
                  </v-tooltip>
                </v-list-tile-action>
                <v-list-tile-action>
                  <v-tooltip top>
                    <template #activator="data">
                      <v-btn v-on="data.on" icon :disabled="replacementFormShow" @click.stop="showReplacementSto(p)">
                        <v-icon small color="grey">fa-medal</v-icon>
                      </v-btn>
                    </template>
                    <span>Setup STOs</span>
                  </v-tooltip>
                </v-list-tile-action>
                <v-list-tile-action>
                  <v-tooltip top>
                    <template #activator="data">
                      <v-btn v-on="data.on" icon :disabled="replacementFormShow" @click.stop="deleteReplacement(p)">
                        <v-icon small color="grey">fa-trash</v-icon>
                      </v-btn>
                    </template>
                    <span>Delete replacement</span>
                  </v-tooltip>
                </v-list-tile-action>
              </v-list-tile>
            </v-list>
            <v-btn v-if="!replacementFormShow" :disabled="loading" block flat @click="newReplacement">Click here to add new Replacement</v-btn>
            <div v-show="replacementFormShow" class="pt-2">
              <v-form ref="replacementFormRef" autocomplete="off" v-model="replacementFormValid">
                <v-layout row wrap>
                  <v-flex md4>
                    <v-autocomplete solo hide-details v-model="clientReplacement.replacementId" :items="replacementsMaster" item-value="replacementId" item-text="replacementProgramDescription" placeholder="Select replacement" :rules="[required]" required></v-autocomplete>
                  </v-flex>
                  <v-flex md2>
                    <v-text-field solo hide-details v-model="clientReplacement.baselinePercent" placeholder="Base" clearable append-icon="fa-percent fa-sm"></v-text-field>
                  </v-flex>
                  <v-flex md3>
                    <v-text-field solo hide-details v-model="clientReplacement.baselineFrom" placeholder="from" return-masked-value required mask="##/##/####" data-vv-name="periodEnd" :rules="errors.collect('periodEnd')" v-validate="'date_format:MM/DD/YYYY'" clearable></v-text-field>
                  </v-flex>
                  <v-flex md3>
                    <v-text-field solo hide-details v-model="clientReplacement.baselineTo" placeholder="from" return-masked-value required mask="##/##/####" data-vv-name="periodEnd" :rules="errors.collect('periodEnd')" v-validate="'date_format:MM/DD/YYYY'" clearable></v-text-field>
                  </v-flex>
                </v-layout>
                <div class="text-xs-right">
                  <v-btn flat @click="cancelReplacementForm">Cancel</v-btn>
                  <v-btn :disabled="!replacementFormValid" color="primary" @click="saveReplacement">Save</v-btn>
                </div>
              </v-form>
            </div>
          </v-card-text>
        </v-card>
      </v-tab-item>
      <problem-sto-dialog v-if="problemStoDialogData" :open="problemStoDialogShow" :data="problemStoDialogData" @closed="closedProblemStoDialog"></problem-sto-dialog>
      <replacement-sto-dialog v-if="replacementStoDialogData" :open="replacementStoDialogShow" :data="replacementStoDialogData" @closed="closedReplacementStoDialog"></replacement-sto-dialog>
    </v-tabs>
  </v-card>
</template>

<script>
import clientApi from '@/services/api/ClientServices';
import tablesApi from '@/services/api/MasterTablesServices';
import problemStoDialog from '@/components/clients/ClinicalData/ProblemStoDialog';
import replacementStoDialog from '@/components/clients/ClinicalData/ReplacementStoDialog';

export default {
  props: {
    clientId: {
      type: [Number, String],
      required: true,
    },
  },

  components: {
    problemStoDialog,
    replacementStoDialog,
  },

  data() {
    return {
      required: (value) => !!value || 'This field is required.',
      loadingProblems: false,
      problemFormShow: false,
      problemFormValid: false,
      problemsMaster: [],
      clientProblems: [],

      clientProblem: {
        clientProblemId: 0,
        problemId: null,
        baselineCount: null,
        baselineFrom: null,
        baselineTo: null,
        clientId: this.clientId,
      },

      problemStoDialogShow: false,
      problemStoDialogData: null,

      replacementStoDialogShow: false,
      replacementStoDialogData: null,

      loadingReplacements: false,
      replacementFormShow: false,
      replacementFormValid: false,
      replacementsMaster: [],
      clientReplacements: [],
      clientReplacement: {
        clientReplacementId: 0,
        replacementId: null,
        baselinePercent: null,
        baselineFrom: null,
        baselineTo: null,
        clientId: this.clientId,
      },
    };
  },

  computed: {
    loading() {
      return this.loadingProblems || this.loadingReplacements;
    },
  },

  mounted() {
    this.loadAllData();
    this.loadProblemsMaster();
    this.loadReplacementsMaster();
    // this.loadClientProblems();
  },

  methods: {
    async loadProblemsMaster() {
      this.loadingProblems = true;
      try {
        this.problemsMaster = await tablesApi.getProblemBehaviors();
      } catch (error) {
        this.$toast.error(error);
      } finally { this.loadingProblems = false; }
    },

    async loadReplacementsMaster() {
      this.loadingReplacements = true;
      try {
        this.replacementsMaster = await tablesApi.getReplacementPrograms();
      } catch (error) {
        this.$toast.error(error);
      } finally { this.loadingReplacements = false; }
    },

    loadAllData() {
      this.cancelProblemForm();
      this.cancelReplacementForm();
      this.loadClientProblems();
      this.loadClientReplacements();
    },

    async loadClientProblems() {
      this.loadingProblems = true;
      try {
        this.clientProblems = await clientApi.getClientProblems(this.clientId);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loadingProblems = false;
      }
    },

    async loadClientReplacements() {
      this.loadingReplacements = true;
      try {
        this.clientReplacements = await clientApi.getClientReplacements(this.clientId);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loadingReplacements = false;
      }
    },

    newProblem() {
      this.clientProblem.clientProblemId = 0;
      this.problemFormShow = true;
      this.$refs.problemFormRef.reset();
    },

    updateClientProblem(p) {
      this.clientProblem.problemId = p.problemId;
      this.clientProblem.baselineCount = p.baselineCount;
      this.clientProblem.clientProblemId = p.clientProblemId;
      this.clientProblem.baselineFrom = this.$moment(p.baselineFrom).utc().format('MM/DD/YYYY');
      this.clientProblem.baselineTo = this.$moment(p.baselineTo).utc().format('MM/DD/YYYY');
      this.problemFormShow = true;
    },

    cancelProblemForm() {
      this.$refs.problemFormRef.reset();
      this.problemFormShow = false;
    },

    async saveProblem() {
      try {
        this.loadingProblems = true;
        this.clientProblem.clientId = this.clientId;
        await clientApi.saveProblem(this.clientProblem);
        this.cancelProblemForm();
        this.loadClientProblems();
      } catch (error) {
        this.$toast.error(error);
      } finally { this.loadingProblems = false; }
    },

    async deleteProblem(p) {
      this.$confirm('Do you want to delete this Problem behavior?')
        .then(async res => {
          if (res) {
            try {
              await clientApi.deleteClientProblem(p.clientProblemId);
              this.loadClientProblems();
            } catch (error) {
              this.$toast.error(error.message || error);
            }
          }
        });
    },

    newReplacement() {
      this.clientReplacement.clientReplacementId = 0;
      this.replacementFormShow = true;
      this.$refs.replacementFormRef.reset();
    },

    updateClientReplacement(p) {
      this.clientReplacement.replacementId = p.replacementId;
      this.clientReplacement.baselinePercent = p.baselinePercent;
      this.clientReplacement.clientReplacementId = p.clientReplacementId;
      this.clientReplacement.baselineFrom = this.$moment(p.baselineFrom).utc().format('MM/DD/YYYY');
      this.clientReplacement.baselineTo = this.$moment(p.baselineTo).utc().format('MM/DD/YYYY');
      this.replacementFormShow = true;
    },

    cancelReplacementForm() {
      this.$refs.replacementFormRef.reset();
      this.replacementFormShow = false;
    },

    async saveReplacement() {
      try {
        this.loadingReplacements = true;
        this.clientReplacement.clientId = this.clientId;
        await clientApi.saveReplacement(this.clientReplacement);
        this.cancelReplacementForm();
        this.loadClientReplacements();
      } catch (error) {
        this.$toast.error(error);
      } finally { this.loadingReplacements = false; }
    },

    async deleteReplacement(p) {
      this.$confirm('Do you want to delete this Replacement?')
        .then(async res => {
          if (res) {
            try {
              await clientApi.deleteClientReplacement(p.clientReplacementId);
              this.loadClientReplacements();
            } catch (error) {
              this.$toast.error(error.message || error);
            }
          }
        });
    },

    showProblemSto(p) {
      this.problemStoDialogData = p;
      this.problemStoDialogShow = true;
    },

    closedProblemStoDialog() {
      this.problemStoDialogShow = false;
      this.loadClientProblems();
      this.problemStoDialogData = null;
    },

    showReplacementSto(p) {
      this.replacementStoDialogData = p;
      this.replacementStoDialogShow = true;
    },

    closedReplacementStoDialog() {
      this.replacementStoDialogShow = false;
      this.loadClientReplacements();
      this.replacementStoDialogData = null;
    },
  },
};
</script>

<style scoped>
.v-list__tile__action {
  min-width: 36px;
  padding-left: 1%;
}
</style>