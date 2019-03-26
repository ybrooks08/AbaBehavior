<template>
  <v-dialog width="600" persistent transition="slide-y-transition" v-model="model">
    <v-card>
      <v-card-title class="teal white--text">
        <div class="title">Edit session time</div>
      </v-card-title>
      <v-container grid-list-md pa-1>
        <v-layout row wrap>
          <v-flex xs12>
            <v-card-text>
              <v-form ref="form" autocomplete="off" v-model="validForm">
                <v-container fluid grid-list-md pa-0>
                  <v-layout row wrap>
                    <v-flex sm12 md6>
                      <v-text-field box :disabled="loading" label="From" v-model="timeStart" type="time" required prepend-icon="fa-clock" :rules="[required]"></v-text-field>
                    </v-flex>
                    <v-flex sm12 md6>
                      <v-text-field box :disabled="loading" label="To" v-model="timeEnd" type="time" required prepend-icon="fa-clock" :rules="[required]"></v-text-field>
                    </v-flex>
                  </v-layout>
                  <v-layout row wrap>
                    <v-flex xs12>
                      <v-alert type="info" :value="durationMins > 0">Units: {{units}} | Mins: {{durationMins}} | Hrs: {{(durationMins / 60).toFixed(1)}}</v-alert>
                    </v-flex>
                  </v-layout>
                </v-container>
              </v-form>
            </v-card-text>
          </v-flex>
        </v-layout>

        <v-card-actions>
          <v-spacer/>
          <v-btn :disabled="loading" flat @click="cancel">Cancel</v-btn>
          <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="saveChanges">Save</v-btn>
        </v-card-actions>
      </v-container>
    </v-card>
  </v-dialog>
</template>

<script>
import sessionServicesApi from '@/services/api/SessionServices';

export default {
  name: 'EditTime',

  props: {
    model: {
      type: Boolean,
      required: true,
      default: false,
    },
    sessionId: null,
  },

  data() {
    return {
      loading: false,
      validForm: false,
      required: (value) => !!value || 'This field is required.',
      orgTimeStart: null,
      orgTimeEnd: null,
      timeStart: null,
      timeEnd: null,
      unitsAvailable: 0,
    };
  },

  computed: {
    durationMins() {
      return this.$moment(this.timeEnd, 'HH:mm').local().diff(this.$moment(this.timeStart, 'HH:mm').local(), 'minutes');
    },
    units() {
      return Math.round(this.durationMins / 15);
    },
  },

  methods: {
    async saveChanges() {
      this.loading = true;
      try {
        let s1 = this.$moment(`${this.orgTimeStart.format('MM/DD/YYYY')} ${this.timeStart}`);
        let s2 = this.$moment(`${this.orgTimeEnd.format('MM/DD/YYYY')} ${this.timeEnd}`);
        let data = {
          sessionId: this.sessionId,
          start: s1,
          end: s2,
        };
        await sessionServicesApi.editSessionTime(data);
        this.$toast.success('Session time saved successful.');
        this.$emit('onSubmit');
        this.$refs.form.reset();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    cancel() {
      this.$emit('cancel');
      this.$refs.form.reset();
    },
  },
};
</script>

<style scoped>
</style>