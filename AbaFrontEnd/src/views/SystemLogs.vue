<template>
  <v-container grid-list-xs>
    <v-layout row wrap>
      <v-flex x12>
        <v-card>
          <v-toolbar dark color="secondary" fluid>
            <v-toolbar-title>System Logs (last 30 days)</v-toolbar-title>
            <v-spacer/>
            <v-select flat hide-details solo-inverted class="pr-2" prepend-inner-icon="fa-filter fa-sm" clearable placeholder="Filter by level" :items="levelItems" v-model="filterLevel" style="width: 100px;"></v-select>
            <v-text-field v-model="search" placeholder="Search" prepend-inner-icon="fa-search fa-sm" solo-inverted flat hide-details clearable @click:clear="search = ''"/>
            <v-menu class="mr-0" bottom left :disabled="loading">
              <v-btn slot="activator" icon :disabled="loading">
                <v-icon>fa-ellipsis-v</v-icon>
              </v-btn>
              <v-list>
                <v-list-tile @click="loadLogs">
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
          <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"/>
          <v-card-text>
            <table class="v-datatable v-table theme--light condensed">
              <thead>
                <tr style="height: auto;">
                  <th class="text-xs-left py-0 px-1" style="width: 80px;">Level</th>
                  <th class="text-xs-left py-0 px-1" style="width: 170px;">DateStamp</th>
                  <th class="text-xs-left py-0 px-1">User/Client</th>
                  <th class="text-xs-left py-0 px-1">Description</th>
                  <th class="text-xs-left py-0 px-1">By</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="log in filteredLogs" :key="log.systemLogId">
                  <td class="text-xs-left px-1">
                    <v-chip dark :color="getIconAndColor(log.systemLogType).color">
                      <v-avatar>
                        <v-icon>{{getIconAndColor(log.systemLogType).icon}}</v-icon>
                      </v-avatar>
                      {{log.systemLogType}}
                    </v-chip>
                  </td>
                  <td class="text-xs-left px-1">{{log.entry | moment('L LTS')}}</td>
                  <td class="text-xs-left px-1" v-html="log.moduleValue"></td>
                  <td class="text-xs-left px-1" v-html="log.description"></td>
                  <td class="text-xs-left px-1" v-html="(log.user.firstname + ' ' + log.user.lastname)"></td>
                </tr>
                <tr v-if="filteredLogs.length === 0">
                  <td colspan="3" class="pa-0">
                    <v-alert :value="true" type="info">There are no records to show</v-alert>
                  </td>
                </tr>
              </tbody>
            </table>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import masterTableApi from '@/services/api/MasterTablesServices';

export default {
  name: 'Logs',

  data() {
    return {
      loading: false,
      search: '',
      filterLevel: '',
      logs: [],
      levelItems: ['Info', 'Warning', 'Error', 'Critical', 'Unknow'],
    };
  },

  computed: {
    filteredLogs: function () {
      return this.logs.filter(item => {
        let regex = new RegExp(this.search == null ? '' : this.search, 'i');
        let regex2 = new RegExp(this.filterLevel == null ? '' : this.filterLevel, 'i');
        return (
          (item.description ? item.description.match(regex) : true) || (item.moduleValue ? item.moduleValue.match(regex) : true) &&
          (item.systemLogType ? item.systemLogType.match(regex2) : true)
        );
      });
    },
  },

  mounted() {
    this.loadLogs();
  },

  methods: {
    async loadLogs() {
      try {
        this.loading = true;
        const logs = await masterTableApi.getSystemLogs();
        this.logs = Object.freeze(logs);
      } catch (error) {
        this.$toast.error('Error getting system logs.');
      } finally {
        this.loading = false;
      }
    },

    getIconAndColor(systemLogType) {
      let data = {
        icon: '',
        color: '',
      };
      switch (systemLogType.toLowerCase()) {
        case 'info':
          data.icon = 'fa-info-circle';
          data.color = 'blue';
          break;
        case 'warning':
          data.icon = 'fa-exclamation-circle';
          data.color = 'orange';
          break;
        case 'error':
          data.icon = 'fa-minus-circle';
          data.color = 'deep-orange';
          break;
        case 'critical':
          data.icon = 'fa-times-circle';
          data.color = 'red';
          break;
        default:
          data.icon = 'fa-question';
          data.color = '';
      }
      return data;
    },

  },
};
</script>

<style scoped>
</style>
