<template>
    <div class="modal" tabindex="-1" role="dialog">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    퀴즈(설문) 목록

                    <button class="btn btn-outline-danger m-0 py-2"
                        @click="ShowCreateQuiz(null)">퀴즈(설문) 생성</button>

                </div>

                <table class="table align-items-center mb-0">
                    <thead>
                        <tr>
                            <th class="text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                                퀴즈(설문)명
                            </th>
                            <th class="text-uppercase text-secondary text-xs font-weight-bolder opacity-7 ps-2">
                                타입
                            </th>

                            <th class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                                수정
                            </th>
                            <th class="text-center text-uppercase text-secondary text-xs font-weight-bolder opacity-7">
                                상태</th>

                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(row,index) in Data" :key="index">
                            <td>
                                <div class="d-flex px-2 py-1">
                                    
                                    <div class="d-flex flex-column justify-content-center">
                                        <h6 class="mb-0 text-sm">{{ row.col_title }}</h6>
                                        <p class="text-xs text-secondary mb-0">
                                            {{ row.creatername }}
                                        </p>
                                    </div>
                                </div>
                            </td>
                            <td>
                                <p class="text-xs font-weight-bold mb-0">{{ row.typename }}</p>

                            </td>

                            <td class="align-middle text-center ">
                                <vsud-badge color="danger" variant="gradient" v-show="row.col_status == 'QZ010101'"
                                    @click="ShowCreateQuiz(row.pk_quizlist_id)">수정</vsud-badge>
                            </td>
                            <td class="align-middle text-center ">
                                <vsud-badge color="warning" variant="gradient" v-if="row.col_status == 'QZ010101'"
                                    @click="ProgressEvent(row)">진행
                                </vsud-badge>
                                <label v-else-if="row.col_status == 'QZ010102'">진행중</label>
                                <vsud-badge v-else color="success" v-show="row.col_status == 'QZ010103'"
                                    @click="ShowResultQuiz(row.pk_quizlist_id)" variant="gradient">
                                    결과보기</vsud-badge>
                            </td>
                        </tr>

                    </tbody>
                </table>

                <div class="modal-footer">
                    <button class="btn btn-primary m-0" @click="CloseEvent(false,-1)">닫기</button>
                </div>
            </div>
        </div>


    </div>
    <QuizCreater v-if="IsShowCreateQuiz" :CloseEvent="CloseCreateQuiz" :CLASSSTDID="CLASSSTDID"
        :QUIZLISTID="SelectedEditID" :GetGroupIDEvent="GetGroupIDEvent">

    </QuizCreater>
    <QuizResult v-if="SelectedResultID > 0" :CloseEvent="CloseResultQuiz" :QUIZLISTID="SelectedResultID"></QuizResult>
</template>
<script setup>
import QuizCreater from '@/views/Quiz/Modal/QuizCreater.vue'
import QuizResult from '@/views/Quiz/Modal/QuizResult.vue'
import VsudBadge from "@/components/VsudBadge.vue";
import { GetTime } from "@/minisoft/MNCommon";
import { ref, defineProps, onMounted, onUnmounted } from 'vue'
import MNClient from '@/minisoft/MNClient'

const IsShowCreateQuiz = ref(false);
const Data = ref(null);
const SelectedEditID = ref(-1);
const SelectedResultID = ref(null);

const Props = defineProps({
    CLASSSTDID: { type: String },
    CloseEvent: { type: Function },
    GetGroupIDEvent: { type: Function },
    ProgressEvent:{type:Function}
})

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
const CloseResultQuiz = () => {
    SelectedResultID.value = -1;
}
onMounted(async () => {
    Load();
})

const Load = async () => { 

    var grpID = -1;
    if (Props.GetGroupIDEvent) {
        grpID = Props.GetGroupIDEvent();
    }

    Data.value = await MNClient.ExecDataTable("QZ", "GETCLASSQUIZLIST", { PID: Props.CLASSSTDID, PGROUPID: grpID });
    
}



onUnmounted(() => { })
</script>

        <style scoped>
            .modal {
                display: block;
                background: rgba(0, 0, 0, 0.3);
            }

            .modal-dialog {
                max-width: 800px;
            }
        </style>
