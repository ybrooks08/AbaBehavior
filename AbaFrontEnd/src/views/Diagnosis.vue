<template>
  <v-card class="elevation-8">
    <v-toolbar dark class="secondary" fluid>
      <v-toolbar-title>Diagnosis</v-toolbar-title>
    </v-toolbar>

    <v-card-text>
      <v-alert :value="true" type="info">
        There are {{count}} diagnosis in the system. Please enter code bellow following by Enter key to find.
      </v-alert>
      <v-layout row wrap>
        <v-flex md6 lg4>
          <v-text-field box label="Code" prepend-icon="fa-search" v-model="code" clearable v-on:keyup.enter="getData()"></v-text-field>
        </v-flex>
        <v-flex xs12>
          <v-alert :value="diag.diagnosisId === 0" color="orange lighten-3 black--text">
            <v-icon large>help</v-icon>
            <v-chip label small color="secondary" text-color="white">{{this.codeFake}}</v-chip> not found. You can
            <v-btn dark color="success" @click="createNewDiagnosis">Create</v-btn>
            a new one.
          </v-alert>
          <v-alert :value="diag.diagnosisId != null && diag.diagnosisId !== 0" color="green lighten-3 black--text">
            <v-chip color="secondary" text-color="white" label>
              <v-avatar>
                <v-icon>fa-stethoscope</v-icon>
              </v-avatar>
              {{diag.description}}
            </v-chip>
            <v-btn dark color="primary" @click="editDiagnosis">Edit</v-btn>
          </v-alert>
        </v-flex>
      </v-layout>
    </v-card-text>
  </v-card>
</template>

<script>
import tablesApi from '@/services/api/MasterTablesServices';

export default {
  data() {
    return {
      loading: false,
      count: 0,
      code: null,
      codeFake: null,
      diag: {
        diagnosisId: null,
        code: null,
        description: null,
      },
    };
  },

  mounted() {
    this.getCount();
  },

  methods: {
    async getCount() {
      this.loading = true;
      try {
        this.count = await tablesApi.getDiagnosisCount();
      } catch (error) {
        this.$toast.error(error);
      } finally { this.loading = false; }
    },

    async getData() {
      if (!this.code) {
        this.$toast.warning("You must enger a valid code.");
        return;
      }
      this.loading = true;
      try {
        this.diag = await tablesApi.getDiagnosis(this.code);
        this.codeFake = this.code;
      } catch (error) {
        this.$toast.error(error);
      } finally { this.loading = false; }
    },

    createNewDiagnosis() {
      this.$prompt(null, { title: "New diagnosis", label: `Description for ${this.code.toUpperCase()}` })
        .then(desc => {
          this.addOrEditDiagnosis(desc);
        })
    },

    editDiagnosis() {
      this.$prompt(this.diag.description, { title: "Edit diagnosis", label: `Description for ${this.code.toUpperCase()}` })
        .then(desc => {
          this.addOrEditDiagnosis(desc);
        })
    },

    async addOrEditDiagnosis(description) {
      if (!description) return;
      this.diag.code = this.code;
      this.diag.description = description;
      this.loading = true;
      try {
        await tablesApi.addEditDiagnosis(this.diag);
        if (this.diag.diagnosisId === 0) {
          this.$toast.success("Diagnosis added successful");
          this.count++;
        } else {
          this.$toast.success("Diagnosis edited successful");
        }
        setTimeout(() => {
          this.diag.diagnosisId = null;
          this.code = null;
        }, 2000)
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    }
  },
};
</script>

<style scoped>
</style>