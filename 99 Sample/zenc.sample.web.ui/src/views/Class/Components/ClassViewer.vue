<template>
    <div class="modal " tabindex="-1" role="dialog">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    강의 영상
                </div>
                <div class="modal-body text-xs">
                    <h6 class="mb-0">{{ ClassScdItem.subjectname}}-{{ ClassScdItem.col_few_weeks }}주차 강의</h6>
                    <div style="height:500px" class="m-3">
                        <div class="mb-2" style="padding-left:20px">
                            <ul class="nav nav-tabs card-header-tabs">
                                <li v-for="item,index in Data" :key="index" class="nav-item" @click="NavTab = index">
                                    <a class="nav-link" :class="NavTab == index ? 'active' : ''" aria-current="true">
                                        {{ 1+index }}. {{ item.col_filename}}</a>
                                </li>
                            </ul>
                        </div>
                        <div class="video-wrapper h-100">
                            <video class="video" :src="Data[NavTab].url + Guid()" v-if="Data && Data.length > 0" playsinline autoplay
                                controls></video>
                        </div>
                    </div>
                </div>
                <div class="modal-footer mt-4">
                    <button class="btn btn-primary mx-1 m-0" @click="CloseEvent()">닫기</button>
                </div>
            </div>
        </div>
    </div>
</template>
<script setup>
import { onMounted ,ref} from 'vue'
import MNClient from '@/minisoft/MNClient.js';
import { Guid } from "@/minisoft/MNCommon"
import Swal from 'sweetalert2';
const Props = defineProps({
    ClassItem: { type: Object },
    CloseEvent: { type: Function }
})

const Data = ref(null);
const NavTab = ref(0);
const ClassScdItem = ref(Object);
onMounted(async () => {

    const videoList = await MNClient.ExecDataTable("CLASS", "CLASSVIDEO",   { PID: Props.ClassItem.pk_class_schedule_id, PDATE: new Date });
    const classScd = await MNClient.ExecDataTable("CLASS", "CLASSSCHEDULEDTL", { PID: Props.ClassItem.pk_class_schedule_id });
    
    if (classScd) {
        ClassScdItem.value = classScd[0];
    }
    if (videoList && videoList.length> 0) {
        for (var i in videoList) {
            videoList[i].url = process.env.VUE_APP_API_URL + 'api/downloader/video?id=' + videoList[i].col_solidocs_document_id+"&guid=";
        }
        Data.value = videoList;
    }
    else {
        Swal.fire({
            text: '강의 영상이 없습니다.',
            icon: 'warning'
        });

        Props.CloseEvent();
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
    background: #111;
    /* Random BG colour for unloaded video elements */
}
</style>
