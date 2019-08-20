<template>
  <v-card class="elevation-8">
    <v-toolbar dark class="secondary" fluid>
      <v-toolbar-title>System Users ({{users.length}})</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-text-field v-model="search" placeholder="Search" prepend-icon="fa-search" hide-details single-line solo-inverted></v-text-field>
      <v-menu class="mr-0" bottom left :disabled="loading">
        <v-btn slot="activator" icon :disabled="loading">
          <v-icon>fa-ellipsis-v</v-icon>
        </v-btn>
        <v-list>
          <v-list-tile to="/users/add_edit">
            <v-list-tile-action>
              <v-icon medium>fa-user-plus</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>New</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
          <v-divider></v-divider>
          <v-list-tile @click="showInactive = !showInactive">
            <v-list-tile-action>
              <v-icon>{{showInactive ? 'fa-user-check':'fa-user-times'}}</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>{{showInactive ? 'Show active users':'Show inactived users'}}</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
          <v-list-tile @click="loadUsers">
            <v-list-tile-action>
              <v-icon medium>fa-sync-alt</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>Refresh</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
        </v-list>
      </v-menu>
    </v-toolbar>

    <!-- <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear> -->
    <v-data-table :headers="headers" :search="search" :items="filterUsers" :loading="loading" hide-actions>
      <template slot="items" slot-scope="props">
        <tr :class="{'red lighten-5': !props.item.active}">
          <td class="text-xs-left pl-3 pr-0 hidden-sm-and-down">{{props.item.username}}</td>
          <td class="text-xs-left px-1 hidden-sm-and-down">
            <v-chip small :color="(props.item.active ? 'blue' : 'red')" text-color="white">
              <v-avatar>
                <v-icon>fa-user-circle</v-icon>
              </v-avatar>
              {{props.item.rolname}}
            </v-chip>
          </td>
          <td class="text-xs-left px-1 text-truncate">{{props.item.firstname}} {{props.item.lastname}}</td>
          <td class="text-xs-left px-1 hidden-sm-and-down">{{props.item.email}}</td>
          <td class="text-xs-left px-1 hidden-xs-only">{{props.item.created | moment('MM/DD/YYYY')}}</td>
          <td class="text-xs-center px-1 hidden-xs-only text-truncate text-no-wrap">
            <v-btn flat icon small color="error" class="mx-0" @click="passDown(props.item)">
              <v-icon small>fa-minus-circle</v-icon>
            </v-btn>
            <span class="body-2">{{props.item.passesAvailable}}</span>
            <v-btn flat icon small color="success" class="mx-0" @click="passUp(props.item)">
              <v-icon small>fa-plus-circle</v-icon>
            </v-btn>
          </td>
          <td class="text-xs-center px-0">
            <v-switch hide-details color="primary" v-model="props.item.active" @change="changeActive(props.item)"></v-switch>
          </td>
          <td class="text-xs-left justify-center px-0 text-no-wrap">
            <v-tooltip top>
              <v-btn slot="activator" icon class="mx-0" @click="changePassword(props.item.userId)">
                <v-icon color="warning">fa-lock-open</v-icon>
              </v-btn>
              <span>Change the password</span>
            </v-tooltip>
            <v-tooltip top>
              <v-btn slot="activator" icon class="mx-0" @click="editUser(props.item.userId)">
                <v-icon color="success">fa-edit</v-icon>
              </v-btn>
              <span>Edit this user</span>
            </v-tooltip>
            <v-tooltip top>
              <v-btn slot="activator" icon class="mx-0" :to="'/users/user_details/' + props.item.userId">
                <v-icon color="teal">fa-eye</v-icon>
              </v-btn>
              <span>User details</span>
            </v-tooltip>
          </td>
        </tr>
      </template>
    </v-data-table>

    <change-password :userId="userId" :model="changePasswordDialog" @cancel="changePasswordDialog = false" />
  </v-card>
</template>

<script>
import ChangePassword from "@/components/users/ChangePassword";
import userApi from "@/services/api/UserServices";

export default {
  data() {
    return {
      users: [],
      search: "",
      loading: false,
      showInactive: false,
      headers: [
        { text: "Username", align: "left", value: "username", class: "pl-3 pr-0 hidden-sm-and-down" },
        { text: "Rol", align: "left", value: "rolname", class: "pr-1 pl-4 hidden-sm-and-down" },
        { text: "Fullname", align: "left", value: "firstname", class: "px-1 hidden-sm-and-down" },
        { text: "Email", align: "left", value: "email", class: "px-1" },
        { text: "Created", align: "left", value: "created", class: "px-1  hidden-xs-only" },
        { text: "Passes", align: "center", value: "passesAvailable", sortable: false, class: "px-1" },
        { text: "Active", align: "left", value: "active", sortable: false, class: "px-1" },
        { text: "", align: "center", value: "active", sortable: false, class: "px-1" }
      ],
      //        pagination: {
      //          sortBy: 'username',
      //        },
      changePasswordDialog: false,
      userId: null
    };
  },

  computed: {
    filterUsers() {
      return this.users.filter(s => s.active !== this.showInactive);
    }
  },

  components: {
    ChangePassword
  },

  mounted() {
    this.loadUsers();
  },

  methods: {
    async loadUsers() {
      this.users = [];
      this.loading = true;
      try {
        this.users = await userApi.getUsers();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async changeActive(item) {
      const newStatus = {
        status: item.active,
        userId: item.userId
      };
      this.loading = true;
      try {
        await userApi.changeUserStatus(newStatus);
        this.$toast.success("Status changed successful.");
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loadUsers();
        this.loading = false;
      }
    },

    changePassword(userId) {
      this.userId = userId;
      this.changePasswordDialog = true;
    },

    editUser(userId) {
      this.$router.push(`/users/add_edit/${userId}`);
    },

    async passUp(user) {
      try {
        await userApi.grantPass(user.userId);
        user.passesAvailable++;
      } catch (error) {
        this.$toast.error(error);
      }
    },

    async passDown(user) {
      if (user.passesAvailable < 1) return;
      try {
        await userApi.revokePass(user.userId);
        user.passesAvailable--;
      } catch (error) {
        this.$toast.error(error);
      }
    }
  }
};
</script>