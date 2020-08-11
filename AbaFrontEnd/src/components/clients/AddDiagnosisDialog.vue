<template>
  <v-dialog width="600" persistent transition="slide-y-transition" v-model="model">
    <v-card>
      <v-card-title class="teal white--text">
        <div class="title">Add new diagnosis to the client</div>
      </v-card-title>
      <v-container grid-list-md pa-1>
        <v-layout row wrap>
          <v-flex xs12>
            <v-card-text>
              <v-form ref="form" autocomplete="off" v-model="validForm">
                <v-container fluid grid-list-md pa-0>
                  <v-layout row wrap>
                    <v-flex sm12 md12>
                      <v-text-field box :disabled="loading" label="Code" v-model="code" required :rules="[required]" prepend-icon="fa-tag" />
                    </v-flex>
                  </v-layout>
                  <v-layout row wrap>
                    <v-flex sm12 md6>
                      <v-text-field
                        box
                        :disabled="loading"
                        label="Start"
                        v-model="startDate"
                        prepend-icon="fa-calendar-plus"
                        return-masked-value
                        required
                        mask="##/##/####"
                        data-vv-name="startDate"
                        :rules="errors.collect('referral')"
                        v-validate="'required|date_format:MM/dd/yyyy'"
                      ></v-text-field>
                    </v-flex>
                    <v-flex sm12 md6>
                      <v-text-field
                        box
                        :disabled="loading"
                        label="End"
                        v-model="endDate"
                        prepend-icon="fa-calendar-minus"
                        return-masked-value
                        mask="##/##/####"
                        required
                        data-vv-name="endDate"
                        :rules="errors.collect('expires')"
                        v-validate="'required|date_format:MM/dd/yyyy'"
                      ></v-text-field>
                    </v-flex>
                  </v-layout>
                </v-container>
              </v-form>
            </v-card-text>
          </v-flex>
        </v-layout>

        <v-card-actions>
          <v-spacer />
          <v-btn :disabled="loading" flat @click="cancel">Cancel</v-btn>
          <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="saveChanges">Add</v-btn>
        </v-card-actions>
      </v-container>
    </v-card>
  </v-dialog>
</template>

<script>
//import clientApi from "@/services/api/ClientServices";

export default {
  props: {
    model: {
      type: Boolean,
      required: true,
      default: false
    },
    loading: {
      type: Boolean,
      default: false
    }
  },

  data() {
    return {
      //      loading: false,
      validForm: false,
      required: (value) => !!value || "This field is required.",
      code: null,
      startDate: null,
      endDate: null
    };
  },

  methods: {
    async saveChanges() {
      let data = {
        code: this.code,
        startDate: this.startDate,
        endDate: this.endDate
      };
      this.$emit("onSubmit", data);
      //      this.loading = true;
      //      try {
      //        let referral = await clientApi.addEditReferral(this.data);
      //        this.$emit("onSubmit", referral);
      //        this.$refs.form.reset();
      //      } catch (error) {
      //        this.$toast.error(error);
      //      } finally {
      //        this.loading = false;
      //      }
    },

    cancel() {
      this.$emit("cancel");
      this.$refs.form.reset();
    }
  }
};
</script>

<style scoped></style>
