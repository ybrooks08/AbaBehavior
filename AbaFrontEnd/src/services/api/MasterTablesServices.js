import Vue from "vue";

export default {

  getProblemBehaviors() {
    return new Promise((resolve, reject) => {
      Vue.axios.get("api/master-tables/problems-behaviors")
          .then(response => resolve(response.data))
          .catch(error => reject(error.response.data || error.message));
    });
  },

  problemBehaviorsChangeStatus(item) {
    return new Promise((resolve, reject) => {
      Vue.axios.post("api/master-tables/problems-behaviors/change-status", item)
          .then(() => resolve())
          .catch(error => reject(error.response.data || error.message));
    });
  },

  addEditProblemBehaviors(item) {
    return new Promise((resolve, reject) => {
      Vue.axios.post("api/master-tables/problems-behaviors/add-edit", item)
          .then((res) => resolve(res))
          .catch(error => reject(error.response.data || error.message));
    });
  },

  getReplacementPrograms() {
    return new Promise((resolve, reject) => {
      Vue.axios.get("api/master-tables/replacement-programs")
          .then(response => resolve(response.data))
          .catch(error => reject(error.response.data || error.message));
    });
  },

  replacementProgramsChangeStatus(item) {
    return new Promise((resolve, reject) => {
      Vue.axios.post("api/master-tables/replacement-programs/change-status", item)
          .then(() => resolve())
          .catch(error => reject(error.response.data || error.message));
    });
  },

  addEditReplacementPrograms(item) {
    return new Promise((resolve, reject) => {
      Vue.axios.post("api/master-tables/replacement-programs/add-edit", item)
          .then((res) => resolve(res))
          .catch(error => reject(error.response.data || error.message));
    });
  },

  getDiagnosisCount() {
    return new Promise((resolve, reject) => {
      Vue.axios.get("api/master-tables/get-diagnosis-count")
          .then(response => resolve(response.data))
          .catch(error => reject(error.response.data || error.message));
    });
  },

  getDiagnosis(code) {
    return new Promise((resolve, reject) => {
      Vue.axios.get("api/master-tables/get-diagnosis/" + code)
          .then(response => resolve(response.data))
          .catch(error => reject(error.response.data || error.message));
    });
  },

  addEditDiagnosis(diagnosis) {
    return new Promise((resolve, reject) => {
      Vue.axios.post("api/master-tables/add-edit-diagnosis", diagnosis)
          .then(() => resolve())
          .catch(error => reject(error.response.data || error.message));
    });
  },

  getBehaviorAnalysisCodes(onlyCheck = false) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/master-tables/get-behavior-analysis-codes/${onlyCheck}`)
          .then(response => resolve(response.data))
          .catch(error => reject(error.response.data || error.message));
    });
  },

  getPosCodes() {
    return new Promise((resolve, reject) => {
      Vue.axios.get("api/master-tables/get-pos-codes")
          .then(response => resolve(response.data))
          .catch(error => reject(error.response.data || error.message));
    });
  },

  getCompetencyCheckTypes() {
    return new Promise((resolve, reject) => {
      Vue.axios.get("api/master-tables/get-competency-check-types")
          .then(response => resolve(response.data))
          .catch(error => reject(error.response.data || error.message));
    });
  },

  getCompetencyCheckParams() {
    return new Promise((resolve, reject) => {
      Vue.axios.get("api/master-tables/get-competency-check-params")
          .then(response => resolve(response.data))
          .catch(error => reject(error.response.data || error.message));
    });
  },

  getRiskBehaviorCodes() {
    return new Promise((resolve, reject) => {
      Vue.axios.get("api/master-tables/get-risk-behavior-codes")
          .then(response => resolve(response.data))
          .catch(error => reject(error.response.data || error.message));
    });
  },

  getParticipationLevelCodes() {
    return new Promise((resolve, reject) => {
      Vue.axios.get("api/master-tables/get-participation-level-codes")
          .then(response => resolve(response.data))
          .catch(error => reject(error.response.data || error.message));
    });
  },

  getSessionSupervicionWorkWithCodes() {
    return new Promise((resolve, reject) => {
      Vue.axios.get("api/master-tables/GetSessionSupervisionWorkWith")
          .then(response => resolve(response.data))
          .catch(error => reject(error.response.data || error.message));
    });
  },

  getOversightSessionSupervision() {
    return new Promise((resolve, reject) => {
      Vue.axios.get("api/master-tables/GetOversightSessionSupervision")
          .then(response => resolve(response.data))
          .catch(error => reject(error.response.data || error.message));
    });
  },

  getSystemLogs() {
    return new Promise((resolve, reject) => {
      Vue.axios.get("api/master-tables/GetSystemLogs")
          .then(response => resolve(response.data))
          .catch(error => reject(error.response.data || error.message));
    });
  },

  getDayOfWeekBitValues() {
    return new Promise((resolve, reject) => {
      Vue.axios.get("api/master-tables/GetDayOfWeekBitValues")
          .then(response => resolve(response.data))
          .catch(error => reject(error.response.data || error.message));
    });
  }

};
