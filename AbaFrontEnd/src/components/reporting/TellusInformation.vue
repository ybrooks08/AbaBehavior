<template>
  <v-layout row wrap>
    <v-flex xs12 class="no-print">
      <v-card>
        <v-toolbar dark class="secondary" fluid dense>
          <v-toolbar-title>Tellus Information</v-toolbar-title>
        </v-toolbar>
        <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
        <v-card-text class="pa-1">
          <form-wizard @onNextStep="nextStep" @onPreviousStep="previousStep" ref="wizard" :disabled="loading || !validForm" :loading="loading">
            <tab-content title="Date Selecting" :selected="true" key="step1" id="step1">
                <v-form ref="form" autocomplete="off" v-model="validForm">
            <v-layout row wrap>
              <v-flex xs12>
                <date-picker-menu :isLarge="true" :isDark="false" :btnColor="'primary'" :disabled="loading" v-model="datePickerModel" />
              </v-flex>
              <v-flex md12>                
              </v-flex>
            </v-layout>
          </v-form>
            </tab-content>
            <tab-content title="Visits to Update"> 
                <p>Matching results among tellus and local sessions</p>
            </tab-content>
            <tab-content title="Finishing Up">
                <p>Done.</p>                  
            </tab-content>  
          </form-wizard>
          
        </v-card-text>
        <v-card-actions>
          <small class="pl-4 grey--text">* Tellus credentials used are defined in settings section.</small>
          <v-spacer />
          <!-- <v-btn :disabled="loading || !validForm" :loading="loading" color="primary" @click="getTellusStepOne">Generate</v-btn> -->
        </v-card-actions>
      </v-card>
    </v-flex>
    <v-flex xs12 v-if="systemData.length > 0" class="no-print">
      <v-card>
        <v-toolbar dense dark class="secondary no-print">
          <v-toolbar-title>Visits to Update</v-toolbar-title>
          <!-- <v-spacer />
          <v-btn dark icon @click="print">
            <v-icon>fa-print</v-icon>
          </v-btn> -->
        </v-toolbar>
        <v-card-text class="pa-0 print-full-width">
          <table v-if="systemData.length > 0" class="v-datatable v-table theme--light print-font-small">
            <thead>
              <tr>
                <th class="text-xs-center py-0 px-1" colspan="7" style="border-right: 1px solid rgba(0,0,0,.12);">System Sessions</th>
                <th class="text-xs-center py-0 px-1" colspan="7">Tellus Sessions</th>
              </tr>
              <tr>
                <!--System -->
                <th class="text-xs-left py-0 px-1">User</th>
                <th class="text-xs-left py-0 px-1">Client/Code</th>
                <th class="text-xs-left py-0 px-1">Date</th>
                <th class="text-xs-left py-0 px-1">Start / End</th>
                <th class="text-xs-left py-0 px-1">Type</th>
                <th class="text-xs-left py-0 px-1">Pos</th>
                <th class="text-xs-left py-0 px-1" style="border-right: 1px solid rgba(0,0,0,.12);">Units</th> 
              </tr>            
            </thead>
            <tbody>
              <tr v-for="(r, indexOne) in systemData" :key="('system'+indexOne)">
                 <!--System -->
                <td class="px-1 text-truncate">
                  <strong>{{ r.userFullname }}</strong>                  
                </td>
                <td class="px-1 text-truncate">
                  <strong>{{ r.clientFullname }}</strong>
                  <br />
                  {{ r.code }}
                </td>
                <td class="px-1">{{ r.sessionStart | moment("MM/DD/YYYY") }}</td>
                <!-- <td class="px-1">{{ r.sessionStart }}</td> -->
                <td class="hidden-sm-and-down px-1 text-truncate">
                  <v-icon color="green" small>fa-sign-in-alt</v-icon>
                  <!-- {{ r.sessionStart | moment("LT") }} -->
                  <span v-show = "r.edit !== 'sessionStart'">
                    <label @dblclick = "r.edit = 'sessionStart'"> {{ r.sessionStart | moment("LT") }}</label>
                  </span>
                    <input name="sessionStart" 
                        v-show = "r.edit == 'sessionStart'" 
                        v-model = "r.sessionStart" 
                        v-on:blur= "updateItemHour" 
                        @keyup.enter = "r.edit=true">
                  <br />
                  <v-icon color="red" small>fa-sign-out-alt</v-icon>
                  <!-- {{ r.sessionEnd | moment("LT") }} -->
                  <span v-show = "r.edit !== 'sessionEnd'">
                    <label @dblclick = "r.edit = 'sessionEnd'"> {{ r.sessionEnd | moment("LT") }}</label>
                  </span>
                    <input name="sessionEnd" 
                        v-show = "r.edit == 'sessionEnd'" 
                        v-model = "r.sessionEnd" 
                        v-on:blur= "updateItemHour" 
                        @keyup.enter = "r.edit=true">
                </td>
                <td class="hidden-sm-and-down px-1">{{ r.sessionType }}</td>
                <td class="hidden-sm-and-down px-1">{{ r.pos }}</td>
                <td class="px-1" style="border-right: 1px solid rgba(0,0,0,.12);">
                  <strong>
                    <v-icon small>fa-star</v-icon>
                    {{ r.totalUnits.toLocaleString() }}
                  </strong>
                  <br />
                  <v-icon small>fa-clock</v-icon>
                  <span v-show="!r.edit">{{(r.totalUnits / 4).toLocaleString()}}</span>
                  <input type="text" v-model="r.totalUnits" v-show="r.edit">
                  {{ (r.totalUnits / 4).toLocaleString() }}
                </td>
                <!--Tellus -->
                <td class="px-4" style="padding: 0!important;margin: 0!important;">
                    <table v-if="tellusData.length > 0" class="v-datatable v-table theme--light print-font-small">
                        <!-- <thead>              
                          <tr>                           
                            <th class="text-xs-left py-0 px-1">User</th>
                            <th class="text-xs-left py-0 px-1">Client/Code</th>
                            <th class="text-xs-left py-0 px-1">Date</th>
                            <th class="text-xs-left py-0 px-1">Start / End</th>
                            <th class="text-xs-left py-0 px-1">Type</th>
                            <th class="text-xs-left py-0 px-1">Pos</th>
                            <th class="text-xs-left py-0 px-1">Units</th>                               
                            <th class="text-xs-left py-0 px-1">Edit</th>                               
                          </tr>            
                        </thead> -->
                        <tbody>
                          <tr v-for="(k, index) in tellusData" :key="('tellus'+index)">
                            <div id="div-td" v-if="r.clientFullname === k.clientFullname.toLowerCase().split(' ').map((s) => s.charAt(0).toUpperCase() + s.substring(1)).join(' ') && r.userFullname === k.userFullname.toLowerCase().split(' ').map((s) => s.charAt(0).toUpperCase() + s.substring(1)).join(' ')">
                              <td class="px-4 text-truncate">
                                <strong>{{ k.userFullname }}</strong>                  
                              </td>
                              <td class="px-4 text-truncate">
                                <strong>{{ k.clientFullname }}</strong>
                                <!-- <br />
                                {{ k.code }} -->
                              </td>
                              <td>{{ k.sessionStart | moment("MM/DD/YYYY") }}</td>
                              <!-- <td>{{ k.sessionStart }}</td> -->
                              <td>
                                <v-icon color="green" small>fa-sign-in-alt</v-icon>
                                <!-- <span v-show="r.sessionStart.seconds(0).milliseconds(0).toISOString()==k.sessionStart.seconds(0).milliseconds(0).toISOString()">{{ k.sessionStart | moment("LT") }}</span>
                                <span v-show="r.sessionStart.seconds(0).milliseconds(0).toISOString()!=k.sessionStart.seconds(0).milliseconds(0).toISOString()" style="color: red;">{{ k.sessionStart | moment("LT") }}</span> -->
                                {{ k.sessionStart | moment("LT") }}
                                <br />
                                <v-icon color="red" small>fa-sign-out-alt</v-icon>
                                {{ k.sessionEnd | moment("LT") }}
                              </td>
                              <td>{{ k.sessionType }}</td>
                              <!-- <td>{{ k.pos }}</td> -->
                              <!-- <td>
                                <strong>
                                  <v-icon small>fa-star</v-icon>
                                  {{ k.totalUnits.toLocaleString() }}
                                </strong>
                                <br />
                                <v-icon small>fa-clock</v-icon>
                                {{ (k.totalUnits / 4).toLocaleString() }}
                              </td> -->
                              <td>
                                <input
                                      :id="`id-${index}`"
                                      class="form-check-input"
                                      type="radio"
                                      :name="`tellusData[${index}][clientFullname]`"
                                      v-if="r.clientFullname === k.clientFullname.toLowerCase().split(' ').map((s) => s.charAt(0).toUpperCase() + s.substring(1)).join(' ') && r.userFullname === k.userFullname.toLowerCase().split(' ').map((s) => s.charAt(0).toUpperCase() + s.substring(1)).join(' ')"
                                      value=""                                      
                                  >

                                  <label
                                      :for="`id-${index}`"
                                      class="form-check-label"
                                  ></label>                              
                              </td>                                            
                            </div>
                          </tr>
                        </tbody>
                    </table>                
                </td>
                <td class="hidden-sm-and-down px-1">
                    <button v-on:click="removeElement(systemData[indexOne])">Remove</button>                    
               </td>                
              </tr>
            </tbody>            
          </table>
        </v-card-text>
      </v-card>
    </v-flex>    
  </v-layout>
</template>

<script>
import userApi from "@/services/api/UserServices";
// import clientApi from '@/services/api/ClientServices';
import tellusApi from "@/services/api/TellusServices";
import sessionServicesApi from "@/services/api/SessionServices";
import {FormWizard, TabContent} from 'vue-step-wizard'
import 'vue-step-wizard/dist/vue-step-wizard.css'

export default {
  data() {
    return {
      loading: false,
      required: (value) => !!value || "This field is required.",
      validForm: false,
      datePickerModel: {
        start: this.$moment().subtract(1, "month").startOf("month").format("YYYY-MM-DDTHH:mm"),
        end: this.$moment().subtract(1, "month").endOf("month").format("YYYY-MM-DDTHH:mm")
      },
      users: [],
      systemData: [],
      tellusData: [],
      dataBySession: [],
      currentStep: 1,
      userId: null,
      actualClientName: null,
      processState: 0
    };
  },

  components: {
  FormWizard,
  TabContent
  },

  computed: {
    totalUnits() {
      return this.systemData.map((a) => a.totalUnits).reduce((a, b) => a + b);
    }
  },

  mounted() {
    this.loadUsers();
  },

  methods: {
    dateSelected(range) {
      this.serviceLog.from = range.from;
      this.serviceLog.to = range.to;
    },

    previousStep: function(){
        this.currentStep--;
        alert('previousStep');
    },

    nextStep: function(){
      let buttons = document.querySelector(".step-footer");
      buttons.style.display = "none";
      switch (this.currentStep) {
        case 1:
          this.getTellusStepOne(); 
          //buttons.style.display = "block";         
          break;
        case 2:
          if(this.processState == 1){
            this.saveData();
            //buttons.style.display = "block"; 
          }          
          break;
        default:
          break;
      }
      this.currentStep++;
    },

    editPerson: function(person){
          this._originalPerson = Object.assign({}, person);
          person.edit = true;
    },

    cancelPerson: function(person){
          Object.assign(person, this._originalPerson);
          person.edit = false;
    },
    removeElement: function (value) {
      var index = this.systemData.indexOf(value);
      this.systemData.splice(index, 1);
    },
    updateItemHour (e) {
      console.info(e.target.value);
      console.info(this.systemData);
      /*this.dataBySession = [];
      this.tellusData.forEach((e) => { 
        if(e.clientFullname == clientFullname){
           this.dataBySession.push(e);
        }       
      });
      return this.dataBySession;*/
    },

    async loadUsers() {
      this.users = [];
      this.loading = true;
      try {
        this.users = await userApi.getUsersCanCreateSessions();
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async getTellusStepOne() {
      try {
        this.loading = true;
        this.gottenData = [];
        this.systemData = [];
        this.tellusData = [];
        let gottenData = await tellusApi.GetTellusStepOne(this.datePickerModel.start, this.datePickerModel.end/*, this.user, this.pass*/);
        let systemData = gottenData["system_visits"];
        let tellusData = gottenData["tellus_visits"];
        ///console.info(gottenData["system_visits"]);
        if (systemData.length == 0) {
          this.$toast.info("No data");        
          return;
        }
        //console.info("this.systemData");
        systemData.forEach((e) => {
          e.sessionStart = this.$moment(e.sessionStart).local();
          e.sessionEnd = this.$moment(e.sessionEnd).local();
          /*tellusData.forEach((o) => {
          //console.info(e.sessionStart.seconds(0).milliseconds(0).toISOString());
         
          
          if((e.clientFullname === o.clientFullname.toLowerCase().split(' ').map((s) => s.charAt(0).toUpperCase() + s.substring(1)).join(' ')) && (e.userFullname === o.userFullname.toLowerCase().split(' ').map((s) => s.charAt(0).toUpperCase() + s.substring(1)).join(' '))){
            o.sessionStart = this.$moment(o.sessionStart).local();
            o.sessionEnd = this.$moment(o.sessionEnd).local();
            // console.info("iguales y con diferencia de hora");
            // console.info(e.clientFullname);
            // console.info(o.clientFullname.toLowerCase().split(' ').map((s) => s.charAt(0).toUpperCase() + s.substring(1)).join(' '));
            // console.info(e.userFullname);
            // console.info(o.userFullname.toLowerCase().split(' ').map((s) => s.charAt(0).toUpperCase() + s.substring(1)).join(' '));
            e.sessionStart = this.$moment(e.sessionStart).local();
            e.sessionEnd = this.$moment(e.sessionEnd).local();
            o.clientFullname = o.clientFullname.toLowerCase().split(' ').map((s) => s.charAt(0).toUpperCase() + s.substring(1)).join(' ');
            o.userFullname = o.userFullname.toLowerCase().split(' ').map((s) => s.charAt(0).toUpperCase() + s.substring(1)).join(' ');            
            //this.tellusData.push(o);
          }
          });*/
          this.systemData.push(e);
          //console.info(e.sessionStart.seconds(0).milliseconds(0).toISOString());
          
        });
        
        tellusData.forEach((e) => {
          //console.info(this.$moment(e.sessionStart).local());
          e.sessionStart = this.$moment(e.sessionStart).local();
          e.sessionEnd = this.$moment(e.sessionEnd).local();
          e.clientFullname = e.clientFullname.toLowerCase().split(' ').map((s) => s.charAt(0).toUpperCase() + s.substring(1)).join(' ');
          e.userFullname = e.userFullname.toLowerCase().split(' ').map((s) => s.charAt(0).toUpperCase() + s.substring(1)).join(' ');
          this.tellusData.push(e);
        });
        this.processState = 1;
        let buttons = document.querySelector(".step-footer");
        buttons.style.display = "block";
        //console.info(this.tellusData);
      } catch (error) {
        //this.$toast.error(error.response.data || error.message);
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async saveData() {      
      this.loading = true;
      try {
        for (let index = 0; index < this.systemData.length; index++) {
          let s1 = null;
          let s2 = null; 
          for (let ind = 0; ind < this.tellusData.length; ind++) {
            if(this.systemData[index].edit){
              s1 = this.$moment(this.systemData[index].sessionStart).local();
              s2 = this.$moment(this.systemData[index].sessionEnd).local();
            }
            //Se toma el primero que 
            else if(this.tellusData[ind].clientFullname === this.systemData[index].clientFullname && this.tellusData[ind].userFullname === this.systemData[index].userFullname)
            {
              //let s1 = this.$moment(`${this.orgTimeStart.format("MM/DD/YYYY")} ${this.timeStart}`);              
              //let s2 = this.$moment(`${this.orgTimeEnd.format("MM/DD/YYYY")} ${this.timeEnd}`);
              s1 = this.tellusData[ind].sessionStart;
              s2 = this.tellusData[ind].sessionEnd;
              break;
            }         
          }
          console.info("s1");
          console.info(s1);
          let data = {
            sessionId: this.systemData[index].sessionId,
            start: s1,
            end: s2
          };
          await sessionServicesApi.editSessionTime(data);
          this.removeElement(this.systemData[index]);
        }
        this.$toast.success("Sessions saved successful.");
        
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    print() {
      window.print();
    }
  }
};
</script>
