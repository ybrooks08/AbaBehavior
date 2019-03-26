<template>
  <v-menu :close-on-content-click="false" :close-on-click="true" v-model="showDatePicker" transition="slide-y-transition" :disabled="disabled">

    <v-btn slot="activator" :class="btnColor" :style="[marginLeftStyle, marginBottomStyle]" :dark="isDark" :large="isLarge" :disabled="disabled">
      <v-icon left>fa-calendar-alt</v-icon>
      {{btnText}}
    </v-btn>

    <v-toolbar light class="teal">
      <v-toolbar-title class="white--text">Select date</v-toolbar-title>
      <v-spacer />
      <v-select solo hide-details style="min-width: 300px;" :items="items" :value="predefValue" @input="onChangeRange" />
    </v-toolbar>

    <v-card>
      <v-card-text class="pa-0">
        <vc-date-picker mode="range" show-popover="false" show-caps is-inline is-double-paned :value='value' :from-page.sync="myFromPage" :to-page.sync="myToPage" @input="manuallyChange">
        </vc-date-picker>
      </v-card-text>
    </v-card>

  </v-menu>
</template>

<script>
export default {
  props: {
    value: {},
    btnColor: String,
    pickerStartInit: String,
    pickerEndInit: String,
    initialValue: String,
    disabled: Boolean,
    isDark: {
      type: Boolean,
      default: true,
    },
    isLarge: {
      type: Boolean,
      default: false,
    },
    leftMargin: {
      type: Boolean,
      default: true,
    },
    extraBottomMargin: {
      type: Boolean,
      default: false,
    },
  },

  data() {
    return {
      showDatePicker: false,
      marginLeftStyle: {
        'margin-left': this.leftMargin ? '33px' : '0'
      },
      marginBottomStyle: {
        'margin-bottom': this.extraBottomMargin ? '25px' : '0'
      },
      myFromPage: {
        month: this.$moment(this.value.start).month() + 1,
        year: this.$moment(this.value.start).year(),
      },
      myToPage: null,

      items: [
        {
          text: 'Fixed',
          value: 'Fixed',
          from: null,
          to: null,
        },
        { divider: true, inset: false },
        {
          text: 'Yesterday',
          value: 'Yesterday',
          from: this.$moment().subtract(1, 'days').format('YYYY-MM-DDTHH:mm'),
          to: this.$moment().subtract(1, 'days').format('YYYY-MM-DDTHH:mm'),
        },
        {
          text: 'Last 7 days',
          value: 'Last 7 days',
          from: this.$moment().subtract(7, 'days').format('YYYY-MM-DDTHH:mm'),
          to: this.$moment().subtract(1, 'days').format('YYYY-MM-DDTHH:mm'),
        },
        {
          text: 'Last 30 days',
          value: 'Last 30 days',
          from: this.$moment().subtract(30, 'days').format('YYYY-MM-DDTHH:mm'),
          to: this.$moment().subtract(1, 'days').format('YYYY-MM-DDTHH:mm'),
        },
        {
          text: 'Last 90 days',
          value: 'Last 90 days',
          from: this.$moment().subtract(90, 'days').format('YYYY-MM-DDTHH:mm'),
          to: this.$moment().subtract(1, 'days').format('YYYY-MM-DDTHH:mm'),
        },
        { divider: true, inset: false },
        {
          text: 'WTD',
          value: 'WTD',
          from: this.$moment().startOf('isoWeek').format('YYYY-MM-DDTHH:mm'),
          to: this.$moment().format('YYYY-MM-DDTHH:mm'),
        },
        {
          text: 'Last week',
          value: 'Last week',
          from: this.$moment().subtract(1, 'weeks').startOf('isoWeek').format('YYYY-MM-DDTHH:mm'),
          to: this.$moment().subtract(1, 'weeks').endOf('isoWeek').format('YYYY-MM-DDTHH:mm'),
        },
        {
          text: 'Last weekend',
          value: 'Last weekend',
          from: this.$moment().subtract(1, 'weeks').startOf('isoWeek').add(5, 'days').format('YYYY-MM-DDTHH:mm'),
          to: this.$moment().subtract(1, 'weeks').startOf('isoWeek').add(6, 'days').format('YYYY-MM-DDTHH:mm'),
        },
        { divider: true, inset: false },
        {
          text: 'MTD',
          value: 'MTD',
          from: this.$moment().startOf('month').format('YYYY-MM-DDTHH:mm'),
          to: this.$moment().format('YYYY-MM-DDTHH:mm'),
        },
        {
          text: 'Last month',
          value: 'Last month',
          from: this.$moment().subtract(1, 'month').startOf('month').format('YYYY-MM-DDTHH:mm'),
          to: this.$moment().subtract(1, 'month').endOf('month').format('YYYY-MM-DDTHH:mm'),
        },
        {
          text: 'Last 3 months',
          value: 'Last 3 months',
          from: this.$moment().subtract(3, 'month').startOf('month').format('YYYY-MM-DDTHH:mm'),
          to: this.$moment().subtract(1, 'month').endOf('month').format('YYYY-MM-DDTHH:mm'),
        },
        { divider: true, inset: false },
        {
          text: 'Last year',
          value: 'Last year',
          from: this.$moment().subtract(1, 'year').startOf('year').format('YYYY-MM-DDTHH:mm'),
          to: this.$moment().subtract(1, 'year').endOf('year').format('YYYY-MM-DDTHH:mm'),
        },
        {
          text: 'Last fiscal',
          value: 'Last fiscal',
          from: this.GetFiscalYearStart(true).format('YYYY-MM-DDTHH:mm'),
          to: this.GetFiscalYearEnd().format('YYYY-MM-DDTHH:mm'),
        },
        {
          text: 'Calendar YTD',
          value: 'Calendar YTD',
          from: this.$moment().startOf('year').format('YYYY-MM-DDTHH:mm'),
          to: this.$moment().format('YYYY-MM-DDTHH:mm'),
        },
        {
          text: 'Fiscal YTD',
          value: 'Fiscal YTD',
          from: this.GetFiscalYearStart().format('YYYY-MM-DDTHH:mm'),
          to: this.$moment().format('YYYY-MM-DDTHH:mm'),
        },
      ],
      // items: [
      //   {
      //     text: 'Fixed',
      //     value: 'Fixed',
      //     from: null,
      //     to: null,
      //   },
      //   { divider: true, inset: false },
      //   {
      //     text: 'Yesterday',
      //     value: 'Yesterday',
      //     from: this.$moment().subtract(1, 'days').format('YYYY-MM-DD'),
      //     to: this.$moment().subtract(1, 'days').format('YYYY-MM-DD'),
      //   },
      //   {
      //     text: 'Last 7 days',
      //     value: 'Last 7 days',
      //     from: this.$moment().subtract(7, 'days').format('YYYY-MM-DD'),
      //     to: this.$moment().subtract(1, 'days').format('YYYY-MM-DD'),
      //   },
      //   {
      //     text: 'Last 30 days',
      //     value: 'Last 30 days',
      //     from: this.$moment().subtract(30, 'days').format('YYYY-MM-DD'),
      //     to: this.$moment().subtract(1, 'days').format('YYYY-MM-DD'),
      //   },
      //   {
      //     text: 'Last 90 days',
      //     value: 'Last 90 days',
      //     from: this.$moment().subtract(90, 'days').format('YYYY-MM-DD'),
      //     to: this.$moment().subtract(1, 'days').format('YYYY-MM-DD'),
      //   },
      //   { divider: true, inset: false },
      //   {
      //     text: 'WTD',
      //     value: 'WTD',
      //     from: this.$moment().startOf('isoWeek').format('YYYY-MM-DD'),
      //     to: this.$moment().format('YYYY-MM-DD'),
      //   },
      //   {
      //     text: 'Last week',
      //     value: 'Last week',
      //     from: this.$moment().subtract(1, 'weeks').startOf('isoWeek').format('YYYY-MM-DD'),
      //     to: this.$moment().subtract(1, 'weeks').endOf('isoWeek').format('YYYY-MM-DD'),
      //   },
      //   {
      //     text: 'Last weekend',
      //     value: 'Last weekend',
      //     from: this.$moment().subtract(1, 'weeks').startOf('isoWeek').add(5, 'days').format('YYYY-MM-DD'),
      //     to: this.$moment().subtract(1, 'weeks').startOf('isoWeek').add(6, 'days').format('YYYY-MM-DD'),
      //   },
      //   { divider: true, inset: false },
      //   {
      //     text: 'MTD',
      //     value: 'MTD',
      //     from: this.$moment().startOf('month').format('YYYY-MM-DD'),
      //     to: this.$moment().format('YYYY-MM-DD'),
      //   },
      //   {
      //     text: 'Last month',
      //     value: 'Last month',
      //     from: this.$moment().subtract(1, 'month').startOf('month').format('YYYY-MM-DD'),
      //     to: this.$moment().subtract(1, 'month').endOf('month').format('YYYY-MM-DD'),
      //   },
      //   {
      //     text: 'Last 3 months',
      //     value: 'Last 3 months',
      //     from: this.$moment().subtract(3, 'month').startOf('month').format('YYYY-MM-DD'),
      //     to: this.$moment().subtract(1, 'month').endOf('month').format('YYYY-MM-DD'),
      //   },
      //   { divider: true, inset: false },
      //   {
      //     text: 'Last year',
      //     value: 'Last year',
      //     from: this.$moment().subtract(1, 'year').startOf('year').format('YYYY-MM-DD'),
      //     to: this.$moment().subtract(1, 'year').endOf('year').format('YYYY-MM-DD'),
      //   },
      //   {
      //     text: 'Last fiscal',
      //     value: 'Last fiscal',
      //     from: this.GetFiscalYearStart(true).format('YYYY-MM-DD'),
      //     to: this.GetFiscalYearEnd().format('YYYY-MM-DD'),
      //   },
      //   {
      //     text: 'Calendar YTD',
      //     value: 'Calendar YTD',
      //     from: this.$moment().startOf('year').format('YYYY-MM-DD'),
      //     to: this.$moment().format('YYYY-MM-DD'),
      //   },
      //   {
      //     text: 'Fiscal YTD',
      //     value: 'Fiscal YTD',
      //     from: this.GetFiscalYearStart().format('YYYY-MM-DD'),
      //     to: this.$moment().format('YYYY-MM-DD'),
      //   },
      // ],
    };
  },

  computed: {
    predefValue() {
      const h = this.items.find(s => s.from === this.value.start && s.to === this.value.end)
      if (h) return h.text; else return 'Fixed';
    },
    btnText() {
      if (!this.value.start || !this.value.end) return 'NA';
      return (
        this.predefValue +
        ' | ' +
        (this.$moment(this.value.start).format('MM/DD/YYYY') === this.$moment(this.value.end).format('MM/DD/YYYY') ? this.$moment(this.value.start).format('MM/DD/YYYY') : this.$moment(this.value.start).format('MM/DD/YYYY') + ' - ' + this.$moment(this.value.end).format('MM/DD/YYYY'))
      );
    },
  },

  methods: {
    GetFiscalYearStart(last = false) {
      let year = new Date().getFullYear();
      let month = new Date().getMonth() + 1;
      let res = month > 6 ? new Date(year, 6, 1) : new Date(year - 1, 6, 1); //in javascript 6 is July  [January 0...11 december]
      return last ? this.$moment(res).subtract(1, 'year') : this.$moment(res);
    },

    GetFiscalYearEnd() {
      let year = new Date().getFullYear();
      let month = new Date().getMonth() + 1;
      let res = month > 5 ? new Date(year, 5, 30) : new Date(year - 1, 5, 30); //in javascript 6 is July  [January 0...11 december]
      return this.$moment(res);
    },

    onChangeRange(value) {
      let index = this.items.findIndex(o => o.value === value);
      let from = this.items[index].from;
      let to = this.items[index].to;
      if (!from || !to) return;

      let fromPage = this.$moment(this.items[index].from);
      let toPage = this.$moment(this.items[index].to);
      this.myFromPage = {
        month: fromPage.month() + 1,
        year: fromPage.year(),
      };
      this.myToPage = {
        month: toPage.month() + 1,
        year: toPage.year(),
      };

      this.value.start = from;
      this.value.end = to;
      this.submit();
    },

    manuallyChange(date) {
      console.log('yo');
      this.value.start = this.$moment(date.start).format('YYYY-MM-DDTHH:mm');
      this.value.end = this.$moment(date.end).format('YYYY-MM-DDTHH:mm');
      this.submit();
    },

    submit() {
      this.showDatePicker = false;
      this.$emit('input', this.value);
    },
  },
};
</script>