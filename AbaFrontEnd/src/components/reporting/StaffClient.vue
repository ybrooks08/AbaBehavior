<template>
  <v-layout row wrap>
    <v-flex xs12 v-if="loading">
      <v-subheader>Loading, please wait <v-icon class="ml-1" small>fas fa-cog fa-spin</v-icon></v-subheader>
    </v-flex>
    <v-flex xs12 v-else>
      <v-layout row wrap>
        <v-flex xs12>
          <v-card width="750">
            <v-card-text class="pa-0">
              <v-tabs color="secondary" dark>
                <v-tab key="clientTab" ripple> Clients </v-tab>
                <v-tab key="userTab" ripple> Staffs </v-tab>
                <v-tab-item key="clientTab">
                  <v-card flat>
                    <v-card-text class="pa-0">
                      <v-expansion-panel>
                        <v-expansion-panel-content v-for="client in report.clients" :key="'client' + client.clientId">
                          <template v-slot:header>
                            <v-layout row wrap>
                              <v-flex xs6> {{ client.firstname }} {{ client.lastname }} </v-flex>
                              <v-flex xs2 :set="(count = getServiceCount(client.assignments, 'H2019'))">
                                <div>
                                  Analyst: <strong :class="count == 0 ? 'red--text' : 'blue--text'">{{ count }}</strong>
                                </div>
                              </v-flex>
                              <v-flex xs2 :set="(count = getServiceCount(client.assignments, 'H2014'))">
                                <div>
                                  Rbt: <strong :class="count == 0 ? 'red--text' : 'blue--text'">{{ count }}</strong>
                                </div>
                              </v-flex>
                              <v-flex xs2 :set="(count = getServiceCount(client.assignments, 'H2012'))">
                                <div>
                                  Assistant: <strong :class="count == 0 ? 'red--text' : 'blue--text'">{{ count }}</strong>
                                </div>
                              </v-flex>
                            </v-layout>
                          </template>
                          <v-card>
                            <v-card-text class="pa-0">
                              <v-list two-line dense v-if="client.assignments.length > 0">
                                <template v-for="u in client.assignments">
                                  <v-list-tile :key="u.userFullname" avatar>
                                    <v-list-tile-avatar>
                                      <v-icon>fa-user</v-icon>
                                    </v-list-tile-avatar>

                                    <v-list-tile-content>
                                      <v-list-tile-title>{{ u.userFullname }}</v-list-tile-title>
                                      <v-list-tile-sub-title v-if="u.rol">{{ u.rol.description }}</v-list-tile-sub-title>
                                    </v-list-tile-content>
                                    <v-list-tile-action>
                                      <v-chip small label text-color="white" :color="u.active ? 'green darken-3' : 'red darken-3'">{{ u.active ? "ACTIVE" : "INACTIVE" }}</v-chip>
                                    </v-list-tile-action>
                                  </v-list-tile>
                                </template>
                              </v-list>
                              <v-alert v-else type="info" :value="true">
                                NO STAFF ASSIGNED
                              </v-alert>
                            </v-card-text>
                          </v-card>
                        </v-expansion-panel-content>
                      </v-expansion-panel>
                    </v-card-text>
                  </v-card>
                </v-tab-item>
                <v-tab-item key="userTab">
                  <v-card flat>
                    <v-card-text class="pa-0">
                      <v-expansion-panel>
                        <v-expansion-panel-content v-for="user in report.users" :key="'user' + user.userId">
                          <template v-slot:header>
                            <v-layout row wrap>
                              <v-flex xs10> {{ user.firstname }} {{ user.lastname }} </v-flex>
                              <v-flex xs2 :set="(count = user.assignments.length)">
                                <div>
                                  clients: <strong :class="count == 0 ? 'red--text' : 'blue--text'">{{ count }}</strong>
                                </div>
                              </v-flex>
                            </v-layout>
                          </template>
                          <v-card>
                            <v-card-text class="pa-0">
                              <v-list two-line dense v-if="user.assignments.length > 0">
                                <template v-for="c in user.assignments">
                                  <v-list-tile :key="c.clientFullname" avatar>
                                    <v-list-tile-avatar>
                                      <v-icon>fa-user</v-icon>
                                    </v-list-tile-avatar>

                                    <v-list-tile-content>
                                      <v-list-tile-title>{{ c.clientFullname }}</v-list-tile-title>
                                      <v-list-tile-sub-title>{{ c.code }}</v-list-tile-sub-title>
                                    </v-list-tile-content>
                                    <v-list-tile-action>
                                      <v-chip small label text-color="white" :color="c.active ? 'green darken-3' : 'red darken-3'">{{ c.active ? "ACTIVE" : "INACTIVE" }}</v-chip>
                                    </v-list-tile-action>
                                  </v-list-tile>
                                </template>
                              </v-list>
                              <v-alert v-else type="info" :value="true">
                                NO STAFF ASSIGNED
                              </v-alert>
                            </v-card-text>
                          </v-card>
                        </v-expansion-panel-content>
                      </v-expansion-panel>
                    </v-card-text>
                  </v-card>
                </v-tab-item>
              </v-tabs>
            </v-card-text>
          </v-card>
        </v-flex>
      </v-layout>
    </v-flex>
  </v-layout>
</template>

<script>
import reportingApi from "@/services/api/ReportingServices";

export default {
  data() {
    return {
      loading: true,
      report: null
    };
  },

  mounted() {
    this.loadData();
  },

  methods: {
    async loadData() {
      this.loading = true;
      try {
        this.report = await reportingApi.getStaffClientRelationship();
        console.log(this.report);
      } catch (error) {
        console.log(error);
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    getServiceCount(r, service) {
      if (r.length === 0) {
        return 0;
      }
      let count = r.filter(s => s.active && s.rol.hcpcs === service);
      return count.length;
    }
  }
};
</script>

<style>
</style>