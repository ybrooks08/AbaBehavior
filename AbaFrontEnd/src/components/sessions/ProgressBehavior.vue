<template>
  <v-container grid-list-xs pa-0>
    <template v-if="chartOptions.series.length === 0">
      <v-alert :value="true" type="warning">No data to show</v-alert>
    </template>
    <template v-if="chartOptions.series.length > 0">
      <v-layout row wrap>
        <v-flex class="text-xs-center">
          <v-progress-circular style="position: absolute; z-index: 343948394" v-show="loading" indeterminate></v-progress-circular>
        </v-flex>
        <v-flex xs12>
          <highcharts ref="hc" v-show="finish" :options="chartOptions"></highcharts>
        </v-flex>
      </v-layout>

      <v-expansion-panel class="no-print">
        <v-expansion-panel-content expand-icon="fa-angle-up" class="grey lighten-4">
          <div slot="header">Notes</div>
          <v-card>
            <v-container grid-list-xs pa-2>
              <v-layout row wrap>
                <v-flex d-flex sm6 md4 lg3 xl2 v-for="(n, index) in notes" :key="index">
                  <v-card class="elevation-3 yellow lighten-5">
                    <v-card-title primary-title class="yellow lighten-3 pa-1">
                      <span class="text-no-wrap text-truncate">{{n.title}}</span>
                      <v-spacer></v-spacer>
                      <v-btn small flat icon class="ma-0" color="grey darken-3" @click="editNote(n)">
                        <v-icon small>fa-edit</v-icon>
                      </v-btn>
                      <v-btn small flat icon class="ma-0" color="grey darken-3" @click="deleteNote(n)">
                        <v-icon small>fa-trash</v-icon>
                      </v-btn>
                    </v-card-title>
                    <v-card-text class="pa-1 yellow lighten-5">
                      {{n.note}}
                      <br>
                      <span class="grey--text">{{n.chartNoteDate | moment("ddd, MM/DD/YYYY")}}</span>
                    </v-card-text>
                  </v-card>
                </v-flex>
              </v-layout>
            </v-container>
          </v-card>
        </v-expansion-panel-content>
      </v-expansion-panel>
    </template>
  </v-container>
</template>

<script>
import sessionServicesApi from "@/services/api/SessionServices";

export default {
  props: {
    behaviorId: {
      type: Number,
      default: 0,
      required: false
    },
    dateStart: {
      type: String,
      default: null,
      required: false
    },
    dateEnd: {
      type: String,
      default: null,
      required: false
    }
  },

  data() {
    return {
      loading: false,
      finish: false,
      notes: [],
      chartOptions: {
        series: []
      }
    };
  },

  computed: {
    activeClientId() {
      return this.$store.getters.activeClientId;
    }
  },

  watch: {
    activeClientId() {
      this.loadAll();
    }
  },

  mounted() {
    this.loadAll();
    this.finish = true;
  },

  methods: {
    async loadAll() {
      try {
        this.loading = true;
        let data = await sessionServicesApi.getProblemsChartData(this.activeClientId, this.behaviorId, this.dateStart, this.dateEnd);
        this.chartOptions = data.chartOptions;
        this.notes = data.notes;
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loading = false;
      }
    },

    editNote(note) {
      this.$router.push(`/clients/add_edit_chart_note/${note.clientChartNoteId}`);
    },

    async deleteNote(note) {
      this.$confirm("Do you want to delete this note?").then(async res => {
        if (res) {
          try {
            await sessionServicesApi.deleteChartNote(note.clientChartNoteId);
            await this.loadAll();
          } catch (error) {
            this.$toast.error(error);
          }
        }
      });
    }
  }
};
</script>
