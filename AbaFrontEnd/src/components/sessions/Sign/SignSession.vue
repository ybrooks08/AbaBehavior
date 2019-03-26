<template>
  <v-card>
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
              <v-flex xs8>{{sessionDetailed.sessionStart | moment('utc','ddd')}}, {{sessionDetailed.sessionStart | moment('utc','ll')}}</v-flex>
              <v-flex class="body-2 text-xs-right" xs4>Time IN:</v-flex>
              <v-flex xs8>
                <v-icon color="green" small>fa-sign-in-alt</v-icon>
                {{sessionDetailed.sessionStart | moment('utc','LT')}}
              </v-flex>
              <v-flex class="body-2 text-xs-right" xs4>Time OUT:</v-flex>
              <v-flex xs8>
                <v-icon color="red" small>fa-sign-out-alt</v-icon>
                {{sessionDetailed.sessionEnd | moment('utc','LT')}}
              </v-flex>
              <v-flex class="body-2 text-xs-right" xs4>Units:</v-flex>
              <v-flex xs8>
                <v-icon small>fa-star</v-icon>
                {{sessionDetailed.totalUnits.toLocaleString()}}
                <v-icon small>fa-clock</v-icon>
                {{(sessionDetailed.totalUnits / 4).toLocaleString()}}
              </v-flex>
            </v-layout>
          </v-flex>
          <v-flex xs6>
            <v-layout row wrap>
              <v-flex class="body-2 text-xs-right" xs4>Client:</v-flex>
              <v-flex xs8>{{sessionDetailed.clientFullname}} ({{sessionDetailed.clientCode}})</v-flex>
              <v-flex class="body-2 text-xs-right" xs4>Pos:</v-flex>
              <v-flex xs8>{{sessionDetailed.pos}}</v-flex>
              <v-flex class="body-2 text-xs-right" xs4>Session type:</v-flex>
              <v-flex xs8>{{sessionDetailed.sessionType}}</v-flex>
              <v-flex class="body-2 text-xs-right" xs4>Service:</v-flex>
              <v-flex xs8>{{sessionDetailed.hcpcs}} ({{sessionDetailed.description}})</v-flex>
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
      <v-btn :disabled="loading" flat @click="close">Cancel</v-btn>
      <v-btn :disabled="loading" color="warning" @click="clear">clear</v-btn>
      <v-btn :disabled="loading" :loading="loading" color="primary" @click="save">Save</v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>
import sessionServicesApi from '@/services/api/SessionServices';
import SignaturePad from 'signature_pad';
import trimCanvas from 'trim-canvas';

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
    }
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
        this.sessionDetailed = await sessionServicesApi.getSessionDetailed(this.id);
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loading = false;
      }
    },

    clear() {
      this.signaturePad.clear();
    },

    close() {
      this.$router.push(this.$store.getters.lastPath);
    },

    async save() {
      trimCanvas(this.$refs.canvas);
      let sign = {
        sessionId: this.id,
        sign: this.signaturePad.toDataURL()
      }
      try {
        this.loading = true;
        await sessionServicesApi.saveSessionSign(sign);
        this.close();
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally { this.loading = false; }
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
      }
    }
  }

}
</script>