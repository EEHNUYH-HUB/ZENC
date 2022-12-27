<template>
  <div class="modal" id="modal-otp-input" tabindex="-1" role="dialog">
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-header">
          OTP 등록하기
        </div>
        <div class="modal-body text-center">
          <p class="mb-4 mx-auto text-sm">
            OTP를 등록하는 방법을 모르시나요?
            <router-link :to="{ name: 'ShowManual' }" target="_blank"
              class="text-success text-gradient font-weight-bold">등록방법 알아보기</router-link>
          </p>
          <VueQrcode v-if="qrcodeUri!=null" :value="qrcodeUri"
            :qrOptions="{ typeNumber: '0', mode: 'Byte', errorCorrectionLevel: 'Q' }"
            :imageOptions="{ hideBackgroundDots: true, imageSize: 0.4, margin: 0 }"
            :dotsOptions="{ type: 'square', color: '#05004d' }"
            :cornersSquareOptions="{ type: 'square', color: '#0e013c' }" />
        </div>
        <div class="modal-footer m-0">
          <button class="btn btn-success mx-1 m-0" @click="HideModal()">닫기</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from "vue";
import VueQrcode from "qrcode-vue3";
import MNClient from '@/minisoft/MNClient.js'
import Swal from 'sweetalert2'

const emit = defineEmits([
  'hide',
]);

const props = defineProps({
  email: {
    type: String
  }
})

let qrcodeUri = ref(null);

onMounted(()=>{
  // 해당 사용자의 secret key를 가져온다.
  MNClient.GetSecretKey(props.email).then((res) => {
    if(res == null){
      throw '응답이 null입니다.';
    }
    if(!res.result){
      throw res.msg;
    }

    let secret = res.msg;

    qrcodeUri.value = `otpauth://totp/onclass?secret=${secret.replace(/ /g, '')}`;

  } ).catch((error) => {
    Swal.fire({
      text: error,
      icon: 'error'
    });
  });
});


// OTP를 등록하는 모달을 닫는다.
function HideModal(){
  emit('hide');
}

</script>

<style scoped>
.modal {
  display: block;
  background:rgba(0,0,0,0.3);
}

</style>