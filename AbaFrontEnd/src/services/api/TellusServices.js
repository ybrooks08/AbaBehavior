import Vue from "vue";

export default {
  
  addTellusConfig(user) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .post("api/tellus/add-tellus-config", user)
        .then(() => resolve())
        .catch((error) => reject(error.response.data || error.message));
    });
  },

  GetTellusStepOne(from, to) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/tellus/GetTellusStepOne/${from}/${to}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  }

};
