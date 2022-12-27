<template>

    <div class="card ">
        <div class="card-header pb-0 right">
            <button v-show="ShowCreateBtn" class="btn btn-danger m-0 py-2" @click="ShowCreateQuiz(null)">퀴즈
                생성</button>
        </div>

        <div class="card-body p-3 pt-0">
            <div class="table-responsive mt-4">
                <div class="datatable-wrapper">
                    <table id="datatable-container" class="table align-items-center mb-4 bg-white">
                        <thead>
                            <tr>
                                <th class="text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                                    퀴즈명
                                </th>

                                <th
                                    class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                                    진행여부</th>
                                <th
                                    class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                                    수정
                                </th>
                                <th
                                    class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                                    삭제
                                </th>

                            </tr>
                        </thead>
                        <tbody>
                            <template v-for="(row, index) in Data" :key="index">
                                <tr @mouseover="row.Css = 'bg-light'" @mouseleave="row.Css = ''" :class="row.Css">
                                    <td>
                                        <div class="d-flex px-2 py-1">

                                            <div class="d-flex flex-column justify-content-center">
                                                <h6 class="mb-0 text-sm">{{ row.col_title }}</h6>

                                            </div>
                                        </div>
                                    </td>


                                    <td class="align-middle text-center ">
                                        <p class="text-xs  mb-0" v-if="row.col_status == 'QZ010101'">미진행</p>
                                        <p class="text-xs  mb-0" v-else-if="row.col_status == 'QZ010102'">진행중</p>
                                        <p class="text-xs  mb-0" v-else-if="row.col_status == 'QZ010103'">완료</p>

                                    </td>
                                    <td class="align-middle text-center ">
                                        <button class="btn btn-primary py-1 px-3 m-0"
                                            v-show="IsShowEditBtn && row.col_status == 'QZ010101'"
                                            @click="ShowCreateQuiz(row.pk_quizlist_id)">수정</button>
                                    </td>
                                    <td class="align-middle text-center ">
                                        <button class="btn btn-danger  py-1 px-3 m-0"
                                            v-show="row.col_status != 'QZ010102'"
                                            @click="DeleteQuizList(row.pk_quizlist_id)">삭제</button>
                                    </td>
                                </tr>
                                <template v-if="row.SubItems && row.SubItems.length > 0">
                                    <tr v-if="row.IsShow" @mouseover="row.Css = 'bg-light'" @mouseleave="row.Css = ''"
                                        :class="row.Css">
                                        <td colspan="5">
                                            <div class="row">
                                                <div class="col col-auto">
                                                    <button class="btn btn-info py-1 px-3 m-0"
                                                        @click="ShowResultQuiz(row.pk_quizlist_id)">결과보기</button>
                                                </div>
                                                <div class="col">
                                                    <table class="table align-items-center">
                                                        <thead>
                                                            <tr>
                                                                <th
                                                                    class="align-middle   text-uppercase text-secondary text-xs  opacity-7">
                                                                    질문
                                                                </th>
                                                                <th
                                                                    class="align-middle   text-uppercase text-secondary text-xs  opacity-7 ps-2">
                                                                    정답
                                                                </th>
                                                                <th
                                                                    class="align-middle text-center  text-uppercase text-secondary text-xs  opacity-7">
                                                                    배점</th>
                                                                <th
                                                                    class="align-middle text-center   text-uppercase text-secondary text-xs  opacity-7">
                                                                    정답률
                                                                </th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                            <tr v-for="subitem, subIndex in  row.SubItems"
                                                                :key="subIndex">
                                                                <td class="align-middle  ">
                                                                    <p class="text-xs  mb-0">
                                                                        {{ subitem.col_question }}</p>
                                                                </td>
                                                                <td class="align-middle  ">
                                                                    <p class="text-xs  mb-0">{{ subitem.item }}</p>
                                                                </td>
                                                                <td class="align-middle text-center ">
                                                                    <p class="text-xs  mb-0">{{ subitem.col_score }} 점
                                                                    </p>
                                                                </td>
                                                                <td class="align-middle text-center ">
                                                                    <p class="text-xs  mb-0">{{ subitem.rate }}</p>
                                                                </td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>

                                        </td>
                                    </tr>
                                </template>
                            </template>
                            <tr v-if="!Data || Data.length <= 0">
                                <td colspan="4" class="text-center text-sm">퀴즈 내역이 없습니다.</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>

    </div>

    <QuizCreater v-if="IsShowCreateQuiz" :CloseEvent="CloseCreateQuiz" :CLASSSTDID="ClassStdID"
        :QUIZLISTID="SelectedEditID" QUIZMODE="ONLYQUIZ"> </QuizCreater>
    <QuizResult v-if="SelectedResultID > 0" :CloseEvent="CloseResultQuiz" :QUIZLISTID="SelectedResultID">
    </QuizResult>
</template>
<script setup>

import QuizCreater from '@/views/Quiz/Modal/QuizCreater.vue'
import QuizResult from '@/views/Quiz/Modal/QuizResult.vue'
import Swal from 'sweetalert2'
import { GetTime } from "@/minisoft/MNCommon";
import { ref, defineProps, onMounted, onUnmounted } from 'vue'
import MNClient from '@/minisoft/MNClient'
import { useRoute } from "vue-router";
import { useStore } from "vuex"

const route = useRoute();
const id = parseInt( route.query.id );

const IsShowCreateQuiz = ref(false);
const Data = ref(null);
const SelectedEditID = ref(-1);
const SelectedResultID = ref(null);
const ClassStdID = ref(id);
const ShowCreateBtn = ref(true);
const ClassInfo = ref(null);
const IsShowEditBtn = ref(true);
const ShowCreateQuiz = (id) => {
    IsShowCreateQuiz.value = true;
    SelectedEditID.value = id;
}
const ShowResultQuiz = (id) => {
    SelectedResultID.value = id;
}
const CloseCreateQuiz = async (r) => {
    if (r) {
        Load();
    }

    IsShowCreateQuiz.value = false;
}

const DeleteQuizList = async (id) => {

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
            await MNClient.ExecNonQuery("QZ", "DELETEQUIZLIST", { PID: id });
            Load();
        }

    });

}
const CloseResultQuiz = () => {
    SelectedResultID.value = -1;
}
onMounted(async () => {
    var store = useStore()    
    store.state.navigatorArray = new Array;
    const dt = await MNClient.ExecDataTable("CLASS", "CLASSSCHEDULEDTL", { PID: ClassStdID.value });
    if (dt) {
        ClassInfo.value = dt[0];
        store.state.navigatorArray.push({ Path: 'Dashboard', DisplayName: "대쉬보드" })
        store.state.navigatorArray.push({ Path: 'LECTURELIST', Query: { id: ClassInfo.value.pk_class_schedule_id, year: ClassInfo.value.col_year, semester: ClassInfo.value.col_semester }, DisplayName: ClassInfo.value.col_year + "년 " + ClassInfo.value.col_semester + "학기" })
        
        store.state.navigatorArray.push({ Path: 'CLASSROOM', Query: { id: ClassInfo.value.pk_class_id }, DisplayName: ClassInfo.value.subjectname })
        store.state.currentName = ClassInfo.value.col_few_weeks+"주차 퀴즈 관리";
        
        Load();
    }
})



const Load = async () => {
    var dt = await MNClient.ExecDataTable("QZ", "GETRESULTQUIZLISTSUMMARY", { PID: ClassStdID.value });
    if (dt) {

        Data.value = new Array;
        var beforeItem = null;

        for (var i in dt) {
            var row = dt[i];
            if (!beforeItem || beforeItem.pk_quizlist_id != row.pk_quizlist_id) {

                beforeItem = new Object;
                beforeItem.pk_quizlist_id = row.pk_quizlist_id;
                beforeItem.col_title = row.col_title;
                beforeItem.col_status = row.col_status;
                beforeItem.IsShow = true;
                beforeItem.SubItems = new Array;

                Data.value.push(beforeItem);

            }
            beforeItem.SubItems.push(row);

        }

    }

    if (ClassInfo.value.col_status == 'ST010103') {
        ShowCreateBtn.value = false;
        IsShowEditBtn.value = false;
    }
    else {
        var score = await MNClient.ExecScalar("QZ", "GETTOTALSCORE", { PID: ClassStdID.value });
        ShowCreateBtn.value = score < 15;

    }
}


</script>
<style scoped>
.right {
    display: flex;
    justify-content: flex-end;
}
</style>