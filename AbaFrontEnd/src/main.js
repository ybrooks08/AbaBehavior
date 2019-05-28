/* eslint no-undef:0 */
import Vue from 'vue';
import App from './App.vue';
import axios from 'axios';
import VueAxios from 'vue-axios';
import router from './router';
import store from './store';
import Vuetify from 'vuetify';
import 'vuetify/dist/vuetify.min.css';
import VCalendar from 'v-calendar';
import 'v-calendar/lib/v-calendar.min.css';
import VueIziToast from 'vue-izitoast';
import 'izitoast/dist/css/iziToast.min.css';
import VueMoment from 'vue-moment';
import VeeValidate from 'vee-validate';
import { phone, socialSecurity } from './filters';
import VuetifyConfirm from '@/services/plugins/confirm';
import VuetifyPrompt from '@/services/plugins/prompt';
import '@fortawesome/fontawesome-free/css/all.css';
import DatePickerMenu from '@/components/shared/DatePickerMenu';
import HighchartsVue from 'highcharts-vue';
import Highcharts from 'highcharts';
import exportingInit from 'highcharts/modules/exporting';
import VueInsProgressBar from 'vue-ins-progress-bar';

const http = axios.create({
  baseURL: process.env.VUE_APP_BASE_URI
});

Vue.use(VueAxios, http);
Vue.use(Vuetify, {
  iconfont: 'fa',
  icons: {
    'clear': 'fa-times-circle',
    'error': 'fa-exclamation-triangle',
    'warning': 'fa-exclamation-circle',
    'success': 'fa-check-circle',
    'info': 'fa-info-circle',
    'edit': 'fa-edit',
    'dropdown': 'fa-caret-down',
    'expand': 'fa-chevron-down',
    'prev': 'fa-angle-left',
    'next': 'fa-angle-right'
  }
});

Vue.use(VCalendar, { firstDayOfWeek: 1, componentPrefix: 'vc' });
Vue.use(VueIziToast);
Vue.use(VueMoment);
Vue.use(VeeValidate);
Vue.use(VuetifyConfirm);
Vue.use(VuetifyPrompt);
Vue.use(HighchartsVue);
exportingInit(Highcharts);

Vue.use(VueInsProgressBar, {
  position: 'fixed',
  show: true,
  height: '3px'
});


Vue.config.productionTip = false;

Vue.filter('phone', phone);
Vue.filter('social-security', socialSecurity);

Vue.component('date-picker-menu', DatePickerMenu);

const token = localStorage.getItem('user-token');
if (token) {
  Vue.axios.defaults.headers.common['Authorization'] = 'Bearer ' + token;
}

router.beforeEach((to, from, next) => {
  // if (to.name === 'login' || to.name === '403' || to.name === "sign") return next();
  if (to.name === 'login' || to.name === '403' || to.name === 'sign') return next();
  store.dispatch('CHECK_USER')
    .then((user) => {
      if (!to.meta.rol) {
        store.commit('SET_LAST_PATH', from.path);
        return next();
      }
      else if (to.meta.rol.includes(user.rol2)) {
        store.commit('SET_LAST_PATH', from.path);
        return next();
      }
      else return next({ path: '/403' })
    })
  //.catch(() => { return next({ path: '/403' }) });
});

new Vue({
  router,
  store,
  render: h => h(App),
}).$mount('#app');
