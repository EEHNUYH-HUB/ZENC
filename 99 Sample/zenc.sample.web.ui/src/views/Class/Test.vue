<template>
    <div class="card">
        <div class="card-body">
            <div class="row h-100">
                <div class="col col-10 h-100">
                    <video playsinline controls autoplay :src-object.prop.camel="Strm" style="height: 200px"></video>
                    <video playsinline controls autoplay :src-object.prop.camel="TStrm" style="height: 200px"></video>
                </div>
                <div class="row">
                    <div class="col col-6">
                        <button class="d-flex btn btn-primary " @click="CreateOffer">Create Offer</button>
                        <div v-for="item,index in InfoList" :key="index">
                            <label class="mt-2">{{item.Key}}</label>
                            <input v-model="item.Value" type="text" class="form-control" placeholder="답변문을 입력해주시길 바랍니다."
                                aria-label="답변문을 입력해주시길 바랍니다." aria-describedby="basic-addon1">
                        </div>

                    </div>
                    <div class="col col-6">

                        <select class="form-control" v-model="Key">
                            <option value="SetSDP" selected="">SetSDP</option>
                            <option value="SetICE">SetICE</option>
                        </select>
                        <input v-model="Value" type="text" class="form-control mt-2" placeholder="Value를 입력해주시길 바랍니다."
                            aria-label="Value를 입력해주시길 바랍니다." aria-describedby="basic-addon1">
                        <button class="btn btn-primary  mt-2" @click="CreateAnswer">OK</button>

                        <div v-for="item,index in OnClassInstance.MsgItems" :key="index">
                            <p class=" mb-1" style="font-size:12px"
                                v-if="item.Sender.USER_NAME != OnClassInstance.UserInfo.USER_NAME">
                                {{item.Sender.USER_NAME}} <input v-model="item.Content" type="text"
                                    class="form-control">
                                <button class="btn btn_outline_primary" @click="CreateAnswer(item.Content)">OK</button>
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script setup>

import { ref, onMounted, onUnmounted } from 'vue';
import MNWebRTC from '@/minisoft/MNWebRTC'
import { GetUser, GetTime, GetUserMedia, RunStopWatch } from "@/minisoft/MNCommon"
import MNOnClassMedia from '@/views/Class/js/MNOnClassMedia'

const WebRTC = ref(null)
const Strm = ref(null)
const TStrm = ref(null)
const InfoList = ref(null)
const UserInfo = ref(null)
const Key = ref("SetSDP")
const Value = ref("")
const id = ref(99)
const OnClassInstance = ref(new MNOnClassMedia(false));


const CreateOffer = async () => { 
    alert(OnClassInstance.value.UserInfo.USER_NAME)
    await WebRTC.value.Run(id.value,  Strm.value);
    
}
const CreateAnswer = async (v) => { 

    if (v) {
        Value.value = v;
    }
    
    if (Key.value == "SetSDP") {
        await WebRTC.value.SetSDP(id.value, Value.value, Strm.value)
    }
    else if (Key.value == "SetICE") {
        WebRTC.value.SetICE(id.value, Value.value)
    }
    
    Key.value = "SetICE";
    Value.value = "";

}
onMounted(async () => {
    await OnClassInstance.value.Init(id.value);
    
    InfoList.value = new Array;
    Strm.value = await GetUserMedia()
    WebRTC.value = new MNWebRTC();
    WebRTC.value.OnAddStream = function (event, senderID) {

        if (event.streams) {
            TStrm.value = event.streams[0];
            
        }
    }
    WebRTC.value.OnRemoveStream = function () {
        console.log("OnRemoveStream");
    }
    WebRTC.value.OnICE = function (ice, senderID) {
        InfoList.value.push({ Key: "ICE", Value: ice })

        OnClassInstance.value.SendMsg(ice);
    }
    WebRTC.value.OnSDP = function (sdp, senderID) {
        
        InfoList.value.push({ Key: "SDP", Value: sdp })
        OnClassInstance.value.SendMsg(sdp);
    }     

})
</script>
