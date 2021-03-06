<template>
  <v-layout row wrap>
    <v-flex xs12 class="no-print">
      <v-card>
        <v-toolbar dark class="secondary" fluid dense>
          <v-toolbar-title>Tellus Information</v-toolbar-title>
        </v-toolbar>
        <v-progress-linear style="position: absolute;" v-show="loading" :indeterminate="true" class="ma-0"></v-progress-linear>
        <v-card-text class="pa-1">
          <v-form ref="form" autocomplete="off" v-model="validForm">
            <v-layout row wrap>
              <v-flex xs12>
                <br/>
                <date-picker-menu :isLarge="true" :isDark="false" :btnColor="'primary'" :disabled="loading" v-model="datePickerModel"/>
              </v-flex>
            </v-layout>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <small class="pl-4 grey--text">* Tellus credentials used are defined in "Tellus configuration" settings section.</small>
          <v-spacer />
          <v-btn v-if="!changed" :disabled="loading || !validForm" :loading="loading" color="primary" @click="getTellusStepOne">Search</v-btn>
          <v-btn v-if="changed" :disabled="!changed || !validForm" :loading="loading" color="primary" @click="saveData">Apply changes</v-btn>
        </v-card-actions>
      </v-card>
    </v-flex>
    <v-flex xs12 v-if="systemData.length > 0  || outOfTellus.length > 0 || justTellus.length > 0" class="no-print">
      <v-card no-body>
          <v-tabs card>
            <v-tab>Visits to Update</v-tab>
            <v-tab>Visits not present in Tellus</v-tab>
            <v-tab>Visits only present in Tellus</v-tab>
            <v-tab-item>
              <v-card>
                <v-toolbar dense dark class="secondary no-print">
                  <v-toolbar-title>Visits to Update</v-toolbar-title>
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
                                    <!-- <div id="div-td" v-if="r.clientFullname === k.clientFullname.toLowerCase().split(' ').map((s) => s.charAt(0).toUpperCase() + s.substring(1)).join(' ') && r.userFullname === k.userFullname.toLowerCase().split(' ').map((s) => s.charAt(0).toUpperCase() + s.substring(1)).join(' ')"> -->
                                    <div id="div-td" v-if="r.medicaidId === k.medicaidId && r.mpi === k.mpi && k.sessionStartDate  === r.sessionStartDate/* && r.userFullname.includes(k.userFullname)*/">
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
                                        <br/>
                                        <input
                                          :id="`id-${index}`"
                                          :name="k.medicaidId"
                                          type="radio"
                                          :value="k.sessionId"                                  
                                          @change="updateRadioValue(k)"
                                        />
                                        <!-- <label
                                              :for="`id-${index}`"
                                              class="form-check-label"
                                          ></label>                               -->
                                      </td>                                            
                                    </div>
                                  </tr>
                                </tbody>
                            </table>                
                        </td>
                        <td class="text-xs-center px-0">
                          <v-tooltip top>
                            <v-btn slot="activator" icon class="mx-0" @click="removeElement(systemData[indexOne], systemData)">
                              <v-icon color="error" small>fa-times-circle</v-icon>
                            </v-btn>
                            <span>Dismiss</span>
                          </v-tooltip>
                        </td>
                        <!-- <td class="hidden-sm-and-down px-1">
                            <button v-on:click="removeElement(systemData[indexOne], systemData)">Dismiss</button>                    
                      </td>                 -->
                      </tr>
                    </tbody>            
                  </table>
                </v-card-text>
              </v-card>
            </v-tab-item>
            <v-tab-item>
              <v-card>
                <v-toolbar dense dark class="secondary no-print">
                  <v-toolbar-title>Visits not present in Tellus</v-toolbar-title>                  
                </v-toolbar>
                <v-card-text class="pa-0 print-full-width">
                  <table v-if="outOfTellus.length > 0" class="v-datatable v-table theme--light print-font-small">
                    <thead>
                      <tr>
                        <th class="text-xs-center py-0 px-1" colspan="7">System Sessions</th>                        
                      </tr>
                      <tr>
                        <!--System -->
                        <th class="text-xs-left py-0 px-1">Matched</th>
                        <th class="text-xs-left py-0 px-1">User</th>
                        <th class="text-xs-left py-0 px-1">Client/Code</th>
                        <th class="text-xs-left py-0 px-1">Date</th>
                        <th class="text-xs-left py-0 px-1">Start / End</th>
                        <th class="text-xs-left py-0 px-1">Type</th>
                        <th class="text-xs-left py-0 px-1">Pos</th>
                        <th class="text-xs-left py-0 px-1">Units</th> 
                        <th class="text-xs-left py-0 px-1"></th> 
                      </tr>            
                    </thead>
                    <tbody>
                      <tr v-for="(r, indexOut) in outOfTellus" :key="('sysOut'+indexOut)">
                        <!--System -->
                        <td>
                          <!-- <v-chip v-if="r.matched" dark label :color='green'>Matched</v-chip> -->
                          <v-chip label :color="r.matched ? 'green' : 'red'" text-color="white">{{r.matched ? "Matched" : "Not matched"}}</v-chip>
                        </td>
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
                        <td class="px-1">
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
                        <td class="text-xs-center px-0">
                          <v-tooltip top>
                            <v-btn slot="activator" icon class="mx-0" @click="deleteSession(r.sessionId, indexOut)">
                            <!-- <v-btn slot="activator" icon class="mx-0" @click="deleteSession(r)"> -->
                              <v-icon color="grey" small>fa-trash</v-icon>
                            </v-btn>
                            <span>Delete this visit</span>
                          </v-tooltip>
                        </td> 
                        <td class="text-xs-center px-0">
                          <v-tooltip top>
                            <v-btn slot="activator" icon class="mx-0" @click="removeElement(outOfTellus[indexOut], outOfTellus)">
                              <v-icon color="error" small>fa-times-circle</v-icon>
                            </v-btn>
                            <span>Dismiss</span>
                          </v-tooltip>
                        </td>                       
                        <!-- <td class="hidden-sm-and-down px-1">
                            <button v-on:click="removeElement(outOfTellus[indexOut], outOfTellus)">Dismiss</button>                    
                        </td>                 -->
                      </tr>
                    </tbody>            
                  </table>
                </v-card-text>
              </v-card>
            </v-tab-item>
            <v-tab-item>
              <v-card>
                <v-toolbar dense dark class="secondary no-print">
                  <v-toolbar-title>Visits only present in Tellus</v-toolbar-title>                  
                </v-toolbar>
                <v-card-text class="pa-0 print-full-width">
                  <table v-if="justTellus.length > 0" class="v-datatable v-table theme--light print-font-small">
                    <thead>
                      <tr>
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
                        <th class="text-xs-left py-0 px-1">Units</th> 
                        <th class="text-xs-left py-0 px-1"></th> 
                      </tr>            
                    </thead>
                    <tbody>
                      <tr v-for="(r, indexOut) in justTellus" :key="('tellus'+indexOut)">
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
                          {{ r.sessionStart | moment("LT") }}
                          <!-- <span v-show = "r.edit !== 'sessionStart'">
                            <label @dblclick = "r.edit = 'sessionStart'"> {{ r.sessionStart | moment("LT") }}</label>
                          </span>
                            <input name="sessionStart" 
                                v-show = "r.edit == 'sessionStart'" 
                                v-model = "r.sessionStart" 
                                v-on:blur= "updateItemHour" 
                                @keyup.enter = "r.edit=true"> -->
                          <br />
                          <v-icon color="red" small>fa-sign-out-alt</v-icon>
                          {{ r.sessionEnd | moment("LT") }}
                          <!-- <span v-show = "r.edit !== 'sessionEnd'">
                            <label @dblclick = "r.edit = 'sessionEnd'"> {{ r.sessionEnd | moment("LT") }}</label>
                          </span>
                            <input name="sessionEnd" 
                                v-show = "r.edit == 'sessionEnd'" 
                                v-model = "r.sessionEnd" 
                                v-on:blur= "updateItemHour" 
                                @keyup.enter = "r.edit=true"> -->
                        </td>
                        <td class="hidden-sm-and-down px-1">{{ r.sessionType }}</td>
                        <td class="hidden-sm-and-down px-1">{{ r.pos }}</td>
                        <td class="px-1">
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
                        <!-- <td class="text-xs-center px-0">
                          <v-tooltip top>
                            <v-btn slot="activator" icon class="mx-0" @click="deleteSession(r.sessionId, indexOut)">
                              <v-icon color="grey" small>fa-trash</v-icon>
                            </v-btn>
                            <span>Delete this visit</span>
                          </v-tooltip>
                        </td>  -->
                        <!-- <td class="text-xs-center px-0">
                          <v-tooltip top>
                            <v-btn slot="activator" icon class="mx-0" @click="removeElement(justTellus[indexOut], justTellus)">
                              <v-icon color="error" small>fa-times-circle</v-icon>
                            </v-btn>
                            <span>Dismiss</span>
                          </v-tooltip>
                        </td>                       -->
                      </tr>
                    </tbody>            
                  </table>
                </v-card-text>
              </v-card>
            </v-tab-item>
          </v-tabs>
        </v-card>
    </v-flex>    
  </v-layout>
</template>

<script>
import userApi from "@/services/api/UserServices";
import tellusApi from "@/services/api/TellusServices";
import sessionServicesApi from "@/services/api/SessionServices";

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
      outOfTellus: [],
      justTellus: [],
      dataBySession: [],
      //processState: 0,
      changed: false
    };
  },

  watch:{
    'datePickerModel.start' (val) {      
      if (val) {
        this.systemData = [];
        this.outOfTellus = [];
        this.justTellus = [];
        this.changed = false;
      }
    }
  },

  /*components: {
  FormWizard,
  TabContent
  },*/

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

    editPerson: function(person){
          this._originalPerson = Object.assign({}, person);
          person.edit = true;
    },

    cancelPerson: function(person){
          Object.assign(person, this._originalPerson);
          person.edit = false;
    },
    removeElement: function (value, array) {
      var index = array.indexOf(value);
      array.splice(index, 1);
    },
    updateItemHour () {
      this.changed = true;
    },
    updateRadioValue(row){
      this.changed = true;
      var objIndex = this.tellusData.findIndex((obj => obj.sessionId == row.sessionId));
      this.tellusData[objIndex].difference = true;
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
        this.outOfTellus = [];
        this.justTellus = [];
        let gottenData = await tellusApi.GetTellusStepOne(this.datePickerModel.start, this.datePickerModel.end/*, this.user, this.pass*/);
        let systemData = gottenData["system_visits"];
        let tellusData = gottenData["tellus_visits"];
        let outOfTellus = gottenData["out_tellus"];
        let justTellus = gottenData["just_tellus"];
        if (systemData.length == 0 && outOfTellus.length == 0 && justTellus.length == 0) {
          this.$toast.info("No data");        
          return;
        }
        //console.info("this.systemData");
        if (systemData.length != 0) {
          systemData.forEach((e) => {
            e.sessionStart = this.$moment(e.sessionStart).local();
            e.sessionStartDate = this.$moment(e.sessionStart).format('MM/DD/YYYY');          
            e.sessionEnd = this.$moment(e.sessionEnd).local();
            e.sessionEndDate = this.$moment(e.sessionEnd).format('MM/DD/YYYY');
            this.systemData.push(e);
            //console.info(e.sessionStart.seconds(0).milliseconds(0).toISOString());
            
          });

          tellusData.forEach((e) => {
            //console.info(this.$moment(e.sessionStart).local());
            e.sessionStart = this.$moment(e.sessionStart).local();
            e.sessionStartDate = this.$moment(e.sessionStart).format('MM/DD/YYYY');
            e.sessionEnd = this.$moment(e.sessionEnd).local();
            e.sessionEndDate = this.$moment(e.sessionEnd).format('MM/DD/YYYY');
            e.clientFullname = e.clientFullname.toLowerCase().split(' ').map((s) => s.charAt(0).toUpperCase() + s.substring(1)).join(' ');
            e.userFullname = e.userFullname.toLowerCase().split(' ').map((s) => s.charAt(0).toUpperCase() + s.substring(1)).join(' ');
            this.tellusData.push(e);
            console.info(e);
          });
        }
        
        if (outOfTellus.length != 0) {
          outOfTellus.forEach((e) => {
            e.sessionStart = this.$moment(e.sessionStart).local();
            e.sessionEnd = this.$moment(e.sessionEnd).local();
            this.outOfTellus.push(e);
          });
        }
        
        if (justTellus.length != 0) {
          justTellus.forEach((e) => {
            e.sessionStart = this.$moment(e.sessionStart).local();
            e.sessionEnd = this.$moment(e.sessionEnd).local();
            this.justTellus.push(e);
          });
        }
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
        //Visits to update
        for (let index = 0; index < this.systemData.length; index++) {
          let s1 = null;
          let s2 = null;
          let haschanged = false;
          if(this.systemData[index].edit){
              s1 = this.$moment(this.systemData[index].sessionStart).local();
              s2 = this.$moment(this.systemData[index].sessionEnd).local();
              haschanged = true;
          }
          else { 
            for (let ind = 0; ind < this.tellusData.length; ind++) {
              //Se toma el primero que 
              //r.medicaidId === k.medicaidId && r.mpi === k.mpi && k.sessionStartDate  === r.sessionStartDate
              if(this.tellusData[ind].medicaidId === this.systemData[index].medicaidId && this.tellusData[ind].mpi === this.systemData[index].mpi 
                      && this.tellusData[ind].sessionStartDate === this.systemData[index].sessionStartDate && this.tellusData[ind].difference)
              {
                //let s1 = this.$moment(`${this.orgTimeStart.format("MM/DD/YYYY")} ${this.timeStart}`);              
                //let s2 = this.$moment(`${this.orgTimeEnd.format("MM/DD/YYYY")} ${this.timeEnd}`);
                s1 = this.tellusData[ind].sessionStart;
                s2 = this.tellusData[ind].sessionEnd;
                haschanged = true;
                break;
              }         
            }
          }
          if (haschanged) {
            let data = {
              sessionId: this.systemData[index].sessionId,
              start: s1,
              end: s2
            };
            await sessionServicesApi.matchingSessionTellus(data);
            this.removeElement(this.systemData[index], this.systemData);
          }
        }
        //Visits not present in Tellus
        for (let index = 0; index < this.outOfTellus.length; index++) {
          let s1 = null;
          let s2 = null;
          if(this.outOfTellus[index].edit){
            s1 = this.$moment(this.outOfTellus[index].sessionStart).local();
            s2 = this.$moment(this.outOfTellus[index].sessionEnd).local();

            let data = {
            sessionId: this.outOfTellus[index].sessionId,
            start: s1,
            end: s2
            };
            await sessionServicesApi.editSessionTime(data);
            this.removeElement(this.outOfTellus[index], this.outOfTellus);
          }      
        }
        this.$toast.success("Sessions saved successful.");
        this.changed = false;
      } catch (error) {
        this.$toast.error(error);
      } finally {
        this.loading = false;
      }
    },

    async deleteSession( sessionId, index ) {
      
      this.$confirm("This can't be undone. Do you really want to delete this Session?").then(async (res) => {
        if (res) {
          this.loading = true;          
          try {
            if (sessionId == null) return;
            await sessionServicesApi.deleteSession(sessionId);
            this.removeElement(this.outOfTellus[index], this.outOfTellus);
            this.$toast.success("Session removed successful.");
          } catch (error) {
            this.$toast.error(error.message || error);
          }finally {
            this.loading = false;
          }
        }
      });
    },

    print() {
      window.print();
    }
  }

};
</script>
