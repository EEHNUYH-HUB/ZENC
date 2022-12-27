<template>
  <div class="modal" id="modal-otp-input" tabindex="-1" role="dialog">
    <div class="modal-dialog modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-header">
          OTP를 입력 해주세요
        </div>
        <div class="modal-body text-left">
          <div id="otp" class="inputs d-flex flex-row justify-content-center">
            <input class="m-2 text-center form-control rounded" type="number" id="first" maxlength="1" v-model="first"
              ref="firstFocus" @input="FocusSecond()" @keyup.delete="DeleteInputData('first')" />
            <input class="m-2 text-center form-control rounded" type="number" id="second" maxlength="1" v-model="second"
              ref="secondFocus" @input="FocusThird()" @keyup.delete="DeleteInputData('second')" />
            <input class="m-2 text-center form-control rounded" type="number" id="third" maxlength="1" v-model="third"
              ref="thirdFocus" @input="FocusFourth()" @keyup.delete="DeleteInputData('third')" />
            <input class="m-2 text-center form-control rounded" type="number" id="fourth" maxlength="1" v-model="fourth"
              ref="fourthFocus" @input="FocusFifth()" @keyup.delete="DeleteInputData('fourth')" />
            <input class="m-2 text-center form-control rounded" type="number" id="fifth" maxlength="1" v-model="fifth"
              ref="fifthFocus" @input="FocusSixth()" @keyup.delete="DeleteInputData('fifth')" />
            <input class="m-2 text-center form-control rounded" type="number" id="sixth" maxlength="1" v-model="sixth"
              ref="sixthFocus" @input="VerifyCode()" @keyup.delete="DeleteInputData('sixth')" />
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-primary m-0" @click="HideModal(false)">닫기</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import {ref, onMounted} from 'vue';
import Swal from 'sweetalert2';
import MNClient from '@/minisoft/MNClient.js'

import {useStore} from 'vuex';


const store = useStore();
const props = defineProps({
  email: {
    type: String
  }, secretKey: {
    type: String
    
  }
});

const emit = defineEmits([
  'hide', 
  'showOTPGenModal'
]);

let first = ref('');
let second = ref('');
let third = ref('');
let fourth = ref('');
let fifth = ref('');
let sixth = ref('');

let firstFocus = ref();
let secondFocus = ref();
let thirdFocus = ref();
let fourthFocus = ref();
let fifthFocus = ref();
let sixthFocus = ref();

onMounted(()=>{
  firstFocus.value.focus();
})

// 입력된 코드를 초기화한다.
function InitCode(){
  first.value = '';
  second.value = '';
  third.value = '';
  fourth.value = '';
  fifth.value = '';
  sixth.value = '';
  firstFocus.value.focus();
}

// OTP를 등록하는 모달이 나오도록 이벤트를 발생시킨다.
function ShowOTPGenModal(){
  emit('showOTPGenModal');
}

// 입력된 OTP 코드를 검증하도록 API를 호출한다.
async function VerifyCode(){
  const code = first.value + '' + second.value + '' + third.value + '' + fourth.value + '' + fifth.value + '' + sixth.value;


  if(code == null || code == '' || code.length < 6){
    Swal.fire({ text: 'OTP코드를 제대로 입력해주세요', icon: 'warning' });
    return;
  }

  var res = await MNClient.VerifyOTP(props.email, props.secretKey, code);
  if(res == null){
    Swal.fire({ text: '응답이 없습니다.', icon: 'warning' });
    InitCode();
    return;
  }
  if (!res.result) {
    Swal.fire({ text: res.msg, icon: 'warning' });
    InitCode();
    return;    
  }



  HideModal(true)

}

// OTP 입력 모달을 닫기 위해 이벤트를 발생시킨다.
function HideModal(r){
  emit('hide',r);
}

// 두번째 input에 포커스를 준다.
function FocusSecond(){
  secondFocus.value.focus();
}

// 세번째 input에 포커스를 준다.
function FocusThird(){
  thirdFocus.value.focus();
}

// 네번째 input에 포커스를 준다.
function FocusFourth(){
  fourthFocus.value.focus();
}

// 다섯번째 input에 포커스를 준다.
function FocusFifth(){
  fifthFocus.value.focus();
}

// 마지막 input에 포커스를 준다.
function FocusSixth(){
  sixthFocus.value.focus();
}

function DeleteInputData(order){
  if(order=='first' || order=='second'){
    first.value = '';
    firstFocus.value.focus();
  }else if(order=='third'){
    second.value='';
    secondFocus.value.focus();
  }else if(order=='fourth'){
    third.value='';
    thirdFocus.value.focus();
  }else if(order=='fifth'){
    fourth.value='';
    fourthFocus.value.focus();
  }else if(order=='sixth'){
    fifth.value='';
    fifthFocus.value.focus();
  }
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