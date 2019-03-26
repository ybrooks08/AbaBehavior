<template>
  <v-card class="elevation-8">
    <v-toolbar color="secondary" dark dense>
      <v-toolbar-title>Documents alerts</v-toolbar-title>
    </v-toolbar>
    <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
    <!-- <v-card flat height="400" class="scroll-y pa-1"> -->
    <v-card flat class="pa-1">
      <table v-if="documents && documents.length > 0" class="v-datatable v-table theme--light">
        <tbody>
          <tr v-for="item in documents" :key="item.id">
            <td class="text-xs-left px-1 hidden-xs-only" style="width: 60px;">
              <v-avatar>
                <v-icon :color="item.days <=30 ? 'red' : 'orange' " medium>fa-paperclip</v-icon>
              </v-avatar>
            </td>
            <td class="text-xs-left px-1">
              <router-link class="body-2" :to="'/users/user_details/'+item.userId">{{item.userFullname}}</router-link>
              <br>
              <span>{{item.rolName}}</span>
            </td>
            <td class="text-xs-left px-1 hidden-xs-only">
              <strong class="body-2">{{item.groupName}}</strong>
              <br>
              <span>{{item.documentName}}</span>
            </td>
            <td class="text-xs-left px-1">
              <v-chip label text-color="white" :color="item.days <=30 ? 'red' : 'orange' ">
                <v-avatar>
                  <v-icon>fa-calendar-times</v-icon>
                </v-avatar>
                {{item.expires | moment('from', 'now')}}
              </v-chip>
            </td>
          </tr>
        </tbody>
      </table>
      <v-alert v-else-if="documents && documents.length === 0" type="info" :value="true">No expiring documents found</v-alert>
      <v-alert v-else-if="!documents" type="error" :value="true">Error reading data</v-alert>
    </v-card>
  </v-card>
</template>

<script>
import userApi from '@/services/api/UserServices';

export default {
  props: {
    currentUser: {
      type: Boolean,
      default: false,
    },
  },

  data() {
    return {
      documents: [],
      loading: false,
    };
  },

  async mounted() {
    try {
      this.loading = true;
      this.documents = await userApi.getExpiringDocuments(this.currentUser);
    } catch (error) {
      this.$toast.error(error);
      this.documents = null;
    } finally { this.loading = false; }
  },

};
</script>