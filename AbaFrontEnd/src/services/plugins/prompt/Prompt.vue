<template>
  <v-dialog @input="change" value="true" :width="textArea ? 600 : 300">
    <v-toolbar dark color="teal" dense>
      <v-icon>info</v-icon>
      <v-toolbar-title class="white--text">{{title}}</v-toolbar-title>
    </v-toolbar>
    <v-card tile>
      <v-card-text>
        <v-text-field v-if="!textArea" box :label="label" v-model="text" type="text"/>
        <v-textarea v-else box :label="label" v-model="text"></v-textarea>
      </v-card-text>
      <v-card-actions>
        <v-spacer/>
        <v-btn flat @click="choose(undefined)">Cancel</v-btn>
        <v-btn color="primary" @click="choose(text)" :disabled="!text">Submit</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
export default {
  props: {
    message: {
      type: String,
    },
    title: {
      type: String,
      required: false,
      default: 'Edit',
    },
    label: {
      type: String,
      required: false,
      default: 'Name/value',
    },
    textArea: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  mounted() {
    this.text = this.message;
  },
  data() {
    return {
      value: false,
      text: this.message,
    };
  },
  methods: {
    choose(value) {
      this.$emit('result', value);
      this.value = value;
      this.$destroy();
    },
    change() {
      this.$destroy();
    },
  },
};
</script>