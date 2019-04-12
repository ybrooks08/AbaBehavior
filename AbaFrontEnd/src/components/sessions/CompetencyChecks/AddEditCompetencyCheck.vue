<template>
  <v-card class="elevation-12" max-width="750">
    <v-toolbar color="secondary" dark tabs dense>
      <v-toolbar-title>{{(id === 0 ? 'Create new' : 'Edit')}} competency check</v-toolbar-title>
    </v-toolbar>
    <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
    <v-card-text>
      <v-form ref="form" autocomplete="off" v-model="validForm">
        <v-layout row wrap>
          <v-flex sm12>
            <v-select box hide-details prepend-icon="fa-user-check" :items="competencyCheckTypeEnum" v-model="comp.competencyCheckType" label="Competency type"></v-select>
          </v-flex>
          <v-flex sm6>
            <v-text-field box prepend-icon="fa-calendar" v-model="comp.date" label="Date" mask="##/##/####" return-masked-value :rules="[required]" data-vv-name="notedate" :error-messages="errors.collect('notedate')" v-validate="'required|date_format:MM/dd/yyyy'"></v-text-field>
          </v-flex>
          <v-flex sm6>
            <v-text-field box prepend-icon="fa-clock" v-model="comp.totalDuration" label="Duration" mask="##" suffix="hours" data-vv-name="totalduration" :rules="[required]" :error-messages="errors.collect('totalduration')" v-validate="'required'"></v-text-field>
          </v-flex>
        </v-layout>
        <v-layout v-if="comp.competencyCheckType == 2" row wrap>
          <v-flex xs12>
            <v-select box prepend-icon="fa-user-check" :disabled="loading" label="Caregiver" v-model="comp.caregiverId" :items="clientCaregivers"></v-select>
          </v-flex>
        </v-layout>
        <v-layout v-if="comp.competencyCheckType == 1" row wrap>
          <v-flex xs12>
            <v-select box prepend-icon="fa-user" :disabled="loading" label="RBT" v-model="comp.userId" :items="clientRbt"></v-select>
          </v-flex>
        </v-layout>

        <template v-for="(p, index) in params">
          <v-layout row wrap :key="p.index">
            <v-flex xs5>
              <span>{{p.competencyCheckParam.description}}</span>
            </v-flex>
            <v-flex xs7>
              <v-layout row wrap>
                <v-flex class="text-xs-right">
                  <v-btn-toggle mandatory v-model="p.score">
                    <v-btn color="error" flat>0</v-btn>
                    <v-btn color="success" flat>1</v-btn>
                  </v-btn-toggle>
                </v-flex>
              </v-layout>
              <v-textarea box auto-grow rows="3" v-model="p.comment" label="Comment" :hint="p.competencyCheckParam.comment" persistent-hint></v-textarea>
            </v-flex>
          </v-layout>
          <v-divider :key="'divider ' + index"></v-divider>
        </template>
      </v-form>
    </v-card-text>
    <v-card-actions>
      <v-spacer></v-spacer>
      <v-btn :disabled="loading" flat @click="close">Cancel</v-btn>
      <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="submit">{{id === 0 ? 'Add' : 'Save'}}</v-btn>
    </v-card-actions>
  </v-card>
  <!-- <v-container fluid grid-list-xs pa-0>
    <v-layout row>
      <v-flex sm12 md8 lg6>
        <v-card class="elevation-12">
          <v-toolbar dense dark color="secondary">
            <v-toolbar-title>{{(id === 0 ? 'Create new' : 'Edit')}} competency check</v-toolbar-title>
          </v-toolbar>
          <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
          <v-card-text>
            <v-form ref="form" autocomplete="off" v-model="validForm">
              <v-layout row wrap>
                <v-flex sm12 pl-4>

                </v-flex>
              </v-layout>

            </v-form>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn :disabled="loading" flat @click="close">Cancel</v-btn>
            <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="submit">{{id === 0 ? 'Create' : 'Save'}}</v-btn>
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>-->
</template>
<script>
import masterTableApi from '@/services/api/MasterTablesServices';
import sessionServicesApi from '@/services/api/SessionServices';
import clientApi from '@/services/api/ClientServices';

export default {
  props: {
    id: {
      type: [Number, String],
      required: false,
      default: 0,
    },
  },

  data() {
    return {
      loading: false,
      validForm: false,
      required: (value) => !!value || 'This field is required.',
      competencyCheckTypeEnum: [],
      clientCaregivers: [],
      clientRbt: [],
      comp: {
        competencyCheckType: null,
        date: null,
        totalDuration: null,
        competencyCheckClientParams: [],
      },
    };
  },

  computed: {
    activeClientId() {
      return this.$store.getters.activeClientId;
    },
    activeDate() {
      return this.$store.getters.activeDate;
    },
    params() {
      return this.comp.competencyCheckClientParams.filter(s => s.competencyCheckParam.competencyCheckType == this.comp.competencyCheckType);
    },
  },

  async mounted() {
    this.loading = true;
    this.competencyCheckTypeEnum = await masterTableApi.getCompetencyCheckTypes();
    this.comp = await sessionServicesApi.getCompetencyCheck(this.id);// await masterTableApi.getCompetencyCheckParams();
    this.comp.date = this.$moment(this.comp.date).utc().format('MM/DD/YYYY');
    this.clientCaregivers = await clientApi.getClientCaregivers(this.activeClientId);
    this.clientRbt = await clientApi.getClientUsers(this.activeClientId, 'tech');
    this.loading = false;
  },

  methods: {
    close() {
      this.$router.push('/competency_checks');
    },

    async submit() {
      try {
        this.loading = true;
        this.comp.clientId = this.activeClientId;
        await sessionServicesApi.addEditCompetencyCheck(this.comp);
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