<template >

    <div class="py-4">
        <div class="card  ">
            <div class="card-header ">
                <header-menu :OnClassInstance="OnClassInstance"> </header-menu>
            </div>

            <div class="card-body container-fluid  pt-0 pb-0">
                <div class="row pt-7">
                    <div class="col-lg-10">
                        <div class="col row " style="height: calc(100vh - 192px);overflow-y: auto">
                            <video playsinline controls autoplay
                                v-show="!OnClassInstance.IsShowWhiteBoard && !OnClassInstance.IsStartGroup"
                                style="height: calc(100vh - 205px);"
                                :src-object.prop.camel="OnClassInstance.StreamList.length>0?OnClassInstance.StreamList[0].Stream:null"></video>

                            <WhiteBoard :OnClassInstance="OnClassInstance" v-if="OnClassInstance.IsShowWhiteBoard">
                            </WhiteBoard>
                            <GroupRoom :OnClassInstance="OnClassInstance" v-if="OnClassInstance.IsStartGroup"
                                v-show="!OnClassInstance.IsShowWhiteBoard">
                            </GroupRoom>
                        </div>
                    </div>
                    <RightPanel class="col-lg-2  my-2" :OnClassInstance="OnClassInstance"> </RightPanel>
                </div>
            </div>
        </div>
    </div>
    <div class=" position-fixed bottom-1 start-1 z-index-2">
        <CommentAlert v-for="(item, index) in OnClassInstance.NotifyItems" :key="index" :data="item"
            :icon="{ component: 'ni ni-check-bold', color: 'success' }" color="white" />
    </div>
    <QuizViewer v-if="OnClassInstance.Quiz" :Quiz="OnClassInstance.Quiz" :CloseEvent="CloseEvent">
    </QuizViewer>
    <ModalDataDownload v-if="OnClassInstance.IsVisibleFileList" :scheduleID="OnClassInstance.ClassID"
        @hide="OnClassInstance.IsVisibleFileList = false" />
    <FaceChecker v-if="OnClassInstance.IsVisibleAttendance || OnClassInstance.IsVisibleAttitude"
        :OnClassInstance="OnClassInstance" @hide="OnClassInstance.HideFaceChecker()">

    </FaceChecker>
</template>

<script setup>
import HeaderMenu from '@/views/Class/Components/HeaderMenu'
import RightPanel from '@/views/Class/Components/RightPanel.vue'
import WhiteBoard from "@/views/Class/Components/WhiteBoard.vue";
import GroupRoom from "@/views/Class/Components/GroupRoom.vue";
import QuizViewer from '@/views/Quiz/Modal/QuizViewer.vue'
import CommentAlert from '@/views/Class/Components/CommentAlert'
import ModalDataDownload from '@/views/Common/Modal/ModalDataDownload.vue';
import FaceChecker from "@/views/Class/Components/FaceChecker.vue";

import { ref, onMounted, onUnmounted } from 'vue';
import { useRoute } from 'vue-router';
import MNOnClassMedia from '@/views/Class/js/MNOnClassMedia'
import { useStore } from 'vuex'


const route = useRoute();
const id = route.query.id; 
const store = useStore();

const OnClassInstance = ref(new MNOnClassMedia(false));
const CloseEvent = () => { 
    OnClassInstance.value.Quiz = null
};
onMounted(async () => {
    store.commit("toggleEveryDisplay");
    store.commit("toggleHideConfig");
    store.state.containerCss = "container-fluid bg-dark";
     
    await OnClassInstance.value.Init(id);
    OnClassInstance.value.Snapshot.UseSnapshot = true;
    
})
onUnmounted(() => {
    store.commit("toggleEveryDisplay");
    store.commit("toggleHideConfig");
})

</script>
