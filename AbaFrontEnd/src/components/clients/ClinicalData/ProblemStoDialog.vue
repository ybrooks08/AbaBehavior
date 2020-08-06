<template>
  <div>
    <v-dialog persistent full-width v-model="open">
      <v-card class="grey lighten-3">
        <v-toolbar dark dense fluid>
          <v-toolbar-title>STOs for {{ data.problemBehavior.problemBehaviorDescription }}</v-toolbar-title>
        </v-toolbar>
        <v-card-text class="pa-0">
          <v-layout>
            <v-flex xs12 md5 pa-1>
              <div style="height: 500px; max-height: 500px; min-height: 500px; overflow-y: auto;" class="white">
                <v-list dense subheader>
                  <template v-for="(p, index) in clientProblemStos">
                    <v-hover :key="'h-' + p.clientProblemStoId">
                      <v-list-tile :key="p.clientProblemStoId" @click.stop slot-scope="{ hover }">
                        <v-list-tile-avatar>
                          <!-- <v-icon>fa-medal</v-icon> -->
                          <v-avatar size="32" color="secondary">
                            <span class="white--text headline">{{ index + 1 }}</span>
                          </v-avatar>
                        </v-list-tile-avatar>
                        <v-list-tile-content>
                          <v-list-tile-title class="body-2">
                            Get <span class="purple--text font-weight-black">{{ p.quantity }} or less</span> in <span class="purple--text font-weight-black">{{ p.weeks }}</span> consecutive week(s)
                          </v-list-tile-title>
                          <v-list-tile-sub-title>
                            <!-- Status:
                            <strong :class="p.status.toLowerCase() == 'unknow' ? 'red--text' : p.status.toLowerCase() == 'mastered' ? 'green--text' : 'orange--text'">{{ p.status }}</strong>
                            &nbsp;&nbsp;&nbsp;
                            <small v-if="p.status.toLowerCase() == 'mastered'">{{ p.weekStart | moment("utc", "MM/DD/YYYY") }} - {{ p.weekEnd | moment("utc", "MM/DD/YYYY") }}</small> &nbsp;&nbsp;&nbsp; -->
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
                              <v-list-tile @click="deleteSto(p.clientProblemStoId)">
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
              </div>
              <v-btn v-if="!formShow" :disabled="loading" block flat color="primary" @click="newSto">Click here to add new STO</v-btn>
              <div v-show="formShow" class="pt-2">
                <v-form ref="form" autocomplete="off" v-model="formValid">
                  <v-layout row wrap>
                    <v-flex xs12 md2>
                      <v-subheader style="float: right;">{{ isPercent ? "Percent" : "Frecuency" }}</v-subheader>
                    </v-flex>
                    <v-flex xs12 md3>
                      <v-text-field
                        ref="focusInput"
                        solo
                        hide-details
                        v-model="clientProblemSto.quantity"
                        type="number"
                        :rules="[required]"
                        required
                        :append-icon="isPercent ? 'fa-percent fa-sm' : 'fa-frown fa-sm'"
                      ></v-text-field>
                    </v-flex>
                    <v-flex xs12 md1>
                      <v-subheader>in</v-subheader>
                    </v-flex>
                    <v-flex xs12 md2>
                      <v-text-field solo hide-details v-model="clientProblemSto.weeks" type="number" :rules="[required]" required append-icon="fa-calendar-alt fa-sm"></v-text-field>
                    </v-flex>
                    <v-flex xs12 md4>
                      <v-subheader>consecutive weeks</v-subheader>
                    </v-flex>
                  </v-layout>
                  <div class="text-xs-right">
                    <v-btn flat @click="cancelForm">Cancel</v-btn>
                    <v-btn :disabled="!formValid" color="primary" @click="saveSto">Save</v-btn>
                  </div>
                </v-form>
              </div>
            </v-flex>
            <v-flex xs12 md7 pa-1>
              <client-progress-behavior :behaviorId="data.problemId.toString()" hideNotes :clientId="data.clientId"></client-progress-behavior>
            </v-flex>
          </v-layout>
        </v-card-text>

        <v-card-actions>
          <!-- <v-btn :disabled="loading || formShow" :loading="loading" outline color="purple" @click="reCalculate()">
            <v-icon left small>fa-calculator</v-icon>
            RE-CALC
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
import ClientProgressBehavior from "@/components/sessions/ProgressBehavior";

export default {
  components: {
    forceStoDialog,
    ClientProgressBehavior
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
    this.loadClientProblemStos();
  },

  data() {
    return {
      required: (value) => !!value || "This field is required.",
      loading: false,
      clientProblemStos: [],
      formShow: false,
      formValid: false,
      clientProblemSto: {
        clientProblemStoId: null,
        quantity: null,
        weeks: null,
        masteredForced: false,
        weekEnd: null,
        weekStart: null
      },
      forceStoDialogShow: false,
      forceFromTo: null,
      sto: null
    };
  },

  computed: {
    isPercent() {
      return this.data.problemBehavior.isPercent;
    }
  },

  methods: {
    cancelForm() {
      this.$refs.form.reset();
      this.formShow = false;
    },

    newSto() {
      this.formShow = true;
      this.clientProblemSto.clientProblemStoId = 0;
      this.$refs.form.reset();
      this.$nextTick(() => this.$refs.focusInput.focus());
    },

    updateSto(s) {
      this.clientProblemSto.clientProblemStoId = s.clientProblemStoId;
      this.clientProblemSto.quantity = s.quantity;
      this.clientProblemSto.weeks = s.weeks;
      this.clientProblemSto.masteredForced = s.masteredForced;
      this.clientProblemSto.weekStart = s.weekStart;
      this.clientProblemSto.weekEnd = s.weekEnd;
      this.formShow = true;
      this.$nextTick(() => this.$refs.focusInput.focus());
    },

    async deleteSto(clientProblemStoId) {
      this.$confirm("Do you want to delete this STO?").then(async (res) => {
        if (res) {
          try {
            await clientApi.deleteClientProblemSto(clientProblemStoId);
            await this.loadClientProblemStos();
          } catch (error) {
            this.$toast.error(error.message || error);
          }
        }
      });
    },

    async loadClientProblemStos() {
      this.loading = true;
      try {
        this.clientProblemStos = await clientApi.getClientProblemStos(this.data.clientProblemId);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async saveSto() {
      try {
        this.loading = true;
        this.clientProblemSto.clientProblemId = this.data.clientProblemId;
        await clientApi.saveClientProblemSto(this.clientProblemSto);
        this.cancelForm();
        await this.loadClientProblemStos();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async reCalculate() {
      return;
      // try {
      //   this.loading = true;
      //   await clientApi.adjustStoClientBehavior(this.clientId);
      //   this.cancelForm();
      //   this.loadClientProblemStos();
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
        await clientApi.saveClientProblemSto(s);
        await this.loadClientProblemStos();
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
        await clientApi.saveClientProblemSto(v);
        await this.loadClientProblemStos();
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
