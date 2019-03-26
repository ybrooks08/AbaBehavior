import moment from 'moment';

const state = {
  activeClientId: parseInt(localStorage.getItem('activeClientId')),
  activeDate: moment(new Date(localStorage.getItem('activeDate'))),
  activeSessionId: parseInt(localStorage.getItem('activeSessionId')),
  //  calendarEvents: null
};

const getters = {
  activeClientId: state => state.activeClientId,
  activeDate: state => state.activeDate,
  activeSessionId: state => state.activeSessionId,
  //  calendarEvents: state => state.calendarEvents
};

const actions = {
  //  getCalendarEvents: ({ commit, state }) => {
  //    sessionServicesApi.getSessionsCalendar(state.activeClientId)
  //      .then((events) => {
  //        commit("SET_CALENDAR_EVENTS", events);
  //      })
  //  }
};

const mutations = {
  SET_ACTIVE_CLIENT: (state, clientId) => {
    state.activeClientId = clientId;
    localStorage.setItem('activeClientId', clientId);
  },
  SET_ACTIVE_DATE: (state, date) => {
    state.activeDate = date;
    localStorage.setItem('activeDate', date);
  },
  SET_ACTIVE_SESSION: (state, sessionId) => {
    state.activeSessionId = sessionId;
    localStorage.setItem('activeSessionId', sessionId);
  },
  //  SET_CALENDAR_EVENTS: (state, payload) => {
  //    state.calendarEvents = payload;
  //  },
};

export default {
  state,
  getters,
  actions,
  mutations,
};