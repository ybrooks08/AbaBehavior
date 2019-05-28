<template>
  <v-card class="teal lighten-5">
    <v-card-text class="pa-0">
      <v-data-table class="condensed" :headers="headers" :search="search" :items="items" :loading="loading" :rows-per-page-items="[10]" :pagination.sync="pagination" hide-actions>
        <template slot="items" slot-scope="{item}">
          <td class="px-1 text-xs-center hidden-md-and-down">{{item.sessionId}}</td>
          <td class="px-1">
            <strong>{{item.clientFullname}}</strong>
            <br>
            {{item.clientCode}}
          </td>
          <td class="px-1">
            <strong class="hidden-xs-only">{{item.userFullname}}</strong>
            <br class="hidden-xs-only">
            {{item.userRol.toUpperCase()}}
          </td>
          <td class="px-1 text-xs-center">{{item.sessionStart | moment('MM/DD/YYYY')}}</td>
          <td class="px-1 text-xs-left">
            <v-chip disabled class="hidden-xs-only" dark label text-color="white" :color="item.sessionStatusColor">{{item.sessionStatus}}</v-chip>
            <v-avatar tile size="28" class="hidden-sm-and-up" :color="item.sessionStatusColor">
              <span class="white--text headline">{{item.sessionStatus.charAt(0)}}</span>
            </v-avatar>
          </td>
          <td class="hidden-sm-and-down px-1 text-truncate">
            <v-icon color="green" small>fa-sign-in-alt</v-icon>
            {{item.sessionStart | moment('LT')}}
            <br>
            <v-icon color="red" small>fa-sign-out-alt</v-icon>
            {{item.sessionEnd | moment('LT')}}
          </td>
          <td class="hidden-sm-and-down px-1">{{item.sessionType}}</td>
          <td class="text-xs-left hidden-sm-and-down px-1">{{item.pos}}</td>
          <td class="text-xs-center px-1 hidden-xs-only">
            <strong>
              <v-icon small>fa-star</v-icon>
              {{item.totalUnits.toLocaleString()}}
            </strong>
            <br>
            <v-icon small>fa-clock</v-icon>
            {{(item.totalUnits / 4).toLocaleString()}}
          </td>
          <td class="text-xs-left pr-3 pl-0 right text-no-wrap">
            <v-tooltip top>
              <v-btn slot="activator" icon class="mx-0" @click.stop="sessionNotes(item)">
                <v-icon color="grey darken-2">fa-notes-medical</v-icon>
              </v-btn>
              <span>View Notes</span>
            </v-tooltip>
            <v-tooltip top>
              <v-btn slot="activator" icon class="mx-0" @click.stop="sessionData(item)">
                <v-icon color="grey darken-2">fa-chart-line</v-icon>
              </v-btn>
              <span>View data</span>
            </v-tooltip>
          </td>
        </template>
      </v-data-table>
      <div class="text-xs-center" v-if="pages > 1">
        <v-pagination v-model="pagination.page" :length="pages" :total-visible="7" />
      </div>
    </v-card-text>
  </v-card>

</template>

<script>
export default {
  name: "SessionListTable",

  props: {
    items: {
      type: Array,
      required: false
    },
    loading: {
      type: Boolean,
      required: false,
      default: false
    },
    search: {
      type: String,
      required: false,
      default: ""
    }
  },

  computed: {
    pages() {
      return this.pagination.rowsPerPage ? Math.ceil(this.items.length / this.pagination.rowsPerPage) : 0;
    }
  },

  data() {
    return {
      pagination: {
        sortBy: "sessionStart",
        descending: true
      },
      headers: [
        { text: "SessionId", align: "center", value: "sessionId", class: "py-0 hidden-md-and-down" },
        { text: "Client/Code", align: "left", value: "clientFullname", class: "py-0 px-1" },
        { text: "User/Rol", align: "left", value: "userFullname", class: "py-0 px-1" },
        { text: "Date", align: "center", value: "sessionStart", class: "py-0 px-1" },
        { text: "Status", align: "left", value: "sessionStatus", class: "py-0 px-1" },
        { text: "Start/End", align: "left", value: "sessionStart", class: "py-0 px-1", sortable: false },
        { text: "Type", align: "left", value: "sessionType", class: "py-0 px-1" },
        { text: "Pos", align: "left", value: "pos", class: "py-0 px-1" },
        { text: "Units", align: "center", value: "totalUnits", class: "py-0 px-1" },
        { text: "", align: "center", value: "", class: "py-0 px-1" }
      ]
    };
  },

  methods: {
    sessionNotes(session) {
      this.$store.commit("SET_ACTIVE_DATE", session.sessionStart);
      this.$store.commit("SET_ACTIVE_CLIENT", session.clientId);
      this.$store.commit("SET_ACTIVE_SESSION", session.sessionId);
      this.$router.push("/clients/session_notes");
    },

    sessionData(session) {
      this.$store.commit("SET_ACTIVE_DATE", session.sessionStart);
      this.$store.commit("SET_ACTIVE_CLIENT", session.clientId);
      this.$store.commit("SET_ACTIVE_SESSION", session.sessionId);
      this.$router.push("/session/session_collect_data");
    }
  }
};
</script>