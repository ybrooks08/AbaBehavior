<template>
  <div>
    <v-autocomplete box hid :disabled="loading" :items="users" v-model="userId" label="User" prepend-icon="fa-user" item-text="fullname" item-value="userId" :clearable="clearable" @input="onSelectUser">
      <template slot="item" slot-scope="{ item }">
        <v-list-tile-avatar>
          <v-icon>fa-user</v-icon>
        </v-list-tile-avatar>
        <v-list-tile-content>
          <v-list-tile-title :class="{ 'grey--text text--lighten-1': !item.active }">{{item.fullname}}</v-list-tile-title>
          <v-list-tile-sub-title :class="{ 'grey--text text--lighten-1': !item.active }">{{item.rolname}}</v-list-tile-sub-title>
        </v-list-tile-content>
      </template>
    </v-autocomplete>
  </div>
</template>

<script>
import userApi from '@/services/api/UserServices';

export default {
  props: {
    value: { type: Number },
    clearable: {
      type: Boolean,
      default: false
    }
  },

  data() {
    return {
      loading: false,
      users: [],
      userId: this.value,
    };
  },

  created() {
    this.loadUsers();
  },

  methods: {
    async loadUsers() {
      this.users = [];
      this.loading = true;
      try {
        this.users = await userApi.getUsersCanCreateSessions();
      } catch (error) {
        this.$toast.error(error);
      } finally { this.loading = false; }
    },

    onSelectUser() {
      this.$emit('input', this.userId);
    }
  },

};
</script>