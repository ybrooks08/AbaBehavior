<template>
  <v-container fluid grid-list-xs class="pa-0">
    <v-layout row>
      <v-flex sm12 md10>
        <v-card class="elevation-12">
          <v-toolbar dense dark color="secondary">
            <v-toolbar-title>Tellus configuration</v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <v-form ref="form" autocomplete="off" v-model="validForm">
              <v-subheader>Stored information</v-subheader>
              <v-layout row wrap>
                <v-flex sm12 md6>
                  <v-text-field box :disabled="loading" label="Username" v-model="username" required prepend-icon="fa-user" :rules="[required]"></v-text-field>
                </v-flex>
                <v-flex sm12 md6>
                  <v-text-field box :disabled="loading" label="Password" v-model="password" type="password" required prepend-icon="fa-lock" data-vv-name="password" :rules="[required]"
                                v-validate="'required'"></v-text-field>
                </v-flex>
              </v-layout>            
            </v-form>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn :disabled="loading" flat @click="close">Cancel</v-btn>
            <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="submit">Save</v-btn>
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import tellusApi from "@/services/api/TellusServices";

export default {
  props: {
    id: {
      type: [Number, String],
      required: false,
      default: 0
    }
  },

  data() {
    return {
      loading: false,
      roles: [],
      required: value => !!value || "This field is required.",
      validForm: false,
      username: null,
      email: null,
      password: null,
      repassword: null,
      firstname: null,
      lastname: null,
      rol: -1,

    
      sessionsDateAllowed: null
    };
  },

  computed: {
    states() {
      return this.$store.getters.states;
    },
    user() {
      return this.$store.getters.user;
    }
  },

  async mounted() {
    ///this.roles = await tellusApi.getRoles();
    ///if (this.id !== 0) this.loadUser();
  },

  methods: {
    close() {
      this.$router.push("/users");
    },

    async loadUser() {
      this.loading = true;
      try {
        let user = await tellusApi.getUser(this.id);
        this.username = user.username;
        this.email = user.email;
        this.password = "dummy18";        

        this.sessionsDateAllowed = user.sessionsDateAllowed;
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async submit() {
      const user = {
        userId: this.id,
        username: this.username,
        password: this.password
      };

      this.loading = true;

      try {
        await tellusApi.addTellusConfig(user);
        ///this.$toast.success(`Tellus ${this.id === 0 ? "added" : "edited"} successful.`);
        this.$toast.success("Tellus credentials added successful.");
        this.close();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>