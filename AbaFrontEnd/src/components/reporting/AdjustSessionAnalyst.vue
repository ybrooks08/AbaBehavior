<template>
  <v-layout row wrap>
    <v-flex xs12 class="no-print">
      <v-card>
        <v-toolbar dark class="secondary" fluid dense>
          <v-toolbar-title>Adjust sessions analyst</v-toolbar-title>
        </v-toolbar>
        <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
        <v-card-text>
          <v-form ref="form" autocomplete="off" v-model="validForm">
            <v-layout row wrap>
              <v-flex xs12>
                <date-picker-menu :isLarge="true" :isDark="false" :btnColor="'primary'" :disabled="loading" v-model="datePickerModel" />
              </v-flex>
              <v-flex xs12>
                <v-autocomplete
                  box
                  hide-details
                  :disabled="loading"
                  :items="clients"
                  item-value="clientId"
                  item-text="fullname"
                  v-model="clientId"
                  label="Client"
                  prepend-icon="fa-user"
                  :rules="[required]"
                  required
                  @change="loadAnalysts"
                >
                  <template slot="item" slot-scope="{ item }">
                    <v-list-tile-avatar>
                      <img :style="!item.active ? 'opacity: 0.5' : ''" :src="`images/${item.gender ? item.gender.toLowerCase() : 'nogender'}.png`" />
                    </v-list-tile-avatar>
                    <v-list-tile-content>
                      <v-list-tile-title :class="{ 'grey--text text--lighten-1': !item.active }">{{ item.firstname }} {{ item.lastname }} </v-list-tile-title>
                      <v-list-tile-sub-title :class="{ 'grey--text text--lighten-1': !item.active }">
                        {{ item.dob | moment("utc", "MM/DD/YYYY") }} | Code: {{ item.code || "N/A" }}
                      </v-list-tile-sub-title>
                    </v-list-tile-content>
                  </template>
                </v-autocomplete>
              </v-flex>
              <v-flex xs12>
                <span class="pl-5">All session in the <strong>daterange</strong> for this <strong>client</strong> will change to <strong>analyst</strong>:</span>
                <v-autocomplete
                  box
                  hide-details
                  :disabled="loading"
                  :items="analysts"
                  prepend-icon="fa-user-tie"
                  v-model="analystId"
                  label="Analyst"
                  item-text="fullname"
                  item-value="userId"
                  :loading="loading"
                >
                  <template slot="item" slot-scope="{ item }">
                    <v-list-tile-avatar>
                      <v-icon>fa-user</v-icon>
                    </v-list-tile-avatar>
                    <v-list-tile-content>
                      <v-list-tile-title :class="{ 'grey--text text--lighten-1': !item.active }">{{ item.fullname }}</v-list-tile-title>
                      <v-list-tile-sub-title :class="{ 'grey--text text--lighten-1': !item.active }">{{ item.rolname }}</v-list-tile-sub-title>
                    </v-list-tile-content>
                  </template>
                </v-autocomplete>
              </v-flex>
            </v-layout>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="loading" :loading="loading" color="primary" @click="process">Process</v-btn>
        </v-card-actions>
      </v-card>
    </v-flex>
  </v-layout>
</template>

<script>
import clientApi from "@/services/api/ClientServices";
import userApi from "@/services/api/UserServices";
import sessionServicesApi from "@/services/api/SessionServices";

export default {
  data() {
    return {
      loading: false,
      required: (value) => !!value || "This field is required.",
      validForm: false,
      datePickerModel: {
        start: this.$moment().subtract(1, "month").startOf("month").format("YYYY-MM-DDTHH:mm"),
        end: this.$moment().subtract(1, "month").endOf("month").format("YYYY-MM-DDTHH:mm")
      },
      clients: [],
      clientId: null,
      analysts: [],
      analystId: null
    };
  },

  async mounted() {
    await this.loadData();
  },

  methods: {
    async loadData() {
      try {
        this.clients = [];
        this.loading = true;
        this.clients = await clientApi.getClients();
        this.clients.forEach((s) => {
          s.fullname = `${s.firstname} ${s.lastname}`;
        });
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async loadAnalysts() {
      try {
        this.loading = true;
        this.analysts = [];
        this.analystId = null;
        this.analysts = await userApi.getAnalistFromClient(this.clientId);
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loading = false;
      }
    },

    async process() {
      this.$confirm("Are you sure you want to adjust sessions?").then(async (res) => {
        if (res) {
          try {
            const model = {
              clientId: this.clientId,
              analystId: this.analystId,
              from: this.datePickerModel.start,
              to: this.datePickerModel.end
            };
            this.loading = true;
            const count = await sessionServicesApi.adjustSessionAnalyst(model);
            this.$toast.info(count + " sessions affected.");
            this.datePickerModel.start = this.$moment().subtract(1, "month").startOf("month").format("YYYY-MM-DDTHH:mm");
            this.datePickerModel.end = this.$moment().subtract(1, "month").endOf("month").format("YYYY-MM-DDTHH:mm");
            this.clientId = null;
            this.analystId = null;
          } catch (error) {
            this.$toast.error(error.message || error);
          } finally {
            this.loading = false;
          }
        }
      });
    }
  }
};
</script>
