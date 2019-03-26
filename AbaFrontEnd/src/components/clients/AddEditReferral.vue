<template>
  <v-dialog width="600" persistent transition="slide-y-transition" v-model="model" :key="data.referralId">
    <v-card>
      <v-card-title class="teal white--text">
        <div class="title">{{data.referralId === 0 ? 'New' : 'Edit'}} referral</div>
      </v-card-title>
      <v-container grid-list-md pa-1>
        <v-layout row wrap>
          <v-flex xs12>
            <v-card-text>
              <v-form ref="form" autocomplete="off" v-model="validForm">
                <v-container fluid grid-list-md pa-0>
                  <v-layout row wrap>
                    <v-flex sm12 md12>
                      <v-text-field box :disabled="loading" label="Fullname" v-model="data.referralFullname" required :rules="[required]" prepend-icon="fa-tag"/>
                    </v-flex>
                  </v-layout>
                  <v-layout row wrap>
                    <v-flex sm12 md7>
                      <v-text-field box :disabled="loading" label="Specialty" v-model="data.specialty" required :rules="[required]" prepend-icon="fa-briefcase-medical"/>
                    </v-flex>
                    <v-flex sm12 md5>
                      <v-text-field box :disabled="loading" label="License" v-model="data.license" prepend-icon="fa-shield"/>
                    </v-flex>
                  </v-layout>
                  <v-layout row wrap>
                    <v-flex sm12 md7>
                      <v-text-field box :disabled="loading" label="Provider" v-model="data.provider" prepend-icon="fa-notes-medical"/>
                    </v-flex>
                    <v-flex sm12 md5>
                      <v-text-field box :disabled="loading" label="NPI" v-model="data.npi" prepend-icon="fa-clipboard-list"/>
                    </v-flex>
                  </v-layout>
                  <v-layout row wrap>
                    <v-flex sm12 md12>
                      <v-text-field box :disabled="loading" label="Address" v-model="data.fullAddress" prepend-icon="fa-map-marked-alt"/>
                    </v-flex>
                  </v-layout>
                  <v-layout row wrap>
                    <v-flex sm12 md4>
                      <v-text-field box :disabled="loading" label="Phone" v-model="data.phone" prepend-icon="fa-phone" type="phone" mask="phone"></v-text-field>
                    </v-flex>
                    <v-flex sm12 md6>
                      <v-text-field box :disabled="loading" label="Fax" v-model="data.fax" prepend-icon="fa-fax" type="phone" mask="phone"></v-text-field>
                    </v-flex>
                    <v-flex sm12>
                      <v-text-field box :disabled="loading" label="Email" v-model="data.email" prepend-icon="fa-envelope" type="email" data-vv-name="email" :rules="errors.collect('email')" v-validate="'email'"></v-text-field>
                    </v-flex>
                  </v-layout>
                  <v-layout row wrap>
                    <v-flex sm12 md6>
                      <!-- <v-text-field box :disabled="loading" label="Date referral" v-model="data.dateReferral" prepend-icon="fa-calendar-plus" return-masked-value required mask="##/##/####" data-vv-name="referral" :rules="errors.collect('referral')" v-validate="'required|date_format:MM/DD/YYYY'"></v-text-field> -->
                      <v-text-field box :disabled="loading" label="Date referral" v-model="data.dateReferral" prepend-icon="fa-calendar-plus" return-masked-value required mask="##/##/####" :rules="[required]"></v-text-field>
                    </v-flex>
                    <v-flex sm12 md6>
                      <!-- <v-text-field box :disabled="loading" label="Date expires" v-model="data.dateExpires" prepend-icon="fa-calendar-minus" return-masked-value mask="##/##/####" required data-vv-name="expires" :rules="errors.collect('expires')" v-validate="'required|date_format:MM/DD/YYYY'"></v-text-field> -->
                      <v-text-field box :disabled="loading" label="Date expires" v-model="data.dateExpires" prepend-icon="fa-calendar-minus" return-masked-value mask="##/##/####" required :rules="[required]"></v-text-field>
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
          <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="saveChanges">{{data.referralId === 0 ? 'Add' : 'Save'}}</v-btn>
        </v-card-actions>
      </v-container>
    </v-card>
  </v-dialog>
</template>

<script>
import clientApi from '@/services/api/ClientServices';

export default {
  props: {
    model: {
      type: Boolean,
      required: true,
      default: false,
    },
    data: {
      type: Object,
    },
  },

  data() {
    return {
      loading: false,
      validForm: false,
      required: (value) => !!value || 'This field is required.',
    };
  },

  methods: {
    async saveChanges() {
      this.loading = true;
      try {
        let referral = await clientApi.addEditReferral(this.data);
        this.$emit('onSubmit', referral);
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