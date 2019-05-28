<template>
  <v-layout row wrap>
    <v-flex xs12 class="no-print">
      <v-card>
        <v-toolbar dark class="secondary" fluid dense>
          <v-toolbar-title>Client</v-toolbar-title>
        </v-toolbar>
        <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
        <v-card-text>
          <v-layout row wrap>
            <v-flex xs12 sm6>
              <v-autocomplete box :disabled="loading" :loading="loading" :items="clients" v-model="clientId" label="Client" prepend-inner-icon="fa-user" item-text="clientName" item-value="clientId" @change="clientChanged">
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
            <v-flex xs12 sm6>
              <v-select box :loading="loading" :disabled="loading" :items="notes" v-model="monthlyNoteId" label="Monthly note" prepend-inner-icon="fa-calendar-alt" @change="noteChanged"></v-select>
            </v-flex>
          </v-layout>
        </v-card-text>
      </v-card>
    </v-flex>
    <v-flex xs12 v-if="note">
      <v-card>
        <v-toolbar dark class="secondary no-print" fluid dense>
          <v-toolbar-title>Monthly note</v-toolbar-title>
          <v-spacer></v-spacer>
          <v-menu class="mr-0" bottom left :disabled="loading">
            <v-btn slot="activator" icon :disabled="loading">
              <v-icon>fa-ellipsis-v</v-icon>
            </v-btn>
            <v-list>
              <v-list-tile @click="print">
                <v-list-tile-action>
                  <v-icon medium>fa-print</v-icon>
                </v-list-tile-action>
                <v-list-tile-content>
                  <v-list-tile-title>Print</v-list-tile-title>
                </v-list-tile-content>
              </v-list-tile>
            </v-list>
            <v-divider></v-divider>
          </v-menu>
        </v-toolbar>
        <v-card-text class="print-full-width">
          <v-layout row wrap>
            <v-flex xs2>
              <img style="object-fit: contain;" src="images/logo.jpg" width="100">
            </v-flex>
            <v-flex xs10>
              <h4>MONTHLY REPORT</h4>
              <v-divider></v-divider>
              <table class="v-datatable v-table theme--light print-font-small condensed">
                <tr>
                  <td class="px-1 text-xs-right">Client:</td>
                  <td class="px-1 text-xs-left">{{client.firstname}} {{client.lastname}}</td>
                  <td class="px-1 text-xs-right">Code:</td>
                  <td class="px-1 text-xs-left">{{client.code}}</td>
                </tr>
                <tr>
                  <td class="px-1 text-xs-right">Dob:</td>
                  <td class="px-1 text-xs-left">{{client.dob | moment("utc", "LL")}}</td>
                  <td class="px-1 text-xs-right">RecipientId: </td>
                  <td class="px-1 text-xs-left">{{client.memberNo}}</td>
                </tr>
                <tr>
                  <td class="px-1 text-xs-right">Monthly report:</td>
                  <td class="px-1 text-xs-left"><strong>{{note.monthlyNoteDate | moment("utc", "MMMM, YYYY")}}</strong></td>
                  <td class="px-1 text-xs-right">
                    <span v-if="clientAsistant.length > 0">Assistant:</span>
                  </td>
                  <td class="px-1 text-xs-left">
                    <template v-for="u in clientAsistant">
                      <span :key="'user' + u.userId">{{u.firstname}} {{u.lastname}}</span>
                      <br :key="'br'+u.userId">
                    </template>
                  </td>
                </tr>
                <tr>
                  <td class="px-1 text-xs-right">RBT:</td>
                  <td class="px-1 text-xs-left">
                    <template v-for="u in clientRbt">
                      <span :key="'user' + u.userId">{{u.firstname}} {{u.lastname}}</span>
                      <br :key="'br'+u.userId">
                    </template>
                  </td>
                  <td class="px-1 text-xs-right">Analyst:</td>
                  <td class="px-1 text-xs-left">
                    <template v-for="u in clientAnalyst">
                      <span :key="'user' + u.userId">{{u.firstname}} {{u.lastname}}</span>
                      <br :key="'br'+u.userId">
                    </template>
                  </td>
                </tr>
              </table>
            </v-flex>
            <v-flex xs12>
              <table>
                <tr class="no-page-break" v-if="note.monthlySummary">
                  <h1 class="subheading font-weight-medium">Monthly summary</h1>
                  <p v-html="breakLine(note.monthlySummary)"></p>
                </tr>
                <tr class="no-page-break" v-if="note.commentsAboutCaregiver">
                  <h1 class="subheading font-weight-medium">Comments about caregiver training</h1>
                  <p v-html="breakLine(note.commentsAboutCaregiver)"></p>
                </tr>
                <tr class="no-page-break" v-if="note.services2ProvideNextMonth">
                  <h1 class="subheading font-weight-medium">Services to be provided the next month</h1>
                  <p v-html="breakLine(note.services2ProvideNextMonth)"></p>
                </tr>
                <tr class="no-page-break" v-if="note.recipientHealthIssues">
                  <h1 class="subheading font-weight-medium">Recipient's health issues</h1>
                  <p v-html="breakLine(note.recipientHealthIssues)"></p>
                </tr>
                <tr class="no-page-break" v-if="note.medication">
                  <h1 class="subheading font-weight-medium">Medication</h1>
                  <p v-html="breakLine(note.medication)"></p>
                </tr>
                <tr class="no-page-break" v-if="note.barriers2Treatment">
                  <h1 class="subheading font-weight-medium">Barriers to treatment</h1>
                  <p v-html="breakLine(note.barriers2Treatment)"></p>
                </tr>
                <tr class="no-page-break" v-if="note.familyChanges">
                  <h1 class="subheading font-weight-medium">Family changes</h1>
                  <p v-html="breakLine(note.familyChanges)"></p>
                </tr>
                <tr class="no-page-break" v-if="note.homeChanges">
                  <h1 class="subheading font-weight-medium">Home changes</h1>
                  <p v-html="breakLine(note.homeChanges)"></p>
                </tr>
                <tr class="no-page-break" v-if="note.proviverChanges">
                  <h1 class="subheading font-weight-medium">Provider changes</h1>
                  <p v-html="breakLine(note.proviverChanges)"></p>
                </tr>
                <tr class="no-page-break" v-if="note.extraNotes">
                  <h1 class="subheading font-weight-medium">Extra notes</h1>
                  <p v-html="breakLine(note.extraNotes)"></p>
                </tr>
                <tr class="no-page-break">
                  <h1 class="subheading font-weight-medium">Recomendations</h1>
                  <p class="ma-0">
                    <v-icon small>{{note.continueNextMonth ? "fa-check-circle green--text" : "fa-times-circle red--text"}}</v-icon>
                    Continue plan as is next month
                  </p>
                  <p class="ma-0">
                    <v-icon small>{{note.reassessmentNextMonth ? "fa-check-circle green--text" : "fa-times-circle red--text"}}</v-icon>
                    Re-assessment next month
                  </p>
                  <p class="ma-0">
                    <v-icon small>{{note.refer2OtherServices ? "fa-check-circle green--text" : "fa-times-circle red--text"}}</v-icon>
                    Refer to other services
                  </p>
                  <p class="ma-0">
                    <v-icon small>{{note.changesCurrentPlan ? "fa-check-circle green--text" : "fa-times-circle red--text"}}</v-icon>
                    Changes in current plan
                  </p>
                </tr>
              </table>
            </v-flex>
          </v-layout>
        </v-card-text>
      </v-card>
    </v-flex>
  </v-layout>
</template>

<script>
import userApi from "@/services/api/UserServices";

export default {
  data() {
    return {
      loading: false,
      clients: [],
      clientRbt: [],
      clientAnalyst: [],
      clientAsistant: [],
      clientId: null,
      notes: [],
      client: null,
      monthlyNoteId: null,
      note: null
    };
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

    async clientChanged(clientId) {
      this.notes = [];
      this.note = null;
      this.monthlyNoteId = null;
      this.loading = true;
      try {
        this.notes = await userApi.getClientMonthlyNotes(clientId);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async noteChanged(monthlyNoteId) {
      this.loading = true;
      try {
        this.note = null;
        const data = await userApi.getClientMonthlyNote(monthlyNoteId);
        console.log(data);
        this.note = data.note;
        this.client = data.client;
        this.clientRbt = this.client.assignments.filter(f => f.rolId === 4);
        this.clientAnalyst = this.client.assignments.filter(f => f.rolId === 2);
        this.clientAsistant = this.client.assignments.filter(f => f.rolId === 3);
      } catch (error) {
        console.error(error);
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    breakLine(s) {
      return s.replace(/\n/g, "<br/>");
    },

    print() {
      window.print();
    }
  }
};
</script>
