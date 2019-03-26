<template>
  <v-container fluid grid-list-xs pa-0>
    <v-layout row>
      <v-flex sm12 md10 lg8>
        <v-card class="elevation-12">
          <v-toolbar dense dark color="secondary">
            <v-toolbar-title>{{(id === 0 ? 'Create new client' : 'Edit client ' + client.firstname)}}</v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <v-form ref="form" autocomplete="off" v-model="validForm">
              <v-subheader>Personal info</v-subheader>
              <v-layout row wrap>
                <v-flex sm12 md6>
                  <v-text-field box :disabled="loading" label="Firstname" v-model="client.firstname" required prepend-icon="fa-tag" :rules="[required]"></v-text-field>
                </v-flex>
                <v-flex sm12 md6>
                  <v-text-field box :disabled="loading" label="Lastname" v-model="client.lastname" required prepend-icon="fa-tag" :rules="[required]"></v-text-field>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex sm12 md4>
                  <!-- <v-text-field box required label="Date of birth" v-model="client.dob" return-masked-value prepend-icon="fa-birthday-cake" mask="##/##/####" data-vv-name="dob" :rules="errors.collect('dob')" v-validate="'required|date_format:MM/DD/YYYY'"/> -->
                  <v-text-field box required label="Date of birth" v-model="client.dob" return-masked-value prepend-icon="fa-birthday-cake" mask="##/##/####" :rules="[required]"/>
                </v-flex>
                <v-flex sm12 md4>
                  <v-text-field box :disabled="loading" label="Nickname" v-model="client.nickname" prepend-icon="fa-grin-hearts"></v-text-field>
                </v-flex>
                <v-flex sm12 md4>
                  <v-text-field box :disabled="loading" label="Code" v-model="client.code" prepend-icon="fa-barcode"></v-text-field>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex sm12 md4>
                  <v-text-field box :disabled="loading" label="Phone" v-model="client.phone" prepend-icon="fa-phone" type="phone" mask="phone"></v-text-field>
                </v-flex>
                <v-flex sm12 md8>
                  <v-text-field box :disabled="loading" label="Email" v-model="client.email" prepend-icon="fa-envelope" type="email" data-vv-name="email" :rules="errors.collect('email')" v-validate="'email'"></v-text-field>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex sm12 md9>
                  <v-text-field box :disabled="loading" label="Address" v-model="client.address" prepend-icon="fa-map-marked-alt" :rules="[required]" required></v-text-field>
                </v-flex>
                <v-flex sm12 md3>
                  <v-text-field box :disabled="loading" label="Apt/Ste" v-model="client.apt" prepend-icon="fa-building"></v-text-field>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex sm12 md5>
                  <v-text-field box :disabled="loading" label="City" v-model="client.city" prepend-icon="fa-map" :rules="[required]" required></v-text-field>
                </v-flex>
                <v-flex sm12 md4>
                  <v-select box hide-no-data hide-selected :disabled="loading" :items="states" v-model="client.state" label="State" prepend-icon="fa-map-signs" :rules="[required]" required browser-autocomplete="off"></v-select>
                </v-flex>
                <v-flex sm12 md3>
                  <v-text-field box :disabled="loading" label="Zip" v-model="client.zipcode" prepend-icon="fa-map-marker-alt" mask="#####" :rules="[required]" required></v-text-field>
                </v-flex>
              </v-layout>
              <v-subheader>Extra info</v-subheader>
              <v-layout row wrap>
                <v-flex sm12 md6>
                  <v-select box :disabled="loading" :items="genders" v-model="client.gender" label="Gender" prepend-icon="fa-user" clearable></v-select>
                </v-flex>
                <v-flex sm12 md6>
                  <v-select box :disabled="loading" :items="races" v-model="client.race" label="Race" prepend-icon="fa-user-friends" clearable></v-select>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex sm12>
                  <v-select box :disabled="loading" :items="languages" v-model="client.primaryLanguage" label="Primary language" prepend-icon="fa-language" clearable></v-select>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex sm12>
                  <v-text-field box :disabled="loading" label="Emergency contact" v-model="client.emergencyContact" prepend-icon="fa-first-aid"></v-text-field>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex sm12 md6>
                  <v-text-field box :disabled="loading" label="Emergency Phone" v-model="client.emergencyPhone" prepend-icon="fa-phone" mask="phone" type="phone"></v-text-field>
                </v-flex>
                <v-flex sm12 md6>
                  <v-text-field box :disabled="loading" label="Emergency Email" v-model="client.emergencyEmail" prepend-icon="fa-envelope" type="email"></v-text-field>
                </v-flex>
              </v-layout>
              <v-subheader>Privacy info</v-subheader>
              <v-layout row wrap>
                <v-flex sm12 md6>
                  <v-text-field box :disabled="loading" label="Social Security" v-model="client.socialSecurity" prepend-icon="fa-fingerprint" mask="social"></v-text-field>
                </v-flex>
                <v-flex sm12 md6>
                  <v-text-field box :disabled="loading" label="Insurance" v-model="client.insurance" prepend-icon="fa-notes-medical"></v-text-field>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex sm12 md4>
                  <v-text-field box :disabled="loading" label="Member ID" v-model="client.memberNo" prepend-icon="fa-notes-medical"></v-text-field>
                </v-flex>
                <v-flex sm12 md4>
                  <v-text-field box :disabled="loading" label="MMA Plan" v-model="client.mmaPlan" prepend-icon="fa-notes-medical"></v-text-field>
                </v-flex>
                <v-flex sm12 md4>
                  <v-text-field box :disabled="loading" label="MMA ID No." v-model="client.mmaIdNo" prepend-icon="fa-notes-medical"></v-text-field>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex sm12>
                  <v-textarea box :disabled="loading" label="Notes" v-model="client.notes" prepend-icon="fa-sticky-note"></v-textarea>
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
  </v-container>
</template>

<script>
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
      required: (value) => !!value || 'This field is required.',
      menuDob: false,
      validForm: false,
      client: {
        firstname: null,
        lastname: null,
        nickname: null,
        code: null,
        dob: null,
        phone: null,
        email: null,
        address: null,
        apt: null,
        city: null,
        state: null,
        zipcode: null,
        gender: null,
        race: null,
        primaryLanguage: null,
        emergencyContact: null,
        emergencyPhone: null,
        emergencyEmail: null,
        socialSecurity: null,
        insurance: null,
        memberNo: null,
        mmaPlan: null,
        mmaIdNo: null
      }
    };
  },

  computed: {
    states() {
      return this.$store.getters.states;
    },
    genders() {
      return this.$store.getters.genders;
    },
    races() {
      return this.$store.getters.races;
    },
    languages() {
      return this.$store.getters.languages;
    },
  },

  mounted() {
    if (this.id !== 0) this.loadClient();
  },

  methods: {
    close() {
      //this.$router.go(-1);
      this.$router.push('/clients');
    },

    async loadClient() {
      this.loading = true;
      try {
        const c = await clientApi.getClient(this.id);
        c.dob = this.$moment(c.dob).utc().format('MM/DD/YYYY');
        this.client = c;
      } catch (error) {
        this.$toast.error(error);
      } finally { this.loading = false; }
    },

    async submit() {
      this.loading = true;
      try {
        this.client.id = this.id;
        await clientApi.addEditClient(this.client);
        this.$toast.success(`Client ${this.id === 0 ? 'added' : 'edited'} successful.`);
        this.close();
      } catch (error) {
        this.$toast.error(error);
      } finally { this.loading = false; }
    },
  },
};
</script>

<style scoped>
</style>