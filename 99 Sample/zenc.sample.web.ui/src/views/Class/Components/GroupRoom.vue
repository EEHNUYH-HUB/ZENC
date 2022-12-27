<template>
    <div class="radios  row">
        <div class=" col-lg-2" v-for="(item, index) in GroupInstance.StreamList" :key="index">
            <div class="p-1  border-radius-lg" :class="false ? 'bg-primary' : ''">
                <div class="card m-1 ">
                    <div class="card-header p-3 pb-0  ">
                        <p class="text-sm mb-0 text-capitalize ">
                            {{
                            item.StreamerInfo.USER_NAME + " - " + item.StreamerInfo.STUDENT_NUMBER
                            }}
                        </p>
                    </div>
                    <div class="card-body border-radius-lg p-3 row ">
                        <video class=" col" playsinline controls :src-object.prop.camel="item.Stream"></video>

                    </div>
                </div>
            </div>
        </div>
    </div>
    <QuizList v-if="OnClassInstance.IsShowQuiz" :CloseEvent="CloseQuizList" :CLASSSTDID="OnClassInstance.ClassID"
        :GetGroupIDEvent="GetGroupIDEvent" :ProgressEvent="RunQuiz">

    </QuizList>
</template>
<script setup>
import QuizList from '@/views/Quiz/Modal/QuizList.vue'
import { ref, defineExpose, defineProps, onMounted ,onUnmounted} from 'vue';
import MNClient from '@/minisoft/MNClient'
import MNOnClassGroup from '../js/MNOnClassGroup';


const Props = defineProps({
    OnClassInstance: { type: Object },
    GroupID: { type: String }
})




const GroupInstance = ref(Object);
const GetGroupIDEvent = (() => { 
    var grpID = -1;
    if (Props.OnClassInstance && Props.OnClassInstance.GroupInstance && Props.OnClassInstance.GroupInstance.GroupID)
    {
        grpID = Props.OnClassInstance.GroupInstance.GroupID;
    }
    return grpID;

})
const CloseQuizList = (r, quizId) => { 
  Props.OnClassInstance.CloseQuizList(r, quizId);
  
}

const RunQuiz = async (r) => {
    await Props.OnClassInstance.RunQuiz(r);
  Props.OnClassInstance.CloseQuizList(false,-1);
}

onMounted(async () => { 

    GroupInstance.value = new MNOnClassGroup(Props.OnClassInstance);
    
    if (Props.GroupID && Props.GroupID > 0) {
        
        GroupInstance.value.Init(Props.GroupID);    
    }
    else {
        var dt = await MNClient.ExecDataTable("GRP", "GETCLASSGROUP"
            , { "PCLASSID": Props.OnClassInstance.ClassID, "PSTDNUMBER": Props.OnClassInstance.UserInfo.STUDENT_NUMBER });

        if (dt && dt.length > 0) {
            GroupInstance.value.Init(dt[0].pk_classgroup_id);   
        }


    }
    
    
    
})

const RequestComment = () => { 
    GroupInstance.value.RequestComment();
}

defineExpose({ RequestComment })
onUnmounted(() => { 
    GroupInstance.value.Dispose();
    GroupInstance.value = null;
    Props.OnClassInstance.GroupInstance = null;
    
})


</script>
<style >


.video-wrapper {
  position: relative;
  padding: 20px;
  height: 0;
}

.video {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
background: #000;
    /* Random BG colour for unloaded video elements */
}
</style>