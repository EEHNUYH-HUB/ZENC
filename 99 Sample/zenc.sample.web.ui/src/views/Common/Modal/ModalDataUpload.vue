<template>
  <div class="modal" id="modal-data-upload" tabindex="-1" role="dialog">
    <div class="modal-dialog modal-dialog-centered" id="modal-data-upload-dialog" role="document">
      <div class="modal-content">
        <div class="modal-header">
          자료 관리
          <input class="btnFileLoad d-none" type="file" ref="oFileHandler" @change="FileChangeHandler"  />
        </div>
        <div class="modal-body ">
          <div class="row table-responsive ">
            <div class="datatable-wrapper">
              <table id="datatable-container" class="table align-items-center mb-4 bg-white">
                <thead>
                  <tr>
                    <th scope="col"
                      class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                      분류
                    </th>
                    <th scope="col"
                      class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                      파일명
                    </th>
                    <th scope="col"
                      class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                      공개 여부
                    </th>
                    <th scope="col"
                      class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                      기간설정
                    </th>
                    <th scope="col"
                      class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                      다운로드
                    </th>
                    <th scope="col"
                      class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                      삭제
                    </th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(item, index) in FileList" :key="index">
                    <td class="text-xs text-center">

                      <template v-if="item.col_type == 'DC010101'">
                        강의자료
                      </template>
                      <template v-else-if="item.col_type == 'DC010102'">
                        <span class="text-danger">강의영상</span>
                      </template>
                    </td>
                    <td class="text-xs text-truncate" style="max-width: 350px;" :title="item.name">
                      {{item.col_filename}}
                    </td>
                    <td class="text-xs text-center">
                      <button class="btn   py-1 px-3 m-0" :class="item.col_is_open ? 'btn-success' : 'btn-danger'"
                        @click="ShowEdit(item)">{{ item.col_is_open ?'공개':'비공개'}}</button>

                    </td>
                    <td class="text-xs text-center">
                      {{ moment(item.col_open_start).format('YYYY-MM-DD')}}
                      <span> ~ </span>
                      {{ moment(item.col_open_end).format('YYYY-MM-DD') }}
                    </td>
                    <td class="text-xs text-center">
                      <button class="btn btn-success  py-1 px-3 m-0" @click="DownloadData(item)">다운로드</button>
                    </td>
                    <td class="text-xs text-center">
                      <button class="btn btn-light  py-1 px-3 m-0" @click="removeSelectedFile(item)">삭제</button>
                    </td>

                  </tr>
                  <tr v-if="!FileList || FileList.length == 0">
                    <td colspan="6" class="text-xs text-center">자료가 없습니다.</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-danger mb-0" @click="SelectFiles()">업로드</button>
          <button class="btn btn-primary mb-0" @click="HideModal()">닫기</button>
        </div>
      </div>
    </div>
  </div>
  <UploadProgress v-if="displayProgressModal" :Item="UploadItem" :FileType="FileType" :File="File"
    :CloseEvent="CloseUploadProgress" />
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue';
import Swal from 'sweetalert2';
import MNClient from '@/minisoft/MNClient.js';
import flatPickr from "vue-flatpickr-component";
import moment from 'moment';
import UploadProgress from '@/views/Common/Modal/UploadProgress.vue'

const emit = defineEmits(['hide', 'updated']);

const props = defineProps({
  folderID: {
    type: Number
  },
  scheduleID : {
    type: Number
  }
})


let oFileHandler = ref();
let selectedFiles = reactive([]);
let displayProgressModal = ref(false);

// 업로드 모달 관련 변수
const UploadItem = ref(null);
let FileType = ref('DC010101');
let File = ref();
const FileList = ref(null);
onMounted(() => {
  Load();
})

const Load =async () => {
  var param = {
    scheduleId: props.scheduleID
  }

  FileList.value = await MNClient.ExecDataTable("Common", "GetLectureDataInfoList", param);
}
const CloseUploadProgress = (r) => {
  displayProgressModal.value = false;
  Load();
  //emit('updated');
}

function DownloadData(item) {
  if (!props.scheduleID) {
    Swal.fire({
      icon: 'error',
      text: '현재 강의의 ID를 불러올 수 없습니다.'
    });
  }

  var beforeUrl = process.env.VUE_APP_API_URL + 'api/downloader/download?id=' + item.col_solidocs_document_id + "&fileName=" + item.col_filename;
  const link = document.createElement('a');
  link.href = beforeUrl;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
}

// 자료를 업로드한다.
function SaveData(){
  let fileCnt = selectedFiles.length;
  let errorFileList = [];
  let promiseList=[];
  // 1. [에러] 업로드할 파일이 없을 때
  if(fileCnt <= 0){
    Swal.fire({
      icon:'error',
      text: '업로드할 파일을 선택해주세요.'
    });
    return;
  }

  let noDateFileCnt = 0;

  selectedFiles.forEach((item)=>{
    if(!item.openStart || !item.openEnd){
      noDateFileCnt++;
    }
  })
  

  // 2. [에러] 시작일/종료일 선택이 되지 않았을 때
  if( noDateFileCnt > 0 ){
    Swal.fire({
      icon: 'error',
      text: '시작일 혹은 종료일이 설정되지 않았습니다.'
    });
    return;
  }

  // 파일 쪼개서 서버에 전송하는 로직 시작
  selectedFiles.forEach((file, idx)=>{
    promiseList.push(new Promise((resolve, reject)=>{
      if(file.name && file.fileId && file.solidocsDocId && file.isChanged){
        let param = {
          scheduleId: props.scheduleID,
          isopen: file.isOpen,
          openstart: moment(file.openStart),
          openend: moment(file.openEnd),
          fileId: file.fileId
        };

        MNClient.ExecNonQuery('PRO', 'UpdateLectureNoteInfo', param).then((result)=>{
          resolve(true);
        }).catch((error)=>{
          errorFileList.push({
            filename : file.name,
            errorMsg : '이미 업로드된 파일이나 다음과 같은 에러가 발생하였습니다.\n' + error.message
          });
          resolve(false);
        });
      } else{
        resolve(true);
      }
    }));

    Promise.all(promiseList).then(value=>{
      let successCnt = 0;

      if(value){        
        value.forEach(result=>{
          if(result){
            successCnt++;
          }
        });
      }

      if(successCnt == fileCnt){
        Swal.fire({
          icon: 'success',
          text: '데이터 수정이 완료되었습니다.'
        });
        
        emit('hide');
      } else{
        Swal.fire({
          icon: 'error',
          text: '성공개수: ' + successCnt.toString() + ' / ' + total.toString() + '\n로그를 확인하세요.'
        });
        emit('hide');
      }
    })
    
  })
}

// 자료 업로드 모달을 닫기 위해 이벤트를 발생시킨다.
function HideModal(){
  emit('hide');
}


function SelectFiles(){
  oFileHandler.value.click();
}

async function removeSelectedFile(removeFile){


  Swal.fire({
    text: '정말로 삭제 하시겠습니까.',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: "네, 삭제 하겠습니다.",
    cancelButtonText: "아니요!",
    reverseButtons: true,
    customClass: {
      confirmButton: "btn bg-gradient-success",
      cancelButton: "btn bg-gradient-danger",
    },
    buttonsStyling: false
  }).then(async (r) => {
    if (r.isConfirmed) {
      if (removeFile.pfk_file_id && removeFile.col_solidocs_document_id && removeFile.pfk_class_schedule_id) {
        let param = {
          fileID: removeFile.pfk_file_id,
          scheduleID: removeFile.pfk_class_schedule_id,
          fileName: removeFile.col_filename,
          solidocsDocID: removeFile.col_solidocs_document_id
        }

        await MNClient.Run('FileHandler', 'RemoveDocument', param);
        Load();
      }
    }

  });
  
  

  // 이미 업로드된 파일이면 soliDocs에서도 지우고 맵핑된 db정보도 삭제한다.
  
}

function FileChangeHandler(e){
  let files = [...e.target.files];

  LoadSelectedFiles(files);
}

function onDrop(e){
  let files = [...e.dataTransfer.files];
  
  LoadSelectedFiles(files);
}

function ShowEdit(item) {
  File.value = null;
  UploadItem.value = item;
  displayProgressModal.value = true;
}
function LoadSelectedFiles(files) {
  UploadItem.value = new Object;
  if(files.length == 1){
    UploadItem.value.pk_class_schedule_id = props.scheduleID;
    UploadItem.value.col_solidocs_folder_id = props.folderID;
    UploadItem.value.name = files[0].name;

    File.value = files[0];

    displayProgressModal.value = true;
  }else{
    Swal.fire({
      icon: 'warning',
      text: '파일은 하나만 업로드가 가능합니다.'
    });
    return;
  }
}

</script>

<style scoped>
#modal-data-upload {
  display: block;
  background:rgba(0,0,0,0.3);
}

#modal-data-upload-dialog{
  max-width:900px;
}

.label{
  margin:1px;
  padding:2px;
  position:relative;
}

.remove-label{
  position:absolute;
  right:10px;
  cursor:pointer;
  font-weight: bold;
  color:#8392AB;
  transition: all 0.5s;
}

.remove-label:hover{
  color:#ea0606;
}

.files{
  height:300px;
}

.emty-files{
  border: 1px dashed ;
  margin: 0 auto;
}

.public-btn{
  width:150px;
}

/* Scrollbar */
::-webkit-scrollbar {
  width: .45rem;
}
::-webkit-scrollbar-thumb {
  background-color: rgba(27, 27, 27, .4);
  border-radius: 3px;
}
::-webkit-scrollbar-track{
  background: transparent;
}

</style>