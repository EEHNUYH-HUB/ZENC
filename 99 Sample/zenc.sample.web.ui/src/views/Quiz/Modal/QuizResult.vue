<template>
    <div class="modal" tabindex="-1" role="dialog">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    결과보기 - {{ HeaderTitle }}
                </div>
                <div class="modal-body">

                    <PieChart v-for="item,index in ChartData" :key="index" :CanvasID="item.CanvasID" :Data="item.Data"
                        :Labels="item.Labels" :Title="item.Question" :TotalCnt="item.TotalCnt"
                        :CenterText1="item.CenterText1" :CenterText2="item.CenterText2"
                        :RightPanelData="item.RightPanelData">
                    </PieChart>

                </div>
                <div class="modal-footer">
                    <button class="btn btn-primary m-0" @click="CloseEvent()">닫기</button>
                </div>
            </div>
        </div>
    </div>
</template>
<script setup>

import { ref, onMounted } from 'vue'
import MNClient from '@/minisoft/MNClient'
import { Guid } from "@/minisoft/MNCommon";
import PieChart from '@/views/Quiz/Components/PieChart.vue'
const Props = defineProps({
    CloseEvent: { type: Function },
    QUIZLISTID : {type:String}
})
const HeaderTitle = ref("");
const ChartData = ref(null)
onMounted(async() => { 
    var ds = await MNClient.ExecDataSet("QZ", "GETRESULTQUIZLIST", { PID: Props.QUIZLISTID})
    

    ChartData.value = new Array;
    var beforeItem = null;
    var beforeMsxCnt = -1;
    if (ds && ds.table && ds.table.length > 0) {
        HeaderTitle.value = ds.table[0].col_title;
        for (var i in ds.table) {
            var row = ds.table[i];
            if (!beforeItem || beforeItem.ID != row.pk_quiz_id) {
                beforeMsxCnt = -1;
                beforeItem = new Object;
                beforeItem.ID = row.pk_quiz_id;
                beforeItem.Question = row.col_question;
                beforeItem.CanvasID = "canvasID_" + Guid();
                beforeItem.TotalCnt = row.totalcnt;
                beforeItem.Labels = new Array;
                beforeItem.Data = new Array;
                beforeItem.RightPanelData = new Array;
                ChartData.value.push(beforeItem);
                
            }
            
            if (row.col_type == 'QZ020101') {
                if (row.isok) {
                    beforeItem.CenterText1 = row.col_index;
                    beforeItem.CenterText2 = "정답"; //row.col_item;
                }
            }
            else {
                
                if (beforeMsxCnt < row.itemcnt) {
                    beforeMsxCnt = row.itemcnt;
                    beforeItem.CenterText1 = row.col_index;
                    beforeItem.CenterText2 = "선택";
                }
            }
            beforeItem.Labels.push(row.col_index + ". " + row.rate + "%");
            beforeItem.Data.push(row.itemcnt);

            var ritem = new Object;
            ritem.Title = row.col_index + ". " + row.col_item + " (" + row.itemcnt + "/" + row.totalcnt + ")";
            ritem.ID = row.col_index;
            ritem.Rate = row.rate;
            ritem.IsOK = row.isok;
            
            
            ritem.SubItem = new Array;
            if (ds.table1 && ds.table1.length > 0) {
                for (var j in ds.table1) {
                    var usr = ds.table1[j];
                    if (usr.fk_quiz_id == beforeItem.ID && usr.col_answer == ritem.ID) {
                        ritem.SubItem.push(usr.col_name+" ("+usr.pk_student_number+")");
                    }
                }
            }

            beforeItem.RightPanelData.push(ritem);
            


        }


        
    }
})

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

