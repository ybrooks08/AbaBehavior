<template>
  <v-dialog persistent width="400" v-model="open">
    <v-card class="grey lighten-3">
      <v-toolbar dark dense fluid>
        <v-toolbar-title>Force STO to mastered</v-toolbar-title>
      </v-toolbar>
      <v-card-text class="pa-1">
        <v-form ref="form" autocomplete="off" v-model="formValid">
          <v-container grid-list-sm>
            <v-layout row wrap>
              <v-flex xs6>
                <v-text-field ref="formDateSelect" box required label="From" v-model="from" return-masked-value prepend-icon="fa-calendar" mask="##/##/####" data-vv-name="from" :rules="errors.collect('from')" v-validate="'required|date_format:MM/dd/yyyy'" />
              </v-flex>
              <v-flex xs6>
                <v-text-field box required label="To" v-model="to" return-masked-value prepend-icon="fa-calendar" mask="##/##/####" data-vv-name="to" :rules="errors.collect('to')" v-validate="'required|date_format:MM/dd/yyyy'" />
              </v-flex>
            </v-layout>
          </v-container>
        </v-form>
      </v-card-text>

      <v-card-actions>
        <v-btn flat color="purple" @click="$emit('cancel')">Cancel</v-btn>
        <v-spacer />
        <v-btn :disabled="!valid" color="primary" @click="submit">Mastered</v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script>
export default {
  props: ["open"],

  computed: {
    valid() {
      return this.formValid && (this.from && this.to);
    }
  },

  data() {
    return {
      formValid: false,
      required: value => !!value || "This field is required.",
      from: null,
      to: null
    };
  },

  watch: {
    open(val) {
      if (val) {
        this.$nextTick(() => {
          this.$refs.form.reset();
          this.$refs.formDateSelect.focus();
        });
      }
    }
  },

  methods: {
    submit() {
      this.$emit("submit", { from: this.from, to: this.to });
    }
  }
};
</script>

<style>
</style>