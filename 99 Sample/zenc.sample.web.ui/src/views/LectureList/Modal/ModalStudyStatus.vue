<template>
  <div class="modal" id="modal-data-upload" tabindex="-1" role="dialog">
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-header">
          {{className}} 학습 현황
        </div>
        <div class="modal-body">
          <div class="card">
            <div class="pt-3 pb-0 card-body">
              <div class="row">
                <label>1. 종합 점수</label>
                <div class="text-center">
                  <PieChart v-if="Data" :CanvasID="CanvasID" :Data="Data" Css="mb-0" :Labels="Labels"
                    :CenterText1="TotalScore" CenterText2="총점" :RightPanelData="RightPanelData">
                  </PieChart>

                </div>

              </div>
            </div>
          </div>
          <div class="card mt-3">
            <div class="pt-3 pb-0 card-body">
              <div class="row">
                <label>2. 주차별 점수</label>



                <div class="mt-2">
                  <table>
                    <tbody>
                      <tr>
                        <td>
                          <div class="px-2 py-0 d-flex">
                            <span class="badge  me-2" style="background:#11cdef">&nbsp;</span>
                            <div class="d-flex flex-column justify-content-center">
                              <h6 class="mb-0 text-xs ">통과/득점</h6>
                            </div>
                          </div>
                        </td>
                        <td>
                          <div class="px-2 py-0 d-flex">
                            <span class="badge  me-2" style="background:#f5365c">&nbsp;</span>
                            <div class="d-flex flex-column justify-content-center">
                              <h6 class="mb-0 text-xs ">결석/미통과/감점(-2)</h6>
                            </div>
                          </div>
                        </td>
                        <td>
                          <div class="px-2 py-0 d-flex">
                            <span class="badge  me-2" style="background:#FBD38D">&nbsp;</span>
                            <div class="d-flex flex-column justify-content-center">
                              <h6 class="mb-0 text-xs ">지각/감점(-1)</h6>
                            </div>
                          </div>
                        </td>

                      </tr>
                    </tbody>
                  </table>
                </div>


                <div class="text-center px-4">

                  <div class="row table-responsive">
                    <!-- 현황 테이블 -->
                    <table class="table align-items-center align-middle text-center text-xs">
                      <thead>
                        <th class="text-uppercase text-secondary text-xs font-weight-bolder opacity-7"
                          style="width:100px">목록</th>
                        <th class="text-uppercase text-secondary text-xs font-weight-bolder opacity-7"
                          style="width:100px">점수</th>
                        <th class="text-uppercase text-secondary text-xs font-weight-bolder opacity-7"
                          v-for="(i,index) in maxweek" :key="i">{{statusList[index].col_few_weeks}}</th>
                      </thead>
                      <tbody>

                        <tr>
                          <td class="align-middle text-center ">시험</td>
                          <td>{{examScore}}/50</td>
                          <td v-for="i in maxweek" :key="i" class='normal'></td>
                        </tr>
                        <tr>
                          <td class="align-middle text-center ">퀴즈</td>
                          <td>{{quizScore}}/15</td>
                          <td v-for="(i,index) in maxweek" :key="i"
                            :class="GetScoreClass('quiz', statusList[index].quizscore)">
                            {{statusList[index].quizscore != 0 ?statusList[index].quizscore : ''}}</td>
                        </tr>
                        <tr>
                          <td>출석</td>
                          <td>{{attendanceScore}}/15</td>
                          <td v-for="(i,index) in maxweek" :key="i"
                            :class="GetScoreClass('attend', statusList[index].attendancescore)">{{
                            statusList[index].attendancescore != 0 ? statusList[index].attendancescore : ''}}</td>
                        </tr>
                        <tr>
                          <td class="align-middle text-center ">학습태도</td>
                          <td>{{attitudeScore}}/20</td>
                          <td v-for="(i,index) in maxweek" :key="i"
                            :class="GetScoreClass('attitude', statusList[index].attitudescore)">{{
                            statusList[index].attitudescore != 0 ?statusList[index].attitudescore: ''}}</td>
                        </tr>
                      </tbody>
                    </table>
                  </div>

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
import { defineProps, defineEmits, onMounted, ref, reactive } from 'vue';

import MNClient from '../../../minisoft/MNClient';
import {useStore} from 'vuex';
import PieChart from '@/views/Quiz/Components/PieChart.vue'
import { Guid } from "@/minisoft/MNCommon"
const props = defineProps({
  classId: {
    type: Number
  },
  className:{
    type: String
  }
});

const store = useStore();


let maxweek = ref(0);
let statusList = reactive([]);
let attendanceScore = ref(15);
let attitudeScore = ref(20);
let quizScore = ref(0);
let examScore = ref(0);


const emit = defineEmits(['hide']);



const TotalScore = ref(null);
const Labels = ref(null);
const Data = ref(null);
const RightPanelData = ref(null);
const CanvasID = ref("canvasID_" + Guid());

onMounted(()=>{
  let param = {
    classId: props.classId,
    studentId: store.state.user.STUDENT_NUMBER
  }

  MNClient.ExecDataTable('STD', 'STUDYSTATUS', param).then((result) => {
    statusList.length = 0;
    Object.assign(statusList, result); 
    maxweek.value = result.length;

    for(let i = 0; i < maxweek.value; i++){
      attendanceScore.value += statusList[i].attendancescore;
      attitudeScore.value += statusList[i].attitudescore;
      quizScore.value += statusList[i].quizscore;
    }

    examScore.value = statusList[0].examscore;

    if(attendanceScore.value < 0){
      attendanceScore.value = 0;
    }

    if(attitudeScore.value < 0){
      attitudeScore.value = 0;
    }

    if(quizScore.value < 0){
      quizScore.value = 0;
    }

    TotalScore.value = attendanceScore.value + attitudeScore.value + examScore.value + quizScore.value;
    Labels.value = new Array;
    Labels.value.push('시험');
    Labels.value.push('퀴즈');
    Labels.value.push('출석');
    Labels.value.push('학습태도');
    Labels.value.push('감점');

    Data.value = new Array;
    Data.value.push(examScore.value);
    Data.value.push(quizScore.value);
    Data.value.push(attendanceScore.value);
    Data.value.push(attitudeScore.value);
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
        ritem.ID = parseInt(i) + 1;
        ritem.Title = ritem.ID + ". " + Labels.value[i] + " (" + Data.value[i] + "/" + total[i] + ")";

        ritem.Rate = Math.round(Data.value[i] * 100 / total[i]);
        ritem.IsOK = false;
        RightPanelData.value.push(ritem);
      }
    }
    
  });
});

function HideModal(){
  emit('hide');
}

function GetScoreClass(type, score){
  // if (type == 'quiz' && score == 0) {
  //   return 'normal';
  // } else if (type == 'quiz' && score >= 1) {
  //   return 'attendance';
  // } else

  if (type == 'quiz' && score >= 1) {
      return 'attendance';
  }  
  else if (score == -1) {
    return 'tardy';
  } else if (score == -2) {
    return 'absent';
  } else {
    return 'normal';
  }
}

</script>

<style scoped>





.modal {
  display: block;
  background:rgba(0,0,0,0.3);
}

.modal-dialog{
  max-width:1000px;
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

.status-desc{
  display: inline;
}

.status-color-pass{
  display: inline-block;
  height:20px;
  width:20px;
  background-color:#11cdef;
}

.status-color-fail{
  display: inline-block;
  height: 20px;
  width: 20px;
  background-color: #f5365c;
}

.status-color-tardy{
  display: inline-block;
  height: 20px;
  width: 20px;
  background-color: #FBD38D;
}

/* Scrollbar */
::-webkit-scrollbar {
  height: 10px;              /* height of horizontal scrollbar ← You're missing this */
  width: 4px;               /* width of vertical scrollbar */
  border: 1px solid #d5d5d5;
}
::-webkit-scrollbar-thumb {
  background-color: rgba(27, 27, 27, .4);
  border-radius: 10px;
}
::-webkit-scrollbar-track{
  background: transparent;
}

.absent{
  background-color:#f5365c;
  color:white;
  border-radius: 0.45rem;
}

.attendance{
  background-color:#11cdef;
  color:black;
  border-radius:0.45rem;
  
}

.tardy{
  background-color: #FBD38D;
  color:black;
  border-radius: 0.45rem;
}

.normal{
  background-color: white;
}

</style>