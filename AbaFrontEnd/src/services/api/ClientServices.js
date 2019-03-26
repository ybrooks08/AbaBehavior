import Vue from 'vue';

export default {

  getClients() {
    return new Promise((resolve, reject) => {
      Vue.axios.get('api/clients')
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  addEditClient(client) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/add-edit', client)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getClient(id) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/clients/${id}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  changeClientStatus(item) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/change-status', item)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getCaregiversTypes() {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/clients/get-caregivers-types`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  addEditCaregiver(caregiver) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/add-edit-caregiver', caregiver)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  deleteCaregiver(caregiver) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/delete-caregiver', caregiver)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  addEditReferral(referral) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/add-edit-referral', referral)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  deleteReferral(referral) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/delete-referral', referral)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  changeReferralStatus(item) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/change-referral-status', item)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getAssessments(clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/clients/get-assessments/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  addAssessment(assessment) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/add-assessment', assessment)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  deleteAssessment(assessment) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/delete-assessment', assessment)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  addAssignment(assignment) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/add-assignment', assignment)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getAssignments(clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/clients/get-assignments/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  deleteAssignment(assignment) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/delete-assignment', assignment)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  changeAssignmentStatus(item) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/change-assignment-status', item)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  addClientDiagnosis(item) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/add-client-diagnosis', item)
        .then((diag) => resolve(diag))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  deleteClientDiagnosis(diagnosis) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/client-delete-diagnosis', diagnosis)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getClientExpiringReferrals() {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/clients/get-clients-expiring-referrals`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getClientExpiringAssessments() {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/clients/get-clients-expiring-assessments`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getClientCaregivers(clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/clients/get-client-caregivers/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getClientUsers(clientId, rol) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/clients/get-client-users/${clientId}/${rol}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  savePeriod(period) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/SavePeriod', period)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getClientPeriods(clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/clients/GetPeriods/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  saveProblem(problem) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/SaveProblem', problem)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getClientProblems(clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/clients/GetClientProblems/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  saveReplacement(replacement) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/SaveReplacement', replacement)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getClientReplacements(clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/clients/GetClientReplacements/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  saveClientProblemSto(sto) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/SaveClientProblemSto', sto)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getClientProblemStos(clientProblemId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/clients/GetClientProblemStos/${clientProblemId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  deleteClientProblem(id) {
    return new Promise((resolve, reject) => {
      Vue.axios.delete(`api/clients/deleteClientProblem/${id}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  deleteClientProblemSto(id) {
    return new Promise((resolve, reject) => {
      Vue.axios.delete(`api/clients/deleteClientProblemSto/${id}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  saveClientReplacementSto(sto) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/clients/saveClientReplacementSto', sto)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getClientReplacementStos(clientReplacementId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/clients/getClientReplacementStos/${clientReplacementId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  deleteClientReplacement(id) {
    return new Promise((resolve, reject) => {
      Vue.axios.delete(`api/clients/deleteClientReplacement/${id}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  deleteClientReplacementSto(id) {
    return new Promise((resolve, reject) => {
      Vue.axios.delete(`api/clients/deleteClientReplacementSto/${id}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },
};