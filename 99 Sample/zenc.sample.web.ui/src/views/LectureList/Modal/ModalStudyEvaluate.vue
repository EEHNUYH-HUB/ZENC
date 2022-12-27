<template>
  <div class="modal" id="modal-data-upload" tabindex="-1" role="dialog">
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-header">
          {{ ClassName }} 상세 평가
        </div>
        <div class="modal-body">
          <div class="card">
            <div class="pt-3 card-body">
              <div class="row">

                <label>1. {{ UserItem.col_name }}({{ UserItem.fk_student_number }})</label>

                <div class="text-center mt-3">
                  <UserImg :UserID="UserItem.fk_user_id" Width="128" Height="128" />
                </div>


              </div>
            </div>
          </div>


          <div class="card mt-3">
            <div class="pt-3 pb-0 card-body">
              <div class="row">
                <label>2. 종합 점수</label>
                <div class="text-center">
                  <!-- <MNDoughnutChart canvasHeight="300" :score="TotalScore" id="studyevaluate" ref="doughnutChart" /> -->
                  <PieChart v-if="Data" :CanvasID="CanvasID" :Data="Data" Css="mb-0" :Labels="Labels"
                    :CenterText1="TotalScore" CenterText2="총점" :RightPanelData="RightPanelData">
                  </PieChart>

                </div>

              </div>
            </div>
          </div>

        </div>
        <div class="modal-footer">
          <button class="btn btn-primary mb-0" @click="HideModal()">닫기</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import {ref, defineProps, defineEmits, onMounted} from 'vue';
import VsudAvatar from '../../../components/VsudAvatar.vue';
import MNDoughnutChart from '@/components/MNDoughnutChart.vue';
import UserImg from "@/views/Common/UserImg.vue";
import PieChart from '@/views/Quiz/Components/PieChart.vue'
import { Guid } from "@/minisoft/MNCommon"
const props = defineProps({
  UserItem:{
    type: Object
  },
  ClassName: {type:String}
 
});
// console.log(item)
// UserItem.fk_user_id
// userName.value = UserItem.col_name;
// studentNumber.value = UserItem.fk_student_number;
// //profileUri.value = null; // todo
// attendanceScore.value = UserItem.attendancescore;
// examScore.value = UserItem.examscore;
// attitudeScore.value = UserItem.attitudescore;
// quizScore.value = UserItem.quizscore;



const emit = defineEmits(['hide']);
const TotalScore = ref(null);
let doughnutChart = ref();
const Labels = ref(null);
const Data = ref(null);
const RightPanelData = ref(null);
const CanvasID = ref("canvasID_" + Guid());

onMounted(() => {


  TotalScore.value = props.UserItem.attendancescore + props.UserItem.attitudescore + props.UserItem.quizscore + props.UserItem.examscore;
  
  
  
  Labels.value = new Array;
  Labels.value.push('시험');
  Labels.value.push('퀴즈');
  Labels.value.push('출석');
  Labels.value.push('학습태도');
  Labels.value.push('감점');
  
  Data.value = new Array;
  Data.value.push(props.UserItem.examscore);
  Data.value.push(props.UserItem.quizscore);
  Data.value.push(props.UserItem.attendancescore);
  Data.value.push(props.UserItem.attitudescore);
  Data.value.push(100 - TotalScore.value);

  var total = new Array;
  total.push(50);
  total.push(15);
  total.push(15);
  total.push(20);
  RightPanelData.value = new Array();
  for (var i in Labels.value) {
    if (i < 4) {
      var ritem = new Object;
      ritem.ID = parseInt( i) + 1;
      ritem.Title = ritem.ID + ". " + Labels.value[i] + " (" + Data.value[i] + "/" + total [i]+")";

      ritem.Rate = Math.round( Data.value[i]*100 / total[i]);
      ritem.IsOK = false;
      RightPanelData.value.push(ritem);
    }
  }


  //console.log(Labels.value, Data.value)
  // let tmpDoughnutChart = {
  //   labels: ['시험', '퀴즈', '출석', '학습태도' ],
  //   datasets: [{
  //     label: '# of Votes',
  //     data: [props.UserItem.examscore, props.UserItem.quizscore, props.UserItem.attendancescore, props.UserItem.attitudescore, 100 - TotalScore.value],
  //     backgroundColor: [
  //       'rgb(255, 99, 132)',
  //       'rgb(54, 162, 235)',
  //       'rgb(255, 205, 86)',
  //       'rgb(129, 193, 71)',
  //       '#e9ecef'
  //     ],
  //     hoverOffset: 4
  //   }],
  //   options:{
  //     tooltips: {
  //       callbacks: {
  //         label: function(tooltipItem, data) {
  //           return data['datasets'][0]['data'][tooltipItem['index']];
  //         },
  //         afterLabel: function(tooltipItem, data) {
  //           var dataset = data['datasets'][0];
  //           var percent = Math.round((dataset['data'][tooltipItem['index']] / dataset["_meta"][0]['total']) * 100)
  //           return '(' + percent + '%)';
  //         }
  //       },
  //     }
  //   }
  // }
  // doughnutChart.value.DrawChart(tmpDoughnutChart);


});

function HideModal(){
  emit('hide');
}

</script>

<style scoped>
.modal {
  display: block;
  background:rgba(0,0,0,0.3);
}

.modal-dialog{
  max-width:750px;
}

.status-chart-section{
  position:relative;
}

.status-list{
  position: absolute;
  z-index: 9999;
  bottom: 10%;
  left: 65%;
}

</style>