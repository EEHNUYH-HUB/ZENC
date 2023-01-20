<template>
    <div :class="store.state.useIsMobile ? 'div_root_mobile' : 'div_root_default'">
        <div :class="store.state.useIsMobile ? 'div_mobile' : 'div_default'" class=" document-scroll-container">
            <Breadcrumb></Breadcrumb>

            <n-grid cols="1 s:1 m:1 l:2 xl:2 2xl:2" :x-gap="12" :y-gap="8" responsive="screen">
                <n-grid-item>
                    <n-card id="id1" title="File Upload" hoverable>
                        <n-space v-for="item,index in DownloadList" :key="index" vertical>
                            <n-space horizontal >
                                <n-button @click="Download(item.StaticID,item.Name)" >Download</n-button>
                                <a :href="item.Url" target="_blank">Download Link</a>
                                <a :href="item.ImageUrl" target="_blank">Image Link</a>
                                <n-p>{{ item.Name }}</n-p>
                            </n-space>
                        </n-space>
                        <Uploader class="n-button" @completed="uploadCompleted" @error="uploadError"></Uploader>
                    </n-card>
                </n-grid-item>
                <n-grid-item>
                    <n-card id="id2" title="File Upload Code" hoverable>
                        <n-code word-wrap code="
                    <Uploader Placement='right' Text='업로드' @completed='uploadCompleted' @error='uploadError'></Uploader>                    
<n-space v-for='item,index in DownloadList' :key='index' vertical>
    <n-space horizontal >
        <n-button @click='Download(item.StaticID,item.Name)' >Download</n-button>
        <a :href='item.Url' target='_blank'>Download Link</a>
        <a :href='item.ImageUrl' target='_blank'>Image Link</a>
        <n-p>{{ item.Name }}</n-p>
    </n-space>
</n-space>

import Uploader from '@/zenc/layout/components/Uploader.vue'
import {Download} from '@/zenc/js/Common'
const uploadCompleted =(item)=>{
    DownloadList.value.push(item);
}
const uploadError = (item) => {
    console.log(item)
}
" language="js" />
                    </n-card>
                </n-grid-item>
                <n-grid-item>
                    <n-card id="id3" title="File Download" hoverable>
                        <n-space v-for="item,index in DownloadList" :key="index" vertical>
                            <n-space horizontal >
                                <n-button @click="Download(item.StaticID,item.Name)" >Download</n-button>
                                <a :href="item.Url" target="_blank">Download Link</a>
                                <a :href="item.ImageUrl" target="_blank">Image Link</a>
                                <n-p>{{ item.Name }}</n-p>
                            </n-space>
                        </n-space>
                    </n-card>
                </n-grid-item>
                <n-grid-item>
                    <n-card id="id4" title="File Download Code" hoverable>
                        <n-code word-wrap code="
                        <n-space v-for='item,index in DownloadList' :key='index' vertical>
    <n-space horizontal >
        <n-button @click='Download(item.StaticID,item.Name)' >Download</n-button>
        <a :href='DownloadLink(item.StaticID)' target='_blank'>Download Link</a>
        <a :href='ImageLink(item.StaticID)' target='_blank'>Image Link</a>
        <n-p>{{ item.Name }}</n-p>
    </n-space>
</n-space>

import {Download,DownloadLink,ImageLink} from '@/zenc/js/Common'
const DownloadList = ref(new Array);
" language="js" />
                    </n-card>
                </n-grid-item>
            </n-grid>
        </div>
        <Anchor :Items="AnchorItems" DivID=".document-scroll-container"></Anchor>
    </div>
</template>
  
<script setup>
import { ref } from 'vue';
import { useStore } from "vuex";
import Uploader from "@/zenc/layout/components/Uploader.vue"
import Breadcrumb from "@/zenc/layout/components/Breadcrumb.vue"
import Anchor from "@/zenc/layout/components/Anchor.vue"
import {Download,DownloadLink,ImageLink} from '@/zenc/js/Common'
const store = useStore()
const IsRunUpload = ref(false)
const AnchorItems = ref(new Array);
const DownloadList = ref(new Array);
AnchorItems.value.push({ Title: 'File Upload', ID: '#id1' });
AnchorItems.value.push({ Title: 'File Upload Code', ID: '#id2' });
AnchorItems.value.push({ Title: 'File Download', ID: '#id3' });
AnchorItems.value.push({ Title: 'File Download Code', ID: '#id4' });
const uploadCompleted =(item)=>{
    DownloadList.value.push(item);
}
const uploadError = (item) => {
    console.log(item)
}

</script>
<style scoped>
.n-button {
    float: right;
}

.n-grid {
    margin-top: 24px;
}

.div_root_default {
    display: flex;
    flex-wrap: nowrap;
    padding: 32px 24px 56px 56px;
}

.div_root_mobile {
    display: flex;
    flex-wrap: nowrap;
    padding: 24px;
}

.div_default {
    width: calc(100% - 228px);
    margin-right: 36px;
}

.div_mobile {
    width: calc(100%);
}
</style>