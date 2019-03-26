<template>
  <v-dialog width="600" persistent transition="slide-y-transition" v-model="model">
    <v-card>
      <v-card-title class="teal white--text">
        <div class="title">
          New assignment
        </div>
      </v-card-title>
      <v-container grid-list-md pa-1>
        <v-layout row wrap>
          <v-flex xs12>
            <v-card-text>
              <v-form ref="form" autocomplete="off" v-model="validForm">
                <v-layout row wrap>
                  <v-flex sm12 md12>
                    <v-autocomplete box :disabled="loading" :items="users" v-model="userId" label="User" prepend-icon="fa-user-tie" item-text="fullname" item-value="userId" :rules="[required]" required></v-autocomplete>
                  </v-flex>
                </v-layout>
              </v-form>
            </v-card-text>
          </v-flex>
        </v-layout>
        <v-card-actions>
          <v-spacer/>
          <v-btn :disabled="loading" flat @click="cancel">Cancel</v-btn>
          <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="saveChanges">Add</v-btn>
        </v-card-actions>
      </v-container>
    </v-card>
  </v-dialog>
</template>

<script>
import clientApi from '@/services/api/ClientServices';
import userApi from '@/services/api/UserServices';

export default {
  props: {
    model: {
      type: Boolean,
      required: true,
      default: false,
    },
    clientId: null
  },

  data() {
    return {
      loading: false,
      required: (value) => !!value || 'This field is required.',
      validForm: false,
      users: [],
      userId: null
    };
  },

  async mounted() {
    this.users = await userApi.getUsersCanCreateSessions();
  },

  methods: {
    async saveChanges() {
      this.loading = true;
      try {
        const data = {
          userId: this.userId,
          clientId: this.clientId
        }
        let assigment = await clientApi.addAssignment(data);
        this.$emit('onSubmit', assigment);
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
  }
}
</script>

<style scoped>
</style>