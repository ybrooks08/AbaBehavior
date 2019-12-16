<template>
  <v-layout row wrap v-if="user">
    <v-flex xs12 text-xs-center>
      <div v-if="!user.userSign">
        <div style="min-height: 100px;" class="mb-2"></div>
        <v-divider></v-divider>
      </div>
      <div v-else>
        <div style="max-height: 50px; height: 50px;" class="mb-2">
          <img style="height:100%;" :src="user.userSign.sign" />
        </div>
        <v-divider></v-divider>
      </div>
      <div>
        <label><strong>{{ user.firstname }} {{user.lastname}}</strong>
          <span v-if="date"> on {{ date | moment('utc', showMonth ? 'MMMM/YYYY': 'MM/DD/YYYY') }}</span>
        </label><br />
        <small>{{ user.licenseNo }}</small><br>
      </div>
    </v-flex>
  </v-layout>
</template>

<script>
import userApi from "@/services/api/UserServices";

export default {
  props: {
    userId: {
      type: Number,
      required: true
    },
    date: {
      type: String,
      required: false
    },
    showMonth: {
      type: Boolean,
      required: false,
      default: false
    }
  },

  data() {
    return {
      user: null
    };
  },

  mounted() {
    this.loadData();
  },

  methods: {
    async loadData() {
      try {
        this.loading = true;
        this.user = await userApi.getSign(this.userId);
        console.log(this.user);
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>
