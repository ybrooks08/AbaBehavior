<template>
  <v-container grid-list-md fluid pa-0>
    <v-layout row wrap>
      <v-flex xs12>
        <v-card class="elevation-12" width="750">
          <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>

          <v-toolbar color="secondary" dark tabs dense>
            <v-toolbar-title>Monthly notes {{note.monthlyNoteDate | moment('utc', 'MMMM/YYYY')}}</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-tabs slot="extension" dark show-arrows v-model="tabModel">
              <v-tab key="summary">Summary</v-tab>
              <v-tab key="extra">Extra</v-tab>
              <v-tab key="changes">Changes</v-tab>
              <v-tab key="recomendations">Recomendations</v-tab>
            </v-tabs>
          </v-toolbar>
          <v-tabs-items v-model="tabModel">
            <v-tab-item key="summary">
              <v-card flat>
                <v-card-text class="pa-2">
                  <v-container fluid grid-list-sm pa-0>
                    <v-layout row wrap>
                      <v-flex xs12>
                        <v-textarea box hide-details :disabled="loading" label="Monthly summary" auto-grow v-model="note.monthlySummary"></v-textarea>
                      </v-flex>
                      <v-flex xs12>
                        <v-textarea box hide-details :disabled="loading" label="Comments about caregiver training" auto-grow v-model="note.commentsAboutCaregiver"></v-textarea>
                      </v-flex>
                      <v-flex xs12>
                        <v-textarea box hide-details :disabled="loading" label="Services to be provided the next month" auto-grow v-model="note.services2ProvideNextMonth"></v-textarea>
                      </v-flex>
                    </v-layout>
                  </v-container>
                </v-card-text>
              </v-card>
            </v-tab-item>
            <v-tab-item key="extra">
              <v-card flat>
                <v-card-text class="pa-2">
                  <v-container fluid grid-list-sm pa-0>
                    <v-layout row wrap>
                      <v-flex xs12>
                        <v-textarea box hide-details :disabled="loading" label="Recipient's health issues" auto-grow v-model="note.recipientHealthIssues"></v-textarea>
                      </v-flex>
                      <v-flex xs12>
                        <v-textarea box hide-details :disabled="loading" label="Medication" auto-grow v-model="note.medication"></v-textarea>
                      </v-flex>
                      <v-flex xs12>
                        <v-textarea box hide-details :disabled="loading" label="Barriers to treatment" auto-grow v-model="note.barriers2Treatment"></v-textarea>
                      </v-flex>
                    </v-layout>
                  </v-container>
                </v-card-text>
              </v-card>
            </v-tab-item>
            <v-tab-item key="changes">
              <v-card flat>
                <v-card-text class="pa-2">
                  <v-container fluid grid-list-sm pa-0>
                    <v-layout row wrap>
                      <v-flex xs12>
                        <v-textarea box hide-details :disabled="loading" label="Family changes" auto-grow v-model="note.familyChanges"></v-textarea>
                      </v-flex>
                      <v-flex xs12>
                        <v-textarea box hide-details :disabled="loading" label="Home changes" auto-grow v-model="note.homeChanges"></v-textarea>
                      </v-flex>
                      <v-flex xs12>
                        <v-textarea box hide-details :disabled="loading" label="Provider changes" auto-grow v-model="note.proviverChanges"></v-textarea>
                      </v-flex>
                    </v-layout>
                  </v-container>
                </v-card-text>
              </v-card>
            </v-tab-item>
            <v-tab-item key="recomendations">
              <v-card flat>
                <v-card-text class="pa-2">
                  <v-container fluid grid-list-sm pa-0>
                    <v-layout row wrap>
                      <v-flex xs12>
                        <v-switch hide-details color="primary" label="Continue plan as is next month" v-model="note.continueNextMonth"></v-switch>
                      </v-flex>
                      <v-flex xs12>
                        <v-switch hide-details color="primary" label="Re-assessment next month" v-model="note.reassessmentNextMonth"></v-switch>
                      </v-flex>
                      <v-flex xs12>
                        <v-switch hide-details color="primary" label="Refer to other services" v-model="note.refer2OtherServices"></v-switch>
                      </v-flex>
                      <v-flex xs12>
                        <v-switch hide-details color="primary" label="Changes in current plan" v-model="note.changesCurrentPlan"></v-switch>
                      </v-flex>
                      <v-flex xs12>
                        <v-textarea box hide-details :disabled="loading" label="Extra notes" auto-grow v-model="note.extraNotes"></v-textarea>
                      </v-flex>
                    </v-layout>
                  </v-container>
                </v-card-text>
              </v-card>
            </v-tab-item>
          </v-tabs-items>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn :disabled="loading" @click="close" flat>Cancel</v-btn>
            <v-btn :disabled="loading" :loading="loading" color="primary" @click="save">Save</v-btn>
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>


<script>
import sessionServicesApi from "@/services/api/SessionServices";

export default {
  data() {
    return {
      loading: false,
      tabModel: 0,
      note: {
        monthlySummary: null,
        commentsAboutCaregiver: null,
        services2ProvideNextMonth: null,
        recipientHealthIssues: null,
        medication: null,
        barriers2Treatment: null,
        familyChanges: null,
        homeChanges: null,
        proviverChanges: null,
        continueNextMonth: false,
        reassessmentNextMonth: false,
        refer2OtherServices: false,
        changesCurrentPlan: false,
        extraNotes: null,
        monthlyNoteDate: new Date()
      }
    };
  },

  computed: {
    activeClientId() {
      return this.$store.getters.activeClientId;
    },
    activeDate() {
      return this.$store.getters.activeDate;
    }
  },

  mounted() {
    this.loadMonthlyNote();
  },

  methods: {
    async loadMonthlyNote() {
      try {
        this.note = await sessionServicesApi.getMonthlyNote(this.activeClientId, this.$moment(this.activeDate).format("YYYY-MM-DD"));
      } catch (error) {
        this.$toast.error(error.message || error);
      }
    },

    close() {
      this.$router.push("/clients/sessions_details");
    },

    async save() {
      try {
        this.loading = true;
        await sessionServicesApi.editMonthlyNote(this.note);
        this.close();
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>