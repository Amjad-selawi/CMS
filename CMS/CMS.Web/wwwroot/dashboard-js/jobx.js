const chartContainer = document.querySelector("#accepted-chart");
const percantage = chartContainer.dataset.percent;

var options = {
  series: [percantage],
  chart: { type: "radialBar", width: 105, sparkline: { enabled: true } },
  dataLabels: { enabled: false },
  plotOptions: {
    radialBar: {
      hollow: { margin: 0, size: "70%" },
      track: { margin: 1 },
      dataLabels: {
        show: true,
        name: { show: false },
        value: { show: true, fontSize: "16px", fontWeight: 600, offsetY: 8 },
      },
    },
  }
};  
var chart = new ApexCharts(chartContainer, options);
chart.render();
  