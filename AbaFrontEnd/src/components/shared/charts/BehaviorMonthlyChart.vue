<template>
  <v-container grid-list-xs pa-0>
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
  </v-container>
</template>

<script>
import sessionServicesApi from "@/services/api/SessionServices";

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

  data() {
    return {
      loading: true,
      chartOptions: {
        series: []
      }
    };
  },

  mounted() {
    this.loadData();
  },

  methods: {
    async loadData() {
      try {
        this.loading = true;
        let data = await sessionServicesApi.getBehaviorMontlyChart(this.clientId, this.problemId, this.dateEnd);
        this.chartOptions = data.chartOptions;
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>
