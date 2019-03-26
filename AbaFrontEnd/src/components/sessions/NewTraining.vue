<template>
  <v-container fluid grid-list-xs pa-0>
    <v-layout row>
      <v-flex sm12 md8 lg5>
        <v-card class="elevation-12">
          <v-toolbar dense dark color="secondary">
            <v-toolbar-title>Create new Caregiver training</v-toolbar-title>
          </v-toolbar>
          <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
          <v-card-text>
            <v-form ref="form" autocomplete="off" v-model="validForm">
              <v-layout row wrap>
                <v-flex sm12>
                  <v-subheader>{{activeDate | moment('LL')}}</v-subheader>
                  <!-- <v-text-field box required label="Session date" v-model="dateSelected" return-masked-value prepend-icon="fa-calendar" mask="##/##/####" data-vv-name="dateSelected" :rules="errors.collect('dateSelected')" v-validate="'required|date_format:MM/DD/YYYY'"/> -->
                  <v-text-field box required label="Session date" v-model="dateSelected" return-masked-value prepend-icon="fa-calendar" mask="##/##/####" :rules="required"/>
                </v-flex>
                <v-flex sm12>
                  <v-select box :disabled="loading" label="Pos" v-model="session.pos" required :items="posEnum" prepend-icon="fa-map-marker-alt" :rules="[required]"></v-select>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex sm12 md6>
                  <v-text-field box :disabled="loading" label="From" v-model="timeStart" type="time" required prepend-icon="fa-clock" :rules="[required]"></v-text-field>
                </v-flex>
                <v-flex sm12 md6>
                  <v-text-field box :disabled="loading" label="To" v-model="timeEnd" type="time" required prepend-icon="fa-clock" :rules="[required]"></v-text-field>
                </v-flex>
              </v-layout>
            </v-form>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn :disabled="loading" flat @click="close">Cancel</v-btn>
            <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="submit">Create</v-btn>
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import masterTableApi from '@/services/api/MasterTablesServices';
import sessionServicesApi from '@/services/api/SessionServices';

export default {
  data() {
    return {
      loading: false,
      validForm: false,
      required: (value) => !!value || 'This field is required.',
      posEnum: [],
      timeStart: null,
      timeEnd: null,
      session: {
        pos: null
      },
      dateSelected: null,
    }
  },

  computed: {
    activeClientId() {
      return this.$store.getters.activeClientId;
    },
    activeDate() {
      return this.$store.getters.activeDate;
    }
  },

  async mounted() {
    this.dateSelected = this.$moment(this.activeDate).format('MM/DD/YYYY');
    this.posEnum = await masterTableApi.getPosCodes();
  },

  methods: {
    close() {
      this.$router.push('/clients/sessions_details');
    },

    async submit() {
      try {
        this.loading = true;
        this.session.clientId = this.activeClientId;
        this.session.sessionType = 2; //caregiver training
        let dateFixed = this.dateSelected;
        let start = this.$moment(dateFixed + ' ' + this.timeStart);
        let end = this.$moment(dateFixed + ' ' + this.timeEnd);
        this.session.sessionStart = start;
        this.session.sessionEnd = end;
        await sessionServicesApi.addSession(this.session);
        this.close();
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loading = false;
      }
    }
  }
}
</script>

