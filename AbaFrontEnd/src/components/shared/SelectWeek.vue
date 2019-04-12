<template>
  <!-- <div> -->
  <v-menu v-model="menuDate" max-width="290" full-width :close-on-content-click="true">
    <v-text-field box slot="activator" label="Select week" :value="dateRangeLabel" readonly></v-text-field>
    <v-date-picker v-model="dates" multiple @input="changed" show-week></v-date-picker>
  </v-menu>
  <!-- <vc-calendar :attributes="datesAttr"></vc-calendar> -->
  <!-- {{datesAttr}} -->
  <!-- </div> -->
</template>

<script>
export default {
  data() {
    return {
      dates: [],
      start: null,
      end: null,
      menuDate: false,
    }
  },

  computed: {
    dateRangeLabel() {
      return `From ${this.start.format('L')} to ${this.end.format('L')}`;
    },
    // datesAttr() {
    //   return [{ start: this.start, end: this.end }]
    // }
  },

  created() {
    this.getDates(this.$moment());
  },

  methods: {
    getDates(date) {
      this.dates = [];
      this.start = date.startOf('week').clone();
      this.end = date.endOf('week').clone();
      let d = this.start.clone();
      while (d <= this.end) {
        var day = d.format('YYYY-MM-DD');
        this.dates.push(day);
        d.add(1, 'days');
      }
      this.$emit('input', { start: this.start.format('YYYY-MM-DD'), end: this.end.format('YYYY-MM-DD') });
    },

    changed(days) {
      const last = days.reduceRight(a => a);
      this.getDates(this.$moment(last));
    }
  }
}
</script>
