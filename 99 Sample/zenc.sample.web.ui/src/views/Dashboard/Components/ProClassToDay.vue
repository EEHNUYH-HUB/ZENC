<template>
  <div class="card">
    <div class="card-header p-3">
      <h6 class="mb-0">오늘의 강의</h6>
      <div class="d-flex">
        <div class="mb-0 text-sm p font-weight-bold widget-calendar-day">
          오늘의 강의를 볼수 있습니다.
        </div>

      </div>
    </div>
    <div class="card-body p-3 pt-0">
      <div class="table-responsive p-0">
        <table class="table align-items-center mb-0">
          <thead>
            <tr>
              <th class="text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                과목
              </th>
              <th class="text-uppercase text-secondary text-xs font-weight-bolder opacity-7 ps-2">
                교수
              </th>

              <th class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                시간
              </th>
              <th class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                강의실
              </th>
              <th class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                자료</th>
              <th class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">퀴즈</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(row,index) in Data" :key="index">
              <td>
                <div class="d-flex px-2 py-1">
                  <div>
                    <UserImg :UserID="row.proid" Width="32" Height="32" class="me-3" />
                  </div>
                  <div class="d-flex flex-column justify-content-center">
                    <h6 class="mb-0 text-sm">{{row.subjectname}}</h6>
                    <p class="text-xs text-secondary mb-0">
                      {{row.curriculumanme}}
                    </p>
                  </div>
                </div>
              </td>
              <td>
                <p class="text-xs font-weight-bold mb-0">{{row.proname}}</p>

              </td>

              <td class="align-middle text-center">
                <span class="text-secondary text-xs font-weight-bold">
                  {{GetTime(row.col_start_date)}}~{{GetTime(row.col_end_date)}}
                </span>
              </td>
              <td class="align-middle text-center text-sm">
                <router-link class="nav-link" :to="{ path: 'PROCSS', query: { id: row.pk_class_schedule_id }}">
                  <button class="btn btn-danger  py-1 px-3 m-0">입장</button>
                </router-link>
              </td>
              <td class="align-middle text-center text-sm">
                <button class="btn btn-success  py-1 px-3 m-0" @click="ShowFileList(row)">자료 관리</button>
              </td>
              <td class="align-middle text-center text-sm">
                <router-link class="nav-link" :to="{ path: 'PROQUIZ', query: { id: row.pk_class_schedule_id }}">
                  <button class="btn btn-primary  py-1 px-3 m-0">퀴즈 관리</button>
                </router-link>
              </td>
            </tr>
            <tr v-if="!Data || Data.length ==0">
              <td colspan="6" class="align-middle text-center text-sm">
                강의 내역이 없습니다.
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
  <ModalDataUpload v-if="SelectedItem" :folderID="SelectedItem.col_solidocs_folder_id"
    :scheduleID="SelectedItem.pk_class_schedule_id" @hide="SelectedItem = null">
  </ModalDataUpload>
</template>

<script setup>
import { ref } from 'vue';
import MNClient from "@/minisoft/MNClient";
import { GetTime, GetDate, GetUser } from "@/minisoft/MNCommon";

import ModalDataUpload from '@/views/Common/Modal/ModalDataUpload.vue'
import UserImg from "@/views/Common/UserImg.vue";
const userInfo = ref(GetUser());

const SelectedItem = ref(null)
let Data = ref(null);

const ShowFileList = (item) => {
  SelectedItem.value = item;
}

async function  Load(){

  Data.value = await MNClient.ExecDataTable("PRO", "TODAYCLASS", { PNUMBER: userInfo.value.PROFESSOR_NUMBER, PDATE: GetDate() });

  console.log(Data.value)
}

Load();

</script>
