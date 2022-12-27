<template>
  <div class="card">
    <div class="card-header p-3">
      <h6 class="mb-0">강의 통계</h6>
      <div class="d-flex">
        <div class="mb-0 text-sm p font-weight-bold widget-calendar-day">
          주차 정보 및 강의 현황을 볼수 있습니다.
        </div>

      </div>
    </div>
    <div class="card-body p-3 pt-0">
      <div class="row">
        <NumberCard class="mt-4" suffix="%" css="text-success" :count="comprehensiveStatus" title="종합현황"
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
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue';
import NumberCard from "@/components/NumberCard.vue";
import moment from 'moment';
import MNClient from '@/minisoft/MNClient.js';
import {useStore} from 'vuex';

let classStatus = ref();
let doughnutChart = ref();
const store = useStore();
const comprehensiveStatus = ref(null);
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


  classStatus.value = await MNClient.ExecDataTable("PRO", "CLASSPROGRESSSTATUS", { PYEAR: parseInt(year), PSEMESTER: parseInt(sem), PNUMBER:store.state.user.PROFESSOR_NUMBER});

  

  let wholeCurrentWeek = 0;
  let wholeMaxWeek = 0;
  
  classStatus.value.forEach((data) => {
    if (data.currentfewweeks) {
      wholeCurrentWeek += data.currentfewweeks-1;
    }
    else {
      wholeCurrentWeek += data.maxfewweeks;
    }
    wholeMaxWeek += data.maxfewweeks;
  });
  
  comprehensiveStatus.value = parseInt(wholeCurrentWeek / wholeMaxWeek * 100);
})
</script>