const stoppedCyclesChartContainer = document.querySelector("#stoppedCycles-chart");
const stoppedCyclesPercentage = stoppedCyclesChartContainer.dataset.percent;

var stoppedCyclesChartOptions = {
    series: [stoppedCyclesPercentage],
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
var stoppedCyclesChart = new ApexCharts(stoppedCyclesChartContainer, stoppedCyclesChartOptions);
stoppedCyclesChart.render();