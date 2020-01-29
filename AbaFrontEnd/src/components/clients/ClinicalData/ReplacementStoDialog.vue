<template>
  <div>
    <v-dialog persistent width="800" v-model="open">
      <v-card class="grey lighten-3">
        <v-toolbar dark dense fluid>
          <v-toolbar-title>STOs for {{ data.replacement.replacementProgramDescription }}</v-toolbar-title>
        </v-toolbar>
        <v-card-text class="pa-1">
          <v-list dense subheader>
            <template v-for="(p, index) in clientReplacementStos">
              <v-hover :key="'h-' + p.clientReplacementStoId">
                <v-list-tile :key="p.clientReplacementStoId" @click.stop slot-scope="{ hover }">
                  <v-list-tile-avatar>
                    <v-avatar size="32" color="secondary">
                      <span class="white--text headline">{{ index + 1 }}</span>
                    </v-avatar>
                  </v-list-tile-avatar>
                  <v-list-tile-content>
                    <v-list-tile-title class="body-2">
                      Get <span class="purple--text font-weight-black">{{ p.percent }}% or more</span> in <span class="purple--text font-weight-black">{{ p.weeks }}</span> consecutive week(s)
                      <label v-if="p.levelAssistance">with <span class="blue--text font-weight-black">{{p.levelAssistance}}</span></label>
                    </v-list-tile-title>
                    <v-list-tile-sub-title>
                      <!-- Status:
                      <strong :class="p.status.toLowerCase() == 'unknow' ? 'red--text' : p.status.toLowerCase() == 'mastered' ? 'green--text' : 'orange--text'">{{ p.status }}</strong>
                      &nbsp;&nbsp;&nbsp;
                      <small v-if="p.status.toLowerCase() == 'mastered'">{{ p.weekStart | moment("utc", "MM/DD/YYYY") }} - {{ p.weekEnd | moment("utc", "MM/DD/YYYY") }}</small>
                      &nbsp;&nbsp;&nbsp; -->
                      <label v-if="p.masteredForced" class="red white--text px-1" small label>Mastered Forced</label>
                    </v-list-tile-sub-title>
                  </v-list-tile-content>
                  <v-list-tile-action v-if="hover">
                    <v-btn icon :disabled="formShow" @click.stop="updateSto(p)">
                      <v-icon small color="grey">fa-pen</v-icon>
                    </v-btn>
                  </v-list-tile-action>
                  <v-list-tile-action v-show="hover">
                    <v-menu left ref="menu">
                      <template v-slot:activator="{ on }">
                        <v-btn icon v-on="on" :disabled="formShow">
                          <v-icon small color="grey">fa-ellipsis-v</v-icon>
                        </v-btn>
                      </template>
                      <v-list>
                        <v-list-tile @click="deleteSto(p.clientReplacementStoId)">
                          <v-list-tile-action>
                            <v-icon small color="grey">fa-trash</v-icon>
                          </v-list-tile-action>
                          <v-list-tile-content>
                            <v-list-tile-title>Delete</v-list-tile-title>
                          </v-list-tile-content>
                        </v-list-tile>
                        <v-list-tile v-if="!p.masteredForced" @click="forceStoDialog(p)">
                          <v-list-tile-action>
                            <v-icon small color="grey">fa-link</v-icon>
                          </v-list-tile-action>
                          <v-list-tile-content>
                            <v-list-tile-title>Force mastered</v-list-tile-title>
                          </v-list-tile-content>
                        </v-list-tile>
                        <v-list-tile v-else @click="removeForced(p)">
                          <v-list-tile-action>
                            <v-icon small color="grey">fa-unlink</v-icon>
                          </v-list-tile-action>
                          <v-list-tile-content>
                            <v-list-tile-title>Remove forced</v-list-tile-title>
                          </v-list-tile-content>
                        </v-list-tile>
                      </v-list>
                    </v-menu>
                  </v-list-tile-action>
                </v-list-tile>
              </v-hover>
            </template>
          </v-list>
          <v-btn v-if="!formShow" :disabled="loading" block flat @click="newSto">Click here to add new STO</v-btn>
          <div v-show="formShow" class="pt-2">
            <v-form ref="form" autocomplete="off" v-model="formValid">
              <v-layout row wrap>
                <v-flex xs12 sm3>
                  <v-text-field hide-details ref="focusInput" box label="Percent" v-model="clientReplacementSto.percent" type="number" :rules="[required]" required append-icon="fa-percent fa-sm"
                                background-color="white">
                  </v-text-field>
                </v-flex>
                <v-flex xs12 sm1 align-self-center>
                  <v-subheader>in</v-subheader>
                </v-flex>
                <v-flex xs12 sm2>
                  <v-text-field hide-details box label="Weeks" v-model="clientReplacementSto.weeks" type="number" :rules="[required]" required append-icon="fa-calendar-alt fa-sm"
                                background-color="white"></v-text-field>
                </v-flex>
                <v-flex xs12 sm1 align-self-center>
                  <v-subheader>width</v-subheader>
                </v-flex>
                <v-flex xs12 sm5>
                  <v-select :items="levels" box label="Level of assistance" hide-details background-color="white" v-model="clientReplacementSto.levelAssistance" clearable></v-select>
                </v-flex>
              </v-layout>
              <div class="text-xs-right">
                <v-btn flat @click="cancelForm">Cancel</v-btn>
                <v-btn :disabled="!formValid" color="primary" @click="saveSto">Save</v-btn>
              </div>
            </v-form>
          </div>
        </v-card-text>

        <v-card-actions>
          <!-- <v-btn :disabled="loading || formShow" :loading="loading" outline color="purple" @click="reCalculate()">
            <v-icon left small>fa-calculator</v-icon> RE-CALC
          </v-btn> -->
          <v-spacer />
          <v-btn :disabled="loading" :loading="loading" color="primary" @click="$emit('closed')">Close</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <force-sto-dialog :open="forceStoDialogShow" @cancel="forceStoDialogShow = false" @submit="submitForceMastered" />
  </div>
</template>

<script>
import clientApi from "@/services/api/ClientServices";
import forceStoDialog from "@/components/clients/ClinicalData/ForceStoDialog";

export default {
  components: {
    forceStoDialog
  },

  props: {
    open: {
      type: Boolean,
      required: true,
      default: false
    },
    data: {
      type: Object
    },
    clientId: {
      type: [Number, String],
      required: true
    }
  },

  mounted() {
    this.loadClientReplacementStos();
  },

  data() {
    return {
      required: value => !!value || "This field is required.",
      loading: false,
      clientReplacementStos: [],
      formShow: false,
      formValid: false,
      clientReplacementSto: {
        clientReplacementStoId: null,
        percent: null,
        weeks: null,
        masteredForced: false,
        weekEnd: null,
        weekStart: null,
        levelAssistance: null
      },
      forceStoDialogShow: false,
      forceFromTo: null,
      sto: null,
      levels: ["Full Physical Prompt", "Partial Physical Assistance", "Modeling", "Gestual or Visual Prompt", "Direct verbal Prompt", "Cue (using a light, Cocker, bell)", "independently level"]
    };
  },

  methods: {
    cancelForm() {
      this.$refs.form.reset();
      this.formShow = false;
    },

    newSto() {
      this.formShow = true;
      this.clientReplacementSto.clientReplacementStoId = 0;
      this.$refs.form.reset();
      this.$nextTick(() => this.$refs.focusInput.focus());
    },

    updateSto(s) {
      this.clientReplacementSto.clientReplacementStoId = s.clientReplacementStoId;
      this.clientReplacementSto.percent = s.percent;
      this.clientReplacementSto.weeks = s.weeks;
      this.clientReplacementSto.masteredForced = s.masteredForced;
      this.clientReplacementSto.weekStart = s.weekStart;
      this.clientReplacementSto.weekEnd = s.weekEnd;
      this.clientReplacementSto.levelAssistance = s.levelAssistance;
      this.formShow = true;
    },

    async deleteSto(clientReplacementStoId) {
      this.$confirm("Do you want to delete this STO?").then(async res => {
        if (res) {
          try {
            await clientApi.deleteClientReplacementSto(clientReplacementStoId);
            await this.loadClientReplacementStos();
          } catch (error) {
            this.$toast.error(error.message || error);
          }
        }
      });
    },

    async loadClientReplacementStos() {
      this.loading = true;
      try {
        this.clientReplacementStos = await clientApi.getClientReplacementStos(this.data.clientReplacementId);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async saveSto() {
      try {
        this.loading = true;
        this.clientReplacementSto.clientReplacementId = this.data.clientReplacementId;
        await clientApi.saveClientReplacementSto(this.clientReplacementSto);
        this.cancelForm();
        this.loadClientReplacementStos();
      } catch (error) {
        this.$toast.error(error.message);
      } finally {
        this.loading = false;
      }
    },

    async reCalculate() {
      return;
      // try {
      //   this.loading = true;
      //   await clientApi.adjustStoClientReplacement(this.clientId);
      //   this.cancelForm();
      //   this.loadClientReplacementStos();
      // } catch (error) {
      //   console.error(error);
      //   this.$toast.error(error);
      // } finally {
      //   this.loading = false;
      // }
    },

    forceStoDialog(p) {
      this.sto = p;
      this.forceStoDialogShow = true;
    },

    async submitForceMastered(v) {
      let s = Object.assign({}, this.sto);
      s.weekStart = v.from;
      s.weekEnd = v.to;
      s.masteredForced = true;
      try {
        this.loading = true;
        await clientApi.saveClientReplacementSto(s);
        this.loadClientReplacementStos();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
        this.forceStoDialogShow = false;
      }
    },

    async removeForced(v) {
      v.masteredForced = false;
      try {
        this.loading = true;
        await clientApi.saveClientReplacementSto(v);
        this.loadClientReplacementStos();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
        this.forceStoDialogShow = false;
      }
    }
  }
};
</script>

<style scoped>
.v-list__tile__action {
  min-width: 36px;
  padding-left: 1%;
}
</style>