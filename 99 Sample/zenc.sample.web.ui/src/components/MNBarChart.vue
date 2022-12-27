<template>
  <div class="card border-dashed border-1 text-center h-100">
    <div class="card-body position-relative z-index-1 d-flex flex-column">
      <h6 class="text-start">{{ title }}</h6>
      <div class="chart">
        <canvas :id="id" class="chart-canvas" :height="height"></canvas>
      </div>
    </div>
  </div>
</template>

<script setup>
import Chart from "chart.js/auto";

const props = defineProps({
 id: {
    type: String,
    default: "bar-chart",
  },
  title: {
    type: String,
    default: "",
  },
  height: {
    type: String,
    default: "300px",
  }
});

function DrawChart(chart){
  let ctx = document.getElementById(props.id).getContext("2d");

  let chartStatus = Chart.getChart(props.id);
  if (chartStatus != undefined) {
    chartStatus.destroy();
  }

  new Chart(ctx, {
    type: "bar",
    data: {
      labels: chart.labels,
      datasets: [
        {
          label: chart.datasets.label,
          weight: 5,
          borderWidth: 0,
          borderRadius: 4,
          backgroundColor: "#17c1e8",
          data: chart.datasets.data,
          fill: false,
          maxBarThickness: 35,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        legend: {
          display: false,
        },
      },
      scales: {
        y: {
          grid: {
            drawBorder: false,
            display: true,
            drawOnChartArea: true,
            drawTicks: false,
            borderDash: [5, 5],
          },
          ticks: {
            display: true,
            padding: 10,
            color: "#9ca2b7",
          },
          suggestedMax:chart.maxValue,
        },
        x: {
          grid: {
            drawBorder: false,
            display: false,
            drawOnChartArea: true,
            drawTicks: true,
          },
          ticks: {
            display: true,
            color: "#9ca2b7",
            padding: 10,
          },
        },
      },
    },
  });
};

defineExpose({
  DrawChart
});

</script>
