import router from '@/router';
import Vue from 'vue';

const state = {
  token: localStorage.getItem('user-token') || '',
  user: {
    fullname: null,
    username: null,
    rol: null,
    active: null
  },
  lastPath: null
};

const getters = {
  isAuthenticated: state => !!state.token,
  user: state => state.user,
  lastPath: state => state.lastPath || '\\'
};

const actions = {
  AUTH_REQUEST({ commit }, data) {
    return new Promise((resolve, reject) => {
      Vue.axios.post('api/auth/login', data)
        .then(resp => {
          const token = resp.data;
          localStorage.setItem('user-token', token);
          commit('AUTH_SUCCESS', token);
          Vue.axios.defaults.headers.common['Authorization'] = 'Bearer ' + token;
          resolve();
        })
        .catch(error => {
          localStorage.removeItem('user-token');
          reject(error);
        });
    });
  },
  AUTH_LOGOUT: () => {
    localStorage.removeItem('user-token');
    delete Vue.axios.defaults.headers.common['Authorization'];
    router.push('/login');
  },
  CHECK_USER: ({ commit, dispatch }) => {
    return new Promise((resolve, reject) => {
      Vue.axios.get('api/auth')
        .then(resp => {
          commit('SET_USER', resp.data);
          resolve(resp.data);
        })
        .catch(err => {
          console.log(err.message);
          dispatch('AUTH_LOGOUT');
          reject(err);
        });
    });
  },
};

const mutations = {
  AUTH_SUCCESS: (state, token) => {
    state.token = token;
  },
  SET_USER: (state, user) => {
    state.user = user;
  },
  SET_LAST_PATH: (state, path) => {
    state.lastPath = path;
  },
};

export default {
  state,
  getters,
  actions,
  mutations,
};