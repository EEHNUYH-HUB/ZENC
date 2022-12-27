import MNWebRTC from '@/minisoft/MNWebRTC'
import MNClient from '@/minisoft/MNClient'

import MNSnapshot from '@/views/Class/js/MNSnapShot'
import MNAttitude from '@/views/Class/js/MNAttitude'

import {  GetUser,GetTime ,GetUserMedia,RunStopWatch } from "@/minisoft/MNCommon"

import Swal from 'sweetalert2'
import RecordRTC from 'recordrtc'

export default class MNOnClassMedia {

    constructor(isOwner){
        
        this.ConnectionID = "";
        this.ClassID = -1;
        this.Hub = null;
        
        this.StreamList = new Array;
        this.SharedStream = null;
        this.Rebinging = false;
        this.Snapshot = null; //new MNSnapshot();
        this.Attitude = null; //new MNAttitude();
        
        this.WebRTCList = null;
        this.UserList = null;
        this.InterverHandle = null;
        this.Cnt = 0;
        this.UserInfo = GetUser();
        
        
        this.OnNewStreamer = null;
        this.OnShowWhiteBoard = null;
        this.OnSendObject = null;
        this.OnChangeUserList = null;
        this.OnRqtGroupComment = null;
        this.OnCancelGroupComment = null;
        this.OnResponseEndGroup = null;
        this.OnLoadData = null;
        

        this.NotifyItems = new Array;
        this.MsgItems = new Array;
        this.MyStream = null;
        this.ClassItem = new Object;
        
        this.IsOwner = isOwner;

        this.IsStart = false;        
        this.IsShowWhiteBoard = false;
        this.IsShowGroupSetting = false;
        this.IsStartGroup = false;
        this.IsShowShared = false;
        this.IsAttitude = false;
        this.IsRqtComment = false;
        this.IsRqtGroupComment = false;
        this.IsAudio = true;
        this.IsVideo = true;
        this.IsShowQuiz = false;
        this.IsRunRecord = false;
        this.UploadBlob = null;
        

        this.GroupInstance = null;
        
        
        this.IsVisibleAudio = false;
        this.IsVisibleVideo = false;
        this.IsVisibleRecord = false;
        this.IsVisibleWhiteboard = false;
        this.IsVisibleShare = false;
        this.IsVisibleGroup = false;
        
        this.IsVisibleQuiz = false;
        this.IsVisibleAttitude = false;
        this.IsVisibleAttendance = true;

        this.IsVisibleComment = false;
        this.IsVisibleGroupComment = false;
        this.IsVisibleStart = false;
        this.IsVisibleClose = false;
        this.IsVisibleFileList = false;
        

        this.Quiz = null;
        

        this.Recorder = null;
        
        this.ModeType = "";
        this.SetMode("DEFAULT");
    }
    GetWebRTC(streamerid) {
        if (!this.WebRTCList) {
            this.WebRTCList = {};
        }


        var rtn = this.WebRTCList[streamerid];
        if (!rtn) {
            rtn= new MNWebRTC();
            var _self = this;
            rtn.OnAddStream = function (event, senderID) {
                
                if (event.streams) {
                    _self.BindingStream(streamerid, event.streams[0]);
                
                    _self.Hub.send("Send", senderID, streamerid, "AfterBindingStream", '');
                }
            }
            rtn.OnRemoveStream = function () {
                console.log("OnRemoveStream");
            }
            rtn.OnICE = function (ice, senderID) {
                
                _self.Hub.send("Send", senderID, streamerid, "ICE", ice);
            }
            rtn.OnSDP = function (sdp, senderID) {
                
                _self.Hub.send("Send", senderID, streamerid, "SDP", sdp);
            }       
            rtn.ParentID = streamerid;
            this.WebRTCList[streamerid] = rtn;

        }

        return rtn;
    }
    async Init(id) {
        this.ClassID = Number(id);
        const dt = await MNClient.ExecDataTable("CLASS", "CLASSSCHEDULEDTL", { PID: this.ClassID });
        if (dt) {
            this.ClassItem = dt[0];
            
            if (this.OnLoadData) {
                this.OnLoadData();
            }
            
        }
        
        this.Hub = await MNClient.GetHubConnection(process.env.VUE_APP_CLASSHUB_NAME);

        this.Snapshot = new MNSnapshot(this.ClassID,this.Hub);
        this.Attitude = new MNAttitude(this.ClassID,this.Hub);

        this.ConnectionID = this.Hub.connectionId;
        var _self = this;
        this.Hub.on("Receive", async (senderID, streamerid, command, param) => {
            var rtc = _self.GetWebRTC(streamerid);
            
            if (command == "SDP") {
                
               await rtc.SetSDP( senderID, param, _self.GetStream(streamerid));
            }
            else if (command == "ICE") {
                rtc.SetICE( senderID, param);
            }
            else if (command == "Run") {
                
                _self.Rebinging = false;
                await rtc.Run(param, null);
            }
            else if (command == "ReBinding") {
                _self.Rebinging = true;
                _self.RemoveStreamItem(streamerid)

                rtc.CloseAllConnection();
                await rtc.Run(param, null);
            }
            else if (command == "StartSnapshot") {
                
                _self.Snapshot.RunUsingSignalR(await _self.LoadMyStream());
            }
            else if (command == "EndSnapshot") {
                _self.Snapshot.StopUsingSignalR();
            }
            else if (command == "AfterBindingStream") {
                var strm = _self.GetStream(streamerid);
                 if (_self.SharedStream && strm) {                  
                    rtc.ReplaceStream(senderID, _self.SharedStream, strm);
                }
            }
            else if (command == "END") {
                
                delete _self.WebRTCList[streamerid];
                for (var i in _self.StreamList) {
                    if (_self.StreamList[i].StreamerID == streamerid) {
                        _self.StreamList.splice(i, 1);
                        break;
                    }
                }
           
                if(_self.OnNewStreamer)
                    _self.OnNewStreamer();
                
                rtc.CloseAllConnection();
            }
        });
        this.Hub.on("RunAttitude", async (r) => { 
            
            if (!_self.IsOwner) {
                
                _self.IsAttitude = r;

                if (_self.IsAttitude)
                    _self.Attitude.Run(await _self.LoadMyStream());
                else
                    _self.Attitude.Stop();
            }
        })
        this.Hub.on("CheckAttitude", async (usr) => { 
            if (!_self.IsOwner) {
                this.ShowFaceChecker();
            }
        })
        this.Hub.on("DeductUser", async (usr) => { 
            var item = new Object;
            item.Sender = "";
            item.Content = usr.userInfo.USER_NAME + "님은 자리 비움으로 1점 감점 되었습니다.";
            item.Time = GetTime(new Date());
            this.MsgItems.push(item);
        })
        this.Hub.on("SendMsg", (senderID,msg,date) => {
           
            var item = new Object;
            item.Sender = "";
            item.Content = msg;
            item.Date = date;
            item.Time = "";
            
            if (date)
                item.Time = GetTime(date);

            
            var usr = this.GetUserInfo(senderID)
            if(usr)
                item.Sender = usr.userInfo;
            
            this.MsgItems.push(item);
            
        });

        this.Hub.on("RequestComment", (senderID,title,msg) => {
            var usr = this.GetUserInfo(senderID)
            
            if (usr) {
                usr.isRqtComment = true;
                var item = new Object;
                item.UnqID = usr.connectionID;
                item.Msg = msg;
                item.Title = title;
                item.MsgDate = GetTime(new Date);
                item.OnClick = (isOK) => { 
                   _self.ResponseComment(usr,isOK);
                }
                this.NotifyItems.push(item);
            }
           
        });
        this.Hub.on("ResponseComment", (isOK) => {
            this.IsRqtComment = isOK;
        
            if(isOK)
                this.PlayMyMedia();
            else
                this.StopComment();
            
            
        });

        this.Hub.on("CancelComment", (senderID) => { 
            var usr = this.GetUserInfo(senderID);
            if (usr) {     
                usr.isRqtComment = false; 
                this.RemoveAlert(usr.connectionID);
            }
        });

        this.Hub.on("NewUser", (usr) => {
            if (!_self.UserList) {
                _self.UserList = new Array;
            }
            _self.UserList.push(usr);

             if (this.OnChangeUserList)
                this.OnChangeUserList(usr,"NEW");

        });
        this.Hub.on("RemoveUser", (usr) => {
            for (var i in _self.UserList) {
                if (_self.UserList[i].connectionID == usr.connectionID) {
                    _self.UserList.splice(i, 1);
                    break;
                }
            }
            if (this.OnChangeUserList)
                this.OnChangeUserList(usr,"REMOVE");
        });
        this.Hub.on("ReturnGet", (userList) => {
            _self.UserList = userList;
        });
        this.Hub.on("ReturnAdd", (info) => {
            this.Hub.send("Get", this.ClassID);
        });
        

        this.Hub.on("ShowWhiteBoard", (isShow) => {
            this.IsShowWhiteBoard = isShow;
        });
        
        this.Hub.on("SendObj", (senderID,type, obj) => {
            if (_self.OnSendObject) {
                _self.OnSendObject(type,obj);
            }
        });
        this.Hub.on("CancelCommentGroup", (senderID,groupid) => {
            if (this.IsOwner) {
                if (this.OnCancelGroupComment)
                    this.OnCancelGroupComment(groupid);
            }
            else {
                this.IsRqtGroupComment = false;    
            }
        });

        this.Hub.on("RequestCommentGroup", (senderID, groupID, title, msg) => { 
            
            if (this.IsOwner) {
                if (this.OnRqtGroupComment)
                    this.OnRqtGroupComment(groupID, title,msg);
            }
            else {
                this.IsRqtGroupComment = true;    
            }
        });
        this.Hub.on("StartGroup", async () => {
            this.IsStartGroup = true; 
            if(!this.IsOwner)
                this.SetMode("GROUP");
            else
                this.SetMode("GROUPSTANDBY");
            
        });
         this.Hub.on("RequestEndAllGroup", async () => {
            this.IsStartGroup = false; 
            this.SetMode("DEFAULT");
        });
        this.Hub.on("ResponseEndGroup", async (groupID) => {
            
            if (this.OnResponseEndGroup) {
                this.OnResponseEndGroup(groupID);
            }
        });

        this.Hub.on("RunQuiz", (r) => {
            
            if (r) {
                this.Quiz = r;
                
                var _self = this;
                var item = new Object;
                item.UnqID = r.pk_quizlist_id;
                if(r.col_type == "QZ020101")
                    item.Title = "퀴즈가 시작 되었습니다.";
                else
                    item.Title = "설문이 시작 되었습니다.";
                item.Msg = r.col_title + " (" + r.col_limit_time + "분 후 자동 종료 됩니다.)";
                RunStopWatch(new Date, r.col_limit_time, async (result, isCompleted) => {
                    
                    if (item.ReloadMsg){
                        if (result.Min > 0) {
                            item.ReloadMsg(r.col_title + " (" + result.Min + "분 " + result.Sec + "초 후 자동 종료 됩니다.)");
                        }
                        else {
                               item.ReloadMsg(r.col_title + " (" + result.Sec + "초 후 자동 종료 됩니다.)");
                        }
                    }
                    
                    if (isCompleted) {
                        if (_self.UserInfo.USER_ID== r.col_user_id) {
                            await MNClient.ExecScalar("QZ", "UPDATEQUIZLISTEND", { PID: r.pk_quizlist_id, PDATE: new Date });
                        }
                        _self.RemoveAlert(r.pk_quizlist_id)
                    }
                    
                });
                if (!this.IsOwner) {
                    item.OnMsgClick = () => {
                        this.Quiz = r;
                    }
                }
                
                item.MsgDate = GetTime(new Date);
                    
                this.NotifyItems.push(item);
                
                
            }
        });

       
        this.Hub.on("Start", async () => {
            this.IsStart = true; 
        });   
           this.Hub.on("Stop", async () => {
            this.IsStart = false; 
        });   
        
        
        
        
        

        await this.LoadMyStream();
        this.Hub.send("Add", this.ClassID, this.UserInfo,this.IsOwner);

    }   

    Start() {
        
        if (this.IsStart) {
            this.IsStart = false;
            this.EndSnapshot();
            this.Hub.send("Stop", this.ClassID);
        }
        else {
            this.IsStart = true;
            this.PlayMyMedia();
            this.StartSnapshot();
            this.Hub.send("Start", this.ClassID);
        }

    }

    GetUserInfo(connectionID) {
         for (var i in this.UserList) {
            var usr = this.UserList[i];
            if (usr.connectionID == connectionID){
                return usr;
            }
        }
    }
    
    PlayMyMedia() { 
        this.Rebinging = false;
        
        this.BindingStream(this.ConnectionID, this.MyStream);
    }



    GetStreamItem(streamerid) {
        if (this.StreamList){
            for (var i in this.StreamList) {
                var item = this.StreamList[i];
                if (item.StreamerID == streamerid) {
                    return item;
                }
            }
        }

        return null;
    }

    RemoveStreamItem(streamerid) {
          if (this.StreamList){
            for (var i in this.StreamList) {
                var item = this.StreamList[i];
                if (item.StreamerID == streamerid) {
                    item.Stream = null;
                    this.StreamList.splice(i, 1);
                    break;
                }
            }
        }
    }

    GetStream(streamerid) {
        var item = this.GetStreamItem(streamerid);
        if (item) {
            return item.Stream;
        }

        return null;
    }

    BindingStream(streamerid, strm) { 


        var item = this.GetStreamItem(streamerid);


        if (item) {
            return;
        }
        else {

            item = new Object;
            item.StreamerID = streamerid;
            item.Stream = strm;
            item.StreamerInfo = null;
            var usr = this.GetUserInfo(streamerid);
            if (usr) {
                item.StreamerInfo = usr.userInfo;
            }

            
            this.StreamList.push(item);
            

            if (this.OnNewStreamer) {
                this.OnNewStreamer();    
            }
            this.Hub.send("NextPlay", this.ClassID, streamerid, this.Rebinging);
        }
    }

    StartSnapshot() {
        this.Hub.send("StartSnapshot", this.ClassID);
        var _self = this;
        this.InterverHandle = setInterval(() => {
            
            for (var i in _self.UserList) {
                _self.UserList[i].ImgUrl = _self.Snapshot.GetSnapShotImgUrl(_self.ClassID, _self.UserList[i].connectionID) + "&cnt=" + _self.Cnt;
             
            }
            _self.Cnt += 1;
        }, 2000);
        
    }
    EndSnapshot() {
        this.Hub.send("EndSnapshot", this.ClassID);
        
        clearInterval(this.InterverHandle);
        this.InterverHandle = null;
        var _self = this;
         for (var i in _self.UserList) {
             _self.UserList[i].ImgUrl = "";
                
            }
    }
    
    async LoadMyStream  () {
        
        if (this.MyStream == null) {
            this.MyStream = await GetUserMedia();
        }

        return this.MyStream;
    }

    async SharedScreen() {
        if (!this.IsStart) {
            
             Swal.fire({
                text: '강의를 시작하여야 합니다.',
                icon: 'info'
            });
            return;
        }
        
       var _self = this;
       var rtc = this.GetWebRTC(this.ConnectionID);
       
        if (!this.IsShowShared) {
            
            _self.SharedStream = await navigator.mediaDevices.getDisplayMedia({ audio: false, video: { cursor: 'always', video: { displaySurface: 'application' | 'browser' | 'monitor' | 'window' } } });
       
            if (_self.SharedStream) {
                rtc.ReplaceAllStream(_self.SharedStream, _self.GetStream(this.ConnectionID));
                this.IsShowShared = true;
            }
            else
                this.IsShowShared = false;
        }
        else {
            this.IsShowShared = false;
            rtc.ReplaceAllStream(_self.GetStream(this.ConnectionID), _self.SharedStream);

            if (_self.SharedStream) {
                
                let tracks = _self.SharedStream.getTracks();
                tracks.forEach(track => track.stop());
                _self.SharedStream = null;
            }
        }

    }

    
    async StartGroup(grps, activeMin) {
        var r = await MNClient.Run("ClassGroupDA", "CreateGroup", { "grps": grps, "min": activeMin, "classID": this.ClassID,"time":new Date });
        
        this.Hub.send("StartGroup", this.ClassID);
        return r;
    }
    async UpdateEndTime(groupID) {
        await MNClient.Run("ClassGroupDA", "UpdateGroupEndTime", { "groupID": groupID ,"time":new Date});
        
    }
    SendMsg(msg) {
        this.Hub.send("SendMsg", this.ClassID,this.GetGroupID(),msg,new Date());
    }
    SendObj(type, obj) {
        this.Hub.send("SendObj", this.ClassID,this.GetGroupID(),type, obj);
    }
    GetGroupID() {
        if(this.GroupInstance && this.GroupInstance.GroupID > 0)
            return this.GroupInstance.GroupID;
        
        return -1;
    }
    RequestComment() {
        
        if (!this.IsStart) {
            
             Swal.fire({
                text: '강의를 시작하여야 합니다.',
                icon: 'info'
            });
            return;
        }

       
        if (this.IsRqtComment) {
            this.IsRqtComment = false;
            this.Hub.send("CancelComment", this.ClassID);
            this.StopComment();
        }
        else {
            this.IsRqtComment = true;
            this.Hub.send("RequestComment", this.ClassID, this.UserInfo.USER_NAME, "발언 요청 하였습니다.");
        }
    
    }
    RequestGroupComment() {
         if (!this.IsStart) {
            
             Swal.fire({
                text: '강의를 시작하여야 합니다.',
                icon: 'info'
            });
            return;
        }
        if (this.GroupInstance) {
            if (this.IsRqtGroupComment) {
                this.IsRqtGroupComment = false;
                this.CancelGroupComment(this.GroupInstance.GroupID);
            }
            else {
                this.IsRqtGroupComment = true;
                this.Hub.send("RequestCommentGroup", this.ClassID, this.GroupInstance.GroupID, this.GroupInstance.GroupItem.col_classgroup_name, "그룹 참여를 요청 하였습니다.");
            }
        }
    }

    CancelGroupComment(groupID) {
        this.Hub.send("CancelCommentGroup", this.ClassID,groupID);
    }
    ResponseComment(usr, isOK) {
        usr.isRqtComment = isOK; 
        if(!isOK)
            this.RemoveAlert(usr.connectionID);
        
        this.Hub.send("ResponseComment", this.ClassID,usr.connectionID,isOK);
    }
    RemoveAlert(id) {//교수 화면에서 발언요청 관련 삭제
        for (var i in this.NotifyItems) {
            if (this.NotifyItems[i].UnqID == id) {
                this.NotifyItems.splice(i, 1);
                break;
            }
        }
    }
  

    MuteAudio() { 
        var _self = this;
        _self.IsAudio = !_self.IsAudio;
         this.MyStream.getAudioTracks().forEach(function (track) {track.enabled = _self.IsAudio;})
    }
    MuteVideo() { 
        var _self = this;
        _self.IsVideo = !_self.IsVideo;
        this.MyStream.getVideoTracks().forEach(function (track) { track.enabled = _self.IsVideo;})
    }
    StopComment() {
        this.Hub.send("StopComment", this.ClassID, this.ConnectionID);
    }
    ShowWhiteBoard() {

        if (!this.IsStart) {
            
             Swal.fire({
                text: '강의를 시작하여야 합니다.',
                icon: 'info'
            });
            return;
        }
       //this.IsShowGroupSetting = false;
        if (this.IsShowWhiteBoard) {
            this.IsShowWhiteBoard = false;
     
        } else {
            this.IsShowWhiteBoard = true;
        }
        
        this.Hub.send("ShowWhiteBoard", this.ClassID,this.GetGroupID(), this.IsShowWhiteBoard);
    }
    ShowQuizList() {
        if (!this.IsStart) {
            
             Swal.fire({
                text: '강의를 시작하여야 합니다.',
                icon: 'info'
            });
            return;
        }
        
        this.IsShowQuiz = !this.IsShowQuiz;
    }
    CloseQuizList(r, quizID) { 
        
        this.IsShowQuiz = false;
    }
    
    ShowGroupSetting() {
        if (!this.IsStart) {
            
             Swal.fire({
                text: '강의를 시작하여야 합니다.',
                icon: 'info'
            });
            return;
        }
        this.IsShowGroupSetting = !this.IsShowGroupSetting;

        if (!this.IsShowGroupSetting) {
            this.Hub.send("RequestEndAllGroup", this.ClassID);
        }
    }
    RemoveGroup() {
        if (!this.IsOwner) {
            this.IsStartGroup = false;
            this.SetMode("DEFAULT");
        }
        else {
            if (this.OnRemoveGroup) {
                this.OnRemoveGroup();
            }

            this.SetMode("GROUPSTANDBY");
        }
    }

    async RunQuiz(r) {

        await MNClient.ExecScalar("QZ", "UPDATEQUIZLISTSTART", { PID: r.pk_quizlist_id, PDATE: new Date });
        var grpID = -1;
        if (this.GroupInstance)
            grpID = this.GroupInstance.GroupID;
        this.Hub.send("RunQuiz", this.ClassID,grpID,r);
    }
    SetMode(type) {
        this.ModeType = type;
        
        this.IsVisibleAudio = 
        this.IsVisibleVideo = 
        this.IsVisibleRecord = 
        this.IsVisibleWhiteboard = 
        this.IsVisibleShare = 
        this.IsVisibleGroup = 
        
        this.IsVisibleQuiz = 
        this.IsVisibleAttitude = 
        this.IsVisibleComment = 
        this.IsVisibleGroupComment =
        this.IsVisibleStart = 
        this.IsVisibleClose = false;
        if (this.ModeType == "DEFAULT") {
            if (this.IsOwner) {
                this.IsVisibleAudio = 
                this.IsVisibleVideo = 
                this.IsVisibleRecord = 
                this.IsVisibleWhiteboard = 
                this.IsVisibleShare = 
                this.IsVisibleGroup = 
                
                this.IsVisibleQuiz = 
                this.IsVisibleAttitude = 
                this.IsVisibleStart = true;
            }
            else {
                this.IsVisibleComment = true;
            }
        }
        else if (this.ModeType == "GROUP" ) {
            
            this.IsVisibleAudio =
            this.IsVisibleVideo =
            this.IsVisibleWhiteboard = true;
            this.IsVisibleQuiz = this.IsVisibleGroupComment = !this.IsOwner;
            
            this.IsVisibleClose = this.IsOwner;
            this.IsVisibleClose = true;
        }
        else if (this.ModeType == "GROUPSTANDBY") {
            this.IsVisibleGroup = true;
            
        }
    }

    async RunRecord() {
        if (!this.IsStart) {
            
             Swal.fire({
                text: '강의를 시작하여야 합니다.',
                icon: 'info'
            });
            return;
        }

        var _self = this;
        this.IsRunRecord = !this.IsRunRecord;
        if (this.IsRunRecord) {

            this.Recorder = RecordRTC(this.MyStream,{type: 'video'});
            this.Recorder.startRecording();
        }
        else if (this.Recorder) {
            
            this.Recorder.stopRecording(async function () {
                
                _self.UploadBlob = _self.Recorder.getBlob();
            });
        }
    }
    DeductUser() {
        this.Hub.send("DeductUser", this.ClassID);
    }
    ShowFileListComment() {
        
        this.IsVisibleFileList = !this.IsVisibleFileList;
        
    }

    RunAttitude() {
        this.IsAttitude = !this.IsAttitude;
        this.Hub.send("RunAttitude", this.ClassID, this.IsAttitude);

    }

    HideFaceChecker() {
        this.IsVisibleAttitude = this.IsVisibleAttendance = false;
    }
    ShowFaceChecker() {
        this.IsVisibleAttitude = true;
    }
    Dispose() {
        this.WebRTC.CloseAllConnection(this.ConnectionID);
    }
}