<template>
  <div class="card widget-calendar">
    <div class="p-3 card-body">
      <div :id="id" data-toggle="widget-calendar"></div>
    </div>
  </div>
</template>

<script setup>
import { Calendar } from "@fullcalendar/core";
import timeGridPlugin from "@fullcalendar/timegrid";
import koLocale from '@fullcalendar/core/locales/ko';
import moment from 'moment';
import { defineEmits, defineProps, onMounted, onBeforeUnmount } from 'vue';

let calendar;
const props = defineProps({
  id: {
    type: String,
    default: "widget-calendar",
  },
  title: {
    type: String,
    default: "",
  },
  day: {
    type: String,
    default: "",
  },
  year: {
    type: String,
    default: "",
  },
  initialView: {
    type: String,
    default: "timeGridWeek",
  },
  initialDate: {
    type: String,
    default: "2020-12-01",
  },
  events: {
    type: Array,
    default: () => [

    ],
  },
  selectable: {
    type: Boolean,
    default: true,
  },
  editable: {
    type: Boolean,
    default: true,
  }
});

const emit = defineEmits(['updated']);

onMounted(()=>{
  calendar = new Calendar(document.getElementById(props.id), {
    contentHeight: "auto",
    plugins: [timeGridPlugin],
    initialView: props.initialView,
    selectable: props.selectable,
    editable: props.editable,
    events: props.events,
    initialDate: props.initialDate,
    headerToolbar: {
      start: "prev", // will normally be on the left. if RTL, will be on the right
      center: "title",
      end: "next", // will normally be on the right. if RTL, will be on the left
    },
    dayHeaderDidMount: function(info){              
      var day = moment(info.date).format('D') // custom the text for example
      var week = moment(info.date).day();

      switch(week){
        case 0:
          week = '일';
          break;
        case 1:
          week = '월';
          break;
        case 2:
          week = '화';
          break;
        case 3:
          week = '수';
          break;
        case 4:
          week = '목';
          break;
        case 5:
          week = '금';
          break;
        case 6:
          week = '토';
          break;
      }

      let targetElement = info.el.querySelectorAll(".fc-col-header-cell-cushion");
      targetElement.forEach(e => {
        e.innerHTML = day + '<br/>' + week;
        e.style.color = "#344767";

        if(moment(info.date).isSame(moment(), 'day')){
          e.style.backgroundColor = "rgb(130, 214, 22)";
          e.style.color = "white";
        };

        e.addEventListener('click',  (e)=>{
          // 선택한 날짜 가져오기
          let selectedDate = e.target.getAttribute("aria-label");

          // 날짜 포맷 변경 YYYY-MM-DD
          const regex = /[년월일]/g;
          selectedDate = selectedDate.replace(regex, '-').slice(0, -1).replace(/ /gi, '');

          // 다른 날짜 디자인 초기화
          InitDateStyle();
          
          // 선택날짜에 디자인작업
          e.target.style.backgroundColor = "rgb(130, 214, 22)";
          e.target.style.color = "white";
          // 날짜선택시 부모 comp한테 이벤트 발생 리스트 가져오라고 이벤트 호출
          emit('updated', selectedDate);
        });
      });
      
    },
    locale:koLocale,
    titleFormat: {
      month: "long",
      year: "numeric",
    },
  });

  calendar.render();
});

function InitDateStyle(){
  let targetElement = document.querySelectorAll(".fc-col-header-cell-cushion");
  targetElement.forEach(e => {
    e.style.backgroundColor = "transparent";
    e.style.color = "#344767";
  });
}

onBeforeUnmount(()=>{
  if (calendar) {
    calendar.destroy();
  }
})

</script>
<style>
.fc .fc-scrollgrid-section-body table, .fc .fc-scrollgrid-section-footer table {
  display: none;
} 
.fc-theme-standard td:last-child{
  display: none;
}
.fc-timegrid tbody{
  display: none;
}

.fc-col-header-cell-cushion:hover{
  cursor: pointer;
}

</style>