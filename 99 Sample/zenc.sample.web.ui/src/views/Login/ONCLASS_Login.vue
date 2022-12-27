<template>
  <MNLogin @login="LoginUser" ref="loginComponent">
  </MNLogin>
  <TLModalOTPInput v-if="displayOtpInputModal" :secretKey="SecretKey" :email="emailId" @hide="HideOtpInputModal"
    @showOTPGenModal="ShowOtpGenModal">
  </TLModalOTPInput>
  <TLModalOTPGen v-if="displayOtpGenModal" :email="emailId" @hide="HideOtpGenModal">
  </TLModalOTPGen>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import MNLogin from '@/views/Login/Components/MNLogin.vue'
import MNClient from '@/minisoft/MNClient.js'
import TLModalOTPGen from '@/views/Login/Components/TLModalOTPGen.vue'
import TLModalOTPInput from '@/views/Login/Components/TLModalOTPInput.vue'
import Swal from 'sweetalert2';
import { useStore } from 'vuex';
import jwt_decode from 'jwt-decode';
import router from '@/router';
const store = useStore();
const SecretKey = ref(null);
onMounted( () => {
  store.commit("toggleEveryDisplay", false);
  store.commit("toggleHideConfig");

})

let emailId = ref(null);
let displayOtpInputModal = ref(false);
let displayOtpGenModal = ref(false);
const loginComponent = ref();

// 로그인 API를 호출시켜 로그인을 진행한다.
async function LoginUser(email, password) {
  // 1. 아이디 패스워드 공백 확인
  if (CheckAccountEmpty(email, password) == false) {
    return;
  }

  // 2. 아이디 패스워드 검증
  let login = MNClient.Login(email, password);
  emailId.value = email;
  login.then(
    async (loginSuccess) => {
      if (!loginSuccess) {
        Swal.fire({
          text: '아이디나 비밀번호가 일치하지 않습니다.',
          icon: 'warning'
        });

        return;
      }


      var rst = await MNClient.GetSecretKey(email);
      if (rst) {
        SecretKey.value = rst;
        displayOtpInputModal.value = true;
      }
      else
        HideOtpInputModal(true);

    }
  )
    .catch(
      (error) => {
        Swal.fire({
          text: error,
          icon: 'warning'
        });
      }
    );

}

// 아이디 패스워드 공백 확인
function CheckAccountEmpty(email, password) {
  if (email == null || email == undefined || email.length == 0) {
    Swal.fire({
      text: '아이디를 입력해주세요',
      icon: 'warning'
    });

    return false;
  }

  if (password == null || password == undefined || password.length == 0) {
    Swal.fire({
      text: '비밀번호를 입력해주세요',
      icon: 'warning'
    });

    return false;
  }

  return true;
}

// OTP를 입력받는 모달을 닫는다.
function HideOtpInputModal(r) {
  displayOtpInputModal.value = false;
  if (r) {
    MNClient.GetCertification(emailId.value).then((token) => {
      if (token == null) {
        throw '토큰을 가져오다가 문제가 발생하였습니다.';
      }

      let decoded = jwt_decode(token);
      decoded.token = token;
      console.log(decoded)
      store.commit("setUserData", decoded);
      store.commit("toggleEveryDisplay", true);
      router.push('/dashboard');
    });

  }
  else {
    loginComponent.value.ResetInput();
  }
}

// OTP를 등록하는 모달을 연다.
function ShowOtpGenModal() {
  displayOtpGenModal.value = true;
}

// OTP를 등록하는 모달을 닫는다.
function HideOtpGenModal() {
  displayOtpGenModal.value = false;
}

</script>
