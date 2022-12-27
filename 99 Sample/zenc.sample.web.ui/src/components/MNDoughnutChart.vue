<template>
  <div class="card z-index-2 text-center">
    <div class="p-3 pb-0 card-header">
      <h6>{{ title }}</h6>
    </div>
    <div class="card-body align-items-center d-flex justify-content-center">
      <div class="chart" :style="{height: canvasHeight + 'px', width: canvasHeight + 'px'}">
        <canvas :id="id" class="chart-canvas" ></canvas>
      </div>
      <p class="class-score">{{score}}</p>
    </div>
  </div>
</template>

<script setup>
import Chart from "chart.js/auto";
import {defineExpose, ref} from '@vue/runtime-core';

const props = defineProps({
 id: {
    type: String,
    default: "doughnut-chart",
  },
  title: {
    type: String,
    default: "",
  },
  canvasHeight: {
    type: String,
    default: "300px",
  },
  score:{
    type: Number,
    default: 0
  }
});

function DrawChart(propChart){
  let ctx = document.getElementById(props.id).getContext("2d");

  let chartStatus = Chart.getChart(props.id);
  if (chartStatus != undefined) {
    chartStatus.destroy();
  }
  
  new Chart(ctx, {
    type: "doughnut",
    data: {
      labels: propChart.labels,
      datasets: propChart.datasets,
    }
  });
};

defineExpose({
  DrawChart
});

</script>

<style scoped>
.class-score{
  position: absolute;
  top:63%;
  left: 50%;
  transform: translate(-50%, -50%);
  font-weight: bold;
  font-size:2rem;
  color:black;
}
</style>