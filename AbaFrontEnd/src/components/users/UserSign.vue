<template>
  <v-card>
    <v-toolbar dark class="secondary" fluid dense>
      <v-toolbar-title>User sign session</v-toolbar-title>
    </v-toolbar>
    <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
    <v-card-text>
      <v-container fluid grid-list-sm pa-0>
        <v-layout row wrap>
          <v-flex xs12>
            <canvas :src="!userSign || userSign.sign" color="black" ref="canvas" width=400 height=200 style="border: 1px solid black;"></canvas>
          </v-flex>
        </v-layout>
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
  import userApi from '@/services/api/UserServices';
  import SignaturePad from 'signature_pad';
  import trimCanvas from 'trim-canvas';

  export default {
    props: {
      userId: { require: true }
    },

    data() {
      return {
        loading: false,
        userSign: null,
        signaturePad: null,
        signatureData: null,
      }
    },

    mounted() {
      this.signaturePad = new SignaturePad(this.$refs.canvas);
      this.loadUserSign();
      this.resizeCanvas();
    },

    methods: {
      async loadUserSign() {
        try {
          this.loading = true;
          this.userSign = await userApi.getUserSign(this.userId);
          this.signaturePad.fromDataURL(this.userSign.sign);
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
        let userSign = {
          userSignId: this.userSign.userSignId || 0,
          userId: this.userId,
          sign: this.signaturePad.toDataURL(),
        }
        try {
          this.loading = true;
          await userApi.addEditUserSign(userSign);
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