<template>
  <div class="card widget-calendar h-100">
    <!-- Card header -->
    <div class="p-3 pb-0 card-header">
      <h6 class="mb-0">강의 일정</h6>
      <div class="d-flex">
        <div class="mb-0 text-sm p font-weight-bold widget-calendar-day">
          강의 일정을 볼수 있습니다.
        </div>
        
      </div>
    </div>
    <div class="p-3 card-body">
      <div :id="id" data-toggle="widget-calendar" class="mt-4"></div>
    </div>
  </div>
</template>

<script setup>
import { Calendar } from "@fullcalendar/core";
import dayGridPlugin from "@fullcalendar/daygrid";
import MNClient from "@/minisoft/MNClient";
import { GetDate ,GetUser } from "@/minisoft/MNCommon";
import {  onMounted, onUnmounted, ref } from 'vue'


const StrDate = ref("");
const props = defineProps({
  id: {
    type: Object
  }
});
const Data = ref(null);

const calendar = ref(null);
onMounted(async () => {

  var nowTime = new Date;
  StrDate.value  = GetDate(nowTime,"-")
 
  var user = GetUser();
  var scope = 'PRO'; 
  var number = "";
  if (user.ROLE == 'STUDENT') {
    scope = 'STD';
    number = user.STUDENT_NUMBER;
  } else if (user.ROLE == 'PROFESSOR') {
    scope = 'PRO';
    number = user.PROFESSOR_NUMBER;
  }
  Data.value = new Array;
  var dt = await MNClient.ExecDataTable(scope, "TODAYCLASS", { PNUMBER: number, PDATE: nowTime.getFullYear() + '%' });
  
  if (dt) {
    for (var i in dt) { 
      var item = dt[i];
      
      Data.value.push({
        title: item.subjectname,//+ "-" + item.curriculumanme,
        //title: item.curriculumanme,//+ "-" + item.curriculumanme,
        start: item.col_start_min_date,
        end: item.col_end_min_date,
        class: "bg-gradient-danger",
      })
    }
  }
  
  calendar.value = new Calendar(document.getElementById(props.id), {
    contentHeight: "auto",
    plugins: [dayGridPlugin],
    initialView: "dayGridMonth",
    selectable: false,
    editable: false,
    initialDate: StrDate.value,
    locale:"ko",
    events: Data.value,
    headerToolbar: {
      left: "prev,next,today",
      center: "title",
      right: "dayGridMonth,dayGridWeek",
    }
    ,
    views: {
      month: {
        titleFormat: {
          month: "long",
          year: "numeric",
        },
      },
      agendaWeek: {
        titleFormat: {
          month: "long",
          year: "numeric",
          day: "numeric",
        },
      },
      agendaDay: {
        titleFormat: {
          month: "short",
          year: "numeric",
          day: "numeric",
        },
      },
    },
  });

  calendar.value.render();


})

onUnmounted(()=>{
  if (calendar.value) {
    calendar.value.destroy();
  }
})
</script>
