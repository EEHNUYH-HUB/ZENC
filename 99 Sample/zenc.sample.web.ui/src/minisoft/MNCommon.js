import store from '@/store/index.js'
import { createObjectExpression } from '@vue/compiler-core';
import { useStore } from "vuex"
import adapter from 'webrtc-adapter'
import moment from 'moment';
import 'moment/locale/ko';

export function GetUser() {
  const store = useStore();
  const user = store.state.user;
  
  return user;
}

export function CheckIsStudent(){
  let user = GetUser();

  if(user && user.STUDENT_NUMBER){
    return true;
  }

  return false;
}

export function CheckIsProfessor(){
  let user = GetUser();

  if(user && user.PROFESSOR_NUMBER){
    return true;
  }

  return false;
}

export function GetDate(date, delimiter){
  if(!date){
    date = new Date();
  }

  if(!delimiter){
    delimiter = '-';
  }

  var mm = date.getMonth() + 1; // getMonth() is zero-based
  var dd = date.getDate();

  return [date.getFullYear(),
          (mm>9 ? '' : '0') + mm,
          (dd>9 ? '' : '0') + dd
         ].join(delimiter);
}

export function Guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}

// Axios �서 api�청 보낼�더token�는
export function setInterceptors(instance) {
  instance.interceptors.request.use(
    function(config) {
      config.headers.Authorization = store.state.user ? store.state.user.token : '';
      
      return config;
    },
    function(error) {
      return Promise.reject(error);
    },
  );

  // Add a response interceptor
  instance.interceptors.response.use(
    function(response) {
      return response;
    },
    function(error) {
      if (error && error.response && error.response.status == 401){
        store.dispatch('logout');
      }
      return Promise.reject(error);
    },
  );

  return instance;
}

export function GetTime(date) {
  var d = new Date(date);
    
  var strh = d.getHours();
  var strm = d.getMinutes();
  if (strh < 10) {
    strh = "0"+strh;
  }

  var tmp = "오전 ";
  if (strm > 12) {
    strm = strm - 12;
    tmp = "오후 ";
  } 

  if (strm < 10) {
    strm = "0"+strm;
  }
    
  return tmp + strh + "시" + strm +"분";
}

export function GetFullTime(date){
  return new moment(date).format('YYYY년 MM월 DD일 A HH시mm분');
}


export function RunStopWatch(startDt,intervalMin,prg) {    
  var instance = setInterval(() => {
      var r = StopWatch(startDt, intervalMin);
      var isCompleted = (r.Min + r.Sec) < 0;
      if (prg) {
          prg(r,isCompleted);
      }
      
      if (isCompleted) {
          clearInterval(instance);
          instance = null;
      }

  }, 1000);
  return instance;
}
    
export function StopWatch(startDt, intervalMin) {
  
  
  
  var curdt = new Date;

  var startSec = curdt.getTime() / 1000;
  var endSec = startDt.getTime() / 1000 + (intervalMin * 60);
  
  var time = endSec - startSec;
       //console.log(time+ "초"); 
  
  var minutes = Math.floor(time / 60);
          
  var secd = time % 60;
  var seconds = Math.ceil(secd);

  var rtn = new Object;
  rtn.Min = minutes;
  rtn.Sec = seconds
  
  return rtn ;
}
export async function GetUserMedia() {
  adapter.browserShim.shimGetUserMedia(window, "screen"); // or "window"

  return await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
}

export function GetRect(x1, y1, x2, y2) {
  var rect = new Object;

  var minX = x1 > x2 ? x2 : x1;
  var minY = y1 > y2 ? y2 : y1;
  var maxX = x1 > x2 ? x1  : x2 ;
  var maxY = y1 > y2 ? y1  : y2 ;
  
  rect.X = minX;
  rect.Y = minY;
  rect.Width = maxX - rect.X;
  rect.Height = maxY - rect.Y;

  
  rect.ScaleX = 1;
  rect.ScaleY = 1;
  rect.Rotate = 0;
  rect.SubRects = new Array;

  var size = 10;
  
  rect.SubRects.push(GetSubRect(rect.Width / 2 - 5, -30, size, size,"TR"));//top_roate
  rect.SubRects.push(GetSubRect(rect.Width / 2 -1 , -20, 2, 20,"TRL"));//top_roate_line

  rect.SubRects.push(GetSubRect(-5, rect.Height / 2 - 5, size, size,"LC"));//left
  rect.SubRects.push(GetSubRect( rect.Width-5,rect.Height/2-5,size,size,"RC"));//rigth
  rect.SubRects.push(GetSubRect( rect.Width/2-5,-5,10,10,"TC"));//top
  rect.SubRects.push(GetSubRect(rect.Width / 2 - 5, rect.Height - 5, size, size,"BC"));//bottom
  
  rect.SubRects.push(GetSubRect( -5,-5,size,size,"LT"));
  rect.SubRects.push(GetSubRect( rect.Width-5,-5,size,size,"RT"));
  rect.SubRects.push(GetSubRect( -5,rect.Height-5,size,size,"LB"));
  rect.SubRects.push(GetSubRect(rect.Width-5, rect.Height - 5, size, size,"RB"));
  
  
  function GetSubRect(x, y, w, h,type){
    var subRect = new Object;
    subRect.X = x;
    subRect.Y = y;
    subRect.Width = w;
    subRect.Height = h;
    subRect.Type = type;
    
    return subRect;
  }
  return rect;
} 

export function GetCenterforRect(rect) {
  var p = new Object;
  p.X = rect.Width / 2 + rect.X;
  p.Y = rect.Height / 2 + rect.Y;
  return p;
}

export function GetOffsetPoint(e) {
  var rtn =new Object;
   rtn.offsetX = e.offsetX;
    rtn.offsetY =  e.offsetY;
    if (e.touches) {
        rtn.offsetX = e.touches[0].clientX;
        rtn.offsetY = e.touches[0].clientY;
    }
    return rtn;
}

export function GetRandom(end) {
  
  let rtn = []; let i = 0; while (i < end) {
    let n = Math.floor(Math.random() * end) ; if (!sameNum(n)) { rtn.push(n); i++; }
  }
  function sameNum(n) {
    for (var i = 0; i < rtn.length; i++)
    { if (n === rtn[i]) { return true; } } return false;
  } return rtn;
}



export  function BindingApiKeySend(url, type, complete) {
  var httpReq = new XMLHttpRequest();
  
  
  if (type != null && type != undefined && type.length > 0) {
        //httpReq.responseType = type; //so you can access the response like a normal URL
  
    httpReq.onloadstart = function (ev) {
      httpReq.responseType = type;
        
        
    }
    
  }
  
  httpReq.onreadystatechange = function () {
  
    if (httpReq.readyState == XMLHttpRequest.DONE && httpReq.status == 200) {
  
      complete(httpReq);
  
    }
  };
  
  httpReq.open("GET", url, true);
  httpReq.setRequestHeader('Authorization', store.state.user ? store.state.user.token : ''); 
  httpReq.send(null);
}
