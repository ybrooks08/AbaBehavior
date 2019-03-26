<template>
  <v-dialog max-width="350" persistent v-model="model">
    <v-card>
      <v-card-title class="teal white--text">
        <div class="title">
          Change user password
        </div>
      </v-card-title>
      <v-container class="pa-0">
        <v-layout row wrap>
          <v-flex xs12>
            <v-card-text>
              <v-form ref="form" autocomplete="off" @keyup.enter.native="SaveChanges">
                <v-layout row>
                  <v-flex xs12>
                    <v-text-field box :disabled="loading" label="Password" v-model="password" type="password" required prepend-icon="fa-lock"/>
                  </v-flex>
                </v-layout>
                <v-layout row>
                  <v-flex xs12>
                    <v-text-field box :disabled="loading" label="re-Password" v-model="repassword" type="password" required prepend-icon="fa-lock"  />
                  </v-flex>
                </v-layout>
              </v-form>
            </v-card-text>
          </v-flex>
        </v-layout>

        <v-layout row wrap>
          <v-flex xs12>
            <v-card-actions>
              <v-spacer/>
              <v-btn :disabled="loading" flat @click="close">Cancel</v-btn>
              <v-btn :disabled="loading" :loading="loading" color="primary" @click="SaveChanges">Save</v-btn>
            </v-card-actions>
          </v-flex>
        </v-layout>
      </v-container>
    </v-card>
  </v-dialog>
</template>

<script>
import userApi from '@/services/api/UserServices';

export default {
  props: ['userId', 'model'],

  data() {
    return {
      loading: false,
      password: '',
      repassword: '',
    };
  },

  methods: {
    async SaveChanges() {
      const password = {
        userId: this.userId,
        password: this.password,
        rePassword: this.repassword
      };
      this.loading = true;
      try {
        await userApi.changeUserPassword(password);
        this.$toast.success('Password changed successful.');
        this.close();
      } catch (error) {
        this.$toast.error(error);
      } finally { this.loading = false }
    },

    close() {
      this.$emit('cancel');
      this.$refs.form.reset();
    },
  },
};
</script>