<template>
  <v-dialog persistent width="600" v-model="open">
    <v-card class="grey lighten-3">
      <v-toolbar dark dense fluid>
        <v-toolbar-title>STOs for {{data.problemBehavior.problemBehaviorDescription}}</v-toolbar-title>
      </v-toolbar>
      <v-card-text class="pa-1">
        <v-list dense subheader>
          <v-list-tile v-for="(p, index) in clientProblemStos" :key="p.clientProblemStoId">
            <v-list-tile-avatar>
              <!-- <v-icon>fa-medal</v-icon> -->
              <v-avatar size="32" color="secondary">
                <span class="white--text headline">{{index + 1}}</span>
              </v-avatar>
            </v-list-tile-avatar>
            <v-list-tile-content>
              <v-list-tile-title class="body-2">
                Get
                <span class="purple--text font-weight-black">{{p.quantity}}{{isPercent ? '% or more':' or less'}}</span> in
                <span class="purple--text font-weight-black">{{p.weeks}}</span> consecutive week(s)
              </v-list-tile-title>
              <v-list-tile-sub-title>
                Status:
                <strong :class="p.status.toLowerCase() == 'failed' ? 'red--text': p.status.toLowerCase() == 'success' ? 'green--text':''">{{p.status}}</strong>
                &nbsp;&nbsp;&nbsp;
                <small>{{p.weekStart | moment('utc','MM/DD/YYYY')}} - {{p.weekEnd | moment('utc','MM/DD/YYYY')}}</small>
              </v-list-tile-sub-title>
            </v-list-tile-content>
            <v-list-tile-action>
              <v-btn icon :disabled="formShow" @click.stop="updateSto(p)">
                <v-icon small color="grey">fa-pen</v-icon>
              </v-btn>
            </v-list-tile-action>
            <v-list-tile-action>
              <v-btn icon :disabled="formShow" @click.stop="deleteSto(p.clientProblemStoId)">
                <v-icon small color="grey">fa-trash</v-icon>
              </v-btn>
            </v-list-tile-action>
          </v-list-tile>
        </v-list>
        <v-btn v-if="!formShow" :disabled="loading" block flat @click="newSto">Click here to add new STO</v-btn>
        <div v-show="formShow" class="pt-2">
          <v-form ref="form" autocomplete="off" v-model="formValid">
            <v-layout row wrap>
              <v-flex xs12 md2>
                <v-subheader style="float: right;">{{isPercent ? 'Percent':'Frecuency'}}</v-subheader>
              </v-flex>
              <v-flex xs12 md3>
                <v-text-field ref="focusInput" solo hide-details v-model="clientProblemSto.quantity" type="number" :rules="[required]" required :append-icon="isPercent ? 'fa-percent fa-sm':'fa-frown fa-sm'"></v-text-field>
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
      </v-card-text>

      <v-card-actions>
        <v-spacer />
        <v-btn :disabled="loading" :loading="loading" color="primary" @click="$emit('closed')">Close</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import clientApi from "@/services/api/ClientServices";

export default {
  props: {
    open: {
      type: Boolean,
      required: true,
      default: false
    },
    data: {
      type: Object
    }
  },

  mounted() {
    this.loadClientProblemStos();
  },

  data() {
    return {
      required: value => !!value || "This field is required.",
      loading: false,
      clientProblemStos: [],
      formShow: false,
      formValid: false,
      clientProblemSto: {
        clientProblemStoId: null,
        quantity: null,
        weeks: null
      }
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
      this.formShow = true;
    },

    async deleteSto(clientProblemStoId) {
      this.$confirm("Do you want to delete this STO?").then(async res => {
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
        this.loadClientProblemStos();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
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