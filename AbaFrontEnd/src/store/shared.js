/* eslint no-shadow: 0 */
/* eslint no-param-reassign: 0 */

export default {
  state: {
    states: [
      { value: "AL", text: "Alabama" },
      { value: "AK", text: "Alaska" },
      { value: "AS", text: "American Samoa" },
      { value: "AZ", text: "Arizona" },
      { value: "AR", text: "Arkansas" },
      { value: "CA", text: "California" },
      { value: "CO", text: "Colorado" },
      { value: "CT", text: "Connecticut" },
      { value: "DE", text: "Delaware" },
      { value: "DC", text: "District Of Columbia" },
      { value: "FM", text: "Federated States Of Micronesia" },
      { value: "FL", text: "Florida" },
      { value: "GA", text: "Georgia" },
      { value: "GU", text: "Guam" },
      { value: "HI", text: "Hawaii" },
      { value: "ID", text: "Idaho" },
      { value: "IL", text: "Illinois" },
      { value: "IN", text: "Indiana" },
      { value: "IA", text: "Iowa" },
      { value: "KS", text: "Kansas" },
      { value: "KY", text: "Kentucky" },
      { value: "LA", text: "Louisiana" },
      { value: "ME", text: "Maine" },
      { value: "MH", text: "Marshall Islands" },
      { value: "MD", text: "Maryland" },
      { value: "MA", text: "Massachusetts" },
      { value: "MI", text: "Michigan" },
      { value: "MN", text: "Minnesota" },
      { value: "MS", text: "Mississippi" },
      { value: "MO", text: "Missouri" },
      { value: "MT", text: "Montana" },
      { value: "NE", text: "Nebraska" },
      { value: "NV", text: "Nevada" },
      { value: "NH", text: "New Hampshire" },
      { value: "NJ", text: "New Jersey" },
      { value: "NM", text: "New Mexico" },
      { value: "NY", text: "New York" },
      { value: "NC", text: "North Carolina" },
      { value: "ND", text: "North Dakota" },
      { value: "MP", text: "Northern Mariana Islands" },
      { value: "OH", text: "Ohio" },
      { value: "OK", text: "Oklahoma" },
      { value: "OR", text: "Oregon" },
      { value: "PW", text: "Palau" },
      { value: "PA", text: "Pennsylvania" },
      { value: "PR", text: "Puerto Rico" },
      { value: "RI", text: "Rhode Island" },
      { value: "SC", text: "South Carolina" },
      { value: "SD", text: "South Dakota" },
      { value: "TN", text: "Tennessee" },
      { value: "TX", text: "Texas" },
      { value: "UT", text: "Utah" },
      { value: "VT", text: "Vermont" },
      { value: "VI", text: "Virgin Islands" },
      { value: "VA", text: "Virginia" },
      { value: "WA", text: "Washington" },
      { value: "WV", text: "West Virginia" },
      { value: "WI", text: "Wisconsin" },
      { value: "WY", text: "Wyoming" }
    ],
    races: [
      "Hispanic or Latino",
      "American Indian or Alaska Native",
      "Asian",
      "Black or African American",
      "Native Hawaiian or Other Pacific Islander",
      "White"
    ],
    genders: ["Male", "Female"],
    languages: ["English", "Spanish"],
    notAllowed: [
      {
        place: "school",
        words: [
          "Home",
          "Day Care",
          "Community",
          "Park",
          "Mother",
          "Father",
          "Grandmother",
          "Video Game",
          "Homework",
          "Feedback",
          "Supervision",
          "Happy",
          "Sad",
          "Anger",
          "Frustrated",
          "Excited"

        ]
      },
      {
        place: "home",
        words: [
          "School",
          "Day Care",
          "Community",
          "Teacher",
          "Park",
          "Playground",
          "Homework",
          "Feedback",
          "Supervision",
          "Happy",
          "Sad",
          "Anger",
          "Frustrated",
          "Excited"
        ]
      },
      {
        place: "community",
        words: [
          "Home",
          "School",
          "Day Care",
          "Teacher",
          "Homework",
          "Academic activities",
          "Board game",
          "Feedback",
          "Day Care",
          "Supervision",
          "Happy",
          "Sad",
          "Anger",
          "Frustrated",
          "Excited"
        ]
      },
      {
        place: "other",
        words: [
          "Home",
          "School",
          "Community",
          "Mother",
          "Father",
          "Grandmother",
          "Homerwork",
          "Feedback",
          "Supervision",
          "Happy",
          "Sad",
          "Anger",
          "Frustrated",
          "Excited"
        ]
      }
    ]
  },
  mutations: {},
  actions: {},
  getters: {
    states: state => state.states,
    getStateByCode: (state) => (code) => {
      return state.states.find(st => st.value.toLowerCase() === code.toLowerCase()).text;
    },
    genders: state => state.genders,
    races: state => state.races,
    languages: state => state.languages,
    notAllowed: state => state.notAllowed
  }
};
