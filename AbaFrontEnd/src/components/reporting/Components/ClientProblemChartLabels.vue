<template>
  <v-dialog width="800" persistent transition="slide-y-transition" v-model="model">
    <v-card>
      <v-card-title class="teal white--text">
        <div class="title">Edit chart's plot lines</div>
        {{ clientProblemId }}
      </v-card-title>
      <v-container grid-list-md pa-1>
        <v-layout row wrap>
          <v-flex xs12>
            <v-card-text>
              <v-form ref="form" autocomplete="off">
                <v-layout row wrap>
                  <v-flex xs12 sm9>
                    <v-text-field box :disabled="loading" label="Line label" v-model="chartLabel" hide-details />
                  </v-flex>
                  <v-flex xs12 sm3>
                    <v-select :items="lineRotation" box label="Orientation" v-model="chartOrientation" hide-details></v-select>
                  </v-flex>
                  <v-flex xs12 sm3>
                    <v-select :items="lineStyles" box label="Style" v-model="chartLineStyle" hide-details></v-select>
                  </v-flex>
                  <v-flex xs12 sm3>
                    <v-menu v-model="menuDate" :close-on-content-click="false" :nudge-right="40" lazy transition="scale-transition" offset-y full-width min-width="290px">
                      <template v-slot:activator="{ on }">
                        <v-text-field box v-model="chartDate" label="Line chart date" readonly v-on="on" hide-details></v-text-field>
                      </template>
                      <v-date-picker v-model="date" @input="menuDate = false" :allowed-dates="allowedDates"></v-date-picker>
                    </v-menu>
                  </v-flex>
                  <v-flex xs12 sm3>
                    <v-select :items="lineColors" box label="Line color" v-model="chartLineColor" hide-details></v-select>
                  </v-flex>
                  <v-flex xs12 sm3>
                    <v-select :items="lineLabelSizes" box label="Font size" v-model="chartLabelFontSize" hide-details></v-select>
                  </v-flex>
                  <v-btn :disabled="loading || validForm" :loading="loading" color="success" @click="add">ADD PLOT LINE</v-btn>
                </v-layout>
              </v-form>
              <v-divider class="mt-2" />
              <table v-if="lines.length > 0" class="v-datatable v-table theme--light">
                <tbody>
                  <tr v-for="l in lines" :key="'line' + l.clientProblemChartLineId">
                    <td>{{ l.chartDate | moment("utc", "MM/DD/YYYY") }}</td>
                    <td>{{ l.chartLabel || "No label" }}</td>
                    <td>
                      <span :style="{ color: l.chartLineColor }">{{ l.chartLineColor }}</span>
                    </td>
                    <td>
                      {{ l.chartLineStyle }}
                    </td>
                    <td>
                      {{ l.chartOrientation }}
                    </td>
                    <td class="text-xs-right">
                      <v-btn icon @click="deleteLine(l.clientProblemChartLineId)">
                        <v-icon color="grey" small>fa-trash</v-icon>
                      </v-btn>
                    </td>
                  </tr>
                </tbody>
              </table>
              <v-alert v-else :value="true" type="info">No lines</v-alert>
            </v-card-text>
          </v-flex>
        </v-layout>
      </v-container>
      <v-card-actions>
        <v-spacer />
        <v-btn :disabled="loading" :loading="loading" color="primary" @click="onClose">Close</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
import reportingApi from "@/services/api/ReportingServices";

export default {
  name: "ClientProblemChartLabels",

  props: {
    model: {
      type: Boolean,
      required: true,
      default: false
    },
    clientProblemId: null
  },

  data() {
    return {
      loading: false,
      lineRotation: ["Vertical", "Horizontal", "Diagonal"],
      lineStyles: ["Solid", "ShortDash", "ShortDot", "ShortDashDot", "ShortDashDotDot", "Dot", "Dash", "LongDash", "DashDot", "LongDashDot", "LongDashDotDot"],
      lineColors: ["Red", "Black", "Blue", "Green", "Gray", "Yellow", "Purple"],
      lineLabelSizes: ["8", "9", "10", "11", "12", "13", "14", "15", "16", "18", "22"],
      date: null,
      menuDate: false,
      chartLabel: null,
      chartOrientation: "Vertical",
      chartLineStyle: "ShortDash",
      chartLineColor: "Red",
      chartLabelFontSize: "12",
      lines: []
      //chartDate: null
    };
  },

  async mounted() {
    await this.getLines();
  },

  watch: {
    clientProblemId() {
      this.getLines();
    }
  },

  computed: {
    chartDate() {
      return this.formatDate(this.date);
    },
    validForm() {
      return this.chartDate == null;
    }
  },

  methods: {
    formatDate(date) {
      if (!date) return null;
      const [year, month, day] = date.split("-");
      return `${month}/${day}/${year}`;
    },

    allowedDates: val => new Date(val).getDay() === 5,

    async getLines() {
      this.lines = await reportingApi.getClientProblemChartLines(this.clientProblemId);
    },

    async add() {
      try {
        this.loading = true;
        const line = {
          chartDate: this.chartDate,
          chartLabel: this.chartLabel,
          chartOrientation: this.chartOrientation,
          chartLineStyle: this.chartLineStyle,
          chartLineColor: this.chartLineColor,
          chartLabelFontSize: this.chartLabelFontSize,
          clientProblemId: this.clientProblemId
        };
        await reportingApi.addNewProblemBehaviorChartLine(line);
        //this.lines.push(line);
        await this.getLines();
        this.reset();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async deleteLine(clientProblemChartLineId) {
      this.$confirm("Do you want to delete this plot line?").then(async res => {
        if (res) {
          try {
            this.loading = true;
            await reportingApi.deleteClientProblemChartLine(clientProblemChartLineId);
            await this.getLines();
            // this.lines.splice(
            //   this.lines.findIndex(item => item.clientProblemChartLineId === clientProblemChartLineId),
            //   1
            // );
          } catch (error) {
            this.$toast.error(error);
          } finally {
            this.loading = false;
          }
        }
      });
    },

    reset() {
      this.date = null;
      this.chartLabel = null;
      this.chartOrientation = "Vertical";
      this.chartLineStyle = "ShortDash";
      this.chartLineColor = "Red";
      this.chartLabelFontSize = "12";
    },

    onClose() {
      this.reset();
      this.$emit("onClosed");
    }
  }
};
</script>
