<template>
  <v-app class="blue-grey lighten-5">
    <v-container grid-list-sm>
      <v-layout row wrap>
        <v-flex xs12>
          <v-card v-if="!signed">
            <v-toolbar dark class="secondary" fluid dense>
              <v-toolbar-title>Caregiver sign session</v-toolbar-title>
            </v-toolbar>
            <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
            <v-card-text>
              <v-container fluid grid-list-sm pa-0>
                <v-layout v-if="sessionDetailed" row wrap>
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
                    </v-layout>
                  </v-flex>
                  <v-flex xs6>
                    <v-layout row wrap>
                      <v-flex class="body-2 text-xs-right" xs4>Client:</v-flex>
                      <v-flex xs8>{{ sessionDetailed.clientFullname }} ({{ sessionDetailed.clientCode }})</v-flex>
                      <v-flex class="body-2 text-xs-right" xs4>Pos:</v-flex>
                      <v-flex xs8>{{ sessionDetailed.pos }}</v-flex>
                      <v-flex class="body-2 text-xs-right" xs4>Session type:</v-flex>
                      <v-flex xs8>{{ sessionDetailed.sessionType }}</v-flex>
                      <v-flex class="body-2 text-xs-right" xs4>Service:</v-flex>
                      <v-flex xs8>{{ sessionDetailed.hcpcs }} ({{ sessionDetailed.description }})</v-flex>
                    </v-layout>
                  </v-flex>
                </v-layout>
                <v-container grid-list-xs pa-1>
                  <v-layout row wrap>
                    <v-flex xs12>
                      <canvas color="black" ref="canvas" width="400" height="200" style="border: 1px solid black;"></canvas>
                    </v-flex>
                  </v-layout>
                </v-container>
              </v-container>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn :disabled="loading" color="warning" @click="clear">clear</v-btn>
              <v-btn :disabled="loading" :loading="loading" color="primary" @click="save">Save</v-btn>
            </v-card-actions>
          </v-card>
          <v-card v-else>
            <v-alert type="info" :value="true">
              Session is already signed. Thanks.sss
            </v-alert>
          </v-card>
        </v-flex>
      </v-layout>
    </v-container>
  </v-app>
</template>

<script>
import sessionServicesApi from "@/services/api/SessionServices";
import SignaturePad from "signature_pad";
import trimCanvas from "trim-canvas";

export default {
  props: {
    id: { require: true }
  },

  data() {
    return {
      loading: false,
      sessionDetailed: null,
      signaturePad: null,
      signatureData: null,
      signed: false
    };
  },

  mounted() {
    this.signaturePad = new SignaturePad(this.$refs.canvas);
    this.loadSessionData();
    this.resizeCanvas();
  },

  methods: {
    async loadSessionData() {
      try {
        this.loading = true;
        const sessionDetailed = await sessionServicesApi.getSessionDetailedForSign(this.id);
        let s1 = this.$moment(sessionDetailed.sessionStart).local();
        let s2 = this.$moment(sessionDetailed.sessionEnd).local();
        sessionDetailed.sessionStart = s1;
        sessionDetailed.sessionEnd = s2;
        this.sessionDetailed = sessionDetailed;
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loading = false;
      }
    },

    clear() {
      this.signaturePad.clear();
    },

    async save() {
      trimCanvas(this.$refs.canvas);
      let sign = {
        sign: this.signaturePad.toDataURL(),
        auth: this.id
      };
      try {
        this.loading = true;
        await sessionServicesApi.saveSessionSign(sign);
        this.sessionDetailed.sign.sign = sign.sign;
        this.signed = true;
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loading = false;
      }
    },

    // getData() {
    //   this.signatureData = this.signaturePad.toDataURL();
    //   console.log(this.signatureData);
    //   console.log(this.signatureData.length);
    // },

    resizeCanvas() {
      this.$refs.canvas.width = this.$refs.canvas.parentElement.clientWidth - 10;
      let canvas = this.$refs.canvas;
      window.onresize = function () {
        canvas.width = canvas.parentElement.clientWidth - 10;
      };
    }
  }
};
</script>
