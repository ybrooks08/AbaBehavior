import Vue from 'vue';

export default {

  getUsers() {
    return new Promise((resolve, reject) => {
      Vue.axios.get('api/users')
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getUser(id) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/users/${id}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getUserFull(id) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/users/get-user-full/${id}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  changeUserStatus(item) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/users/change-status', item)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  changeUserPassword(password) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/users/change-password', password)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  addEditUser(user) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/users/add-edit', user)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getRoles() {
    return new Promise((resolve, reject) => {
      Vue.axios.get('api/users/get-roles')
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getUsersCanCreateSessions() {
    return new Promise((resolve, reject) => {
      Vue.axios.get('api/users/get-users-can-create-session')
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getDocumentGroups() {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/users/get-document-groups`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  changeUserDocumentStatus(item) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/users/change-user-document-status', item)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  changeUserDocumentDate(item) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/users/change-user-document-date', item)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getExpiringDocuments(currentUser) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/users/get-expiring-documents/${currentUser}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  addMissingDocuments(userId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/users/add-missing-documents/${userId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  loadUserClients() {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/users/get-clients-for-user`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getCurrentAuthorizationsForCurrentUser() {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/users/get-current-autorizations-for-current-user`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getUserSign(userId) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/users/GetUserSignature/${userId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  addEditUserSign(userSign) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/users/SaveUserSignature', userSign)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getSessionList(days, showClosed) {
    return new Promise((resolve, reject) => {
      Vue.axios.get(`api/users/GetSessionList/${days}/${showClosed}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },
};
