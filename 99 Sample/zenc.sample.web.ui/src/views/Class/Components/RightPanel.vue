<template>
    <div class="card text-center text-xs p-0">
        <div class="" style="padding-left:20px">
            <ul class="nav nav-tabs card-header-tabs">
                <li class="nav-item" @click="NavTab = 0">
                    <a class="nav-link" :class="NavTab == 0 ? 'active' : ''" aria-current="true" href="#">내 화면</a>
                </li>
                <li v-for="item,index in OnClassInstance.StreamList" :key="index" class="nav-item"
                    @click="NavTab = index">
                    <a class="nav-link" :class="NavTab == index ? 'active' : ''" aria-current="true" v-if="index!=0"
                        href="#">{{item.StreamerInfo.USER_NAME}}</a>
                </li>
            </ul>
        </div>
        <div class=" card-body">
            <div class="video-wrapper " style="min-height:200px">
                <video class=" video " playsinline controls autoplay :src-object.prop.camel="OnClassInstance.MyStream"
                    v-if="NavTab==0"></video>
                <video class="video " playsinline controls autoplay v-else
                    :src-object.prop.camel="OnClassInstance.StreamList.length >NavTab?OnClassInstance.StreamList[NavTab].Stream:null "></video>
            </div>

            <hr class="horizontal dark mt-0 mb-4" />
            
                <div style="height: calc(100vh - 600px);overflow-y: auto"
                    class="list-group-item border-0 d-flex p-3 mb-2 bg-gray-100 border-radius-lg mt-0 mb-0">
                    <div class="d-flex flex-column text-sm  text-sm-start text-wrap">
                        <div v-for="item,index in OnClassInstance.MsgItems" :key="index">
                            <p class=" mb-1" style="font-size:12px">
                                <template v-if="item.Sender "> {{item.Sender.USER_NAME}} :
                                </template>
                                {{ item.Content }}
                            </p>
                        </div>

                    </div>
                </div>

            
            <div class="md-form mt-0">
                <textarea id="form7" @keyup.enter="SendMsg()" v-model="Msg" class="col md-textarea form-control"
                    rows="3"></textarea>

            </div>
        </div>
    </div>
</template>
<script setup>
import { ref, defineProps,  onMounted } from 'vue'


const Props = defineProps({
    OnClassInstance: { type: Object }
})

onMounted(async () => {
    Props.OnClassInstance.OnNewStreamer = () => { 
        var tmp = Props.OnClassInstance.StreamList.length * 1 -1;

        if (tmp != NavTab.value) {
            NavTab.value = tmp;
        }
    }
});

const NavTab = ref(0);
const Msg = ref("");

const SendMsg = () => { 
    if (event.keyCode === 13) {
        var m = Msg.value;

        if (m.length > 0) {
            Props.OnClassInstance.SendMsg(m);
        }
        Msg.value = "";
    }

};


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