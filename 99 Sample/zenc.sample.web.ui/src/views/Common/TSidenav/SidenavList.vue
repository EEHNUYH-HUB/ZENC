<template>

  <div class="w-auto h-auto collapse navbar-collapse max-height-vh-100 h-100" id="sidenav-collapse-main">
    <ul class="navbar-nav">
      <li class="nav-item">
        <SidenavCollapse navText="대쉬보드" :to="{ path: 'DASHBOARD' }">
          <template #icon>
            <shop />
          </template>
        </SidenavCollapse>
      </li>
      <li class="nav-item">
        <SidenavCollapse navText="강의 목록" :to="{ path: 'LECTURELIST' }">
          <template #icon>
            <shop />
          </template>
        </SidenavCollapse>
      </li>
      <!-- <li class="nav-item">
        <SidenavCollapse navText="마이 페이지" :to="{ name: '마이페이지' }">
          <template #icon>
            <office />
          </template>
        </SidenavCollapse>
      </li> -->
      <!--
      <li class="nav-item">
        <sidenav-collapse navText="Billing" :to="{ name: 'Billing' }">
          <template #icon>
            <credit-card />
          </template>
        </sidenav-collapse>
      </li>

      <li class="nav-item">
        <sidenav-collapse
          navText="Virtual Reality"
          :to="{ name: 'Virtual Reality' }"
        >
          <template #icon>
            <box3d />
          </template>
        </sidenav-collapse>
      </li>
      <li class="nav-item">
        <sidenav-collapse navText="RTL" :to="{ name: 'Rtl' }">
          <template #icon>
            <settings />
          </template>
        </sidenav-collapse>
      </li>
      <li class="mt-3 nav-item">
        <h6
          class="text-xs ps-4 text-uppercase font-weight-bolder opacity-6"
          :class="this.$store.state.isRTL ? 'me-4' : 'ms-2'"
        >
          PAGES
        </h6>
      </li>
      <li class="nav-item">
        <sidenav-collapse navText="Profile" :to="{ name: 'Profile' }">
          <template #icon>
            <customer-support />
          </template>
        </sidenav-collapse>
      </li>
      <li class="nav-item">
        <sidenav-collapse navText="Sign In" :to="{ name: 'Sign In' }">
          <template #icon>
            <document />
          </template>
        </sidenav-collapse>
      </li>
      <li class="nav-item">
        <sidenav-collapse navText="Sign Up" :to="{ name: 'Sign Up' }">
          <template #icon>
            <spaceship />
          </template>
        </sidenav-collapse>
      </li> -->
    </ul>
  </div>
  <div class="pt-3 mx-3 mt-3 sidenav-footer">
    <SidenavCard :class="cardBg" :textPrimary="userName" :textSecondary="userNumber" label="로그아웃"
      icon="ni ni-diamond" />

    <!-- <Calendar class="side-calendar" @updated="GetTodayClass" :initialDate="GetDate()" />

    <div class="mt-3 card">
      <div class="p-3 card-body overflow-auto mh-200" style="max-height:200px;">
        <div v-for="(item, index) in todayClass" :key="index">
          <p class="card-title text-sm text-bold">{{item.curriculumanme}}</p>
          <div class="row">
            <p class="card-text text-sm col-5">{{item.col_class_type}}</p>
            <p class="card-text text-sm col-7 time">{{getTime(item.col_start_date)}} - {{getTime(item.col_end_date)}}
            </p>
          </div>
        </div>
        <div v-if="todayClass && todayClass.length <= 0">
          <p>선택한 날짜에는 강의가 없습니다.</p>
        </div>
      </div>
    </div> -->
  </div>

</template>
<script setup>
import SidenavCollapse from "@/views/Common/TSidenav/SidenavCollapse.vue";
import SidenavCard from "@/views/Common/TSidenav/SidenavCard.vue";
import Shop from "@/components/Icon/Shop.vue";
import Office from "@/components/Icon/Office.vue";
import CreditCard from "@/components/Icon/CreditCard.vue";
import Box3d from "@/components/Icon/Box3d.vue";
import CustomerSupport from "@/components/Icon/CustomerSupport.vue";
import Document from "@/components/Icon/Document.vue";
import Spaceship from "@/components/Icon/Spaceship.vue";
import Settings from "@/components/Icon/Settings.vue";
// import Calendar from "@/views/Common/TSidenav/SidenavCalendar.vue";
import {GetDate} from "@/minisoft/MNCommon.js";
import {computed, defineProps, onMounted, ref} from 'vue';
import {useStore} from 'vuex'
import MNClient from "@/minisoft/MNClient";
import moment from 'moment';

let props = defineProps({
  cardBg: String,
  user: Object
});

let title = ref("Soft UI Dashboard PRO");
let controls = ref("dashboardsExamples");
let isActive = ref("active");
let todayClass = ref();

const store = useStore();

let userNumber = computed(()=>{
  if(store.state.user && store.state.user.ROLE=='STUDENT'){
    return store.state.user.STUDENT_NUMBER;
  }

  if(store.state.user && store.state.user.ROLE == 'PROFESSOR'){
    return store.state.user.PROFESSOR_NUMBER;
  }

  return 0;
});

let userName = computed(()=>{
  if(store.state.user){
    return store.state.user.USER_NAME;
  }

  return '';
});

onMounted(async ()=>{
  GetTodayClass();
});

async function GetTodayClass(date){
  if(date==undefined || date==null){
    date = moment().format('YYYY-MM-DD');
  }

  if(store.state.user && store.state.user.ROLE == 'STUDENT'){
    todayClass.value = await MNClient.ExecDataTable("STD", "TODAYCLASS", { PNUMBER:'STD00000001',PDATE: date});
  } else if(store.state.user && store.state.user.ROLE == 'PROFESSOR'){
    todayClass.value = await MNClient.ExecDataTable("PRO","TODAYCLASS",{PNUMBER:'PFR00000001',PDATE: date});
  }
}

function getRoute() {
  const routeArr = this.$route.path.split("/");
  return routeArr[1];
};

function getTime(dateStr){
  if(!dateStr){
    return NaN;
  }
  let date = moment(dateStr);
  return date.format("hh:mm");
}

</script>

<style scoped>
.side-calendar{
  margin-top: 1rem;
}
.time{
  color:orange;
}
</style>