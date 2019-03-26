<template>
  <v-layout row wrap>
    <v-flex xs12 class="hidden-sm-and-down">
      <v-responsive height="120" class="grey lighten-2">
        <v-container fill-height>
          <v-layout align-center>
            <v-flex>
              <h3 class="display-2">Welcome back {{user.fullName}}</h3>
              <span class="subheading">Please select a client and date to start working.</span>
            </v-flex>
          </v-layout>
        </v-container>
      </v-responsive>
    </v-flex>
    <v-flex xs12>
      <session-list></session-list>
    </v-flex>
    <v-flex xs12>
      <expiring-documents :currentUser="true"/>
    </v-flex>
    <v-flex xs12>
      <v-card>
        <v-toolbar dense dark class="secondary">
          <v-toolbar-title>Your current authorizations</v-toolbar-title>
        </v-toolbar>
        <v-progress-linear style="position: absolute;" v-show="loadingAuths" :indeterminate="true" class="ma-0"></v-progress-linear>
        <table v-if="auths.length > 0" class="v-datatable v-table theme--light">
          <thead>
            <tr>
              <th class="text-xs-left py-0">Client / Code</th>
              <th class="text-xs-left py-0 hidden-sm-and-down">Start / End</th>
              <th class="text-xs-left py-0">Expires</th>
              <th class="text-xs-left py-0 hidden-sm-and-down">PA Number</th>
              <th class="text-xs-left py-0 hidden-sm-and-down">Initial Units</th>
              <th class="text-xs-left py-0">Available Units</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="r in auths" :key="('auth'+r.assessmentId)">
              <td>
                <strong>{{r.clientFirstName}} {{r.clientLastName}}</strong>
                <br>
                {{r.clientCode}}
              </td>
              <td class="hidden-sm-and-down">
                <v-icon color="green" small>fa-check-circle</v-icon>
                {{r.startDate | moment('MM/DD/YYYY')}}
                <br>
                <v-icon color="red" small>fa-times-circle</v-icon>
                {{r.endDate | moment('MM/DD/YYYY')}}
              </td>
              <td>{{r.endDate | moment('from', 'now')}}</td>
              <td class="hidden-sm-and-down">
                <!-- ...{{r.paNumber.substr(-4, 4)}} -->
                {{r.paNumber}}
              </td>
              <td class="hidden-sm-and-down">
                <strong>
                  <v-icon small>fa-star</v-icon>
                  {{r.totalUnits.toLocaleString()}}
                </strong>
                <br>
                <v-icon small>fa-clock</v-icon>
                {{(r.totalUnits / 4).toLocaleString()}}
              </td>
              <td>
                <strong>
                  <v-icon small>fa-star</v-icon>
                  {{r.availableUnits.toLocaleString()}}
                </strong>
                <br>
                <v-icon small>fa-clock</v-icon>
                {{(r.availableUnits / 4).toLocaleString()}}
              </td>
            </tr>
          </tbody>
        </table>
        <v-alert v-else type="info" :value="true">No authorizations found</v-alert>
      </v-card>
    </v-flex>
  </v-layout>
</template>

<script>
import userApi from '@/services/api/UserServices';
import ExpiringDocuments from '@/components/users/ExpiringDocuments';
import sessionList from '@/components/sessions/SessionList';

export default {
  components: {
    ExpiringDocuments,
    sessionList,
  },

  data() {
    return {
      loadingAuths: false,
      auths: [],
    };
  },

  computed: {
    user() {
      return this.$store.getters.user;
    },
    navigationDrawer() {
      return () => import(`@/views/drawers/${this.user.template}`);
    },
  },

  mounted() {
    this.$store.commit('SET_ACTIVE_DATE', this.$moment());
    this.$store.commit('SET_ACTIVE_CLIENT', null);
    this.getAuths();
  },

  methods: {
    async getAuths() {
      this.loadingAuths = true;
      try {
        this.auths = await userApi.getCurrentAuthorizationsForCurrentUser();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loadingAuths = false;
      }
    },
  },
};
</script>