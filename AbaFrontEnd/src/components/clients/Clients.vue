<template>
  <v-card class="elevation-8">
    <v-toolbar dark class="secondary" fluid>
      <v-toolbar-title>Clients/Patients ({{users.length}})</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-text-field v-model="search" placeholder="Search" prepend-icon="fa-search" hide-details single-line solo-inverted></v-text-field>
      <v-menu class="mr-0" bottom left :disabled="loading">
        <v-btn slot="activator" icon :disabled="loading">
          <v-icon>fa-ellipsis-v</v-icon>
        </v-btn>
        <v-list>
          <v-list-tile to="/clients/add_edit">
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
              <v-icon>{{showInactive ? "fa-user-check":"fa-user-times"}}</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>{{showInactive ? "Show active users":"Show inactived users"}}</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
          <v-list-tile @click="loadClients">
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
          <td class="text-xs-left px-1 hidden-xs-only">
            <v-avatar size="32">
              <img :src="`images/${props.item.gender ? props.item.gender.toLowerCase() : 'nogender'}.png`">
            </v-avatar>
          </td>
          <td class="text-xs-left px-1">{{props.item.firstname}} {{props.item.nickname ? `(${props.item.nickname})` : ""}}</td>
          <td class="text-xs-left px-1">{{props.item.lastname}}</td>
          <td class="text-xs-left px-1 hidden-xs-only">{{props.item.dob | moment("utc", "MM/DD/YYYY")}}</td>
          <td class="text-xs-left px-1 hidden-sm-and-down">{{getAddress(props.item)}}</td>
          <td class="text-xs-center px-0">
            <v-switch color="primary" hide-details v-model="props.item.active" @change="changeActive(props.item)"></v-switch>
          </td>
          <td class="text-xs-left px-0 text-truncate">
            <v-tooltip top>
              <v-btn slot="activator" icon class="mx-0" @click="editClient(props.item.clientId)">
                <v-icon color="success">fa-edit</v-icon>
              </v-btn>
              <span>Edit basic info</span>
            </v-tooltip>
            <v-tooltip top>
              <v-btn slot="activator" icon class="mx-0" @click="clientDetails(props.item.clientId)">
                <v-icon color="teal">fa-eye</v-icon>
              </v-btn>
              <span>Client details</span>
            </v-tooltip>
          </td>
        </tr>
      </template>
    </v-data-table>
  </v-card>
</template>

<script>
import clientApi from "@/services/api/ClientServices";
import utils from "@/services/Utils";

export default {
  data() {
    return {
      users: [],
      search: "",
      loading: false,
      showInactive: false,
      headers: [{ text: "", align: "center", value: "gender", class: "px-0 hidden-xs-only", width: 30, sortable: false }, { text: "Firstname", align: "left", value: "firstname", class: "px-1" }, { text: "Lastname", align: "left", value: "lastname", class: "px-1" }, { text: "DOB", align: "left", value: "dob", class: "px-1 hidden-xs-only" }, { text: "Address", align: "left", value: "address", class: "px-1 hidden-sm-and-down" }, { text: "Active", align: "left", value: "active", class: "px-1", sortable: false }, { text: "Actions", align: "left", value: "active2", class: "px-1", sortable: false }],
      pagination: {
        sortBy: "firstname"
      }
    };
  },

  mounted() {
    this.loadClients();
  },

  computed: {
    filterUsers() {
      return this.users.filter(s => s.active !== this.showInactive);
    }
  },

  methods: {
    ...utils,

    async loadClients() {
      this.users = [];
      this.loading = true;
      try {
        this.users = await clientApi.getClients();
        // clients.forEach(c => {
        //   c.dbo = this.$moment(c.dbo).local();
        //   this.users.push(c);
        // });
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async changeActive(item) {
      const newStatus = {
        status: item.active,
        userId: item.clientId
      };
      this.loading = true;
      try {
        await clientApi.changeClientStatus(newStatus);
        this.$toast.success("Status changed successful.");
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loadClients();
        this.loading = false;
      }
    },

    editClient(clientId) {
      this.$router.push(`/clients/add_edit/${clientId}`);
    },

    clientDetails(clientId) {
      this.$router.push(`/clients/client_details/${clientId}`);
    }
  }
};
</script>
