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
              <th scope=" col" class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
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
              <td class="align-middle text-center">
                
                <h6 class="text-sm">
                  <GettingStarted v-if="item.col_status == 'ST010102'"></GettingStarted>
                  {{item.col_few_weeks}} 주차
                </h6>
                
                <p class="text-xs  mb-0">
                  {{ item.curriculumanme }}</p>
              </td>
              <td class="align-middle text-center">
                <p class="text-xs  mb-0">{{item.col_class_type}} 강의</p>
              </td>
              <td class="align-middle text-center">
                <P class="text-xs mb-0">
                  {{ GetFullTime(item.col_start_date)}}~{{ GetTime(item.col_end_date)}}
                </P>
              </td>
              <template v-if="item.col_class_type == '온라인'">
                <td class="align-middle text-center">
                  <button class="btn btn-danger  py-1 px-3 m-0" v-if="item.col_status =='ST010102'"
                    @click="TakeLecture(item.pk_class_schedule_id)">입장</button>

                </td>
                <td class="align-middle text-center">
                  <button class="btn btn-success  py-1 px-3 m-0" @click="ShowDataUploadModal(item)">
                    자료 관리
                  </button>
                </td>
                <td class="align-middle text-center align-middle">
                  <button class="btn btn-light  py-1 px-3 m-0 mx-1" v-if="item.col_lecture_note_count > 0"
                    @click="PlayLectureNote(item)">
                    재생
                  </button>
                </td>
                <td class="align-middle text-center">
                  <button class="btn btn-primary  py-1 px-3 m-0"
                    @click="MoveToQuizManagement(item.pk_class_schedule_id)">
                    퀴즈 관리
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

  <ModalDataUpload v-if="displayDataUploadModal" :folderID="folderID" :scheduleID="scheduleID"
    @updated="GetLectureDataList" @hide="HideDataUploadModal">
  </ModalDataUpload>
  <ModalOpenLectureNote v-if="displayOpenLectureNoteModal" :scheduleID="scheduleID" :openData="openLectureNote"
    :startDate="noteStartDate" :endDate="noteEndDate" @hide="HideOpenLectureNoteModal">
  </ModalOpenLectureNote>

  <ClassViewer v-if="ViewItem" :ClassItem="ViewItem" :CloseEvent="CloseViewer">

  </ClassViewer>
</template>

<script setup>
import { computed, ref, watch, reactive, defineEmits } from 'vue';
import { GetTime, GetFullTime } from "@/minisoft/MNCommon";
import router from '@/router';
import MNClient from '@/minisoft/MNClient.js';
import ClassViewer from '@/views/Class/Components/ClassViewer.vue'
import GettingStarted from '@/components/Icon/GettingStarted.vue';
import ModalDataUpload from '@/views/Common/Modal/ModalDataUpload.vue'
import ModalOpenLectureNote from './ModalOpenLectureNote.vue';

let schedule = reactive([]);
let displayDataUploadModal = ref(false);
let displayOpenLectureNoteModal = ref(false);

const ViewItem = ref(null);
// 자료 관련 변수
let folderID = ref();
let scheduleID = ref();

let openLectureNote = ref(false);
let noteStartDate = ref();
let noteEndDate = ref();

const emit = defineEmits(['updated']);
const props = defineProps({
  scheduleList:{
    type:Object
  },
  scheduleLength : {
    type: Number,
    required: true
  }
});
function PlayLectureNote(item) {
  ViewItem.value = item
}
const CloseViewer = () => {
  ViewItem.value = null;
}
let computedScheduleList = computed(()=>{
  if(props.scheduleLength > 0 && props.scheduleList)
    return props.scheduleList;
});

// 강의 입장 - 라우터 이동
function TakeLecture(scheduleId){
  router.push({ path: '/PROCSS', query: { id: scheduleId }});
}

// 퀴즈 관리 페이지 이동 - 라우터 이동
function MoveToQuizManagement(scheduleId){
  router.push({ path: '/PROQUIZ', query: { id: scheduleId }});
}

// 자료 업로드 모달을 보인다.
function ShowDataUploadModal(item){
  folderID.value = item.col_solidocs_folder_id;
  scheduleID.value = item.pk_class_schedule_id;
 
  
  
  displayDataUploadModal.value = true;
}

// 자료 업로드 모달을 숨긴다.
function HideDataUploadModal(){
  displayDataUploadModal.value = false;
  emit('updated');
}

// 강의록 공개 설정 모달을 보인다.
function ShowOpenLectureNoteModal(item){
  scheduleID.value = item.pk_class_schedule_id;
  openLectureNote.value = item.col_note_is_open;
  noteStartDate.value = item.col_note_open_start;
  noteEndDate.value = item.col_note_open_end;

  displayOpenLectureNoteModal.value = true;
}

// 강의록 공개 설정 모달을 숨긴다.
function HideOpenLectureNoteModal(){
  displayOpenLectureNoteModal.value = false;
  emit('updated');
}



</script>
