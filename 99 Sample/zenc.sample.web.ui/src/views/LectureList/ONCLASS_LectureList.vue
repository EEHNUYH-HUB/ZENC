<template>
  <div class="card">
    <div class="card-body p-3 pt-4">
      <StdLectureList v-if="isStudent" :Year="Year" :Semester="Semester" ref="listCtrl">
      </StdLectureList>
      <ProLectureList v-if="isProfessor" :Year="Year" :Semester="Semester" ref="listCtrl">
      </ProLectureList>
    </div>
  </div>
</template>

<script setup>
import { useStore } from 'vuex';
import { useRoute } from "vue-router";
import { ref, onMounted, onUpdated } from 'vue';
import ProLectureList from '@/views/LectureList/Components/ProLectureList.vue';
import StdLectureList from '@/views/LectureList/Components/StdLectureList.vue';

const store = useStore();
const route = useRoute();
const listCtrl = ref();
const Year = ref(parseInt(route.query.year));
const Semester = ref(parseInt(route.query.semester));
let isStudent = ref(false);
let isProfessor = ref(false);

if(store.state.user && store.state.user.STUDENT_NUMBER){
  isStudent.value = true;
  isProfessor.value = false;
}else if(store.state.user && store.state.user.PROFESSOR_NUMBER){
  isStudent.value = false;
  isProfessor.value = true;
}

onMounted(() => { 
  BindingHeader();
})

const BindingHeader = () => {

  Year.value = parseInt(route.query.year);
  Semester.value = parseInt(route.query.semester);
  store.state.navigatorArray = new Array;
  store.state.navigatorArray.push({ Path: 'Dashboard', DisplayName: "대쉬보드" })
  store.state.currentName = Year.value + "년 " + Semester.value + "학기";
  listCtrl.value.Load(Year.value, Semester.value)
}
onUpdated(() => {
  BindingHeader();
})
   
</script>