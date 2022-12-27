<template>
  <div class="table-responsive mt-4">
    <div class="datatable-wrapper">
      <table id="datatable-container" class="table align-items-center mb-4 bg-white">
        <thead>
          <tr>
            <th scope="col" class="text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
              강의 명
            </th>
            <th scope="col" class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
              교수
            </th>
            <th scope="col" class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
              강의 코드
            </th>
            <th scope="col" class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
              강의 일정
            </th>
            <th scope="col" class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
              학습 현황
            </th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(item, index) in Data" :key="index">
            <!-- <td class="align-middle  ">
              <div class="d-flex px-2 py-1">
                <div>
                  <UserImg :UserID="item.proid" Width="32" Height="32" class="me-3" />
                </div>
                <div class="d-flex flex-column justify-content-center">
                  <h6 class="mb-0 text-sm">{{ item.subjectname}}</h6>
                </div>
              </div>
            </td> -->
            <td class="align-middle ">
              <p class="text-xs  mb-0">{{item.subjectname}}</p>
            </td>
            <td class="align-middle text-center">
              <p class="text-xs  mb-0">{{item.proname}}</p>
            </td>
            <td class="align-middle text-center">
              <p class="text-xs  mb-0">{{item.col_code}}</p>
            </td>

            <td class="align-middle text-center">
              <button class="btn btn-danger  py-1 px-3 m-0"
                @click="EnterClassRoom(item.subjectname, item.pk_class_id)">일정보기</button>
            </td>
            <td class="align-middle text-center">
              <button class="btn btn-primary py-1 px-3 m-0"
                @click="ShowStudyStatusModal(item.pk_class_id, item.subjectname)">학습 현황 조회</button>
            </td>
          </tr>
          <tr v-if="Data.length <= 0">
            <td colspan="4" class="text-center text-sm">강의 정보가 없습니다.</td>
          </tr>
        </tbody>
      </table>

    </div>
  </div>
  <ModalStudyStatus :classId="classId" :className="className" @hide="HideStudyStatusModal"
    v-if="displayStudyStatusModal" />
</template>

<script setup>
import {  onMounted, ref,  defineExpose } from 'vue';
import MNClient from '@/minisoft/MNClient.js'
import ModalStudyStatus from '../Modal/ModalStudyStatus.vue';
import {useStore} from 'vuex';
import router from '@/router';
import UserImg from "@/views/Common/UserImg.vue";



const Props = defineProps({
  Year: { type: Number },
  Semester: { type: Number }
})

defineExpose({ Load })

const store = useStore();


const Data = ref(Array)
let displayStudyStatusModal = ref(false);
let classId = ref();
let className = ref();

onMounted(async()=>{
  Load(Props.Year,Props.Semester);
});

async function Load(year,sem){

  if (!year) {

    var dt = await MNClient.ExecDataTable("CLASS", "CLASSSEMESTERINFO", null);
    if (dt) {
      for (var i in dt) {
        var item = dt[i];
        if (item.col_status == "ST010102") {
          year = item.col_year;
          sem = item.col_semester;
          break;
        }
      }
    }
  }

  
  if (year) {
    
    Data.value = await MNClient.ExecDataTable("STD", "CLASSPROGRESSSTATUS", { PYEAR: year, PSEMESTER: sem, PNUMBER: store.state.user.STUDENT_NUMBER });
    
  }
}

function EnterClassRoom(className, classId){
  router.push({ path: 'CLASSROOM', query: { id: classId } });
}

// 학습 현황 모달을 보여줍니다.
function ShowStudyStatusModal(paramClassId, paramClassName){
  classId.value = paramClassId;
  className.value = paramClassName;
  displayStudyStatusModal.value = true;
}

function HideStudyStatusModal(){
  displayStudyStatusModal.value = false;
}

</script>
