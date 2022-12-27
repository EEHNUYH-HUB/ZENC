<template>
  <div v-if="!isAuth">
    <Navbar css="bg-transparents text-white shadow-none">

    </Navbar>
    <div class="page-header align-items-start min-vh-50 pt-5 pb-11 m-3 border-radius-lg" :style="{
      backgroundImage:
        'url(' + require('@/assets/img/curved-images/curved8.jpg') + ')',
    }">
      <span class="mask bg-gradient-dark opacity-6"></span>
    </div>
    <div class="container">
      <div class="row mt-lg-n10 mt-md-n11 mt-n10 justify-content-center">
        <div class="col-xl-4 col-lg-5 col-md-7 mx-auto">
          <div class="card py-lg-3">
            <div class="card-body text-center">
              <div class="info mb-4">
                <UserImg :UserID="userInfo.USER_ID"></UserImg>
              </div>
              <h4 class="mb-0 font-weight-bolder">사용자 관리</h4>
              <p class="mb-4">비밀번호를 입력해주시길 바랍니다.</p>
              <div class="mb-3">
                <vsud-input type="password" placeholder="Password" name="password" :isRequired="true" :value="password"
                  @update:value="newValue => password = newValue" />
              </div>
              <div class="text-center">
                <vsud-button class="mt-3 mb-0" variant="gradient" color="dark" size="lg" @click="CheckPassword">인증
                </vsud-button>
              </div>

            </div>
          </div>
        </div>

        <Footer>

        </Footer>
      </div>
    </div>
  </div>
  <div v-else>
    <div class="card">
      <div class="card-body">
        <div class="row">
          <div class="mt-sm-0">
            <label>1. 사용자 이미지</label>

            <div class="text-center  mt-3">
              <UserImg :UserID="userInfo.USER_ID" ref="UserImgCtrl" Width="128" Height="128"></UserImg>
            </div>
            <label class="mt-2">2. 사용자 이미지 저장</label>
            <button class="form-control btn btn-primary " @click="SaveImage">저장</button>
            <input class="btnFileLoad d-none" type="file" ref="oFileHandler" @change="FileChangeHandler" />
          </div>
        </div>
      </div>
    </div>
    <div class="card mt-4">
      <div class="card-body ">
        <div class="row">
          <div class="mt-sm-0">
            <label>1. OTP 설정</label>
            <VsudSwitch @click="OnCheckedOTP" :checked="UseOTP">
              <template v-if="UseOTP">사용</template>
              <template v-else>사용안함</template>

            </VsudSwitch>

            <div v-if="UseOTP">
              <label class="mt-2">2. OTP 비밀키</label>
              <input v-model="SecretKey" type="text" class="form-control text-danger" disabled>
              <label class="mt-2">3. OTP 등록</label>
              <VueQrcode v-if="QRCodeUri !=null" :value="QRCodeUri" class="text-center"
                :qrOptions="{ typeNumber: '0', mode: 'Byte', errorCorrectionLevel: 'Q' }"
                :imageOptions="{ hideBackgroundDots: true, imageSize: 0.4, margin: 0 }"
                :dotsOptions="{ type: 'square', color: '#05004d' }"
                :cornersSquareOptions="{ type: 'square', color: '#0e013c' }" />


            </div>
            <label class="mt-2">{{ UseOTP ?"4":"2"}}. OTP 등록 후 저장</label>
            <button class="form-control btn btn-primary " @click="IsShowOtpInput= true">저장</button>
          </div>
        </div>
      </div>
    </div>
  </div>
  <TLModalOTPInput v-if="IsShowOtpInput" :email="userInfo.USER_EMAIL" :secretKey="SecretKey" @hide="CloseOtpInput">
  </TLModalOTPInput>
</template>

<script setup>
import { ref, defineProps, onMounted, onUnmounted } from 'vue'
import { useStore } from "vuex"
import { GetUser } from "@/minisoft/MNCommon"
import MNClient from '@/minisoft/MNClient.js'
import TLModalOTPInput from '@/views/Login/Components/TLModalOTPInput.vue'
import Navbar from "@/views/Common/Navbar.vue";
import Footer from "@/views/Common/Footer.vue";
import VsudInput from "@/components/VsudInput.vue";
import VsudButton from "@/components/VsudButton.vue";
import VueQrcode from "qrcode-vue3";
import VsudSwitch from "@/components/VsudSwitch.vue";
import Swal from 'sweetalert2';
import router from '@/router';
import MNFileUploader from '@/minisoft/MNFileUploader'
import UserImg from "@/views/Common/UserImg.vue";

const store = useStore();
const password = ref("");
const userInfo = ref("");
userInfo.value = GetUser();
const isAuth = ref(false);
const UseOTP = ref(false);
const SecretKey = ref(null);
const QRCodeUri = ref(null);
const IsShowOtpInput = ref(false);
const oFileHandler = ref();
const UserImgCtrl = ref();
onMounted(async () => { 
  store.commit("toggleEveryDisplay");
  
  
  store.state.containerCss = "container-fluid bg-gray-100 m-0";
  store.state.navigatorArray = new Array;
  store.state.navigatorArray.push({ Path: 'Dashboard', DisplayName: "대쉬보드" })
  store.state.currentName = "사용자 관리";
})
onUnmounted(() => {
  if (!isAuth.value) {
    store.state.containerCss = "container p-0 ";
    store.commit("toggleEveryDisplay",false);
  }
  
})
const FileChangeHandler=async (e) =>{

  var uploader = new MNFileUploader();
  uploader.CompletedEvent = async (staticName) => {
    await MNClient.Run('FileHandler', 'CreateUser', {
      staticName: staticName
    });

    UserImgCtrl.value.Load();
  }

  var fileNmae = "";
  if (userInfo.value.ROLE == 'STUDENT') {
    fileNmae = userInfo.value.STUDENT_NUMBER;
  }
  else {
    fileNmae = userInfo.value.PROFESSOR_NUMBER;
  }
  await uploader.Upload(e.target.files[0], fileNmae+".jpg");
}
const SaveImage =  () => {
  oFileHandler.value.click();
}
const CloseOtpInput =async (r) => {
  IsShowOtpInput.value = false;
  if (r) {

    await MNClient.SetOtp(UseOTP.value, SecretKey.value)
    router.push('/dashboard');
  }
}


const OnCheckedOTP =  async () => {
  UseOTP.value = !UseOTP.value;

  if (UseOTP.value && !SecretKey.value) {
    var rst = await MNClient.GenerateSecretKey();

    if (rst && rst.result) {
      SecretKey.value = rst.msg;
      QRCodeUri.value = `otpauth://totp/onclass?secret=${SecretKey.value.replace(/ /g, '')}`;
    }
    else {
      UseOTP.value = false;
      Swal.fire({
        text: rst.msg,
        icon: 'warning'
      });
    }
  }
}
const CheckPassword = async () => {
  
  if (password.value) {

    var isLogin = await MNClient.Login(userInfo.value.USER_EMAIL, password.value);
    if (!isLogin) {
      Swal.fire({
        text: '비밀번호가 일치하지 않습니다.',
        icon: 'warning'
      });
      return;
    }
    else {

      isAuth.value = true;
      store.state.containerCss = "container p-0 ";
      store.commit("toggleEveryDisplay");
      var rst = await MNClient.GetSecretKey(userInfo.value.USER_EMAIL);
      if (rst) {
        UseOTP.value = true;
        SecretKey.value = rst;
        QRCodeUri.value = `otpauth://totp/onclass?secret=${SecretKey.value.replace(/ /g, '')}`;
      }
      else {
        UseOTP.value = false;
      }
    }
  }
  else {
    Swal.fire({
      text: '비밀번호를 입력해주시길 바랍니다.',
      icon: 'warning'
    });

    return;
  }


  
}
</script>
