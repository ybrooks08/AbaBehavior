<template>
  <v-dialog @input="change" value="true" width="300">
    <v-toolbar dark color="teal" dense>
      <v-icon>warning</v-icon>
      <v-toolbar-title class="white--text">Warning</v-toolbar-title>
    </v-toolbar>
    <v-card tile>
      <v-card-text v-html="message" />
      <v-card-actions>
        <v-spacer />
        <v-btn flat @click="choose(false)">No</v-btn>
        <v-btn color="primary" @click="choose(true)">Yes</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
  export default {
    props: {
      message: {
        type: String
      }
    },
    data() {
      return {
        value: false
      }
    },
    methods: {
      choose(value) {
        this.$emit('result', value)
        this.value = value
        this.$destroy()
      },

      change() {
        this.$destroy()
      }
    }
  }
</script>