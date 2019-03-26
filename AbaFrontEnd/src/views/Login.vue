<template>
  <v-app class="blue-grey lighten-5">
    <v-container fluid fill-height>
      <v-layout align-center justify-center>
        <v-flex xs12 sm6 md4>
          <v-card class="grey lighten-4 elevation-12" width="360">
            <v-form ref="loginForm">
              <div class="text-xs-center" style="padding-top: 10px;">
                <v-icon style="font-size: 72px;" class="mdi">fa-user-circle</v-icon>
                <br>
                <span class="body-1 grey--text text--darken-1">Login to your account</span>
              </div>
              <v-card-text>
                <v-text-field box label="Username" v-model="username" prepend-icon="fa-user" required :disabled="loading" @keypress.enter.native="loginAction"/>
                <v-text-field box label="Password" v-model="password" prepend-icon="fa-lock" required type="password" :disabled="loading" @keypress.enter.native="loginAction"/>
              </v-card-text>
              <v-card-actions>
                <v-btn color="primary" block :disabled="loading " :loading="loading" @click="loginAction">SIGN IN</v-btn>
              </v-card-actions>
            </v-form>
          </v-card>
        </v-flex>
      </v-layout>
    </v-container>
  </v-app>
</template>

<script>
export default {
  data() {
    return {
      username: null,
      password: null,
      loading: false,
    };
  },
  methods: {
    async loginAction() {
      this.loading = true;
      try {
        await this.$store.dispatch('AUTH_REQUEST', { username: this.username, password: this.password });
        this.$router.push('/home');
      } catch (error) {
        if (error.response) {
          this.$toast.error(error.response.data || error.message);
        } else {
          this.$toast.error(error.message);
        }
      } finally { this.loading = false; }
    },
  },
};
</script>

