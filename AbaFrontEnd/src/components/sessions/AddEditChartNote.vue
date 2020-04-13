<template>
  <v-container fluid grid-list-xs pa-0>
    <v-layout row>
      <v-flex sm12 md8 lg6>
        <v-card class="elevation-12">
          <v-toolbar dense dark color="secondary">
            <v-toolbar-title>{{ id === 0 ? "Create new" : "Edit" }} chart quick note</v-toolbar-title>
          </v-toolbar>
          <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
          <v-card-text>
            <v-form ref="form" autocomplete="off" v-model="validForm">
              <v-layout row wrap>
                <v-flex sm12 pl-4>
                  <p>Show note in:</p>
                  <v-radio-group v-model="chartNote.chartNoteType">
                    <v-radio color="primary" v-for="n in chartNoteTypeEnum" :key="n.text" :label="n.text" :value="n.value"></v-radio>
                  </v-radio-group>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex xs12 sm6>
                  <v-text-field box :disabled="loading" label="Title" v-model="chartNote.title" required prepend-icon="fa-tag" counter="40" maxlength="40" :rules="[required]"></v-text-field>
                </v-flex>
                <v-flex xs12 sm6>
                  <v-text-field
                    required
                    label="Note date"
                    box
                    v-model="chartNote.chartNoteDate"
                    mask="##/##/####"
                    prepend-icon="fa-calendar-plus"
                    return-masked-value
                    data-vv-name="notedate"
                    :rules="[required]"
                    :error-messages="errors.collect('notedate')"
                    v-validate="'required|date_format:MM/dd/yyyy'"
                  ></v-text-field>
                </v-flex>
              </v-layout>
              <v-layout row wrap>
                <v-flex xs12>
                  <v-textarea required box :disabled="loading" label="Notes" auto-grow v-model="chartNote.note" :rules="[required]" prepend-icon="fa-sticky-note"></v-textarea>
                </v-flex>
              </v-layout>
            </v-form>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn :disabled="loading" flat @click="close">Cancel</v-btn>
            <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="submit">{{ id === 0 ? "Create" : "Save" }}</v-btn>
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
import sessionServicesApi from "@/services/api/SessionServices";

export default {
  props: {
    id: {
      type: [Number, String],
      required: false,
      default: 0
    }
  },

  data() {
    return {
      loading: false,
      validForm: false,
      required: value => !!value || "This field is required.",
      chartNoteTypeEnum: [
        { value: 0, text: "Problems & Replacements" },
        { value: 1, text: "Problems only" },
        { value: 2, text: "Replacements only" }
      ],
      chartNote: {
        chartNoteType: 0,
        title: null,
        chartNoteDate: null,
        note: null
      }
    };
  },

  computed: {
    activeClientId() {
      return this.$store.getters.activeClientId;
    }
  },

  async mounted() {
    if (this.id !== 0) this.loadNote();
  },

  methods: {
    close() {
      // this.$router.push("/clients/sessions_details");
      this.$router.go(-1);
    },

    async loadNote() {
      this.loading = true;
      try {
        this.chartNote = await sessionServicesApi.getChartNote(this.id);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async submit() {
      try {
        this.loading = true;
        this.chartNote.clientChartNoteId = this.id;
        this.chartNote.clientId = this.activeClientId;
        await sessionServicesApi.addEditChartNote(this.chartNote);
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
