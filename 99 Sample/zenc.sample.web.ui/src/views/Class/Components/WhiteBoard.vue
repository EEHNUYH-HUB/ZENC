<template>
    

        <MNSvgPanel :Eventer="Eventer" ref="svgPanel">
        </MNSvgPanel>

    
</template>
<script setup>

import MNSvgPanel from '@/components/minisoft/MNSvgPanel.vue'
import { ref, defineProps ,onMounted} from 'vue'


const Props = defineProps({
    OnClassInstance: { type: Object }
})

onMounted(() => { 
    Props.OnClassInstance.OnSendObject = function (type, objs) {
        if (type == "SELECT") {
            svgPanel.value.RemoteUnSelectObj()
        }
        for (var i in objs) {
            if (type == "ADD") {
                svgPanel.value.RemoteAddObj(objs[i])               
            } else if (type == "CHANGE") {
                svgPanel.value.RemoteChangeObj(objs[i])
            } else if (type == "REMOVE") {
                svgPanel.value.RemoteRemoveObj(objs[i])
            } else if (type == "SELECT") {
                svgPanel.value.RemoteSelectObj(objs[i])
            }
        }
    }
})

const svgPanel = ref();
const Eventer = ref(null);
Eventer.value = new Object;
Eventer.value.AddedMethod = function (obj) {
    Props.OnClassInstance.SendObj("ADD",[obj]);
}
Eventer.value. ChangedMethod = function (objs) {
    Props.OnClassInstance.SendObj("CHANGE", objs);
}
Eventer.value.RemovedMethod = function (objs) {
    Props.OnClassInstance.SendObj("REMOVE", objs);
}
Eventer.value.SelectedMethod = function (objs) {
    Props.OnClassInstance.SendObj("SELECT", objs);
}
</script>