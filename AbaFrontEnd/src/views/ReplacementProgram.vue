<template>
  <v-card class="elevation-8">
    <v-toolbar dark class="secondary" fluid>
      <v-toolbar-title>Replacement Programs</v-toolbar-title>
      <v-spacer></v-spacer>
      <v-text-field v-model="search" placeholder="Search" prepend-icon="fa-search" hide-details single-line solo-inverted></v-text-field>
      <v-menu class="mr-0" bottom left :disabled="loading">
        <v-btn slot="activator" icon :disabled="loading">
          <v-icon>fa-ellipsis-v</v-icon>
        </v-btn>
        <v-list>
          <v-list-tile @click="addNewItem">
            <v-list-tile-action>
              <v-icon medium>fa-plus-circle</v-icon>
            </v-list-tile-action>
            <v-list-tile-content>
              <v-list-tile-title>Add new</v-list-tile-title>
            </v-list-tile-content>
          </v-list-tile>
        </v-list>
      </v-menu>
    </v-toolbar>

    <v-data-table :headers="headers" :search="search" :items="items" :loading="loading" :pagination.sync="pagination" hide-actions :rows-per-page-items="[{'text':'All','value':-1}]">
      <template slot="items" slot-scope="props">
        <tr :class="{'red lighten-5': !props.item.active}">
          <td class="text-xs-left">
            {{props.item.replacementId}}
          </td>
          <td class="text-xs-left">
            {{props.item.replacementProgramDescription}}
          </td>
          <td class="right">
            <table>
              <tr>
                <td>
                  <v-btn slot="activator" icon @click="editName(props.item)">
                    <v-icon color="success">fa-edit</v-icon>
                  </v-btn>
                </td>
                <td>
                  <v-switch color="primary" v-model="props.item.active" @change="changeActive(props.item)" hide-details/>
                </td>
              </tr>
            </table>


          </td>
        </tr>
      </template>
    </v-data-table>
  </v-card>

</template>

<script>
  import tablesApi from '@/services/api/MasterTablesServices';

  export default {
    data() {
      return {
        items: [],
        search: '',
        loading: false,
        headers: [
          { text: 'Id', align: 'left', value: 'replacementId' },
          { text: 'Description', align: 'left', value: 'replacementProgramDescription' },
          { text: '', align: 'left', value: 'active', sortable: false },
        ],
        pagination: {
          sortBy: 'replacementProgramDescription',
        },
        editModal: false,
        textEdit: '',
        idEdit: null,
      };
    },

    mounted() {
      this.loadItems();
    },

    methods: {
      async loadItems() {
        this.items = [];
        this.loading = true;
        try {
          this.items = await tablesApi.getReplacementPrograms();
        } catch (error) {
          this.$toast.error(error);
        } finally { this.loading = false; }
      },

      async changeActive(item) {
        const newStatus = {
          status: item.active,
          id: item.replacementId,
        };
        this.loading = true;
        try {
          await tablesApi.replacementProgramsChangeStatus(newStatus);
          this.$toast.success('Status changed successful.');
        } catch (error) {
          this.$toast.error(error);
        } finally { this.loading = false; }
      },

      editName(item) {
        this.$prompt(item.replacementProgramDescription, { title: "Edit replacement", label: "Description" })
          .then(async text => {
            let item2 = {
              id: item.replacementId,
              text: text
            }
            if (text) {
              await this.changedText(item2);
            }
          })
      },

      addNewItem() {
        this.$prompt(null, { title: "Add new replacement", label: "Description" })
          .then(async text => {
            let item = {
              id: 0,
              text: text
            }
            if (text) {
              await this.changedText(item);
            }
          })
      },

      async changedText(item) {
        const item2Edit = {
          id: item.id,
          description: item.text,
        };
        this.loading = true;
        try {
          const res = await tablesApi.addEditReplacementPrograms(item2Edit);
          if (item.id !== 0) {
            let index = this.items.findIndex(o => o.replacementId === item.id);
            this.items[index].replacementProgramDescription = item.text;
          } else {
            this.items.unshift({
              replacementId: res.data.replacementId,
              replacementProgramDescription: item.text,
              active: true,
            });
          }

          this.$toast.success(`Item ${item.id === 0 ? 'added' : 'edited'} successful.`);
        } catch (error) {
          this.$toast.error(error);
        } finally { this.loading = false; }
      },
    },

  };
</script>