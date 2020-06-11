<template>
  <session-print :sessionId="activeSessionId"></session-print>
</template>

<script>
//import sessionServicesApi from "@/services/api/SessionServices";
//import masterTableApi from "@/services/api/MasterTablesServices";
import SessionPrint from "@/components/shared/SessionPrint";

export default {
  components: {
    SessionPrint
  },

  computed: {
    activeSessionId() {
      return this.$store.getters.activeSessionId;
    }
    //    hasLeadSign() {
    //      return this.sessionPrint.userId !== this.sessionExtraInfo.lead.userId;
    //    }
  },

  data() {
    return {
      loadingSession: false
      //      sessionPrint: {
      //        clientName: null,
      //        sessionStart: this.$moment(),
      //        sessionEnd: this.$moment(),
      //        userFullname: null,
      //        userRol: null,
      //        totalUnits: 0,
      //        clientCode: null,
      //        pos: null,
      //        posNum: 0,
      //        sessionType: null,
      //        sessionTypeNum: 0,
      //        service: null,
      //        serviceDescription: null,
      //        driveTime: 0,
      //        caregiver: null,
      //        caregiverType: null,
      //        caregiverNote: null,
      //        caregiverTraining: {
      //          caregiverTrainingObservationFeedback: false,
      //          caregiverTrainingParentCaregiverTraining: false,
      //          caregiverTrainingCompetencyCheck: false,
      //          caregiverTrainingOther: null,
      //          CaregiverTrainingSummary: null
      //        },
      //        supervisionNote: {
      //          isDirectSession: false,
      //          workWith: 0,
      //          briefObservation: false,
      //          briefReplacement: false,
      //          briefGeneralization: false,
      //          briefBCaBaTraining: false,
      //          briefInService: false,
      //          briefInServiceSubject: null,
      //          briefOther: false,
      //          briefOtherDescription: null
      //        },
      //        sessionNote: {
      //          riskBehavior: null
      //        }
      //      },
      //      collection: {
      //        collectBehaviors: [],
      //        collectReplacements: []
      //      },
      //      sessionSupervisionWorkWithCodes: [],
      //      sessionExtraInfo: {
      //        lead: {
      //          userId: null
      //        }
      //      }
    };
  }

  //  mounted() {
  //    if (!this.activeSessionId) this.close();
  //    this.loadSessionSupervisionWorkWithCodes();
  //    this.loadSessionData();
  //  },

  //  methods: {
  //    async loadSessionData() {
  //      try {
  //        this.loadingSession = true;
  //
  //        let sessionPrint = await sessionServicesApi.getSessionForPrint(this.activeSessionId);
  //        let d1 = this.$moment(sessionPrint.sessionStart).local();
  //        let d2 = this.$moment(sessionPrint.sessionEnd).local();
  //        sessionPrint.sessionStart = d1;
  //        sessionPrint.sessionEnd = d2;
  //        this.sessionPrint = sessionPrint;
  //        if (this.sessionPrint.sessionTypeNum === 3) {
  //          this.sessionSupervisionWorkWithCodes.forEach(c => {
  //            const a = (c.value & this.sessionPrint.supervisionNote.workWith) != 0;
  //            c.active = a;
  //          });
  //          this.sessionSupervisionWorkWithCodes = this.sessionSupervisionWorkWithCodes.filter(w => w.active);
  //        }
  //        this.collection.collectBehaviors = await sessionServicesApi.getCollectBehaviors(this.activeSessionId);
  //        this.collection.collectReplacements = await sessionServicesApi.getCollectReplacements(this.activeSessionId);
  //        this.sessionExtraInfo = await sessionServicesApi.getSessionPrintExtraInfo(this.activeSessionId);
  //      } catch (error) {
  //        this.$toast.error(error.message || error);
  //      } finally {
  //        this.loadingSession = false;
  //      }
  //    },
  //
  //    async loadSessionSupervisionWorkWithCodes() {
  //      try {
  //        this.sessionSupervisionWorkWithCodes = await masterTableApi.getSessionSupervicionWorkWithCodes();
  //      } catch (error) {
  //        this.$toast.error(error.message || error);
  //      }
  //    },
  //
  //    getTotalBehaviors(b) {
  //      if (b.noData) return "No data collected";
  //      if (!b.behavior.isPercent) return b.total;
  //      let total = b.total;
  //      let completed = b.completed != null ? b.completed : 0;
  //      let percent = total == 0 ? 0 : (completed / total) * 100;
  //      return `${percent.toFixed(0)}%`;
  //    },
  //
  //    getTotalReplacement(c) {
  //      if (c.noData) return "No data collected";
  //      let total = c.total;
  //      let completed = c.completed;
  //      let percent = total == 0 ? 0 : (completed / total) * 100;
  //      return `${percent.toFixed(0)}%`;
  //    },
  //
  //    breakLine(s) {
  //      return !s ? null : s.replace(/\n/g, "<br/>");
  //    }
  //  }
};
</script>
