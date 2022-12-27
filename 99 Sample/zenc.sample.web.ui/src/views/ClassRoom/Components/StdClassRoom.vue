<template>

  <div class="col  m-3 mt-5">
    <div class="table-responsive">
      <div class="datatable-wrapper">
        <table id="datatable-container" class="table align-items-center mb-4 bg-white">
          <thead>
            <tr>
              <th scope="col" class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                강의 주차
              </th>
              <th scope="col" class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                강의 타입
              </th>
              <th class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                강의 시간
              </th>
              <th scope="col" class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                강의실
              </th>
              <th scope="col" class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                자료
              </th>
              <th scope="col" class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                강의 영상
              </th>
              <th scope="col" class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                퀴즈
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, index) in computedScheduleList" :key="index">
              <td class="align-middle text-center ">
                
                <h6 class="text-sm">
                  <GettingStarted v-if="item.col_status == 'ST010102'"></GettingStarted>
                  {{item.col_few_weeks}}주차
                  </h6>
                  <p class="text-xs  mb-0">
                    {{ item.curriculumanme }}</p>
              </td>
              <td class="align-middle text-center">
                <p class="text-xs  mb-0">{{item.col_class_type}} 강의</p>
              </td>
              <td class="align-middle text-center">
                <p class="text-xs mb-0">
                  {{ GetFullTime(item.col_start_date)}} ~ {{ GetTime(item.col_end_date)}}
                </p>
              </td>
              <template v-if="item.col_class_type == '온라인'">
                <td class="align-middle text-center">
                  <button class="btn btn-danger  py-1 px-3 m-0" v-if="item.col_status == 'ST010102'"
                    @click="TakeLecture(item.pk_class_schedule_id)">
                    입장
                  </button>
                </td>
                <td class="align-middle text-center align-middle">
                  <button class="btn btn-success  py-1 px-3 m-0"
                    v-if="item.col_data_count > 0 || item.col_lecture_note_count > 0" @click="ShowDownloadModal(item)">
                    다운로드
                  </button>
                </td>
                <td class="align-middle text-center align-middle">
                  <button class="btn btn-light  py-1 px-3 m-0 mx-1" v-if="item.col_lecture_note_count > 0"
                    @click="PlayLectureNote(item)">
                    재생
                  </button>
                </td>
                <td class="align-middle text-center align-middle">
                  <button class="btn btn-primary  py-1 px-3 m-0" @click="ShowQuizResult(item.pk_class_schedule_id)">
                    퀴즈 결과
                  </button>
                </td>
              </template>
              <template v-else>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
              </template>
            </tr>
            <tr v-if="scheduleLength <= 0">
              <td colspan="7" class="text-center text-sm">강의가 없습니다.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
  <ClassViewer v-if="ViewItem" :ClassItem="ViewItem" :CloseEvent="CloseViewer">

  </ClassViewer>
  <ModalDataDownload v-if="displayDownloadModal" :scheduleID="scheduleID" @hide="HideDonwloadModal" />
</template>

<script setup>
import { computed, ref, watch, reactive } from 'vue';
import router from '@/router';
import ClassViewer from '@/views/Class/Components/ClassViewer.vue'
import MNClient from '@/minisoft/MNClient.js';
import GettingStarted from '@/components/Icon/GettingStarted.vue';
import { BindingApiKeySend, GetTime, GetFullTime } from '@/minisoft/MNCommon.js'

import ModalDataDownload from '@/views/Common/Modal/ModalDataDownload.vue';

let schedule = reactive([]);

const props = defineProps({
  scheduleList:{
    type:Object
  },
  scheduleLength : {
    type: Number,
    required: true
  }
});

const ViewItem = ref(null);
const CloseViewer = ()=>{
  ViewItem.value = null;
}

let computedScheduleList = computed(()=>{
  if(props.scheduleLength > 0 && props.scheduleList)
    return props.scheduleList;
});

// 다운로드 모달 관련 변수
let folderID = ref();
let scheduleID = ref();
let uploadedFileList = reactive([]);
let displayDownloadModal = ref(false);

// 강의재생
function PlayLectureNote(item) {
  ViewItem.value = item
}

function TakeLecture(scheduleId){
  router.push({ path: '/STDCSS', query: { id: scheduleId }});
}

// 다운로드 모달을 보입니다.
function ShowDownloadModal(item){
  folderID.value = item.col_solidocs_folder_id;
  scheduleID.value = item.pk_class_schedule_id;
 
  
  
  displayDownloadModal.value = true;
}


// 다운로드 모달을 숨깁니다.
function HideDonwloadModal(){
  displayDownloadModal.value = false;
}

// 강의 자료 다운로드
// async function DownloadData(item){

//   var beforeUrl = process.env.VUE_APP_API_URL + 'api/downloader/download?id=' + item.pk_class_schedule_id;
//    /*BindingApiKeySend(beforeUrl, "blob", function (req) {

//      var url = window.URL.createObjectURL(req.response);

//      const link = document.createElement('a');
//      link.href = url;
//      document.body.appendChild(link);
//      link.click();
//      link.parentNode.removeChild(link);

//     })*/

//   const link = document.createElement('a');
//   link.href = beforeUrl;
//   document.body.appendChild(link);
//   link.click();
//   document.body.removeChild(link);

// }

// 퀴즈 결과 보기 페이지이동 - 라우터이동
function ShowQuizResult(scheduleId){
  router.push({ path: '/STDQUIZ', query: { id: scheduleId }});
}


</script>

<style scoped>

</style>
