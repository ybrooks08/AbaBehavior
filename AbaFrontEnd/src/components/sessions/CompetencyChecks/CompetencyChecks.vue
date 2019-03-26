<template>
  <v-container pa-0 grid-list-md fluid>
    <v-layout row wrap>
      <v-flex xs12>
        <v-card>
          <v-toolbar dark class="secondary" fluid>
            <v-toolbar-title>Competency checks for selected client</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-text-field v-model="search" placeholder="Search" prepend-icon="fa-search" hide-details single-line solo-inverted></v-text-field>
            <v-menu class="mr-0" bottom left :disabled="loading">
              <v-btn slot="activator" icon :disabled="loading">
                <v-icon>fa-ellipsis-v</v-icon>
              </v-btn>
              <v-list>
                <v-list-tile to="/competency_checks/new_edit">
                  <v-list-tile-action>
                    <v-icon medium>fa-user-check</v-icon>
                  </v-list-tile-action>
                  <v-list-tile-content>
                    <v-list-tile-title>New competency check</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
              </v-list>
            </v-menu>
          </v-toolbar>
          <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
          <v-card-text class="pa-1">
            <v-data-table :headers="headers" :search="search" :items="comps" :loading="loading" :pagination.sync="pagination" :rows-per-page-items="[10]">
              <template slot="items" slot-scope="props">
                <tr>
                  <td class="text-xs-left px-1 hidden-xs-only">{{props.item.date | moment('utc', 'MM/DD/YYYY')}}</td>
                  <td class="text-xs-left px-1">{{props.item.date | moment('utc', 'MMM/YYYY')}}</td>
                  <td class="text-xs-left px-1 hidden-xs-only">{{props.item.competencyCheckType}}</td>
                  <td class="text-xs-left px-1">{{props.item.subject}}</td>
                  <td class="text-xs-right px-1 hidden-sm-and-down">{{props.item.totalDuration}} hours</td>
                  <td class="text-xs-right px-1">{{props.item.totalScore.toLocaleString('en', {style: 'percent'})}}</td>
                  <td class="text-xs-right px-0 text-truncate">
                    <v-btn icon class="ma-0" @click="editCompetencyCheck(props.item.competencyCheckId)">
                      <v-icon color="grey">fa-edit</v-icon>
                    </v-btn>
                    <v-btn icon class="ma-0" @click="exportCompetencyCheck(props.item)">
                      <v-icon color="grey">fa-print</v-icon>
                    </v-btn>
                    <v-btn icon class="ma-0" @click="deleteCompetencyCheck(props.item.competencyCheckId)">
                      <v-icon color="grey">fa-trash</v-icon>
                    </v-btn>
                  </td>
                </tr>
              </template>
            </v-data-table>
          </v-card-text>
        </v-card>
      </v-flex>
      <v-flex xs12 v-if="progress.length > 0">
        <v-card>
          <v-toolbar dark class="secondary" fluid>
            <v-toolbar-title>Progress</v-toolbar-title>
            <v-spacer></v-spacer>
          </v-toolbar>
          <v-progress-linear style="position: absolute;" v-show="loadingCharts" :indeterminate="true" class="ma-0"></v-progress-linear>
          <v-card-text class="pa-2 grey lighten-3">
            <v-layout row wrap>
              <v-flex v-for="c in progress" :key="'flex-'+c.chartOptions.title.text" xs12 sm6>
                <v-card>
                  <v-card-text>
                    <competency-check-progress :key="c.chartOptions.title.text" :options="c.chartOptions"></competency-check-progress>
                  </v-card-text>
                </v-card>
              </v-flex>
            </v-layout>
          </v-card-text>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import sessionServicesApi from '@/services/api/SessionServices';
import fileSaver from 'file-saver';
import competencyCheckProgress from '@/components/sessions/CompetencyChecks/CompetencyCheckProgress';

export default {
  components: {
    competencyCheckProgress,
  },

  data() {
    return {
      loading: false,
      loadingCharts: false,
      comps: [],
      progress: [],
      search: '',
      headers: [
        { text: 'Date', align: 'left', value: 'date', class: 'px-1 hidden-xs-only' },
        { text: 'Month', align: 'left', value: 'month', class: 'px-1' },
        { text: 'Type', align: 'left', value: 'competencyCheckType', class: 'px-1 hidden-xs-only' },
        { text: 'Subject', align: 'left', value: 'subject', class: 'px-1' },
        { text: 'Duration', align: 'center', value: 'totalDuration', class: 'px-1  hidden-sm-and-down' },
        { text: 'Score', align: 'center', value: 'totalScore', class: 'px-1' },
        { text: 'Actions', align: 'right', value: 'active2', class: 'px-1', sortable: false },
      ],
      pagination: {
        sortBy: 'date',
        descending: true,
      },
    };
  },

  computed: {
    activeClientId() {
      return this.$store.getters.activeClientId;
    },
  },

  mounted() {
    this.loadCompetencyChecks();
    this.loadProgress();
  },

  methods: {
    async loadCompetencyChecks() {
      try {
        this.loading = true;
        this.comps = await sessionServicesApi.getCompetencyChecks(this.activeClientId);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    editCompetencyCheck(id) {
      this.$router.push(`/competency_checks/new_edit/${id}`);
    },

    async deleteCompetencyCheck(id) {
      this.$confirm('Do you want to delete this competency check?')
        .then(async res => {
          if (res) {
            try {
              await sessionServicesApi.deleteCompetencyCheck(id);
              await this.loadCompetencyChecks();
              await this.loadProgress();
            } catch (error) {
              this.$toast.error(error.message || error);
            }
          }
        });
    },

    async exportCompetencyCheck(item) {
      try {
        let response = await sessionServicesApi.exportCompetencyChecks(item.competencyCheckId);
        fileSaver(response.data, `${item.competencyCheckType} ${item.subject} competency check.xlsx`);
      } catch (error) {
        this.$toast.error(error.message || error);
      }
    },

    async loadProgress() {
      try {
        this.loadingCharts = true;
        this.progress = await sessionServicesApi.loadCompetencyCheckProgress(this.activeClientId);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loadingCharts = false;
      }
    },
  },

};
</script>