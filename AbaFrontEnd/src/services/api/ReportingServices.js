import Vue from 'vue';

export default {

  getBillingGuide(from, to, clientId, behaviorAnalysisCodeId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/reporting/GetBillingGuide/${from}/${to}/${clientId}/${behaviorAnalysisCodeId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message))
    });
  },

  getServiceLog(from, to, clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/reporting/GetServiceLog/${from}/${to}/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message))
    });
  },

  getSessionsHistory(from, to, clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/reporting/GetSessionsHistory/${from}/${to}/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getSessionsByUser(from, to, userId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/reporting/GetSessionsByUser/${from}/${to}/${userId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

}