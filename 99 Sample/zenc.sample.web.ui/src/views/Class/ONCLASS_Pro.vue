<template>
  <div class="py-4">
    <div class="card ">
      <div class="card-header">
        <header-menu :OnClassInstance="OnClassInstance"> </header-menu>
      </div>

      <div class="card-body container-fluid pt-0 pb-0">
        <div class="row pt-7">
          <div class="col-lg-10">
            <div class=" col row" style="height: calc(100vh - 192px);overflow-y: auto">
              <UserThumb v-if="!OnClassInstance.IsShowWhiteBoard && !OnClassInstance.IsShowGroupSetting"
                :OnClassInstance="OnClassInstance">
              </UserThumb>
              <WhiteBoard :OnClassInstance="OnClassInstance" v-if="OnClassInstance.IsShowWhiteBoard">
              </WhiteBoard>
              <GroupSetting :OnClassInstance="OnClassInstance" v-if="OnClassInstance.IsShowGroupSetting"
                v-show="!OnClassInstance.IsShowWhiteBoard">
              </GroupSetting>

            </div>
          </div>
          <RightPanel class="col-lg-2 my-2" :OnClassInstance="OnClassInstance"></RightPanel>
        </div>
      </div>
    </div>
  </div>

  <div class=" position-fixed bottom-1 start-1 z-index-2">
    <CommentAlert v-for="(item, index) in OnClassInstance.NotifyItems" :key="index" :data="item"
      :icon="{ component: 'ni ni-check-bold', color: 'success' }" color="white" />
  </div>


  <QuizList v-if="OnClassInstance.IsShowQuiz" :CLASSSTDID="OnClassInstance.ClassID" :ProgressEvent="RunQuiz"
    :CloseEvent="CloseQuizList"></QuizList>
  <UploadProgress v-if="OnClassInstance && OnClassInstance.UploadBlob != null" :File="OnClassInstance.UploadBlob"
    :Item="OnClassInstance.ClassItem" :CloseEvent="CloseUploadProgress" FileType="DC010102"></UploadProgress>
  <ModalDataUpload v-if="OnClassInstance.IsVisibleFileList" :folderID="OnClassInstance.ClassItem.col_solidocs_folder_id"
    :scheduleID="OnClassInstance.ClassID" @hide="OnClassInstance.IsVisibleFileList =false">
  </ModalDataUpload>
</template>

<script setup>
import HeaderMenu from '@/views/Class/Components/HeaderMenu'
import CommentAlert from '@/views/Class/Components/CommentAlert'
import UserThumb from "@/views/Class/Components/UserThumb.vue";
import RightPanel from "@/views/Class/Components/RightPanel.vue";
import WhiteBoard from "@/views/Class/Components/WhiteBoard.vue";
import GroupSetting from "@/views/Class/Components/GroupSetting.vue";
import QuizList from '@/views/Quiz/Modal/QuizList.vue'
import ModalDataUpload from '@/views/Common/Modal/ModalDataUpload.vue'
import UploadProgress from '@/views/Common/Modal/UploadProgress.vue'

import { ref, onMounted, onUnmounted } from "vue";
import { useRoute } from "vue-router";
import MNOnClassMedia from "@/views/Class/js/MNOnClassMedia";
import { useStore } from "vuex";


const route = useRoute();

const id = route.query.id; 
const store = useStore();
const OnClassInstance = ref(new MNOnClassMedia(true)); 
const CloseQuizList = (r, quizId) => { 
  OnClassInstance.value.CloseQuizList(r, quizId);
  
}
const CloseUploadProgress = (r) => {
  OnClassInstance.value.UploadBlob = null;
}

const RunQuiz = async (r) => {
  await OnClassInstance.value.RunQuiz(r);
  OnClassInstance.value.CloseQuizList(false,-1);
}
onMounted(async () => {
  store.commit("toggleEveryDisplay");
  store.commit("toggleHideConfig");
  store.state.containerCss = "container-fluid bg-dark";
  await OnClassInstance.value.Init(id);
  
})
onUnmounted(() => {
  store.state.containerCss = "container-fluid bg-white";
  store.commit("toggleEveryDisplay");
  store.commit("toggleHideConfig");
})


</script>
