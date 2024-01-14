const pendingChartContainer = document.querySelector("#pending-chart");
const pendingPercentage = pendingChartContainer.dataset.percent;

var pendingChartOptions = {
    series: [pendingPercentage],
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
var pendingChart = new ApexCharts(pendingChartContainer, pendingChartOptions);
pendingChart.render();