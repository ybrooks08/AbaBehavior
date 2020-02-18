<template>
  <v-layout row wrap>
    <v-flex xs12>
      <v-card>
        <v-toolbar dark class="secondary" fluid dense>
          <v-toolbar-title>Monthly/Week data summary and average</v-toolbar-title>
        </v-toolbar>
        <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
        <v-card-text>
          <v-form ref="form" autocomplete="off" v-model="validForm">
            <v-layout row wrap>
              <v-flex xs12 md6>
                <v-menu v-model="menuPickerMonth" :close-on-content-click="false" lazy offset-y full-width min-width="290px">
                  <template v-slot:activator="{ on }">
                    <v-text-field box :value="formattedMonth" label="Month" prepend-icon="event" readonly v-on="on" :rules="[required]"></v-text-field>
                  </template>
                  <v-date-picker v-model="month" type="month" @input="menuPickerMonth = false" :show-current="false"></v-date-picker>
                </v-menu>
              </v-flex>
              <v-flex xs12 md6>
                <v-autocomplete box hid :disabled="loading" :items="clients" v-model="clientId" label="Client" prepend-icon="fa-user" item-text="clientName" item-value="clientId" :rules="[required]"
                                required>
                  <template slot="item" slot-scope="{ item }">
                    <v-list-tile-avatar>
                      <img :style="!item.active ? 'opacity: 0.5': ''" :src="`images/${item.gender ? item.gender.toLowerCase() : 'nogender'}.png`">
                    </v-list-tile-avatar>
                    <v-list-tile-content>
                      <v-list-tile-title :class="{ 'grey--text text--lighten-1': !item.active }">{{item.clientName}}</v-list-tile-title>
                      <v-list-tile-sub-title :class="{ 'grey--text text--lighten-1': !item.active }">{{item.dob | moment('utc', 'MM/DD/YYYY')}} | Code: {{item.clientCode || 'N/A' }}
                      </v-list-tile-sub-title>
                    </v-list-tile-content>
                  </template>
                </v-autocomplete>
              </v-flex>
            </v-layout>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="viewReport">Generate</v-btn>
        </v-card-actions>
      </v-card>
    </v-flex>
    <v-flex xs12 v-if="report">
      <v-card>
        <v-toolbar dense dark class="secondary no-print">
          <v-toolbar-title>Problems</v-toolbar-title>
        </v-toolbar>
        <v-card-text class="pa-2 print-full-width">
          <table class="v-datatable v-table theme--light print-font-small">
            <thead>
              <tr>
                <th :class="i == 0 ?'text-xs-left':'text-xs-center'" v-for="(h,i) in report.headers" :key="'header-'+i" v-html="h"></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(r,i) in report.rowsProblems" :key="('rowProblem'+i)">
                <td :class="j == 0 ?'text-xs-left':'text-xs-center'" v-for="(c,j) in r" :key="'c'+i+j">
                  {{c}}
                </td>
              </tr>
            </tbody>
          </table>
        </v-card-text>
      </v-card>
    </v-flex>
    <v-flex xs12 v-if="report">
      <v-card>
        <v-toolbar dense dark class="secondary no-print">
          <v-toolbar-title>Replacements</v-toolbar-title>
        </v-toolbar>
        <v-card-text class="pa-2 print-full-width">
          <table class="v-datatable v-table theme--light print-font-small">
            <thead>
              <tr>
                <th :class="i == 0 ?'text-xs-left':'text-xs-center'" v-for="(h,i) in report.headersReplacement" :key="'headerr-'+i" v-html="h"></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(r,i) in report.rowsReplacements" :key="('rowReplacement'+i)">
                <td :class="j == 0 ?'text-xs-left':'text-xs-center'" v-for="(c,j) in r" :key="'c1'+i+j" v-html="c"> </td>
              </tr>
            </tbody>
          </table>
        </v-card-text>
      </v-card>
    </v-flex>
  </v-layout>
</template>

<script>
import userApi from "@/services/api/UserServices";
import reportingApi from "@/services/api/ReportingServices";

export default {
  data() {
    return {
      loading: false,
      required: value => !!value || "This field is required.",
      validForm: false,
      clients: [],
      clientId: null,
      menuPickerMonth: false,
      month: this.$moment()
        .subtract(1, "months")
        .format("YYYY-MM"),
      report: null
    };
  },

  computed: {
    formattedMonth() {
      return this.month ? this.$moment(this.month).format("MMMM/YYYY") : "";
    }
  },

  mounted() {
    this.$store.commit("SET_ACTIVE_CLIENT", null);
    this.loadUserClients();
  },

  methods: {
    async loadUserClients() {
      this.clients = [];
      this.loading = true;
      try {
        this.clients = await userApi.loadUserClients();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async viewReport() {
      try {
        this.loading = true;
        this.report = null;
        this.report = await reportingApi.getMonthWeekData(this.month, this.clientId);
      } catch (error) {
        this.$toast.error(error.response.data || error.message);
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>