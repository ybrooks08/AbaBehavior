<template>
  <v-layout row wrap>
    <v-flex xs12 class="no-print">
      <v-card>
        <v-toolbar dark class="secondary" fluid dense>
          <v-toolbar-title>Client</v-toolbar-title>
        </v-toolbar>
        <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
        <v-card-text>
          <v-form ref="form" autocomplete="off" v-model="validForm">
            <v-layout row wrap>
              <v-flex xs12>
                <v-radio-group v-model="radioScopeDate">
                  <v-radio color="primary" label="All data" value="1"></v-radio>
                  <v-radio color="primary" label="Custom range" value="2"></v-radio>
                  <date-picker-menu v-show="radioScopeDate==2" v-model="datePickerModel" :isLarge="true" :isDark="false" :btnColor="'primary'" :disabled="loading" />
                </v-radio-group>
              </v-flex>
              <v-flex xs12>
                <v-autocomplete box hid :disabled="loading" :items="clients" v-model="clientId" label="Client" prepend-icon="fa-user" item-text="clientName" item-value="clientId" :rules="[required]" required>
                  <template slot="item" slot-scope="{ item }">
                    <v-list-tile-avatar>
                      <img :style="!item.active ? 'opacity: 0.5': ''" :src="`images/${item.gender ? item.gender.toLowerCase() : 'nogender'}.png`">
                    </v-list-tile-avatar>
                    <v-list-tile-content>
                      <v-list-tile-title :class="{ 'grey--text text--lighten-1': !item.active }">{{item.clientName}}</v-list-tile-title>
                      <v-list-tile-sub-title :class="{ 'grey--text text--lighten-1': !item.active }">{{item.dob | moment("utc", "MM/DD/YYYY")}} | Code: {{item.clientCode || "N/A" }}</v-list-tile-sub-title>
                    </v-list-tile-content>
                  </template>
                </v-autocomplete>
              </v-flex>
            </v-layout>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn color="primary" :loading="loading" :disabled="loading || !validForm" @click="clientChanged">View</v-btn>
        </v-card-actions>
      </v-card>
    </v-flex>
    <v-flex xs12 v-if="activeClientId != 0">
      <v-card>
        <v-toolbar dark class="secondary" fluid dense>
          <v-toolbar-title>Client Graphs</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-menu class="mr-0" bottom left :disabled="loading">
            <v-btn slot="activator" icon :disabled="loading">
              <v-icon>fa-ellipsis-v</v-icon>
            </v-btn>
            <v-list>
              <v-list-tile to="/clients/add_edit_chart_note">
                <v-list-tile-action>
                  <v-icon medium>fa-sticky-note</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                  <v-list-tile-title>New quick note</v-list-tile-title>
                </v-list-tile-content>
              </v-list-tile>
              <v-divider></v-divider>
              <v-list-tile @click="printVersion">
                <v-list-tile-action>
                  <v-icon medium>fa-print</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                  <v-list-tile-title>Print version</v-list-tile-title>
                </v-list-tile-content>
              </v-list-tile>
            </v-list>
            <v-divider></v-divider>
          </v-menu>
        </v-toolbar>
        <v-card-text class="pa-1">
          <v-subheader inset class="red--text">Problem behaviors</v-subheader>
          <client-progress-behavior :dateStart="(radioScopeDate == '1' ? null : datePickerModel.start)" :dateEnd="(radioScopeDate == '1' ? null : datePickerModel.end)">></client-progress-behavior>
          <v-divider></v-divider>
          <v-subheader inset class="blue--text">Replacements program</v-subheader>
          <client-progress-replacement :dateStart="(radioScopeDate == '1' ? null : datePickerModel.start)" :dateEnd="(radioScopeDate == '1' ? null : datePickerModel.end)">></client-progress-replacement>
        </v-card-text>
      </v-card>
    </v-flex>
  </v-layout>
</template>

<script>
import userApi from "@/services/api/UserServices";
import ClientProgressBehavior from "@/components/sessions/ProgressBehavior";
import ClientProgressReplacement from "@/components/sessions/ProgressReplacement";

export default {
  data() {
    return {
      loading: false,
      required: value => !!value || "This field is required.",
      validForm: false,
      clients: [],
      clientId: null,
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
      radioScopeDate: "1"
    };
  },

  components: {
    ClientProgressBehavior,
    ClientProgressReplacement
  },

  computed: {
    activeClientId() {
      return this.$store.getters.activeClientId;
    }
  },

  mounted() {
    this.$store.commit("SET_ACTIVE_CLIENT", 0);
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

    clientChanged() {
      this.$store.commit("SET_ACTIVE_CLIENT", 0);
      this.$nextTick(() => {
        this.$store.commit("SET_ACTIVE_CLIENT", this.clientId);
      });
    },

    printVersion() {
      const dateStart = this.radioScopeDate == "1" ? null : this.$moment(this.datePickerModel.start).format("YYYY-MM-DD");
      const dateEnd = this.radioScopeDate == "1" ? null : this.$moment(this.datePickerModel.end).format("YYYY-MM-DD");

      this.$router.push({ name: "ClientProgressPrint", query: { dateStart: dateStart, dateEnd: dateEnd } });
    }
  }
};
</script>
