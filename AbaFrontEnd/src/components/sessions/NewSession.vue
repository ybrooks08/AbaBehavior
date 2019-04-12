<template>
  <v-container fluid grid-list-xs pa-0>
    <v-layout row>
      <v-flex sm12 md8 lg5>
        <v-card class="elevation-12">
          <v-toolbar dense dark color="secondary">
            <v-toolbar-title>Create new Session</v-toolbar-title>
          </v-toolbar>
          <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
          <v-card-text>
            <v-chip label class="mb-3 ml-5" :color="unitsAvailable > 0 ? 'success' : 'error'" text-color="white">
              <v-avatar>
                <v-icon>fa-star</v-icon>
              </v-avatar>
              Units available: {{unitsAvailable.toLocaleString()}}
            </v-chip>
            <v-form ref="form" autocomplete="off" v-model="validForm">
              <v-layout row wrap>
                <v-flex sm12>
                  <v-subheader>{{activeDate | moment('LL')}}</v-subheader>
                  <v-text-field box required label="Session date" v-model="dateSelected" return-masked-value prepend-icon="fa-calendar" mask="##/##/####" data-vv-name="dateSelected" :rules="errors.collect('dateSelected')" v-validate="'required|date_format:MM/dd/yyyy'"/>
                </v-flex>
                <v-flex sm12>
                  <v-select box :disabled="loading" label="Pos" v-model="session.pos" required :items="posEnum" prepend-icon="fa-map-marker-alt" :rules="[required]">
                    <template slot="selection" slot-scope="data">
                      <div class="input-group__selections__comma">
                        {{ data.item.text }} &nbsp;
                        <span class="grey--text text--darken-1">({{data.item.value}})</span>
                      </div>
                    </template>
                    <template slot="item" slot-scope="data">
                      <template>
                        <v-list-tile-avatar>
                          <v-icon>fa-map-marker-alt</v-icon>
                        </v-list-tile-avatar>
                        <v-list-tile-content>
                          <v-list-tile-title v-html="data.item.text"></v-list-tile-title>
                          <v-list-tile-sub-title>Code: {{data.item.value}}</v-list-tile-sub-title>
                        </v-list-tile-content>
                      </template>
                    </template>
                  </v-select>
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
              <v-layout row wrap>
                <v-flex xs12>
                  <v-alert type="info" :value="durationMins > 0">Units: {{units}} | Mins: {{durationMins}} | Hrs: {{(durationMins / 60).toFixed(1)}}</v-alert>
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
  props: {
    sessionType: {
      type: [Number, String],
      required: false,
      default: 1,
    },
  },

  data() {
    return {
      loading: false,
      validForm: false,
      required: (value) => !!value || 'This field is required.',
      posEnum: [],
      timeStart: null,
      timeEnd: null,
      session: {
        pos: null,
      },
      unitsAvailable: 0,
      dateSelected: null,
    };
  },

  computed: {
    activeClientId() {
      return this.$store.getters.activeClientId;
    },
    activeDate() {
      return this.$store.getters.activeDate;
    },
    durationMins() {
      return this.$moment(this.timeEnd, 'HH:mm').diff(this.$moment(this.timeStart, 'HH:mm'), 'minutes');
    },
    units() {
      return Math.round(this.durationMins / 15);
    },
  },

  async mounted() {
    this.dateSelected = this.$moment(this.activeDate).format('MM/DD/YYYY');
    this.timeStart = this.$moment(this.activeDate).format('HH:00');
    this.posEnum = await masterTableApi.getPosCodes();
    this.unitsAvailable = await sessionServicesApi.getUnitsAvailable(this.activeClientId, this.$moment(this.activeDate).format('YYYY-MM-DD'), '');
  },

  methods: {
    close() {
      this.$router.push('/clients/sessions_details');
    },

    async submit() {
      try {
        this.loading = true;
        this.session.clientId = this.activeClientId;
        this.session.sessionType = this.sessionType;
        let dateFixed = this.dateSelected;//.toLocaleDateString('en-US');
        let start = new Date(dateFixed + ' ' + this.timeStart);
        let end = new Date(dateFixed + ' ' + this.timeEnd);
        this.session.sessionStart = start;
        this.session.sessionEnd = end;
        this.session.totalUnits = this.units;
        await sessionServicesApi.addSession(this.session);
        //this.$store.dispatch('getCalendarEvents');
        this.close();
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loading = false;
      }
    },
  },
};
</script>