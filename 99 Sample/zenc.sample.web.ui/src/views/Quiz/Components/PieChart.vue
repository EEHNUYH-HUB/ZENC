<template>
    <div :class="Css ? Css :'card mb-2'">
        <div v-if="Title" class="p-3 pb-0 card-header">
            <div class="d-flex align-items-center">
                <h6 class="mb-0">{{ Title }} ({{TotalCnt}})</h6>
            </div>
        </div>
        <div class=" card-body">
            <div class=" row">
                <div class="text-center col col-lg-5" style="height:220px">
                    <div class="chart">
                        <canvas :id="CanvasID" class="chart-canvas" :height="CanvasHeight"></canvas>
                    </div>
                    <h4 class="font-weight-bold mt-n8">
                        <span>{{ CenterText1}}</span>
                        <span class="text-sm d-block text-body">{{CenterText2}}</span>
                    </h4>
                </div>
                <div class="col col-lg-7">
                    <div class="table-responsive" style="height:220px">
                        <table class="table mb-0 align-items-center">
                            <tbody>
                                <template v-for="item,index in RightPanelData" :key="index">
                                    <tr @click="ShowUserList(item)">
                                        <td>
                                            <div class="px-2 py-0 d-flex">
                                                <span class="badge bg-gradient-primary me-2"
                                                    :style="'background:'+BackgroundColor[index]+''">&nbsp;</span>
                                                <div class="d-flex flex-column justify-content-center">
                                                    <h6 v-if="item.IsOK" class="mb-0 text-sm text-danger">{{item.Title}}
                                                    </h6>
                                                    <h6 v-else class="mb-0 text-sm ">{{item.Title}}</h6>
                                                </div>
                                            </div>
                                        </td>
                                        <td class="text-sm text-center align-middle">
                                            <span class="text-xs font-weight-bold">{{item.Rate}}%</span>
                                        </td>
                                    </tr>
                                    <tr v-show="item.IsShow" v-for="sub,subIndex in item.SubItem" :key="subIndex">
                                        <td colspan="2">
                                            <div class="px-5 py-0 d-flex">
                                                <div class="px-2 d-flex flex-column justify-content-center">
                                                    <h6 class="mb-0 text-sm" style="font-size:10px">{{ sub }}</h6>
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                </template>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>


<script setup>

import { ref, onMounted } from 'vue'
import Chart from "chart.js/auto";
const Props = defineProps({
    CanvasID: { type: String },
    Labels: { type: Array },
    Data: { type: Array },
    Title: { type: String },
    CenterText1: { type: String },
    CenterText2: { type: String },
    TotalCnt: { type: String },
    Css: { type: String },
    RightPanelData:{type:Array}

})

const CanvasHeight = ref(196);
const BackgroundColor = ref([
    "#FF0080",
    "#A8B8D8",
    "#21d4fd",
    "#98ec2d",
    "white",
]);
const ShowUserList = (item) => { 
    item.IsShow = !item.IsShow;
    
}

onMounted(() => { 
    CanvasHeight.value = 196;

    var ctx1 = document.getElementById(Props.CanvasID).getContext("2d");

    let chartStatus = Chart.getChart(Props.CanvasID);
    
   
    if (chartStatus != undefined) {
        chartStatus.destroy();
   
    }

    new Chart(ctx1, {
        type: "doughnut",
        data: {
            labels: Props.Labels,
            datasets: [
                {
                    label: "Consumption",
                    weight: 9,
                    cutout: 90,
                    tension: 0.9,
                    pointRadius: 2,
                    borderWidth: 2,
                    backgroundColor: BackgroundColor.value,
                    data: Props.Data,
                    fill: true,
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
            interaction: {
                intersect: false,
                mode: "index",
            },
            scales: {
                y: {
                    grid: {
                        drawBorder: false,
                        display: false,
                        drawOnChartArea: false,
                        drawTicks: false,
                    },
                    ticks: {
                        display: false,
                    },
                },
                x: {
                    grid: {
                        drawBorder: false,
                        display: false,
                        drawOnChartArea: false,
                        drawTicks: false,
                    },
                    ticks: {
                        display: false,
                    },
                },
            },
        },
    });
})
</script>