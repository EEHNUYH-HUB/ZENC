<template>
    <template v-if="ImgID && ImgID > 0">
        <div class="avatar" :style="'width:' + Width + 'px;height:' + Height +'px'">
            <img :src="ImgUrl" class="col" />
        </div>
    </template>
    <template v-else>
        <svg xmlns="http://www.w3.org/2000/svg" :width="Width" :height="Height" fill="currentColor"
            class="bi bi-person-bounding-box" viewBox="0 0 16 16">
            <path
                d="M1.5 1a.5.5 0 0 0-.5.5v3a.5.5 0 0 1-1 0v-3A1.5 1.5 0 0 1 1.5 0h3a.5.5 0 0 1 0 1h-3zM11 .5a.5.5 0 0 1 .5-.5h3A1.5 1.5 0 0 1 16 1.5v3a.5.5 0 0 1-1 0v-3a.5.5 0 0 0-.5-.5h-3a.5.5 0 0 1-.5-.5zM.5 11a.5.5 0 0 1 .5.5v3a.5.5 0 0 0 .5.5h3a.5.5 0 0 1 0 1h-3A1.5 1.5 0 0 1 0 14.5v-3a.5.5 0 0 1 .5-.5zm15 0a.5.5 0 0 1 .5.5v3a1.5 1.5 0 0 1-1.5 1.5h-3a.5.5 0 0 1 0-1h3a.5.5 0 0 0 .5-.5v-3a.5.5 0 0 1 .5-.5z" />
            <path d="M3 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H3zm8-9a3 3 0 1 1-6 0 3 3 0 0 1 6 0z" />
        </svg>
    </template>
</template>
<script setup>
import { ref, onMounted } from 'vue';
import MNClient from '@/minisoft/MNClient.js'
const props = defineProps({
    UserID: {   type: Number },
    Width: { type: Number ,default:64},
    Height: { type: Number, default: 64 }
})
const ImgID = ref(null);
const ImgUrl = ref(null);
onMounted(async () => { 
    
    Load();
    
})

const Load = async () => {

    ImgID.value = await MNClient.ExecScalar("Common", "GETUSERIMGID", { PID: parseInt(props.UserID) });

    ImgUrl.value =
        process.env.VUE_APP_API_URL
        + "api/Image/GetUserImageHandler?id="
        + ImgID.value;

}

defineExpose({Load})

</script>