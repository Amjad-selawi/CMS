const onHoldChartContainer = document.querySelector("#onhold-chart");
const onHoldPercentage = onHoldChartContainer.dataset.percent;

var onHoldChartOptions = {
    series: [onHoldPercentage],
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
var onHoldChart = new ApexCharts(onHoldChartContainer, onHoldChartOptions);
onHoldChart.render();