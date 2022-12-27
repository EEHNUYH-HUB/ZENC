<template>
  <div class="card">
    <div class="card-body p-3 pt-4">
      <div class="table-responsive mt-4">
        <div class="datatable-wrapper">
          <table id="datatable-container" class="table align-items-center mb-4 bg-white">
            <thead>
              <tr>
                <th scope="col" class=" text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                  No
                </th>
                <th scope="col" class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                  이름
                </th>
                <th scope="col" class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                  학과
                </th>
                <th scope="col" class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                  학번
                </th>
                <th scope="col" class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                  학습 성취도
                </th>
                <th scope="col" class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                  평가 상세 확인
                </th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(item, index) in studentList" :key="index">
                <td class="align-middle text-center">
                  <p class="text-xs  mb-0">{{index + 1}}</p>
                </td>
                <td class="align-middle text-center">
                  <p class="text-xs  mb-0">{{item.col_name}}</p>
                </td>
                <td class="align-middle text-center">
                  <p class="text-xs  mb-0">{{item.col_department_name}}</p>
                </td>
                <td class="align-middle text-center">
                  <p class="text-xs  mb-0">{{item.fk_student_number}}</p>
                </td>
                <td class="align-middle">
                  <div style="width:90%; display: inline-block;">
                    <VsudProgressVue :percentage="GetTotalScore(item)" size='large' />
                  </div>
                  <p class="text-xs" style="width:10%; display: inline-block;">
                    {{GetTotalScore(item)}}점
                  </p>
                </td>
                <td class="align-middle text-center">
                  <button class="btn btn-danger  py-1 px-3 m-0" @click="ShowModalStudyEvaluate(item)">상세 확인</button>
                </td>
              </tr>
              <tr v-if="studentLength <= 0">
                <td colspan="6" class="text-center text-sm">조건에 맞는 수강생이 없습니다.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>

  <ModalStudyEvaluate v-if="SelectedItem" :UserItem="SelectedItem" :ClassName="className"
    @hide="HideModalStudyEvaluate" />
</template>

<script setup>
import {onMounted, reactive, ref, computed} from 'vue';
import {useStore} from 'vuex';
import MNClient from '@/minisoft/MNClient.js';
import { useRoute } from 'vue-router'
import ModalStudyEvaluate from '../Modal/ModalStudyEvaluate.vue';
import VsudProgressVue from '../../../components/VsudProgress.vue';
import { useImperativeHandle } from 'preact/hooks';

const route = useRoute();

let studentList = reactive([]);
let studentLength = ref(0);
let searchType = ref(0); // 1: 이름, 2: 학과, 3: 학번
let searchValue = ref('');

let className = ref(null);
// 상세모달 관련 변수

const SelectedItem = ref(null);

let computedSearchType = computed(()=>{
  if(searchType.value == 0){
    return '전체';
  }else if(searchType.value == 1){
    return '이름';
  }else if (searchType.value == 2){
    return '학과';
  }else if (searchType.value == 3){
    return '학번';
  }
});

onMounted(async()=>{
  const store = useStore()
  store.state.navigatorArray = new Array;
  store.state.currentName = "";
  const classDT = await MNClient.ExecDataTable("CLASS", "CLASSDTL", { PID: parseInt(route.query.id) });
  if (classDT) {
    let classDtl = classDT[0];
    store.state.navigatorArray.push({ Path: 'Dashboard', DisplayName: "대쉬보드" })
    store.state.navigatorArray.push({ Path: 'LECTURELIST', Query: { id: classDtl.pk_class_schedule_id, year: classDtl.col_year, semester: classDtl.col_semester }, DisplayName: classDtl.col_year + "년 " + classDtl.col_semester + "학기" })
    store.state.currentName = classDtl.subjectname;
    className.value = classDtl.subjectname;
  }
  UpdateStudyEvaluate();
});

function UpdateStudyEvaluate(){
  let param = { 
    CLASSID: parseInt(route.query.id),
    SearchType: searchType.value,
    SearchValue: searchValue.value.trim()
  };

  MNClient.ExecDataTable("PRO", "GETSTUDENTEVALUATION", param).then((results) => {    
    
    results.sort((a,b) =>{
      if(a.col_few_weeks < b.col_few_weeks){
        return -1;
      }
      if(a.col_few_weeks > b.col_few_weeks){
        return 1;
      }

      return 0;
    });
    
    Object.assign(studentList, results);
    
    studentLength.value = studentList.length;
  });
}

function GetTotalScore(item){
  return item.attendancescore + item.attitudescore + item.examscore + item.quizscore;
};

// 학습평가 상세 모달을 보인다.
function ShowModalStudyEvaluate(item) {
  SelectedItem.value = item;
  
}

// 학습평가 상세 모달을 숨긴다.
function HideModalStudyEvaluate() {
  SelectedItem.value = null;
  
}
</script>