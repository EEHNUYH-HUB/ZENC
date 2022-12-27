<template>
    
        <div class="card">


            <div class="card-body p-3 pt-0">
                <div class="table-responsive mt-4">
                    <div class="datatable-wrapper">
                        <table id="datatable-container" class="table align-items-center mb-4 bg-white">
                            <thead>
                                <tr>
                                    <th class="text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                                        퀴즈명
                                    </th>
                                    <th class="text-uppercase text-secondary text-xs font-weight-bolder opacity-7 ps-2">
                                        정답 결과
                                    </th>
                                    <th
                                        class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                                        점수 결과</th>
                                </tr>
                            </thead>
                            <tbody>
                                <template v-for="(row, index) in Data" :key="index">
                                    <tr @click="OnClickRow(row)" @mouseover="row.Css = 'bg-light'"
                                        @mouseleave="row.Css = ''" :class="row.Css">
                                        <td>
                                            <div class="d-flex px-2 py-1">

                                                <div class="d-flex flex-column justify-content-center">
                                                    <h6 class="mb-0 text-sm">{{ row.Title }}</h6>

                                                </div>
                                            </div>
                                        </td>


                                        <td class="align-middle text-center ">
                                            <p class="text-xs  mb-0">{{ row.Cnt }}/{{ row.TotalCnt }}</p>
                                        </td>
                                        <td class="align-middle text-center ">
                                            <p class="text-xs  mb-0">{{ row.Score }}/{{ row.TotalScore }}</p>
                                        </td>

                                    </tr>
                                    <template v-if="row.SubItems && row.SubItems.length > 0">
                                        <tr v-if="row.IsShow" @mouseover="row.Css = 'bg-light'"
                                            @mouseleave="row.Css = ''" :class="row.Css">
                                            <td colspan="5">

                                                <div class="col mx-3">
                                                    <table class="table align-items-center">
                                                        <thead>
                                                            <tr>
                                                                <th
                                                                    class="align-middle   text-uppercase text-secondary text-xs  opacity-7">
                                                                    질문
                                                                </th>
                                                                <th
                                                                    class="align-middle   text-uppercase text-secondary text-xs  opacity-7 ">
                                                                    정답
                                                                </th>
                                                                <th
                                                                    class="align-middle    text-uppercase text-secondary text-xs  opacity-7">
                                                                    선택 항목
                                                                </th>
                                                                <th
                                                                    class="align-middle text-center  text-uppercase text-secondary text-xs  opacity-7">
                                                                    배점</th>

                                                                <th
                                                                    class="align-middle text-center   text-uppercase text-secondary text-xs  opacity-7">
                                                                    결과
                                                                </th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <tr v-for="subitem, subIndex in  row.SubItems"
                                                                :key="subIndex">
                                                                <td class="align-middle  ">
                                                                    <p class="text-xs  mb-0">{{ subitem.col_question }}
                                                                    </p>
                                                                </td>
                                                                <td class="align-middle  ">
                                                                    <p class="text-xs  mb-0">{{ subitem.item }}</p>
                                                                </td>

                                                                <td class="align-middle  ">
                                                                    <p class="text-xs  mb-0">{{ subitem.myanswer }}</p>
                                                                </td>
                                                                <td class="align-middle text-center ">
                                                                    <p class="text-xs  mb-0">{{ subitem.col_score }} 점
                                                                    </p>
                                                                </td>
                                                                <td class="align-middle text-center ">
                                                                    <p v-if="subitem.isok"
                                                                        class="text-xs  mb-0 text-danger">정답
                                                                    </p>

                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </div>

                                            </td>
                                        </tr>
                                    </template>
                                </template>
                                <tr v-if="!Data || Data.length <= 0">
                                    <td colspan="5" class="text-center text-sm">퀴즈 내역이 없습니다.</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    
</template>

<script setup>
import { ref, defineProps, onMounted, onUnmounted } from 'vue'
import { useRoute } from "vue-router";
import MNClient from '@/minisoft/MNClient'
import { GetUser } from "@/minisoft/MNCommon"
import { useStore } from "vuex"

const route = useRoute();
const id = route.query.id * 1;
const ClassStdID = ref(id);
const Data = ref(null);


onMounted(async () => {
    var userInfo = GetUser();
    var store = useStore()
    store.state.navigatorArray = new Array;
    const classDT = await MNClient.ExecDataTable("CLASS", "CLASSSCHEDULEDTL", { PID: ClassStdID.value });
    if (classDT) {
        var classDtl = classDT[0];
        store.state.navigatorArray.push({ Path: 'Dashboard', DisplayName: "대쉬보드" })
        store.state.navigatorArray.push({ Path: 'LECTURELIST', Query: { id: classDtl.pk_class_schedule_id, year: classDtl.col_year, semester: classDtl.col_semester }, DisplayName: classDtl.col_year + "년 " + classDtl.col_semester + "학기" })
        store.state.navigatorArray.push({ Path: 'CLASSROOM', Query: { id: classDtl.pk_class_id }, DisplayName: classDtl.subjectname })
        store.state.currentName = classDtl.col_few_weeks + "주차 퀴즈 결과";

    }
    const dt = await MNClient.ExecDataTable("QZ", "GETSTDRESULTQUIZLISTSUMMARY", { PID: ClassStdID.value, PSTDNUMBER: userInfo.STUDENT_NUMBER });
    if (dt) {

        Data.value = new Array;
        var beforeItem = null;
        
        
            
        for (var i in dt) {
            var row = dt[i];
            if (!beforeItem || beforeItem.ID != row.pk_quizlist_id) {
                
                beforeItem = new Object;
                beforeItem.ID = row.pk_quizlist_id;
                beforeItem.Title = row.col_title;
                beforeItem.TotalCnt = 0;
                beforeItem.TotalScore = 0;
                beforeItem.Cnt = 0;
                beforeItem.Score = 0;
                beforeItem.IsShow = true;
                beforeItem.SubItems = new Array;
                
                Data.value.push(beforeItem);

            }
            if (row.isok) {
                beforeItem.Cnt += 1;
                beforeItem.Score += row.col_score;
            }

            beforeItem.TotalCnt += 1;
            beforeItem.TotalScore += row.col_score;
            
            beforeItem.SubItems.push(row);
            
        }
    }
})
</script>