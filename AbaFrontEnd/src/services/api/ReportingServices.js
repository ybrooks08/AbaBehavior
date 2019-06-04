import Vue from 'vue';

export default {

  getBillingGuide(from, to, clientId, behaviorAnalysisCodeId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/reporting/GetBillingGuide/${from}/${to}/${clientId}/${behaviorAnalysisCodeId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message))
    });
  },

  getServiceLog(from, to, clientId, userId = -1) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/reporting/GetServiceLog/${from}/${to}/${clientId}/${userId}`)
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

  getTimeSheet(from, to, userId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/reporting/GetTimeSheet/${from}/${to}/${userId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getSessionsReady2Bill() {
    return new Promise((resolve, reject) => {
      Vue.axios.get('api/reporting/GetSessionsReady2Bill')
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getMonthWeekData(month, clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/reporting/GetMonthWeekData/${month}/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },
}