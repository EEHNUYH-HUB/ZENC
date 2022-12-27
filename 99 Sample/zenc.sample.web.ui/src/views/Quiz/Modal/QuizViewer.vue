<template>
    <div class="modal" tabindex="-1" role="dialog">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    {{ HeaderTitle }} - (남은시간 : {{ IntervalMin }})
                </div>
                <div class="modal-body ">
                    <div class="multisteps-form__content">
                        <div class="row">
                            <h6>{{ HeaderTitle }}명</h6>

                            <div class="ms-4 ">
                                <div class="input-group mt-2 text-info">
                                    <label>{{QuizTitle}}</label>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div style="height: calc(100vh - 420px);overflow-y: auto;overflow-x: hidden;">
                                <div class="row" v-for="item,index in QuizList" :key="index">
                                    <hr class="horizontal dark m-4" />
                                    <label>{{ index + 1}}번 ) {{ item.Text }} <template v-if="QuizType == 'QZ020101'">
                                            ({{item.Score}}점)</template></label>

                                    <div class="ms-4 " v-for="subItem,subIndex in item.SubItems" :key="subIndex">
                                        <div class="input-group mt-2 text-info" @click="SelectQuizItem(item, subIndex)">
                                            <label class="text-danger" v-if=" item.Answer == subIndex + 1">
                                                {{ subIndex +1}}. {{ subItem.Text }}</label>
                                            <label v-else>
                                                {{ subIndex +1}}. {{ subItem.Text }}</label>
                                        </div>
                                    </div>


                                </div>

                            </div>
                        </div>


                    </div>
                </div>
                <div class="modal-footer m-0">


                    <button class="btn btn-danger mx-1 m-0" @click="Save">저장</button>

                </div>
            </div>
        </div>
    </div>
</template>
<script setup>
import { ref, defineProps, onMounted } from 'vue'
import Swal from 'sweetalert2'
import MNClient from '@/minisoft/MNClient'
import { GetUser, RunStopWatch } from "@/minisoft/MNCommon"

const Props = defineProps({
    Quiz: { type: Object },
    CloseEvent :{type:Function}
})
const QuizList = ref(null);
const TotalScore = ref(0);
const BeforeScore = ref(0);
const HeaderTitle = ref("퀴즈");
const QuizType = ref("QZ020101");
const QuizTitle = ref(null);
const QuizLimitTime= ref(null);
const IntervalMin = ref(null);
const IntervalIntance = ref(null);
const QuizListID = ref(null);


onMounted(async () => { 

    
    if (Props.Quiz.pk_quizlist_id) {
        QuizListID.value = Props.Quiz.pk_quizlist_id;
        var usr = GetUser();
        var dt = await MNClient.ExecDataTable("QZ", "GETQUIZLISTDTLANSWER",
            { PID: Props.Quiz.pk_quizlist_id, PSTDNUMBER: usr.STUDENT_NUMBER });    

        if (dt) {
            QuizTitle.value = dt[0].quizlisttitle;
            QuizLimitTime.value = dt[0].col_limit_time;
            QuizType.value = dt[0].col_type;

            if (QuizType.value == "QZ020102") {
                HeaderTitle.value = "설문";
            }
            QuizList.value = new Array;

            var beforeQuiz = null;

            for (var i in dt) {
                var quizItem = dt[i];
                var quizId = quizItem.pk_quiz_id;
                if (quizId) {

                    if (!beforeQuiz || beforeQuiz.ID != quizId) {
                        beforeQuiz = GetQuizInstance();
                        beforeQuiz.ID = quizId;
                        beforeQuiz.Text = quizItem.col_question
                        beforeQuiz.Answer = quizItem.stdanswer; 
                        beforeQuiz.ProAnswer = quizItem.proanswer;
                        beforeQuiz.Score = quizItem.col_score;
                        BeforeScore.value -= beforeQuiz.Score;
                        QuizList.value.push(beforeQuiz);
                    }

                    var subItemText = quizItem.col_item;
                    if (subItemText) {
                        var subItem = GetSubItem();
                        subItem.Text = subItemText;
                        subItem.Index = quizItem.itemindex;
                        beforeQuiz.SubItems.push(subItem);
                    }

                }
            }
            IntervalIntance.value = RunStopWatch(new Date(dt[0].col_start_time), QuizLimitTime.value, (r, isCompleted) => {
                IntervalMin.value = r.Min + "분" + r.Sec + "초";
                if (isCompleted) {

                    Save();
                }
            });
            
        }
    }
    
})

const GetQuizInstance = () => {
    var rtn = new Object;
    rtn.ID = -1;
    rtn.SubItems = new Array;
    rtn.Answer = null;
    rtn.Score = null;
   
    return rtn;
}
function GetSubItem() {
    var subItem = new Object;
    subItem.Text = null;
    subItem.Index = null;
    return subItem;
}


const SelectQuizItem = (item, index) => {
    item.Answer = index + 1
}
const Save = async () => { 
    
    clearInterval(IntervalIntance.value);



    await MNClient.Run("QuizDA", "CreateQuizAnswer", { "quizListID": QuizListID.value, "quizList": QuizList.value,"time": new Date });    

    if (Props.CloseEvent)
    Props.CloseEvent();

}

</script>


<style scoped>

.modal {
    display: block;
    background: rgba(0, 0, 0, 0.3);
}

.modal-dialog {
    max-width: 820px;
}
</style>
