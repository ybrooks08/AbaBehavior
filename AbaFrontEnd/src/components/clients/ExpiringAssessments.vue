<template>
  <v-card class="elevation-8">
    <v-toolbar color="secondary" dark dense>
      <v-toolbar-title>Authorization alerts</v-toolbar-title>
    </v-toolbar>
    <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
    <!-- <v-card flat height="400" class="scroll-y pa-1"> -->
    <v-card flat class="pa-1">
      <table v-if="assignments && assignments.length > 0" class="v-datatable v-table theme--light">
        <tbody>
          <tr v-for="item in assignments" :key="item.referralId">
            <td class="text-xs-left px-1 hidden-xs-only" style="width: 60px;">
              <v-avatar>
                <v-icon :color="item.days <=30 ? 'red' : 'orange' " medium>fa-briefcase-medical</v-icon>
              </v-avatar>
            </td>
            <td class="text-xs-left px-1">
              <router-link class="body-2" :to="'/clients/client_details/'+item.clientId">{{item.clientFullname}}</router-link>
              <br>
              <span>{{item.code}}</span>
            </td>
            <td class="text-xs-left px-1 hidden-xs-only">
              <strong class="body-2">{{item.behaviorAnalysisCode.description}}</strong>
              <br>
              <span class="primary white--text pa-1">{{item.behaviorAnalysisCode.hcpcs}}</span>
            </td>
            <td class="text-xs-right px-1" :class="{'red--text': item.totalUnits <= 10}">
              <strong class="body-2">
                {{item.totalUnits}}
                <span class="hidden-xs-only">units left</span>
              </strong>
              <br class="hidden-xs-only">
              <span class="hidden-xs-only">{{(item.totalUnits / 4).toLocaleString()}} hrs</span>
            </td>
            <td class="text-xs-left px-1 hidden-sm-and-down">
              <strong>
                <v-icon small>fa-calendar-plus</v-icon>
                {{item.startDate | moment('utc', 'MM/DD/YYYY')}}
              </strong>
              <br>
              <strong>
                <v-icon small>fa-calendar-minus</v-icon>
                {{item.endDate | moment('utc', 'MM/DD/YYYY')}}
              </strong>
            </td>
            <td class="text-xs-left px-1">
              <v-chip label text-color="white" :color="item.days <=30 ? 'red' : 'orange'">
                <v-avatar>
                  <v-icon>fa-calendar-times</v-icon>
                </v-avatar>
                {{item.endDate | moment('utc', 'from', 'now')}}
              </v-chip>
            </td>
          </tr>
        </tbody>
      </table>
      <v-alert v-else-if="assignments && assignments.length === 0" type="info" :value="true">No expiring assignments found</v-alert>
      <v-alert v-else-if="!assignments" type="error" :value="true">Error reading data</v-alert>
    </v-card>
  </v-card>
</template>

<script>
import clientApi from '@/services/api/ClientServices';

export default {

  data() {
    return {
      assignments: [],
      loading: false,
    };
  },

  async mounted() {
    try {
      this.loading = true;
      this.assignments = await clientApi.getClientExpiringAssessments();
    } catch (error) {
      this.$toast.error(error);
      this.assignments = null;
    } finally { this.loading = false; }
  },

  methods: {
    clientDetails(clientId) {
      this.$router.push(`/clients/client_details/${clientId}`);
    },
  },

};
</script>