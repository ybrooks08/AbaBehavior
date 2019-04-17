<template>
  <v-dialog max-width="800" persistent v-model="active">
    <v-card>
      <v-card-title class="teal white--text">
        <div class="title">
          {{!docUser || docUser.document.documentName}}
        </div>
      </v-card-title>

      <v-card-text>
        <v-container class="pa-0">
          <v-layout row wrap>
            <v-flex xs9>
              <v-text-field box prepend-inner-icon="fa-paperclip fa-lg" v-model="fileNameTxt" label="Filename" @click.native="onFocus" :disabled="loading" ref="fileTextField" hint="Click in the file box to select valid PDF file" persistent-hint></v-text-field>
              <input type="file" accept=".pdf" :disabled="loading" ref="fileInput" @change="onFileChange">
            </v-flex>
            <v-flex xs3 class="text-xs-center">
              <v-btn large color="primary" :disabled="loading || !this.filename" :loading="loading" @click="uploadFile">UPLOAD
                <v-icon right dark>fa-cloud-upload-alt</v-icon>
              </v-btn>
            </v-flex>
          </v-layout>
          <v-divider></v-divider>
          <v-layout row wrap>
            <v-flex xs12>
              <v-list dense subheader v-if="files.length > 0">
                <v-list-tile v-for="f in files" :key="f.filename" @click="viewFile(f)">
                  <v-list-tile-avatar>
                    <v-avatar size="32">
                      <v-icon>fa-file-pdf</v-icon>
                    </v-avatar>
                  </v-list-tile-avatar>
                  <v-list-tile-content>
                    <v-list-tile-title class="body-2">
                      {{f.name}}
                    </v-list-tile-title>
                    <v-list-tile-sub-title>
                      Upload: {{f.creationTime | moment("utc", "MM/DD/YYYY")}}
                      <span class="ml-3">Size: {{(f.length / 1024).toFixed(1)}} Kb</span>
                    </v-list-tile-sub-title>
                  </v-list-tile-content>
                  <v-list-tile-action>
                    <v-btn icon :disabled="loading" @click.stop="deleteFile(f)">
                      <v-icon small color="grey">fa-trash</v-icon>
                    </v-btn>
                  </v-list-tile-action>
                </v-list-tile>
              </v-list>
              <v-alert v-else type="info" :value="true">No files uploaded</v-alert>
            </v-flex>
          </v-layout>
        </v-container>
      </v-card-text>

      <v-card-actions>
        <v-spacer />
        <v-btn :disabled="loading" flat @click="close">Close</v-btn>
      </v-card-actions>

    </v-card>
  </v-dialog>
</template>

<script>
import userApi from "@/services/api/UserServices";

export default {
  name: "DocumentsPdf",

  props: {
    active: {
      type: Boolean,
      default: false,
      required: true
    },
    docUser: {}
  },

  data() {
    return {
      loading: false,
      filename: null,
      fileNameTxt: null,
      files: []
    };
  },

  created() {
    this.loadFiles();
  },

  methods: {
    async loadFiles() {
      try {
        if (!this.docUser) return;
        this.loading = true;
        this.files = await userApi.getUserPdfs(this.docUser.id);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    onFocus() {
      if (!this.loading) {
        this.$refs.fileInput.click();
      }
    },

    onFileChange($event) {
      if ($event.target.files.length === 0) {
        this.filename = null;
        this.fileNameTxt = null;
        return;
      }
      const file = $event.target.files[0];
      this.filename = file;
      this.fileNameTxt = file.name;
    },

    async uploadFile() {
      if (!this.filename) {
        this.$toast.warning("You must select a valid PDF file");
        return;
      }

      const formData = new FormData();
      formData.append("body", this.filename);
      this.loading = true;
      try {
        await userApi.uploadDocument(this.docUser.id, formData);
        this.$toast.success("Document upload successful.");
        this.filename = null;
        this.fileNameTxt = null;
        this.$refs.fileInput.value = "";
        this.loadFiles();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    close() {
      this.$emit("close");
    },

    viewFile(file) {
      const path = `${process.env.VUE_APP_BASE_URI}/UserDocuments/${this.docUser.id}/${file.name}`;
      window.open(path, "_blank");
    },

    async deleteFile(f) {
      this.$confirm("Do you want to delete this document?").then(async res => {
        if (res) {
          try {
            const model = {
              id: this.docUser.id,
              value: f.name
            };
            await userApi.deletePdf(model);
            this.loadFiles();
            this.$toast.success("Document deleted successful.");
          } catch (error) {
            this.$toast.error(error.message || error);
          }
        }
      });
    }
  }
};
</script>

<style scoped>
input[type="file"] {
  position: absolute;
  left: -99999px;
}
</style>
