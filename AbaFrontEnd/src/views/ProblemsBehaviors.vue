<template>
  <v-card class="elevation-8">
    <v-toolbar dark class="secondary" fluid>
      <v-toolbar-title>Problems behaviors</v-toolbar-title>
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
            {{props.item.problemId}}
          </td>
          <td class="text-xs-left">
            {{props.item.problemBehaviorDescription}}
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
          { text: 'Id', align: 'left', value: 'problemId' },
          { text: 'Description', align: 'left', value: 'problemBehaviorDescription' },
          { text: '', align: 'left', value: 'active', sortable: false },
        ],
        pagination: {
          sortBy: 'problemBehaviorDescription',
        },
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
          this.items = await tablesApi.getProblemBehaviors();
        } catch (error) {
          this.$toast.error(error);
        } finally { this.loading = false; }
      },

      async changeActive(item) {
        const newStatus = {
          status: item.active,
          id: item.problemId,
        };
        this.loading = true;
        try {
          await tablesApi.problemBehaviorsChangeStatus(newStatus);
          this.$toast.success('Status changed successful.');
        } catch (error) {
          this.$toast.error(error);
        } finally { this.loading = false; }
      },

      editName(item) {
        this.$prompt(item.problemBehaviorDescription, { title: "Edit problem", label: "Description" })
          .then(async text => {
            let item2 = {
              id: item.problemId,
              text: text
            }
            if (text) {
              await this.changedText(item2);
            }
          })
      },

      addNewItem() {
        this.$prompt(null, { title: "Add new problem", label: "Description" })
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
          const res = await tablesApi.addEditProblemBehaviors(item2Edit);
          if (item.id !== 0) {
            let index = this.items.findIndex(o => o.problemId === item.id);
            this.items[index].problemBehaviorDescription = item.text;
          } else {
            this.items.unshift({
              problemId: res.data.problemId,
              problemBehaviorDescription: item.text,
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