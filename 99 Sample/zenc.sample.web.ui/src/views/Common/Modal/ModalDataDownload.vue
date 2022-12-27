<template>
  <div class="modal" id="modal-data-download" tabindex="-1" role="dialog">
    <div class="modal-dialog modal-dialog-centered" id="modal-data-download-dialog" role="document">
      <div class="modal-content">
        <div class="modal-header">
          자료 관리
        </div>
        <div class="modal-body">
          <div class="row table-responsive">
            <div class="datatable-wrapper">
              <table id="datatable-container" class="table align-items-center bg-white">
                <thead>
                  <tr>
                    <th scope="col"
                      class=" text-center  text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                      분류
                    </th>
                    <th scope="col"
                      class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                      파일명
                    </th>
                    <th scope="col"
                      class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                      다운로드 기간
                    </th>
                    <th scope="col"
                      class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                      다운로드
                    </th>
                  </tr>
                </thead>
                <tbody>
                  <template v-for="(item, index) in FileList" :key="index">
                    <tr v-if="item.col_is_open">
                      <td class="text-xs text-center">
                        <template v-if="item.col_type == 'DC010101'">
                          강의자료
                        </template>
                        <template v-else-if="item.col_type == 'DC010102'">
                          <span class="text-danger">강의영상</span>
                        </template>
                      </td>
                      <td class="text-xs text-truncate" style="max-width: 350px;" :title="item.name">
                        {{ item.col_filename }}
                      </td>
                      <td class="text-xs text-center">
                        {{ moment(item.col_open_start).format('YYYY-MM-DD')}}
                        <span> ~ </span>
                        {{ moment(item.col_open_end).format('YYYY-MM-DD') }}
                      </td>
                      <td class="text-xs text-center">
                        <button class="btn btn-success  py-1 px-3 m-0" v-if="!item.isdisable"
                          @click="DownloadData(item)">다운로드</button>
                      </td>
                    </tr>
                    <tr v-if="!FileList || FileList.length == 0">
                      <td colspan="5" class="text-xs">자료가 없습니다.</td>
                    </tr>
                  </template>
                </tbody>
              </table>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-primary mb-0" @click="HideModal()">닫기</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import {ref,  onMounted} from 'vue';
import Swal from 'sweetalert2';
import MNClient from '@/minisoft/MNClient.js';
import moment from 'moment';

const emit = defineEmits(['hide']);

const props = defineProps({
  
  scheduleID : {
    type: Number
  }
})

const FileList = ref(null);
onMounted(() => { 

  var param = {
    scheduleId: props.scheduleID
  }

  MNClient.ExecDataTable("Common", "GetLectureDataInfoList", param).then((result) => {
    FileList.value = result;
  })

})


// 자료를 다운로드한다.
function DownloadData(item){
  if(!props.scheduleID){
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

// 자료 다운로드 모달을 닫기 위해 이벤트를 발생시킨다.
function HideModal(){
  emit('hide');
}

</script>

<style scoped>
#modal-data-download {
  display: block;
  background:rgba(0,0,0,0.3);
}

#modal-data-download-dialog{
  max-width:900px;
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