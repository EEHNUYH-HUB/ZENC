<template>

  <div class="card border-dashed border-1 text-center h-100">
    <div class="card-body position-relative z-index-1 d-flex flex-column">
      <h6 class="text-start">{{ title }}</h6>

      <div class="chart">
        <canvas :id="id" class="chart-canvas" :height="height"></canvas>
        <p class="class-progress">{{progress}}%</p>
        <p class="guage-start">0</p>
        <p class="guage-end">100</p>
      </div>

    </div>
  </div>
</template>

<script setup>
import Chart from "chart.js/auto";
import {defineExpose, ref} from '@vue/runtime-core';

const props = defineProps({
 id: {
    type: String,
    default: "half-doughnut-chart",
  },
  title: {
    type: String,
    default: "",
  },
  height: {
    type: String,
    default: "300px",
  },
});

let progress = ref(0);

function DrawChart(propChart){
  let ctx = document.getElementById(props.id).getContext("2d");

  progress.value = propChart.datasets[0].needleValue;
 
  let chartStatus = Chart.getChart(props.id);
  if (chartStatus != undefined) {
    chartStatus.destroy();
  }
  
  new Chart(ctx, {
    type: "doughnut",
    data: {
      labels: propChart.labels,
      datasets: propChart.datasets,
    },
    options: {
      rotation: -90,
      circumference: 180,
      cutout: '80%',
      plugins: [{
        id: 'arrow',
      }]
    },
    plugins:[{
      afterDraw: chart => {
        var needleValue = chart.config.data.datasets[0].needleValue;
        var dataTotal = chart.config.data.datasets[0].data.reduce((a, b) => a + b, 0);
        var angle = Math.PI + (1 / dataTotal * needleValue * Math.PI);
        var ctx = chart.ctx;
        var cw = chart.canvas.offsetWidth;
        var ch = chart.canvas.offsetHeight;
        var cx = cw / 2;
        var cy = ch - 80;

        ctx.translate(cx, cy);
        ctx.rotate(angle);
        ctx.beginPath();
        ctx.moveTo(0, -3);
        ctx.lineTo(ch - 200, 0);
        ctx.lineTo(0, 2);
        ctx.fillStyle = '#C1E823';
        ctx.fill();
        ctx.rotate(-angle);
        ctx.translate(-cx, -cy);
        ctx.beginPath();
        ctx.arc(cx, cy, 5, 0, Math.PI * 2);
        ctx.fill();
      }
    }]
  });
};

defineExpose({
  DrawChart
});

</script>

<style scoped>
.class-progress{
  position: absolute;
  top:80%;
  left: 50%;
  transform: translate(-50%, 0);
  font-weight: bold;
  color:black;
}

.guage-start{
  position:absolute;
  top:80%;
  left:10%;
}

.guage-end{
  position:absolute;
  top:80%;
  right:8%;
}
</style>