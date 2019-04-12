<template>
  <v-container fluid grid-list-xs class="pa-0">
    <v-layout row>
      <v-flex sm12 md10>
        <v-card class="elevation-12">
          <v-toolbar dense dark color="secondary">
            <v-toolbar-title>{{(id === 0 ? 'Create new user' : 'Edit user ' + username)}}</v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <v-form ref="form" autocomplete="off" v-model="validForm">
              <v-subheader>Account info</v-subheader>
              <v-layout row wrap>
                <v-flex sm12 md6>
                  <v-text-field box :disabled="loading || id !==0" label="Username" v-model="username" required prepend-icon="fa-user" :rules="[required]"></v-text-field>
                </v-flex>
                <v-flex sm12 md6>
                  <v-select box :disabled="loading" label="Rol" v-model="rol" required :items="roles" item-text="rolName" item-value="rolId" prepend-icon="fa-users-cog" hint="Documents will be added automatically" :persistent-hint="id == 0"></v-select>
                </v-flex>
              </v-layout>
              <v-layout row wrap v-if="id === 0">
                <v-flex sm12 md6>
                  <v-text-field box :disabled="loading" label="Password" v-model="password" type="password" required prepend-icon="fa-lock" data-vv-name="password" :rules="errors.collect('password')" v-validate="'required'"></v-text-field>
                </v-flex>
                <v-flex sm12 md6>
                  <v-text-field box :disabled="loading" label="rePassword" v-model="repassword" type="password" required prepend-icon="fa-lock" data-vv-name="repassword" :rules="errors.collect('repassword')" v-validate="{required, confirmed:password}"></v-text-field>
                </v-flex>
              </v-layout>
              <v-subheader>Personal info</v-subheader>
              <v-layout row wrap>
                <v-flex sm12 md6>
                  <v-text-field box :disabled="loading" label="Firstname" v-model="firstname" required prepend-icon="fa-tag" :rules="[required]"></v-text-field>
                </v-flex>
                <v-flex sm12 md6>
                  <v-text-field box :disabled="loading" label="Lastname" v-model="lastname" required prepend-icon="fa-tag" :rules="[required]"></v-text-field>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex sm12 md8>
                  <v-text-field box :disabled="loading" label="Email" v-model="email" prepend-icon="fa-envelope" type="email" required data-vv-name="email" :rules="errors.collect('email')" v-validate="'required|email'"></v-text-field>
                </v-flex>
                <v-flex sm12 md4>
                  <v-text-field box :disabled="loading" label="Phone" v-model="phone" prepend-icon="fa-phone" type="phone" mask="phone"></v-text-field>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex sm12 md9>
                  <v-text-field box :disabled="loading" label="Address" v-model="address" prepend-icon="fa-map-marked-alt"></v-text-field>
                </v-flex>
                <v-flex sm12 md3>
                  <v-text-field box :disabled="loading" label="Apt/Ste" v-model="apt" prepend-icon="fa-building"></v-text-field>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex sm12 md5>
                  <v-text-field box :disabled="loading" label="City" v-model="city" prepend-icon="fa-map"></v-text-field>
                </v-flex>
                <v-flex sm12 md4>
                  <v-select box hide-no-data hide-selected :disabled="loading" :items="states" v-model="state" label="State" prepend-icon="fa-map-signs" browser-autocomplete="off"></v-select>
                </v-flex>
                <v-flex sm12 md3>
                  <v-text-field box :disabled="loading" label="Zip" v-model="zipcode" prepend-icon="fa-map-marker-alt" mask="#####"></v-text-field>
                </v-flex>
              </v-layout>
              <v-subheader>Privacy info</v-subheader>
              <v-layout row wrap>
                <v-flex sm12 md3>
                  <v-text-field box :disabled="loading" label="NPI" v-model="npi" prepend-icon="fa-address-card"></v-text-field>
                </v-flex>
                <v-flex sm12 md3>
                  <v-text-field box :disabled="loading" label="Provider Id" v-model="mpi" prepend-icon="fa-id-card"></v-text-field>
                </v-flex>
                <v-flex sm12 md3>
                  <v-text-field box :disabled="loading" label="License No" v-model="licenseNo" prepend-icon="fa-file-alt"></v-text-field>
                </v-flex>
                <v-flex sm12 md3>
                  <v-text-field box :disabled="loading" label="Social Security" v-model="socialSecurity" prepend-icon="fa-fingerprint" mask="social"></v-text-field>
                </v-flex>
              </v-layout>
              <template>
                <v-subheader>Bank info</v-subheader>
                <v-layout row wrap>
                  <v-flex sm12 md4>
                    <v-text-field box :disabled="loading" label="Bank name" v-model="bankName" prepend-icon="fa-address-card"></v-text-field>
                  </v-flex>
                  <v-flex sm12 md8>
                    <v-text-field box :disabled="loading" label="Bank full address" v-model="bankAddress" prepend-icon="fa-id-card"></v-text-field>
                  </v-flex>
                </v-layout>
                <v-layout row wrap>
                  <v-flex sm12 md3>
                    <v-text-field box :disabled="loading" label="Route number" v-model="bankRoutingNumber" prepend-icon="fa-route"></v-text-field>
                  </v-flex>
                  <v-flex sm12 md3>
                    <v-text-field box :disabled="loading" label="Account number" v-model="bankAccountNumber" prepend-icon="fa-file-invoice-dollar"></v-text-field>
                  </v-flex>
                  <v-flex sm12 md3>
                    <v-text-field box :disabled="loading" label="Pay rate" v-model="payRate" prepend-icon="fa-dollar-sign" type="number"></v-text-field>
                  </v-flex>
                  <v-flex sm12 md3>
                    <v-text-field box :disabled="loading" label="Drive Time Pay rate" v-model="driveTimePayRate" prepend-icon="fa-dollar-sign" type="number"></v-text-field>
                  </v-flex>
                </v-layout>
              </template>
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
import userApi from '@/services/api/UserServices';

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
      roles: [],
      required: (value) => !!value || 'This field is required.',
      validForm: false,
      username: null,
      email: null,
      password: null,
      repassword: null,
      firstname: null,
      lastname: null,
      rol: -1,

      phone: null,
      address: null,
      apt: null,
      city: null,
      state: null,
      zipcode: null,
      npi: null,
      mpi: null,
      licenseNo: null,
      socialSecurity: null,
      bankName: null,
      bankAddress: null,
      bankRoutingNumber: null,
      bankAccountNumber: null,
      payRate: null,
      driveTimePayRate: null
    };
  },


  computed: {
    states() {
      return this.$store.getters.states;
    },
    user() {
      return this.$store.getters.user;
    },
  },

  async mounted() {
    this.roles = await userApi.getRoles();
    if (this.id !== 0) this.loadUser();
  },

  methods: {
    close() {
      this.$router.push('/users');
    },

    async loadUser() {
      this.loading = true;
      try {
        let user = await userApi.getUser(this.id);
        this.username = user.username;
        this.email = user.email;
        this.password = "dummy18";
        this.repassword = "dummy18";
        this.firstname = user.firstname;
        this.lastname = user.lastname;
        this.rol = user.rolId;

        this.phone = user.phone;
        this.address = user.address;
        this.apt = user.apt;
        this.city = user.city;
        this.state = user.state;
        this.zipcode = user.zipcode;
        this.npi = user.npi;
        this.mpi = user.mpi;
        this.licenseNo = user.licenseNo;
        this.socialSecurity = user.socialSecurity;
        this.bankName = user.bankName;
        this.bankAddress = user.bankAddress;
        this.bankRoutingNumber = user.bankRoutingNumber;
        this.bankAccountNumber = user.bankAccountNumber;
        this.payRate = user.payRate;
        this.driveTimePayRate = user.driveTimePayRate;
      } catch (error) {
        this.$toast.error(error);
      } finally { this.loading = false; }
    },

    async submit() {
      const user = {
        userId: this.id,
        username: this.username,
        email: this.email,
        password: this.password,
        repassword: this.repassword,
        firstname: this.firstname,
        lastname: this.lastname,
        rolId: this.rol,
        phone: this.phone,
        address: this.address,
        apt: this.apt,
        city: this.city,
        state: this.state,
        zipcode: this.zipcode,
        npi: this.npi,
        mpi: this.mpi,
        licenseNo: this.licenseNo,
        socialSecurity: this.socialSecurity,
        bankName: this.bankName,
        bankAddress: this.bankAddress,
        bankRoutingNumber: this.bankRoutingNumber,
        bankAccountNumber: this.bankAccountNumber,
        payRate: this.payRate,
        driveTimePayRate: this.driveTimePayRate
      };

      this.loading = true;

      try {
        await userApi.addEditUser(user);
        this.$toast.success(`User ${this.id === 0 ? 'added' : 'edited'} successful.`);
        this.close();
      } catch (error) {
        this.$toast.error(error);
      } finally { this.loading = false; }
    },
  },
};
</script>