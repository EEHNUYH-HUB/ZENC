<template>
  <div class="modal" id="modal-upload-progress" tabindex="0" role="dialog">
    <div class="modal-dialog modal-dialog-centered" id="modal-upload-progress-dialog" role="document">
      <div class="modal-content">
        <div class="modal-header">
            자료 저장          
        </div>
        <div class="modal-body ">
          <div class="multisteps-form__content">
            <h6 class="mb-0" >강의 자료를 저장 하시겠습니까?</h6>

            <div class="row mt-2">
              <div class="mt-4 mt-sm-0">
                <div class="d-flex mb-2">
                  <span class="me-2 text-sm font-weight-bold text-capitalize" >
                    {{ DisplayName }}
                  </span>
                  <span v-if="File" class="ms-auto text-sm font-weight-bold">{{ PrgValue }}%</span>
                </div>
                <div v-if="File">
                  <vsud-progress color="primary" :percentage="PrgValue" />
                </div>
              </div>
              <div class="mt-4 mt-sm-0">
                <label class="mt-2">공개설정</label>
                <select class="form-control" v-model="IsOpen">
                  <option value="true">공개</option>
                  <option value="false">비공개</option>
                </select>
              </div>
              <div class="mt-4 mt-sm-0 ">
                <label class="mt-2">시작일</label>
                <flat-pickr class="d-flex form-control datetimepicker bg-white " :config="config" v-model="StartDate"
                  placeholder="선택">
                </flat-pickr>
              </div>
              <div class="mt-4 mt-sm-0 ">
                <label class="mt-2">종료일</label>
                <flat-pickr class="d-flex form-control datetimepicker bg-white " :config="config" v-model="EndDate"
                  placeholder="선택">
                </flat-pickr>
              </div>
            </div>
          </div>
        </div>
        <div class="modal-footer m-0">
          <button class="btn btn-danger mx-1 m-0" @click="Save">저장</button>
          <button class="btn btn-primary mx-1 m-0" @click="CloseEvent(false)">닫기</button>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup>
import VsudProgress from "@/components/VsudProgress.vue";
import MNFileUploader from '@/minisoft/MNFileUploader'
import MNClient from '@/minisoft/MNClient'
import flatPickr from "vue-flatpickr-component";
import Swal from 'sweetalert2';
import moment from 'moment';
import { ref, reactive ,onMounted} from "vue"
import { shallowEqual } from "@babel/types";

const Props = defineProps({
    Item: { type: Object },
    FileType: { type: String },
    File: { type: Object },
    CloseEvent: { type: Function }
})
const IsOpen = ref("true");
const PrgValue = ref(0);
const StartDate = ref(null);
const EndDate = ref(null);
const DisplayName = ref(null);

let config = reactive({
  allowInput: false
});

const IsEditMode = ref(false);
let savingMode = ref(false);

onMounted(() => { 
  if (Props.Item.subjectname)
    Props.Item.DisplayName = Props.Item.subjectname + "-" + Props.Item.col_few_weeks + "주차 강의.mp4"
  else if (Props.Item.name) {
    Props.Item.DisplayName = Props.Item.name;
  }
  else if (Props.Item.col_filename){
    Props.Item.DisplayName = Props.Item.col_filename;
    IsEditMode.value = true;
    IsOpen.value = Props.Item.col_is_open?"true":"false";
    StartDate.value = Props.Item.col_open_start;
    EndDate.value = Props.Item.col_open_end;
  }

  DisplayName.value = Props.Item.DisplayName;
})
const Save = async () => { 
  if(savingMode.value){
    Swal.fire({
      icon: 'warning',
      text: '업로드가 진행중입니다.'
    });

    return;
  }

  savingMode.value = true;

  if (!StartDate.value || !EndDate.value) {
    Swal.fire({
      icon: 'warning',
      text: '시작일 혹은 종료일이 설정되지 않았습니다.'
    });
    return;
  }

  var st = moment(StartDate.value);
  var et = moment(EndDate.value + ' 23:59:59'); 
  if (et.diff(st) < 0) {
    EndDate.value = null;
    Swal.fire({
      icon: 'warning',
      text: '공개기간 종료일은 시작일보다 이를 수 없습니다.'
    });

    return;
  }

  if (!IsEditMode.value) {
    var uploader = new MNFileUploader();
    uploader.ProgressEvent = (r) => { PrgValue.value = Math.ceil(r); }
    uploader.CompletedEvent = async (staticName) => {
      var docParam = {
        folderID: Props.Item.col_solidocs_folder_id,
        staticName: staticName,
        fileName: Props.Item.DisplayName,
        scheduleID: Props.Item.pk_class_schedule_id,
        isOpen: IsOpen.value,
        type: Props.FileType,
        openStart: st,
        openEnd: et
      };

      await MNClient.Run('FileHandler', 'CreateDocument', docParam);

      Props.CloseEvent(true);
    }
    await uploader.Upload(Props.File, Props.Item.DisplayName);
  }
  else {
    
    let param = {
      scheduleId: Props.Item.pfk_class_schedule_id, 
      isopen: IsOpen.value == "true" ,
      openstart: st,
      openend: et,
      fileId: Props.Item.pfk_file_id
    };
    await MNClient.ExecNonQuery('PRO', 'UpdateLectureNoteInfo', param);
    Props.CloseEvent(true);
  }
}
</script>
<style scoped>


#modal-upload-progress {
  display: block;
  background: rgba(0, 0, 0, 0.3);
}


#modal-upload-progress-dialog {
  max-width: 600px;
}
</style>
