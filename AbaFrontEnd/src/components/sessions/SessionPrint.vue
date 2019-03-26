<template>
  <v-card>
    <v-card-text class="print-full-width print-font">
      <v-container fluid grid-list-sm pa-0 v-if="sessionDetailed">
        <v-layout row wrap>
          <v-flex xs6>
            <span class="headline">{{sessionDetailed.clientFullname}}</span>
          </v-flex>
          <v-flex xs6 class="text-xs-right">
            <span class="title grey--text text--darken-2">{{sessionDetailed.sessionStart | moment('LL')}}</span>
          </v-flex>
          <v-flex xs12>
            <v-divider></v-divider>
          </v-flex>
          <v-flex xs6>
            <v-layout row wrap>
              <v-flex class="body-2 text-xs-right" xs4>Provider:</v-flex>
              <v-flex xs8 v-if="session.user">{{session.user.firstname}} {{session.user.lastname}}</v-flex>
              <v-flex class="body-2 text-xs-right" xs4>Time IN:</v-flex>
              <v-flex xs8>
                <v-icon color="green" small>fa-sign-in-alt</v-icon>
                {{sessionDetailed.sessionStart | moment('LT')}}
              </v-flex>
              <v-flex class="body-2 text-xs-right" xs4>Time OUT:</v-flex>
              <v-flex xs8>
                <v-icon color="red" small>fa-sign-out-alt</v-icon>
                {{sessionDetailed.sessionEnd | moment('LT')}}
              </v-flex>
              <v-flex class="body-2 text-xs-right" xs4>Units:</v-flex>
              <v-flex xs8>
                <v-icon small>fa-star</v-icon>
                {{sessionDetailed.totalUnits.toLocaleString()}}
                <v-icon small>fa-clock</v-icon>
                {{(sessionDetailed.totalUnits / 4).toLocaleString()}}
              </v-flex>
            </v-layout>
          </v-flex>
          <v-flex xs6>
            <v-layout row wrap>
              <v-flex class="body-2 text-xs-right" xs4>Code:</v-flex>
              <v-flex xs8>{{sessionDetailed.clientCode}}</v-flex>
              <v-flex class="body-2 text-xs-right" xs4>Pos:</v-flex>
              <v-flex xs8>
                <span class="text-no-wrap text-truncate">{{sessionDetailed.pos}}</span>
              </v-flex>
              <v-flex class="body-2 text-xs-right" xs4>Session type:</v-flex>
              <v-flex xs8>{{sessionDetailed.sessionType}}</v-flex>
              <v-flex class="body-2 text-xs-right" xs4>Service:</v-flex>
              <v-flex xs8>{{sessionDetailed.hcpcs}} ({{sessionDetailed.description}})</v-flex>
            </v-layout>
          </v-flex>

          <!-- <v-subheader>Session notes</v-subheader> -->
          <v-flex xs12>
            <v-card flat>
              <v-divider></v-divider>
              <!-- <v-subheader>Session notes:</v-subheader> -->
              <v-card-text v-html="session.sessionNote.notes"></v-card-text>
            </v-card>
          </v-flex>
          <v-flex class="pt-5" text-xs-center xs6 v-if="session.user">
            <div v-if="!session.user.userSign">
              <div style="min-height: 100px;" class="mb-2"></div>
              <v-divider></v-divider>
              {{session.user.firstname}} {{session.user.lastname}}
            </div>
            <div v-else>
              <div style="height: 100px;" class="mb-2">
                <img style="height:100%" :src="session.user.userSign.sign">
              </div>
              <v-divider></v-divider>
              {{session.user.firstname}} {{session.user.lastname}}
            </div>
          </v-flex>
          <v-flex class="pt-5" text-xs-center xs6 v-if="hasLeadSign">
            <div v-if="!sessionExtraInfo.lead.userSign">
              <div style="min-height: 100px;" class="mb-2"></div>
              <v-divider></v-divider>
              {{sessionExtraInfo.lead.firstname}} {{sessionExtraInfo.lead.lastname}}
            </div>
            <div v-else>
              <div style="height: 100px;" class="mb-2">
                <img style="height:100%" :src="sessionExtraInfo.lead.userSign.sign">
              </div>

              <v-divider></v-divider>
              {{sessionExtraInfo.lead.firstname}} {{sessionExtraInfo.lead.lastname}}
            </div>
          </v-flex>
        </v-layout>
      </v-container>
    </v-card-text>
  </v-card>
</template>

<script>
import sessionServicesApi from '@/services/api/SessionServices';

export default {
  computed: {
    activeSessionId() {
      return this.$store.getters.activeSessionId;
    },
    hasLeadSign() {
      return this.session.user.userId !== this.sessionExtraInfo.lead.userId;
    }
  },

  data() {
    return {
      loadingSession: false,
      sessionDetailed: null,
      sessionExtraInfo: {
        lead: {
          userId: null,
        }
      },
      session: {
        sessionType: 0,
        sessionNote: {
          caregiverId: null,
          caregiverNote: null,
          notes: null,
        },
        user: {
          userId: null
        },
        sessionSupervisionNote: {
          workWith: 0,
        },
      },
    }
  },

  mounted() {
    if (!this.activeSessionId) this.close();
    //await this.loadCaregivers();
    this.loadSessionData();
  },

  methods: {
    async loadSessionData() {
      try {
        this.loadingSession = true;
        let sessionDetailed = await sessionServicesApi.getSessionDetailed(this.activeSessionId);
        let s1 = this.$moment(sessionDetailed.sessionStart).local();
        let s2 = this.$moment(sessionDetailed.sessionEnd).local();
        sessionDetailed.sessionStart = s1;
        sessionDetailed.sessionEnd = s2;
        this.sessionDetailed = sessionDetailed;
        this.session = await sessionServicesApi.getSession(this.activeSessionId);
        this.sessionExtraInfo = await sessionServicesApi.getSessionPrintExtraInfo(this.activeSessionId);
        console.log(this.sessionExtraInfo)
        console.log(this.session)
      } catch (error) {
        this.$toast.error(error.message || error);
      } finally {
        this.loadingSession = false;

      }
    },
  }

}
</script>
