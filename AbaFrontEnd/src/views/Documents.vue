<template>
  <v-container grid-list-md fluid class="pa-0">
    <v-layout row wrap>
      <v-flex xs12 v-for="g in groups" :key="g.documentGroupId">
        <v-card>
          <v-toolbar dark class="secondary" fluid dense>
            <v-toolbar-title>{{g.groupName}}</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-menu class="mr-0" bottom left :disabled="loading">
              <v-btn slot="activator" icon :disabled="loading">
                <v-icon>fa-ellipsis-v</v-icon>
              </v-btn>
              <v-list>
                <v-list-tile @click="renameDocumentGroup(g)">
                  <v-list-tile-action>
                    <v-icon medium>fa-pen-alt</v-icon>
                  </v-list-tile-action>
                  <v-list-tile-content>
                    <v-list-tile-title>Rename this group</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
                <v-list-tile @click="addDocumentToGroup(g)">
                  <v-list-tile-action>
                    <v-icon medium>fa-paperclip</v-icon>
                  </v-list-tile-action>
                  <v-list-tile-content>
                    <v-list-tile-title>Add document to this group</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
                <v-list-tile @click="deleteDocumentGroup(g)">
                  <v-list-tile-action>
                    <v-icon medium>fa-trash-alt</v-icon>
                  </v-list-tile-action>
                  <v-list-tile-content>
                    <v-list-tile-title>Delete group and all documents</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
              </v-list>
            </v-menu>
          </v-toolbar>
          <v-card-text class="pa-0">
            <v-list two-line v-if="g.documents.length > 0">
              <template v-for="d in g.documents">
                <v-list-tile avatar :key="d.documentId">
                  <v-list-tile-avatar>
                    <v-icon>fa-certificate</v-icon>
                  </v-list-tile-avatar>
                  <v-list-tile-content>
                    <v-list-tile-title>{{ d.documentName }}</v-list-tile-title>
                    <v-list-tile-sub-title>
                      <v-checkbox color="primary" class="ma-0 pa-0" v-model="d.documentExpires" label="Expires" hide-details @change="changeDocumentExpires(d)"></v-checkbox>
                      <!-- <v-chip small :color="d.documentExpires ? 'red' : 'blue'" text-color="white">{{d.documentExpires ? 'YES' : 'NO'}}</v-chip> -->
                    </v-list-tile-sub-title>
                  </v-list-tile-content>
                  <v-list-tile-action>
                    <v-btn icon ripple @click="editDocument(g.documentGroupId, d)">
                      <v-icon color="grey">fa-pen-alt</v-icon>
                    </v-btn>
                  </v-list-tile-action>
                  <v-list-tile-action>
                    <v-btn icon ripple @click="deleteDocument(g.documentGroupId, d)">
                      <v-icon color="grey">fa-trash-alt</v-icon>
                    </v-btn>
                  </v-list-tile-action>
                </v-list-tile>
                <v-divider :key="'d-'+d.documentId"></v-divider>
              </template>
            </v-list>
            <v-alert type="info" :value="true" v-else>No documents in this group</v-alert>
          </v-card-text>
        </v-card>
      </v-flex>
      <v-btn color="primary" dark fixed bottom right fab @click="addNewDocumentGroup()">
        <v-icon>add</v-icon>
      </v-btn>
    </v-layout>
  </v-container>
</template>

<script>
import userApi from '@/services/api/UserServices';

export default {
  data() {
    return {
      groups: [],
      loading: false,
    };
  },

  mounted() {
    this.loadData();
  },

  methods: {
    async loadData() {
      this.loading = true;
      try {
        this.groups = await userApi.getDocumentGroups();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    addNewDocumentGroup() {
      this.$prompt(null, { title: 'Add new group', label: 'Group name' })
        .then(async text => {
          if (text) {
            try {
              this.loading = true;
              const documentGroup = {
                documentGroupId: 0,
                groupName: text
              }
              await userApi.addEditDocumentGroup(documentGroup);
              this.loadData();
            } catch (error) {
              this.$toast.error(error.message);
            } finally { this.loading = false; }
          }
        });
    },

    renameDocumentGroup(group) {
      this.$prompt(group.groupName, { title: 'Rename', label: 'Group name' })
        .then(async text => {
          if (text) {
            try {
              this.loading = true;
              const documentGroup = {
                documentGroupId: group.documentGroupId,
                groupName: text
              }
              await userApi.addEditDocumentGroup(documentGroup);
              this.loadData();
            } catch (error) {
              this.$toast.error(error.message);
            } finally { this.loading = false; }
          }
        });
    },

    deleteDocument(documentGroupId, document) {
      this.$confirm('Do you want to delete this document?')
        .then(async res => {
          if (res) {
            try {
              this.loading = true;
              await userApi.deleteDocument(document);
              let group = this.groups.find(g => g.documentGroupId == documentGroupId);
              group.documents = group.documents.filter(f => f.documentId !== document.documentId);
            } catch (error) {
              this.$toast.error(error.message);
            } finally {
              this.loading = false;
            }
          }
        })
    },

    addDocumentToGroup(group) {
      this.$prompt(null, { title: 'Add new document', label: 'Document name' })
        .then(async text => {
          if (text) {
            try {
              this.loading = true;
              const document = {
                documentId: 0,
                documentName: text,
                documentExpires: false,
                documentGroupId: group.documentGroupId
              }
              await userApi.addEditDocument(document);
              this.loadData();
            } catch (error) {
              this.$toast.error(error.message);
            } finally { this.loading = false; }
          }
        });
    },

    editDocument(documentGroupId, document) {
      this.$prompt(document.documentName, { title: 'Edit document', label: 'Document name' })
        .then(async text => {
          if (text) {
            try {
              this.loading = true;
              const doc = {
                documentId: document.documentId,
                documentName: text,
                documentGroupId: documentGroupId,
                documentExpires: document.documentExpires
              }
              await userApi.addEditDocument(doc);
              this.loadData();
            } catch (error) {
              this.$toast.error(error.message);
            } finally { this.loading = false; }
          }
        });
    },

    async changeDocumentExpires(d) {
      try {
        this.loading = true;
        await userApi.addEditDocument(d);
      } catch (error) {
        this.$toast.error(error.message);
      } finally { this.loading = false; }
    },

    deleteDocumentGroup(group) {
      this.$confirm('Do you want to delete this group and all its documents?')
        .then(async res => {
          if (res) {
            try {
              this.loading = true;
              await userApi.deleteDocumentGroup(group);
              this.loadData();
            } catch (error) {
              this.$toast.error(error.message);
            } finally {
              this.loading = false;
            }
          }
        })
    },


  },
};
</script>