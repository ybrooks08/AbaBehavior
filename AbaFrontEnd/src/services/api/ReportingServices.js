import Vue from "vue";

export default {
  getBillingGuide(from, to, clientId, behaviorAnalysisCodeId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/reporting/GetBillingGuide/${from}/${to}/${clientId}/${behaviorAnalysisCodeId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getServiceLog(from, to, clientId, userId = -1) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/reporting/GetServiceLog/${from}/${to}/${clientId}/${userId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getSessionsHistory(from, to, clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/reporting/GetSessionsHistory/${from}/${to}/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getSessionsByUser(from, to, userId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/reporting/GetSessionsByUser/${from}/${to}/${userId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getTimeSheet(from, to, userId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/reporting/GetTimeSheet/${from}/${to}/${userId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getSessionsReady2Bill() {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get("api/reporting/GetSessionsReady2Bill")
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getMonthWeekData(month, clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/reporting/GetMonthWeekData/${month}/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getCaregiversCollectionHistory(from, to, clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/reporting/GetCaregiversCollectionHistory/${from}/${to}/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getStaffClientRelationship() {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/reporting/GetStaffClientRelationship`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  addNewProblemBehaviorChartLine(line) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .post("api/reporting/AddNewProblemBehaviorChartLine", line)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getClientProblemChartLines(clientProblemId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/reporting/GetClientProblemChartLines/${clientProblemId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  deleteClientProblemChartLine(clientProblemChartLineId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .delete(`api/reporting/DeleteClientProblemChartLine/${clientProblemChartLineId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  }
};
