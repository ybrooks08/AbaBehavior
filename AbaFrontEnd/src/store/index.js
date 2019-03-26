import Vue from 'vue';
import Vuex from 'vuex';
import auth from './auth';
import shared from './shared';
import session from './session';

Vue.use(Vuex);

export default new Vuex.Store({
  modules: {
    auth,
    session,
    shared
  }
});