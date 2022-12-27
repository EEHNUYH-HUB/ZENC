<template>
  <div class="card">
    <div class="card-header p-3">

      <h6 class="mb-0">강의 정보</h6>
      <div class="d-flex">
        <div class="mb-0 text-sm p font-weight-bold widget-calendar-day">
          주차 정보를 볼수 있습니다.
        </div>

      </div>

    </div>
    <div class="card-body p-3 pt-0">
      <div class="row">
        <NumberCard class="mt-4" suffix="%" css="text-success" :count="comprehensiveStatus" title="종합현황" :key="index"
          subtitle="진행평균" />
        <template v-for="data, index in classStatus" :key="index">
          <NumberCard v-if="data.currentfewweeks" class="mt-4" suffix="주차" :count="data.currentfewweeks"
            :title="data.subjectname" :subtitle="data.curriculumname" />

          <NumberCard v-else class="mt-4" css="text-primary" suffix="완료" :count="data.currentfewweeks"
            :title="data.subjectname" subtitle="강의종료" />
        </template>

      </div>

    </div>
  </div>
  <div class="card mt-4">
    <div class="card-header p-3">

      <h6 class="mb-0">강의 점수</h6>
      <div class="d-flex">
        <div class="mb-0 text-sm p font-weight-bold widget-calendar-day">
          강의별 점수를 확인 할수 있습니다.
        </div>

      </div>

    </div>
    <div class="card-body p-3 pt-0">

      <div class="row">
        <div class="col col-12 mt-4 mt-lg-0">
          <MNBarChartVue height="340" ref="barChart" />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue';
import MNBarChartVue from '@/components/MNBarChart.vue';
import NumberCard from "@/components/NumberCard.vue";
import moment from 'moment';
import MNClient from '@/minisoft/MNClient.js';
import {useStore} from 'vuex';

let classStatus = ref();
let barChart = ref();
let comprehensiveStatus = ref();

const store = useStore();

onMounted(async()=>{
  
  var year = '';
  var sem = '';
 
  

  var dt = await MNClient.ExecDataTable("CLASS", "CLASSSEMESTERINFO", null);
  if (dt) {
    for (var i in dt) {
      var item = dt[i];
      if (item.col_status == "ST010102") {
        year = item.col_year;
        sem = item.col_semester;
        break;
      }
    }
  }

  classStatus.value = await MNClient.ExecDataTable("STD", "CLASSPROGRESSSTATUS", { PYEAR: parseInt(year), PSEMESTER: parseInt(sem), PNUMBER:store.state.user.STUDENT_NUMBER});

  
  
  let wholeCurrentWeek = 0;
  let wholeMaxWeek = 0;

  classStatus.value.forEach((data) => {
    if (data.currentfewweeks) {
      wholeCurrentWeek += data.currentfewweeks - 1;
    }
    else {
      wholeCurrentWeek += data.maxfewweeks;
    }
    wholeMaxWeek += data.maxfewweeks;
  });

  comprehensiveStatus.value = parseInt(wholeCurrentWeek / wholeMaxWeek * 100);
  let tmpBarChart = {
    labels: [],
    datasets: {
      label: '총 점수',
      data: [],
      maxValue: 10
    },
  };

  tmpBarChart.maxValue = 100;
  classStatus.value.forEach((data)=>{

    let param = {
      classId: data.pk_class_id,
      studentId: store.state.user.STUDENT_NUMBER
    }

    MNClient.ExecDataTable("STD","STUDYSTATUS",param).then(result =>{
      let attendanceTotal = 15;
      let attitudeTotal = 20;
      let quizTotal = 0;
      if(result){
        result.forEach((item) =>{
          attendanceTotal += item.attendancescore;
          attitudeTotal += item.attitudescore;
          quizTotal += item.quizscore;
        });
        tmpBarChart.labels.push(data.subjectname);
        tmpBarChart.datasets.data.push(result[0].examscore + attendanceTotal + attitudeTotal + quizTotal);
      }

      barChart.value.DrawChart(tmpBarChart);
    });
  });
})
</script>