import Vue from "vue";

export default {
  addSession(session) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .post("api/session/add-session", session)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getSessions(clientId, date) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/get-sessions/${clientId}/${Vue.moment(date).format("YYYY-MM-DD")}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getSession(sessionId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/get-session/${sessionId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getSessionDetailed(sessionId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetSessionDetailed/${sessionId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getSessionForPrint(sessionId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetSessionForPrint/${sessionId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  addSessionMissingProblems(sessionId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/add-session-missing-problems/${sessionId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  editSessionNotes(sessionNote) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .post("api/session/edit-session-notes", sessionNote)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getUnitsAvailable(clientId, date, userName) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/get-units-available/${clientId}/${date}/${userName}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getSessionsCalendar(clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/get-sessions-calendar/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getMonthlyNote(clientId, dateStr) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/get-monthly-note/${clientId}/${dateStr}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  editMonthlyNote(monthlyNote) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .post("api/session/edit-monthly-note", monthlyNote)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getMetricSessionProblems(sessionId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/get-metric-session-problems/${sessionId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getMetricSessionReplacements(sessionId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/get-metric-session-replacements/${sessionId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  changeMetricProblem(metricProblem) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .post("api/session/change-metric-problem", metricProblem)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  changeMetricReplacement(metricReplacement) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .post("api/session/change-metric-replacement", metricReplacement)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getProblemsChartData(clientId, behaviorId, dateStart, dateEnd) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetProblemBehaviorsChart/${clientId}/${behaviorId}/${dateStart}/${dateEnd}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getBehaviorMontlyChart(clientId, behaviorId, dateEnd) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetBehaviorMontlyChart/${clientId}/${behaviorId}/${dateEnd}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getReplacementsChartData(clientId, replacementId, dateStart, dateEnd) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetReplacementProgramChart/${clientId}/${replacementId}/${dateStart}/${dateEnd}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getReplacementMontlyChart(clientId, replacementId, dateEnd) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetReplacementMontlyChart/${clientId}/${replacementId}/${dateEnd}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  addEditChartNote(note) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .post("api/session/add-edit-chart-note", note)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getChartNote(id) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/get-chart-note/${id}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  deleteChartNote(id) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .delete(`api/session/delete-chart-note/${id}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  deleteSession(id, check = true) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .delete(`api/session/delete-session/${id}/${check}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getCompetencyCheck(id) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/get-competency-check/${id}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getCompetencyChecks(id) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/get-competency-checks/${id}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  addEditCompetencyCheck(comp) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .post("api/session/add-edit-competency-check", comp)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  deleteCompetencyCheck(id) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .delete(`api/session/delete-competency-check/${id}`)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  exportCompetencyChecks(id) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/export-competency-check/${id}`, {
          responseType: "blob"
        })
        .then(response => resolve(response))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  changeSessionStatus(status) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .put("api/session/ChangeSessionStatus", status)
        .then(newStatus => resolve(newStatus))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  saveSessionSign(sign) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .post("api/session/SaveSessionSign", sign)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  sendUrlSign(url, sessionId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .post(`api/session/SendUrlSign/${sessionId}`, url)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getClientBehaviors(clientId, onlyActive = true) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetClientBehaviors/${clientId}/${onlyActive}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  saveSessionCollectBehavior(data) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .post("api/session/SaveSessionCollectBehavior", data)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getCollectBehaviors(sessionId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetCollectBehaviors/${sessionId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  deleteSessionCollectBehavior(id) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .delete(`api/session/DeleteSessionCollectBehavior/${id}`)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  //replacements
  getClientReplacements(clientId, onlyActive = true) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetClientReplacements/${clientId}/${onlyActive}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  saveSessionCollectReplacement(data) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .post("api/session/SaveSessionCollectReplacement", data)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getCollectReplacements(sessionId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetCollectReplacements/${sessionId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  deleteSessionCollectReplacement(id) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .delete(`api/session/DeleteSessionCollectReplacement/${id}`)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  rejectSession(session) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .post("api/session/RejectSession", session)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getSessionsCalendar2(clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetSessionsCalendar2/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  editSessionTime(data) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .put("api/session/EditSessionTime", data)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  editSessionPos(data) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .put("api/session/EditSessionPos", data)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  loadCompetencyCheckProgress(clientId, chartMaxDate = null) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetCompetencyCheckCaregiversCharts/${clientId}/${chartMaxDate}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getSessionPrintExtraInfo(sessionId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetSessionPrintExtraInfo/${sessionId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  editSessionDriveTime(data) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .put("api/session/EditSessionDriveTime", data)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getSessionDetailedForSign(token) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetSessionDetailedForSign/${token}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getCaregiverCollectionData(date, clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetCaregiverCollectionData/${date}/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  saveCaregiverCollectionData(caregiverDataCollection) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .post("api/session/SaveCaregiverCollectionData", caregiverDataCollection)
        .then(() => resolve())
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getCaregiverCollectionDataForCalendar(clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetCaregiverCollectionDataForCalendar/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  deleteSign(id) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .delete(`api/session/DeleteSign/${id}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getSessionsCalendar3(clientId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetSessionsCalendar3/${clientId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getClientBehaviorChartValuesWeek(clientId, behaviorId, dateStart, dateEnd) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetClientBehaviorChartValuesWeek/${clientId}/${behaviorId}/${dateStart}/${dateEnd}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getClientReplacementChartValuesWeek(clientId, replacementId, dateStart, dateEnd) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetClientReplacementChartValuesWeek/${clientId}/${replacementId}/${dateStart}/${dateEnd}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  markAssessmentAsBilled(assessmentId) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/MarkAssessmentAsBilled/${assessmentId}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  getUnitsByWeek(clientId, date, userName) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/GetUnitsByWeek/${clientId}/${date}/${userName}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  checkMatchingPercentaje(id) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .get(`api/session/CheckMatchingPercentaje/${id}`)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  },

  checkMatchingPercentajeString(obj) {
    return new Promise((resolve, reject) => {
      Vue.axios
        .post(`api/session/CheckMatchingPercentajeString`, obj)
        .then(response => resolve(response.data))
        .catch(error => reject(error.response.data || error.message));
    });
  }
};
