<template>
  <v-layout row wrap>
    <v-flex xs12 class="no-print">
      <v-card>
        <v-toolbar dark class="secondary" fluid dense>
          <v-toolbar-title>Caregiver data collections report</v-toolbar-title>
        </v-toolbar>
        <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
        <v-card-text class="pa-1">
          <v-form ref="form" autocomplete="off" v-model="validForm">
            <v-layout row wrap>
              <v-flex xs12>
                <date-picker-menu :isLarge="true" :isDark="false" :btnColor="'primary'" :disabled="loading" v-model="datePickerModel" />
              </v-flex>
              <v-flex md12>
                <v-autocomplete box hid :disabled="loading" :items="clients" v-model="clientId" label="Client" prepend-icon="fa-user" item-text="clientName" item-value="clientId" :rules="[required]" required>
                  <template slot="item" slot-scope="{ item }">
                    <v-list-tile-avatar>
                      <img :style="!item.active ? 'opacity: 0.5': ''" :src="`images/${item.gender ? item.gender.toLowerCase() : 'nogender'}.png`">
                    </v-list-tile-avatar>
                    <v-list-tile-content>
                      <v-list-tile-title :class="{ 'grey--text text--lighten-1': !item.active }">{{item.clientName}}</v-list-tile-title>
                      <v-list-tile-sub-title :class="{ 'grey--text text--lighten-1': !item.active }">{{item.dob | moment('utc', 'MM/DD/YYYY')}} | Code: {{item.clientCode || 'N/A' }}</v-list-tile-sub-title>
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
    <v-flex xs12 v-if="collections.length > 0">
      <v-card>
        <v-toolbar dense dark class="secondary">
          <v-toolbar-title>Collections</v-toolbar-title>
          <v-spacer />
        </v-toolbar>
        <v-card-text class="pa-0">
          <table v-if="collections.length > 0" class="v-datatable v-table theme--light">
            <thead>
              <tr>
                <th class="text-xs-left">Caregiver</th>
                <th class="text-xs-left">Collected</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="r in collections" :key="r.caregiverDataCollectionId">
                <td>{{r.caregiver.caregiverFullname}}</td>
                <td>{{r.collectionDate | moment('utc','MM/DD/YYYY')}}</td>
                <td class="text-xs-left">
                  <v-tooltip top>
                    <v-btn slot="activator" icon class="mx-0" @click.stop="sessionData(r)">
                      <v-icon color="grey darken-2">fa-chart-line</v-icon>
                    </v-btn>
                    <span>View data</span>
                  </v-tooltip>
                </td>
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
      datePickerModel: {
        start: this.$moment()
          .subtract(1, "month")
          .startOf("month")
          .format("YYYY-MM-DDTHH:mm"),
        end: this.$moment()
          .subtract(1, "month")
          .endOf("month")
          .format("YYYY-MM-DDTHH:mm")
      },
      clients: [],
      clientId: null,
      collections: []
    };
  },

  computed: {
    user() {
      return this.$store.getters.user;
    }
  },

  mounted() {
    //this.$store.commit('SET_ACTIVE_DATE', new Date());
    this.$store.commit("SET_ACTIVE_CLIENT", null);
    this.loadUserClients();
  },

  methods: {
    dateSelected(range) {
      this.serviceLog.from = range.from;
      this.serviceLog.to = range.to;
    },

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
        this.collections = [];
        this.collections = await reportingApi.getCaregiversCollectionHistory(this.datePickerModel.start, this.datePickerModel.end, this.clientId);

        if (this.collections.length == 0) {
          this.$toast.info("No data");
          return;
        }

        console.log(this.collections);
      } catch (error) {
        console.error(error);
        this.$toast.error(error.response.data || error.message);
      } finally {
        this.loading = false;
      }
    },

    sessionData(collection) {
      const d = this.$moment(collection.collectionDate).utc();
      this.$store.commit("SET_ACTIVE_DATE", d);
      this.$store.commit("SET_ACTIVE_CLIENT", collection.clientId);
      this.$router.push("/clients/caregiver_data_collection");
    }
  }
};
</script>