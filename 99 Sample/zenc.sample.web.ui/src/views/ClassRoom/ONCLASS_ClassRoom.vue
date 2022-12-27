<template>
  <div class="card">
    <div class="card-body p-3 pt-4">
      <StdClassRoom v-if="isStudent" :scheduleList="scheduleList" :scheduleLength="scheduleLength" />
      <ProClassRoom v-if="isProfessor" :scheduleList="scheduleList" :scheduleLength="scheduleLength"
        @updated="UpdateScheduleList" />
    </div>
  </div>
</template>

<script setup>
import { onMounted, reactive, ref } from 'vue';
import { useRoute } from 'vue-router'
import { CheckIsStudent, CheckIsProfessor } from '@/minisoft/MNCommon.js'
import MNClient from '@/minisoft/MNClient';
import StdClassRoom from './Components/StdClassRoom.vue';
import ProClassRoom from './Components/ProClassRoom.vue';
import { useStore } from "vuex"

const route = useRoute();

let scheduleList = reactive([]);
let scheduleLength = ref(0);

let isStudent = ref(false);
let isProfessor = ref(false);

onMounted(async()=>{
  isStudent.value = CheckIsStudent();
  isProfessor.value = CheckIsProfessor();
  
  var store = useStore()
  store.state.navigatorArray = new Array;
  store.state.currentName = "";
  const classDT = await MNClient.ExecDataTable("CLASS", "CLASSDTL", { PID: parseInt(route.query.id) });
   if (classDT) {
     var classDtl = classDT[0];
     store.state.navigatorArray.push({ Path: 'Dashboard', DisplayName: "대쉬보드" })
     store.state.navigatorArray.push({ Path: 'LECTURELIST', Query: { id: classDtl.pk_class_schedule_id, year: classDtl.col_year, semester: classDtl.col_semester }, DisplayName: classDtl.col_year + "년 " + classDtl.col_semester + "학기" })
     store.state.currentName = classDtl.subjectname;

   }
  UpdateScheduleList();
})

function UpdateScheduleList() {
  
  MNClient.ExecDataTable("CLASS", "CLASSSCHEDULE", { CLASSID: parseInt(route.query.id),PDATE:new Date() }).then((results) => {

    
    results.sort((a,b) =>{
      if(a.col_few_weeks < b.col_few_weeks){
        return -1;
      }
      if(a.col_few_weeks > b.col_few_weeks){
        return 1;
      }

      return 0;
    });

    Object.assign(scheduleList, results);
    
    scheduleLength.value = scheduleList.length;
  });
}


</script>