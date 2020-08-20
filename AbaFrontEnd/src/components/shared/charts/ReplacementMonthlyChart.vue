<template>
  <v-container grid-list-xs pa-0>
    <v-card flat class="ma-0 pa-0">
      <v-btn
        v-if="!loading && clientReplacementId != -1"
        flat
        icon
        color="grey"
        class="no-print"
        fab
        small
        absolute
        top
        left
        style="margin-left: -20px; margin-top: 20px;"
        @click="editClientReplacementLineChart"
      >
        <v-icon>fa-wrench</v-icon>
      </v-btn>
      <v-btn v-if="!loading" flat icon color="grey" class="no-print" fab small absolute top left style="margin-left: -20px; margin-top: 60px;" @click="loadData">
        <v-icon>fa-sync-alt</v-icon>
      </v-btn>
      <template v-if="loading">
        <v-alert :value="true" type="info" icon="fa-cog fa-spin" color="teal">Generating chart...</v-alert>
      </template>
      <template v-if="!loading && chartOptions.series.length === 0">
        <v-alert :value="true" type="info" color="grey">No data chart</v-alert>
      </template>
      <template v-if="chartOptions.series.length > 0">
        <v-layout row wrap>
          <v-flex xs12>
            <highcharts :options="chartOptions"></highcharts>
          </v-flex>
        </v-layout>
      </template>
    </v-card>
    <client-replacement-chart-labels :model="chartLabelDialog" :clientReplacementId="clientReplacementId" @onClosed="chartLabelsDialogOnClosed" />
  </v-container>
</template>

<script>
import sessionServicesApi from "@/services/api/SessionServices";
import ClientReplacementChartLabels from "@/components/reporting/Components/ClientReplacementChartLabels";

export default {
  props: {
    clientId: {
      type: Number,
      required: true
    },
    problemId: {
      type: Number,
      required: true
    },
    clientReplacementId: {
      default: -1,
      type: Number,
      required: false
    },
    dateEnd: {
      type: String,
      default: null,
      required: false
    },
    hideNotes: {
      type: Boolean,
      default: false,
      required: false
    }
  },

  components: {
    ClientReplacementChartLabels
  },

  data() {
    return {
      loading: true,
      chartOptions: {
        series: []
      },
      chartLabelDialog: false
    };
  },

  mounted() {
    this.loadData();
  },

  methods: {
    async loadData() {
      try {
        this.loading = true;
        let data = await sessionServicesApi.getReplacementMontlyChart(this.clientId, this.problemId, this.dateEnd);
        this.chartOptions = data.chartOptions;
      } catch (error) {
        console.log(error.message || error);
        // this.$toast.error(error.message || error);
      } finally {
        this.loading = false;
      }
    },

    editClientReplacementLineChart() {
      this.chartLabelDialog = true;
    },

    async chartLabelsDialogOnClosed() {
      this.chartLabelDialog = false;
      await this.loadData();
    }
  }
};
</script>
