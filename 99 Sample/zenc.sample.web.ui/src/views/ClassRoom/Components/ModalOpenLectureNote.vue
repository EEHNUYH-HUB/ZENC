<template>
  <div class="modal" id="modal-open-lecture-open" tabindex="-1" role="dialog">
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-header">
          <h6>강의록 공개 설정</h6> 
        </div>
        <div class="modal-body text-center mx-3">
          <div class="row">
            <p class="text-justify text-left"><strong>공개설정</strong></p>
            <div class="col-3 dropdown">
              <a class="btn dropdown-toggle btn-light public-btn" data-bs-toggle="dropdown" id="openLectureNoteModalDropDown">
                {{computedOpenData}}
              </a>
              <ul class="dropdown-menu" aria-labelledby="openLectureNoteModalDropDown">
                <li @click="localOpenData=true">공개</li>
                <li @click="localOpenData=false">비공개</li>
              </ul>
            </div>
          </div>
          <div class="row mt-4">
            <p class="text-justify text-left"><strong>공개기간</strong></p>
            <div class="col-12">
              <flat-pickr
                v-model="localStartDate"
                class="form-control datetimepicker w-45"
                placeholder="시작일 선택"
                :config="config"
                @input="onDateChange"
                style="display:inline-block"
              ></flat-pickr>
              <span> ~ </span>
              <flat-pickr
                v-model="localEndDate"
                class="form-control datetimepicker w-45"
                placeholder="종료일 선택"
                :config="config"
                @input="onDateChange"
                style="display:inline-block"
              ></flat-pickr>
            </div>
          </div>
        </div>
        <div class="modal-footer"> 
          <button class="btn btn-success" @click="SaveData()">저장</button> 
          <button class="btn btn-secondary" @click="HideModal()">취소</button> 
        </div> 
      </div>
    </div>
  </div>
</template>

<script setup>
import {ref, reactive, computed, onMounted, onUnmounted, watch} from 'vue';
import Swal from 'sweetalert2';
import MNClient from '@/minisoft/MNClient.js';
import flatPickr from "vue-flatpickr-component";
import moment from 'moment';

const emit = defineEmits(['hide']);
const props = defineProps({
  scheduleID : {
    type: Number
  },
  openData: {
    type: Boolean
  },
  startDate:{
    type: String
  },
  endDate:{
    type: String
  }
})

let localStartDate = ref(props.startDate);
let localEndDate = ref(props.endDate);
let localOpenData = ref(props.openData);
let config = reactive({
  allowInput: true
})

let computedOpenData = computed(()=>{
  if(localOpenData.value){
    return '공개';
  }
  return '비공개';
})

// 자료 업로드 모달을 닫기 위해 이벤트를 발생시킨다.
function HideModal(){
  emit('hide');
}

// 날짜 선택시 시작날짜가 종료날짜보다 이른지 체크한다.
function onDateChange(e){
  if(!localEndDate.value || !localStartDate.value){
    return;
  }

  if(moment(localEndDate.value).diff(moment(localStartDate.value)) < 0){
    localEndDate.value = '';
    Swal.fire({
      icon: 'error',
      text: '공개기간 종료일은 시작일보다 이를 수 없습니다.'
    });
  }
}

// 업데이트 api 호출
function SaveData(){
  let param = {
    scheduleId: props.scheduleID,
    isopen: localOpenData.value,
    openstart: moment(localStartDate.value),
    openend: moment(localEndDate.value)
  }

  MNClient.ExecNonQuery('PRO', 'UpdateLectureNoteInfo', param).then((result)=>{
    Swal.fire({
      icon: 'success',
      text: '공개여부와 기간을 업데이트하였습니다.'
    });
    
    HideModal();
  }).catch((error)=>{
    Swal.fire({
      icon: 'error',
      text : error.message
    });

    HideModal();
  });
}

</script>

<style scoped>
.modal {
  display: block;
  background:rgba(0,0,0,0.3);
}

.modal-dialog{
  max-width:600px;
}
</style>