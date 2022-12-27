<template>
    <div class="modal" id="modal-otp-input" tabindex="1111" role="dialog">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    {{ HeaderTitle }}
                    <span v-if="!Props.OnClassInstance.IsVisibleAttendance"
                        class="position-flex text-left text-sm">{{Sec}}초후
                        감점</span>
                </div>
                <div class="modal-body text-center">
                    <UserImg :UserID="OnClassInstance.UserInfo.USER_ID" Width="200" Height="200" class="mt-5 pb-0">
                    </UserImg>
                </div>
                <div class="modal-footer">

                    <button class="btn btn-primary m-0" variant="gradient" color="dark" size="lg"
                        :disabled="!IsProgress" @click="CheckFace">인증
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>
<script setup>
import UserImg from "@/views/Common/UserImg.vue";

import Swal from 'sweetalert2'
import { ref,onMounted } from "vue";
const Props = defineProps({
    OnClassInstance: { type: Object }
})
const IsProgress = ref(true);
const HeaderTitle = ref("");
const emit = defineEmits(['hide']);
const Sec = ref(60);
const InterverHandle = ref(null);

const CheckFace = () => { 
    IsProgress.value = false;
    Sec.value = 60;
    Props.OnClassInstance.Attitude.CheckFace(Props.OnClassInstance.MyStream, Props.OnClassInstance.IsVisibleAttendance, (r) => {

        if (r == 0) {
         Hide();   
        }
        else if (r == 1) {

            IsProgress.value = true;
            Swal.fire({
                text: '등록된 얼굴과 다릅니다.',
                icon: 'info'
            });

        }
        else {
            IsProgress.value = true;
            Swal.fire({
                text: '얼굴을 인식 할 수 없습니다.',
                icon: 'info'
            });

        }
    });

}
onMounted(() => { 
    if (Props.OnClassInstance.IsVisibleAttendance) {
        HeaderTitle.value = "출석 체크를 위해 얼굴을 인증 해주세요.";   
    }
    else {
        HeaderTitle.value = "자리 비움 상태 입니다.재인증 해주세요.";

        Sec.value = 60;

        InterverHandle.value = setInterval(async () => {
            if (IsProgress.value) {
                Sec.value -= 1;
                if (Sec.value <= 0) {
                    Props.OnClassInstance.DeductUser();
                    Hide();
                }
            }

        }, 1000);

    }

})

const Hide = () => {
    if (InterverHandle.value)
        clearInterval(InterverHandle.value);
    InterverHandle.value = null;
    emit('hide');
}
</script>
<style scoped>

.modal {
  display: block;
  background:rgba(0,0,0,0.3);
}

.register:hover{
  cursor:pointer;
}
</style>