<template>
  <highcharts :options="options"></highcharts>
</template>

<script>
export default {
  name: 'CompetencyCheckProgress',

  props: {
    options: {
      type: Object,
      default: function () {
        return { series: [] };
      },
    },
  },
};
</script>

<style scoped>
</style>