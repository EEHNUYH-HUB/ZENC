<template>
    <div class="modal" tabindex="-1" role="dialog">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    {{ QuizType == 'QZ020101' ? '퀴즈' :'설문' }} 생성
                </div>
                <div class="modal-body ">
                    <div class="multisteps-form__content">
                        <div class="row">
                            <div class="mt-4 mt-sm-0">
                                <label>{{ QuizType == 'QZ020101' ? '퀴즈' :'설문' }} 명</label>
                                <input class=" form-control" type="text" v-model="QuizTitle"
                                    placeholder="퀴즈명을 입력해주시길 바랍니다." />

                                <label class="mt-2">제한시간(분)</label>
                                <input class=" form-control" type="text" v-model="QuizLimitTime" @input="CheckIsNumber" @keyup.delete="CheckIsNumber"
                                    placeholder="제한시간을 설정해주시길 바랍니다." />
                                <p class="text-xs text-danger mt-2 mx-2" v-show="displayLimitTimeWarning" >숫자만 입력이 가능합니다.</p>
                                <label class="mt-2">타입</label>
                                <select class="form-control" v-model="QuizType" :disabled="DisableQuizType">
                                    <option value="QZ020101" selected="">퀴즈</option>
                                    <option value="QZ020102">설문</option>
                                </select>
                            </div>
                        </div>

                        <div class="row">
                            <!-- <div style="height: calc(100vh - 420px);overflow-y: auto;overflow-x: hidden;"> -->
                            <div>
                                <div class="row" v-for="item,index in QuizList" :key="index">
                                    <hr class="horizontal dark m-4" />
                                    <label>{{ index + 1}}번</label>
                                    <div>
                                        <div class="input-group">

                                            <input v-model="item.Text" type="text" class="form-control"
                                                placeholder="질문을 입력해주시길 바랍니다." aria-label="질문을 입력해주시길 바랍니다."
                                                aria-describedby="basic-addon1">
                                            <span class="input-group-text" id="basic-addon1"
                                                @click="RemoveItem(QuizList, index)">
                                                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
                                                    fill="currentColor" class="bi bi-x" viewBox="0 0 16 16">
                                                    <path
                                                        d="M4.646 4.646a.5.5 0 0 1 .708 0L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 0 1 0-.708z">
                                                    </path>
                                                </svg>
                                            </span>
                                        </div>
                                        <div class="ms-4 " v-for="subItem,subIndex in item.SubItems" :key="subIndex">
                                            <div class="input-group mt-2">

                                                <input v-model="subItem.Text" type="text"
                                                    class="form-control text-danger" v-if="(item.Answer-1) == subIndex"
                                                    placeholder="답변문을 입력해주시길 바랍니다." aria-label="답변문을 입력해주시길 바랍니다."
                                                    aria-describedby="basic-addon1">
                                                <input v-else v-model="subItem.Text" type="text" class="form-control"
                                                    placeholder="답변문을 입력해주시길 바랍니다." aria-label="답변문을 입력해주시길 바랍니다."
                                                    aria-describedby="basic-addon1">
                                                <span class="input-group-text" id="basic-addon1"
                                                    @click="RemoveItem(item.SubItems,subIndex)">
                                                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16"
                                                        fill="currentColor" class="bi bi-x" viewBox="0 0 16 16">
                                                        <path
                                                            d="M4.646 4.646a.5.5 0 0 1 .708 0L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 0 1 0-.708z">
                                                        </path>
                                                    </svg>
                                                </span>
                                            </div>
                                        </div>

                                        <div class="mt-2 row text-right"
                                            style="justify-content:flex-end;display: flex;">
                                            <label class="btn m-0 col col-2 text-danger" style="margin-top:12px"
                                                @click="AddSubItem(item)">답변추가</label>
                                            <label class="col col-2" v-if="QuizType == 'QZ020101'"
                                                style="width:48px;margin-top:12px;justify-content:flex-end">답변</label>
                                            <select class="form-control col col-2 form-control text-end"
                                                v-if="QuizType == 'QZ020101'"
                                                style="width:60px;justify-content:flex-end" v-model="item.Answer">
                                                <option v-for="subItem,index in item.SubItems" :key="index+1"
                                                    :value="index+1" selected="">{{index+1}}
                                                </option>

                                            </select>
                                            <label class="col col-2" v-if="QuizType == 'QZ020101'"
                                                style="width:48px;margin-top:12px;justify-content:flex-end">배점</label>
                                            <input class="col mx-2 col-2 form-control text-end" v-model="item.Score"
                                                v-if="QuizType == 'QZ020101'"
                                                style="width:60px;justify-content:flex-end" type="text" placeholder="배점"
                                                @input="CheckIsScoreNumber(item.Score)"
                                                @blur="ReSetScore()" />
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>


                    </div>
                </div>
                <div class="modal-footer m-0">
                    <div class="" style="justify-content:flex-start;display: flex;">
                        <label class="btn m-0 text-danger" style="margin-top:12px" @click="AddItem">질문 추가 <template
                                v-if="QuizType == 'QZ020101'">(배점:
                                {{TotalScore}}/15)
                            </template></label>

                    </div>

                    <button class="btn btn-danger mx-1 m-0" @click="Save">{{ IsEdit ?"수정":"생성"}}</button>
                    <button class="btn btn-primary mx-1 m-0" @click="CloseEvent(false)">닫기</button>

                </div>
            </div>
        </div>
    </div>
</template>
<script setup>
import { ref, defineProps, onMounted } from 'vue'
import Swal from 'sweetalert2'
import MNClient from '@/minisoft/MNClient'
const Props = defineProps({
    CLASSSTDID: { type: String },
    QUIZLISTID: { type: String },
    QUIZMODE : {type:String},
    GetGroupIDEvent: { type: Function },
    CloseEvent :{type:Function}
})
const QuizList = ref(null);
const QuizType = ref("QZ020101");
const DisableQuizType = ref(false);
const TotalScore = ref(0);
const BeforeScore = ref(0);
const GroupID = ref(-1);

const QuizTitle = ref(null);
const QuizLimitTime= ref(null);
const QuizPublic = ref(true);

const IsEdit = ref(false);

let displayLimitTimeWarning = ref(false);
let textScoreCnt = ref(0);

onMounted(async () => { 
    BeforeScore.value = await MNClient.ExecScalar("QZ", "GETTOTALSCORE", { PID: Props.CLASSSTDID });    
    if (Props.GetGroupIDEvent)
        GroupID.value = Props.GetGroupIDEvent();

    QuizList.value = new Array;

    if (Props.QUIZLISTID) {
        IsEdit.value = true;
        var dt = await MNClient.ExecDataTable("QZ", "GETQUIZLISTDTL", { PID: Props.QUIZLISTID });    

        if (dt) {
            QuizTitle.value = dt[0].quizlisttitle;
            QuizLimitTime.value = dt[0].col_limit_time;
            QuizType.value = dt[0].col_type;
            

            var beforeQuiz = null;

            for (var i in dt) {
                var quizItem = dt[i];
                var quizId = quizItem.pk_quiz_id;
                if (quizId) {

                    if (!beforeQuiz || beforeQuiz.ID != quizId) {
                        beforeQuiz = GetQuizInstance();
                        beforeQuiz.ID = quizId;
                        beforeQuiz.Text = quizItem.col_question
                        beforeQuiz.Answer = quizItem.col_answer;
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

            
        }
    }
    else {
        AddItem();

    }
    ReSetScore();
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
const AddItem = () => {
    var item = GetQuizInstance();
    item.SubItems.push(GetSubItem());
    item.SubItems.push(GetSubItem());
    QuizList.value.push(item);
}
const AddSubItem = (item) => {
    
    item.SubItems.push(GetSubItem());

    QuizList.value = QuizList.value;
}
const RemoveItem = (item,index) => {
    item.splice(index, 1);
}
const ReSetScore = () => {
    TotalScore.value = BeforeScore.value;
    
    if (BeforeScore.value >= 15 && !IsEdit.value) {
        QuizType.value = "QZ020102";
        DisableQuizType.value = true;

    }

    if (IsEdit.value) {
        DisableQuizType.value = true;
    }

    if (GroupID.value && GroupID.value > 0) {
        QuizType.value = "QZ020102";
        DisableQuizType.value = true;
    }
    for (var i in QuizList.value) {
        var item = QuizList.value[i];
        if (item.Score) {
            TotalScore.value += item.Score *1;
        }
    }
    if (Props.QUIZMODE) {
        if (Props.QUIZMODE == "ONLYQUIZ") {
            QuizType.value = "QZ020101";
            DisableQuizType.value = true;
        }
        else if (Props.QUIZMODE == "ONLYSURVEY") {
            QuizType.value = "QZ020102";
            DisableQuizType.value = true;
        }
    }
}

const Save = async () => { 
    var msg = Validation();
    if (msg) {
        Swal.fire({
            text: msg,
            icon: 'info'
        });
        return;
    }

    if (IsEdit.value) {
        await MNClient.Run("QuizDA", "UpdateQuiz", { "classSchID": Props.CLASSSTDID, "quizListID": Props.QUIZLISTID, "quizTitle": QuizTitle.value, "limitTime": QuizLimitTime.value, "isPublic": QuizPublic.value, "quizList": QuizList.value, "time": new Date });    
    }
    else {
        await MNClient.Run("QuizDA", "CreateQuiz",
            { "classSchID": Props.CLASSSTDID, "quizTitle": QuizTitle.value, "limitTime": QuizLimitTime.value, "isPublic": QuizPublic.value, "quizList": QuizList.value, "quizType": QuizType.value, "time": new Date, "groupID": GroupID.value });    
    }
    Props.CloseEvent(true);

}

const Validation = () => {
  if(displayLimitTimeWarning.value){
    return '제한시간은 숫자만 입력가능합니다.';
  }

  if(textScoreCnt.value != 0){
    return '배점은 숫자만 입력가능합니다. 질문에 대한 배점을 확인해주세요';
  }

  if(QuizList.value && QuizList.value.length == 0){
    return '질문이 최소 한개 이상 있어야합니다.';
  }

    if (!QuizTitle.value) {
        return "퀴즈명을 입력해 주세요.";
    }
    if (!QuizLimitTime.value) {
        return "제한시간을 설정해 주세요.";
    }

    ReSetScore();
    
    if (QuizType.value == "QZ020101" && TotalScore.value > 15) {
        return "최대 배점을 초과 하였습니다.";
    }


    for (var i in QuizList.value) {
        var item = QuizList.value[i];

        var number = (i * 1) + 1;
        if (!item.Text) {
            return number +"번 질문을 입력해 주세요."
        }

        var cnt = item.SubItems.length ;
        if (cnt < 2) {
            return number + "번 질문의 답변은 2개 이상  입력해 주세요."
        }

        if (cnt > 5 && QuizType.value == "QZ020101") {
            return number + "번 질문의 답변은 5개까지 입력 가능합니다."
        }

        for (var j in item.SubItems) {
            var subItem = item.SubItems[j];
            if (!subItem.Text) {
                return number + "번 질문의 답변을 입력해 주세요."
            }
        }

        if (!item.Score && QuizType.value == "QZ020101") {
            return number + "번 질문의 배점을 입력해 주세요."
        }
        

        if (item.Answer > cnt) {
            item.Answer = null;
        }
        
        if (!item.Answer && QuizType.value == "QZ020101") {
            return number + "번 질문의 답변을 선택해 주세요."
        }
    }


    return null;
}

let check = /^[0-9]+$/;
const CheckIsNumber = (e) =>{
  if(!QuizLimitTime.value){
    displayLimitTimeWarning.value = false;
    return;  
  }

  if(!check.test(QuizLimitTime.value)){
    displayLimitTimeWarning.value = true;
    return;
  }
  displayLimitTimeWarning.value = false;
}

const CheckIsScoreNumber = (value) => {
  if(!value){
    textScoreCnt.value--;
    return;
  }

  if(!check.test(value)){
    textScoreCnt.value++;
    return;
  }
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
