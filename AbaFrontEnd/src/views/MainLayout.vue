<template>
  <v-app>
    <v-navigation-drawer :permanent="!isMobile" :temporary="isMobile" clipped app class="no-print" v-model="drawer">
      <component :is="navigationDrawer"></component>
    </v-navigation-drawer>
    <v-toolbar app class="blue darken-3 no-print" dense dark clipped-left>
      <v-toolbar-side-icon v-if="isMobile" @click.stop="drawer = !drawer"></v-toolbar-side-icon>
      <v-toolbar-title>
        <router-link to="/" tag="span" style="cursor:pointer;">ABA Behavior</router-link>
      </v-toolbar-title>
      <v-spacer/>
      <v-toolbar-items>
        <v-menu offset-y>
          <v-btn flat slot="activator" dark>
            <v-icon left>fa-user-circle</v-icon>
            {{user.fullName}}
            <v-icon right>fa-angle-down</v-icon>
          </v-btn>
          <v-list>
            <v-list-tile @click="signOut">
              <v-list-tile-action>
                <v-icon>fa-lock-open</v-icon>
              </v-list-tile-action>
              <v-list-tile-content>
                <v-list-tile-title>Sign out</v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
          </v-list>
        </v-menu>
      </v-toolbar-items>
    </v-toolbar>

    <v-content>
      <v-container grid-list-md fluid pa-2 class="print-container">
        <router-view :key="$route.fullPath"/>
      </v-container>
    </v-content>
  </v-app>
</template>

<script>
export default {
  data() {
    return {
      drawer: null
    };
  },

  computed: {
    user() {
      return this.$store.getters.user;
    },
    navigationDrawer() {
      return () => import(`@/views/drawers/${this.user.template}`);
    },
    isMobile() {
      return this.$vuetify.breakpoint.xs || this.$vuetify.breakpoint.sm;
    }
  },

  methods: {
    signOut() {
      this.$store.dispatch("AUTH_LOGOUT");
    }
  }
};
</script>