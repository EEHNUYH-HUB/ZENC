<template>
  <main class="mt-0 main-content main-content-bg bg-gray-100">
    <section>
      <div class="page-header min-vh-75">
        <div class="container">
          <div class="row">
            <div class="mx-auto col-xl-4 col-lg-5 col-md-6 d-flex flex-column">
              <div class="mt-8 card card-plain">
                <div class="pb-0 card-header text-start bg-gray-100">
                  <h3 class="font-weight-bolder text-success text-gradient bg-gray-100">
                    Login
                  </h3>
                  <p class="mb-0 text-sm">로그인을 위해 아이디와 비밀번호를 입력해주세요.</p>
                </div>
                <div class="card-body">
                  <form role="form" class="text-start" v-on:submit.prevent="LoginUser">
                    <label>Email</label>
                    <VsudInput id="email" type="email" placeholder="Email" name="email" :isRequired="true"
                      :value="email" @update:value="newValue => email = newValue" />
                    <label>Password</label>
                    <VsudInput id="password" type="password" placeholder="Password" name="password" :isRequired="true"
                      :value="password" @update:value="newValue => password = newValue" />
                    <VsudSwitch id="rememberMe" name="rememberMe" :checked="rememberUser">
                      Remember me
                    </VsudSwitch>
                    <div class="text-center">
                      <VsudButton class="my-4 mb-2" variant="gradient" color="success" full-width>로그인
                      </VsudButton>
                    </div>
                  </form>
                </div>
                <!--<div class="px-1 pt-0 text-center card-footer px-lg-2">
                  <p class="mx-auto mb-4 text-sm">
                    계정이 없으신가요?
                    <router-link
                      :to="{ name: 'Sign Up' }"
                      class="text-success text-gradient font-weight-bold"
                      >회원가입</router-link
                    > 
                  </p>
                </div>-->
              </div>
            </div>
            <div class="col-md-6">
              <div class="top-0 oblique position-absolute h-100 d-md-block d-none me-n8">
                <div class="bg-cover oblique-image position-absolute fixed-top ms-auto h-100 z-index-0 ms-n6" :style="{
                    backgroundImage:
                      'url(' +
                      require('@/assets/img/curved-images/curved9.jpg') +
                      ')',
                  }"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  </main>
</template>

<script setup>
import VsudInput from "@/components/VsudInput.vue";
import VsudSwitch from "@/components/VsudSwitch.vue";
import VsudButton from "@/components/VsudButton.vue";
import {onMounted, ref, watch} from 'vue';

const emit = defineEmits(['login']);

let email = ref(''); // 이메일(ID)
let password = ref(''); // 비밀번호(Password)
let rememberUser = ref(true);

onMounted(()=>{
  email.value = localStorage.getItem("userId");
});

// 로그인 이벤틀들 발생시킨다.
function LoginUser() {
  if(rememberUser.value){
    localStorage.setItem("userId", email.value);
  }else{
    localStorage.removeItem("userId");
  }
  emit('login', email.value, password.value );
}

// 이메일과 비밀번호를 초기화시킨다.
function ResetInput(){
  email.value = localStorage.getItem("userId");
  password.value = null;
}

// 외부에 노출시킬 목록
defineExpose({
  ResetInput,
});
</script>

<style scope>
</style>