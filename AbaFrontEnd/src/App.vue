<template>
  <div id="app">
    <router-view />
    <vue-ins-progress-bar></vue-ins-progress-bar>
  </div>
</template>

<script>
// let isSafari = /^((?!chrome|android).)*safari/i.test(navigator.userAgent);

export default {
  mounted() {
    this.$insProgress.finish();
  },
  created() {
    this.$insProgress.start();

    this.$router.beforeEach((to, from, next) => {
      // if (isSafari) {
      //   alert("This application does not run correctly in Safari Browser. Please use an alternative browser like Chrome, Edge or Firefox.");
      //   return;
      // }
      this.$insProgress.start();
      next();
    });

    // eslint-disable-next-line no-unused-vars
    this.$router.afterEach((to, from) => {
      this.$insProgress.finish();
    });
  }
};
</script>

<style>
.no-wrap {
  text-overflow: ellipsis;
  white-space: nowrap;
  overflow: hidden;
}

.table-print {
  width: 100%;
}

.table-print,
.table-print td,
.table-print th {
  border: 1px solid grey;
  border-collapse: collapse;
  font-family: "Roboto Condensed", Arial, Helvetica, sans-serif;
  font-size: 12px;
  vertical-align: top;
  padding: 2px;
}

.condensed {
  font-family: "Roboto Condensed", sans-serif !important;
}

.table-print td small {
  font-weight: bold;
}

.table-print td span {
  padding-left: 5px;
}

.table-horizontal {
  width: 100%;
  border-collapse: collapse;
}

.table-horizontal tr {
  border-bottom: 1px dotted rgb(207, 207, 207) !important;
}

@keyframes pulse {
  0% {
    box-shadow: 0 0 0 0px rgba(255, 0, 0, 0.5);
  }
  100% {
    box-shadow: 0 0 0 15px rgba(255, 0, 0, 0);
  }
}

.pulse {
  animation: pulse 1s infinite;
}

@media print {
  * {
    background: transparent !important;
    color: #000 !important;
    box-shadow: none !important;
    text-shadow: none !important;
  }

  .no-page-break {
    page-break-inside: avoid !important;
  }

  .pagebreak {
    /* clear: both; */
    break-after: page;
  }

  .pagebreak-before {
    /* clear: both; */
    break-before: page;
  }

  .no-print {
    display: none !important;
  }

  .print-full-width {
    width: 100%;
  }

  .print-font {
    font-size: 11px;
  }

  main {
    margin: 0px !important;
    padding: 0px !important;
  }

  .print-area {
    background-color: red !important;
    width: 100% !important;
    margin: 0px !important;
    padding: 0px !important;
  }

  .print-borders .flex {
    border-top: #000 solid 1px;
  }

  .print-borders .flex:nth-last-child(-n + 2) {
    border-bottom: #000 solid 1px;
  }

  .print-font-small * {
    font-size: 11px !important;
  }

  @page {
    margin: 0.3cm;
  }

  thead.report-header {
    display: table-header-group;
  }

  tfoot.report-footer {
    display: table-footer-group;
  }

  table.report-container {
    page-break-after: auto;
  }

  .highcharts-exporting-group {
    display: none !important;
  }
}
</style>
