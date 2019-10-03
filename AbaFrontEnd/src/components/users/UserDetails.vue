<template>
  <v-container grid-list-md fluid class="pa-0">
    <v-layout row wrap>
      <v-flex xs12>
        <v-card class="elevation-12" width="750">
          <v-toolbar dark class="secondary" fluid dense>
            <v-toolbar-title>User basic info</v-toolbar-title>
            <v-spacer/>
            <v-btn dark flat :to="`/users/add_edit/${id}`">
              <v-icon left>fa-edit</v-icon>
              EDIT
            </v-btn>
          </v-toolbar>
          <v-card-title class="pb-0">
            <h3 class="headline">{{user.firstname}} {{user.lastname}}</h3>
          </v-card-title>
          <v-card-text>
            <v-container fluid grid-list-xs class="pa-0">
              <v-layout row>
                <v-flex xs6>
                  <v-layout row wrap>
                    <v-flex class="body-2" xs4>Username:</v-flex>
                    <v-flex xs8>{{user.username || "N/A"}}</v-flex>
                    <v-flex class="body-2" xs4>Email:</v-flex>
                    <v-flex xs8>{{user.email || "N/A"}}</v-flex>
                    <v-flex class="body-2" xs4>Phone:</v-flex>
                    <v-flex xs8>{{user.phone | phone}}</v-flex>
                    <v-flex class="body-2" xs4>Created:</v-flex>
                    <v-flex xs8>{{user.created || new Date() | moment("MMM Do, YYYY")}}</v-flex>
                    <v-flex class="body-2" xs4>Actived:</v-flex>
                    <v-flex xs8>{{user.active ? "YES" : "NO"}}</v-flex>
                  </v-layout>
                </v-flex>
                <v-flex xs6>
                  <v-layout row wrap>
                    <v-flex class="body-2" xs6>Rol:</v-flex>
                    <v-flex xs6>{{!user.rol || user.rol.rolName}}</v-flex>
                    <v-flex class="body-2" xs6>Create sessions:</v-flex>
                    <v-flex xs6>{{!user.rol || user.rol.canCreateSession ? "YES" : "NO"}}</v-flex>
                    <v-flex class="body-2" xs6>Edit all client sessions:</v-flex>
                    <v-flex xs6>{{!user.rol || user.rol.canEditAllClientSession ? "YES" : "NO"}}</v-flex>
                    <v-flex class="body-2" xs6>Must have documents:</v-flex>
                    <v-flex xs6>{{!user.rol || user.rol.hasDocuments ? "YES" : "NO"}}</v-flex>
                  </v-layout>
                </v-flex>
              </v-layout>
            </v-container>
          </v-card-text>
        </v-card>
      </v-flex>
      <v-flex xs12>
        <v-card class="elevation-12" width="750">
          <v-toolbar dark class="secondary" fluid dense>
            <v-toolbar-title>Days allowed to create sessions</v-toolbar-title>
          </v-toolbar>
          <v-progress-linear style="position: absolute;" v-show="loadingDayOfWeekBit" :indeterminate="true" class="ma-0"></v-progress-linear>
          <v-card-text>
            <v-layout row wrap>
              <v-flex xs3 v-for="w in dayOfWeekBitValues" :key="w.text">
                <v-switch hide-details color="primary" :label="w.text" v-model="dayOfWeekBitValuesArray" :value="w.value" @change="changeDayOfWeekValue"></v-switch>
              </v-flex>
            </v-layout>
          </v-card-text>
        </v-card>
      </v-flex>
      <v-flex xs12>
        <v-card class="elevation-12" width="750">
          <v-toolbar dark class="secondary" fluid dense>
            <v-toolbar-title>User e-sign</v-toolbar-title>
            <v-spacer/>
            <v-btn dark flat :to="`/user-sign/${id}`">
              <v-icon left>fa-signature</v-icon>
              {{user.userSign ? "Change" : "Create"}}
            </v-btn>
          </v-toolbar>
          <v-card-text>
            <v-container fluid grid-list-xs class="pa-0">
              <v-layout row>
                <v-flex xs12>
                  <template v-if="!user.userSign">
                    <v-alert type="info" :value="true">NO SIGN</v-alert>
                  </template>
                  <template v-else>
                    <v-img max-width="300" :contain="true" max-height="100" :src="!user.userSign || user.userSign.sign"/>
                  </template>
                </v-flex>
              </v-layout>
            </v-container>
          </v-card-text>
        </v-card>
      </v-flex>
      <v-flex xs12>
        <v-card class="elevation-12">
          <v-toolbar dark class="secondary" fluid dense>
            <v-toolbar-title>User documents</v-toolbar-title>
            <v-spacer></v-spacer>
            <v-menu class="mr-0" bottom left :disabled="loadingDocuments">
              <v-btn slot="activator" icon :disabled="loadingDocuments">
                <v-icon>fa-ellipsis-v</v-icon>
              </v-btn>
              <v-list>
                <v-list-tile @click="addMissingDocuments()">
                  <v-list-tile-action>
                    <v-icon medium>fa-paperclip</v-icon>
                  </v-list-tile-action>
                  <v-list-tile-content>
                    <v-list-tile-title>Add missing documents</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
                <v-list-tile @click="deleteDocuments()">
                  <v-list-tile-action>
                    <v-icon medium>fa-trash-alt</v-icon>
                  </v-list-tile-action>
                  <v-list-tile-content>
                    <v-list-tile-title>Delete all documents</v-list-tile-title>
                  </v-list-tile-content>
                </v-list-tile>
              </v-list>
            </v-menu>
          </v-toolbar>
          <v-card-text>
            <v-expansion-panel v-if="user.documents.length > 0">
              <v-expansion-panel-content v-for="group in groups" :key="group.documentGroupId">
                <div slot="header">{{group.groupName}}</div>
                <table class="v-datatable v-table theme--light">
                  <tbody>
                    <tr v-for="doc in getDocuments(group.documentGroupId)" :key="doc.id">
                      <td>
                        <v-switch hide-details color="primary" v-model="doc.active" @change="changeActive(doc)"></v-switch>
                      </td>
                      <td class="text-xs-left">{{doc.document.documentName}}</td>
                      <td class="text-xs-right">
                        <v-text-field prepend-inner-icon="fa-calendar-times" box v-if="doc.document.documentExpires" @change="dateChanged(doc)" hide-actions hide-details label="Expires" v-model="doc.expires" mask="##/##/####" return-masked-value></v-text-field>
                      </td>
                      <td class="pa-0">
                        <v-btn icon @click="uploadForm(doc)">
                          <v-icon color="primary">fa-cloud fa-lg</v-icon>
                        </v-btn>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </v-expansion-panel-content>
            </v-expansion-panel>
            <v-alert type="info" :value="true" v-else>This user haven't any document yet.</v-alert>
          </v-card-text>
        </v-card>
      </v-flex>

      <documents-pdf :docUser="docUser" :active="showPdfForm" @close="showPdfForm = false" :key="new Date().toString()"></documents-pdf>

    </v-layout>
  </v-container>
</template>

<script>
import userApi from "@/services/api/UserServices";
import masterTableApi from "@/services/api/MasterTablesServices";

export default {
  props: {
    id: {
      type: [Number, String],
      required: true
    }
  },

  components: {
    DocumentsPdf: () => import(/* webpackChunkName: "DocPdf" */ "@/components/users/DocumentsPdf")
  },

  data() {
    return {
      groups: [],
      user: { documents: [] },
      loadingBasicInfo: false,
      loadingDocuments: false,
      showPdfForm: false,
      docUser: null,
      dayOfWeekBitValues: [],
      dayOfWeekBitValuesArray: [],
      loadingDayOfWeekBit: false
    };
  },

  mounted() {
    this.loadBasicInfo();
  },

  methods: {
    async loadBasicInfo() {
      this.loadingBasicInfo = true;
      this.loadingDayOfWeekBit = true;
      try {
        this.dayOfWeekBitValues = await masterTableApi.getDayOfWeekBitValues();
        this.groups = await userApi.getDocumentGroups();
        this.user = await userApi.getUserFull(this.id);
        this.user.documents.forEach(d => {
          d.expires = this.$moment(d.expires)
              .utc()
              .format("MM/DD/YYYY");
        });
        this.dayOfWeekBitValues.forEach(c => {
          const a = (c.value & this.user.sessionsDateAllowed) !== 0;
          this.dayOfWeekBitValuesArray.push(a ? c.value : 0);
        });
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loadingBasicInfo = false;
        this.loadingDayOfWeekBit = false;
      }
    },

    getDocuments(groupId) {
      if (!this.user.documents) return;
      return this.user.documents.filter(x => x.document.documentGroupId === groupId).sort((a, b) => a.documentId - b.documentId);
    },

    async changeActive(doc) {
      const newStatus = {
        status: doc.active,
        id: doc.id
      };
      this.loadingDocuments = true;
      try {
        await userApi.changeUserDocumentStatus(newStatus);
        this.$toast.success("Document changed successful.");
      } catch (error) {
        this.$toast.error(error);
        doc.active = !doc.active;
      } finally {
        this.loadingDocuments = false;
      }
    },

    async dateChanged(doc) {
      const newDate = {
        date: doc.expires || null,
        id: doc.id
      };
      this.loadingDocuments = true;
      try {
        await userApi.changeUserDocumentDate(newDate);
        this.$toast.success("Document date changed successful.");
        doc.active = true;
      } catch (error) {
        this.$toast.error(error);
        doc.expires = null;
      } finally {
        this.loadingDocuments = false;
      }
    },

    async addMissingDocuments() {
      this.loadingDocuments = true;
      try {
        let added = await userApi.addMissingDocuments(this.id);
        added === 0 ? this.$toast.info(`No document(s) missing.`) : this.$toast.success(`Added ${added} documents successful`);
        if (added !== 0) this.loadBasicInfo();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loadingDocuments = false;
      }
    },

    async deleteDocuments() {
      this.loadingDocuments = true;
      try {
        this.$confirm("Do you want to delete all documents?").then(async res => {
          if (res) {
            await userApi.deleteDocuments(this.id);
            this.$toast.success(`Delete all documents successful`);
            this.loadBasicInfo();
          }
        });
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loadingDocuments = false;
      }
    },

    uploadForm(doc) {
      this.docUser = doc;
      this.showPdfForm = true;
    },

    async changeDayOfWeekValue() {
      let value = 0;
      this.dayOfWeekBitValuesArray.forEach(c => {
        value |= c;
      });

      try {
        this.loadingDayOfWeekBit = true;
        await userApi.changeDayOfWeekBit(this.id, value);
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loadingDayOfWeekBit = false;
      }

    }
  }
};
</script>
