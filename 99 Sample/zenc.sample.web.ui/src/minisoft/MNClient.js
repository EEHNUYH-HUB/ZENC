import axios from "axios";
import VueCookies from "vue-cookies";
import {HubConnectionBuilder} from "@microsoft/signalr";
import {setInterceptors} from '@/minisoft/MNCommon.js'
import Swal from "sweetalert2";

function createInstance(){
  const instance = axios.create({
    baseURL: process.env.VUE_APP_API_URL
  });

  return setInterceptors(instance);
}

const axiosService = createInstance();

const MNClient =  new Object;

MNClient.GetHubConnection = async function(hubName){
  const connection = new HubConnectionBuilder().withUrl(process.env.VUE_APP_API_URL+hubName).build();
  await connection.start();
  return connection;
}

// OTP 검증 후 토큰을 반환한다.
MNClient.GetCertification = async function(email, digit){
  const param = {
    parameters:{
      email : email
    }
  }

  const  rtn = await axiosService.post('/api/Auth/GetCertification', param);
  return rtn.data;
}

// 아이디와 비밀번호를 검증한다.
MNClient.Login = async function(email, pwd){
  const param = {
    parameters:{
      email : email,
      password : pwd
    }
  }

  const rtn = await   axiosService.post('/api/Auth/Login', param);
  return rtn.data;
}

MNClient.GetSecretKey = async function(email){
  const param = {
    parameters: {
      email : email
    }
  }

  const rtn = await axiosService.post('/api/OTP/GetSecretKey', param);
  return rtn.data;
}

MNClient.VerifyOTP = async function(email, secret, code){
  const rtn = await axiosService.post('/api/OTP/VerifyOTP?email='+email+'&secret='+secret+'&code='+code);
  return rtn.data;
}

MNClient.GenerateSecretKey = async function(){
  const rtn = await axiosService.post('/api/OTP/GenerateSecretKey', null);
  return rtn.data;
}
MNClient.SetOtp = async function(useOtp,skey){
  const rtn = await axiosService.post('/api/OTP/SetOtp?useotp='+useOtp+'&skey='+skey, null);
  return rtn.data;
}




MNClient.ExecDataTable= async function(scope, statementId, parameter){
  const param = {
    scope: scope,
    statementId : statementId,
    parameters: parameter
  }

  const  rtn = await  axiosService.post('/api/QueryMapper/ExecuteDataTable', param);
  
  return rtn.data;
}


MNClient.Run=async function(className,methodName,parma){
  const param ={
    assemblyName: "MINI.ONCLASS.BIZ",
    className: className,
    methodName: methodName,
    parameter: parma
  }


  const rtn = await axiosService.post('/api/Dynamic/LoadAssembly', param);
  

  return rtn;
}

MNClient.ExecDataSet=async function(scope, statementId, parameter){
  const param = {
    scope: scope,
    statementId : statementId,
    parameters: parameter
  }


  const  rtn = await  axiosService.post('/api/QueryMapper/ExecuteDataSet',param);
  

  return rtn.data;
}

MNClient.ExecScalar=async function(scope, statementId, parameter){
  const param = {
    scope: scope,
    statementId : statementId,
    parameters: parameter
  }

  const  rtn = await axiosService.post('/api/QueryMapper/ExecuteScalar', param);
  

  return rtn.data;
}

MNClient.ExecNonQuery =async function(scope, statementId, parameter){
  const param = {
    scope: scope,
    statementId : statementId,
    parameters: parameter
  }


  const rtn = await axiosService.post('/api/QueryMapper/ExecuteNonQuery', param);
  

  return rtn.data;
}

export default MNClient;
