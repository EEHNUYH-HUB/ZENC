import { createRouter, createWebHashHistory } from "vue-router";
import Dashboard                              from "@/views/Dashboard/ONCLASS_Dashboard.vue";
import Login                                  from "@/views/Login/ONCLASS_Login.vue";
import PROCLASS                               from "@/views/Class/ONCLASS_Pro.vue";
import STDCLASS                               from "@/views/Class/ONCLASS_Std.vue";
import MANUAL                                 from "@/views/Manual/ONCLASS_Manual.vue";
import LectureList                            from "@/views/LectureList/ONCLASS_LectureList.vue";
import ClassRoom                              from "@/views/ClassRoom/ONCLASS_ClassRoom.vue";
import QuizProResult                          from "@/views/Quiz/QuizProResult.vue";
import QuizStdResult                          from "@/views/Quiz/QuizStdResult.vue";
import ProStudyEvaluate                       from "@/views/LectureList/Components/ProStudyEvaluate.vue";
import ManageAuthentication                   from "@/views/Manage/ManageAuthentication.vue";


import TEST from "@/views/Class/Test.vue";

const routes = [
  {
    path: "/",
    name: "/",
    redirect: "/DASHBOARD",
  },
  {
    path: "/DASHBOARD",
    name: "대쉬보드",
    component: Dashboard,
  }
  ,
  {
    path: "/LECTURELIST",
    name: "강의목록",
    component: LectureList,
  }
  ,
  {
    path: "/CLASSROOM",
    name: "강의실",
    component: ClassRoom,
  }
  
  ,
  {
    path: "/login",
    name: "Login",
    component: Login,
  }
  , {
    path: "/PROCSS",
    name: "강의1",
    component:PROCLASS
  }

  , {
    path: "/STDCSS",
    name: "강의2",
    component:STDCLASS
  },
  {
    path: "/MANUAL",
    name: "ShowManual",
    component: MANUAL
  },
  {
    path: "/PROQUIZ",
    name: "퀴즈관리",
    component: QuizProResult
  },
  {
    path: "/STDQUIZ",
    name: "퀴즈내역",
    component: QuizStdResult
  },
  {
    path: "/ProStudyEvaluate",
    name: "학습평가",
    component: ProStudyEvaluate
  },
  {
    path:"/MNGAUTH",
    name: "인증관리",
    component: ManageAuthentication
  },
  {
    path:"/TEST",
    name: "TEST",
    component: TEST
  }
];

const router = createRouter({
  history : createWebHashHistory(), //해시 히스토리 모드(SPA 방식이여서 페이지가 바뀔대 마다 서버에 페이지에 대한 정보를 다시 요청하지 않음)
  routes,
  linkActiveClass: "active",
});

router.beforeEach((to,from,next)=>{
  const loggedIn = localStorage.getItem('user');
  
  if(to.matched.some(record=>record.meta.requiresAuth) && !loggedIn){ // to.matched는 이동할 route와 match되는 route들
    next('/Login') // redirect to login page
  }else{
    next() // direct to 'to'
  }
})



export default router;
