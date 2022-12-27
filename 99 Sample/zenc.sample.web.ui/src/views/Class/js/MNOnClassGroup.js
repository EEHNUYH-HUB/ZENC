import MNWebRTC from '@/minisoft/MNWebRTC'
import MNClient from '@/minisoft/MNClient'
//import { userInfo } from 'os';
import {  RunStopWatch } from "@/minisoft/MNCommon"


export default class MNOnClassGroup {

    constructor(onClassInstance){
        this.OnClassInstance = onClassInstance;
        this.Hub = this.OnClassInstance.Hub;
        this.ClassID = this.OnClassInstance.ClassID;
        this.GroupID = null;
        this.GroupItem = null;
        this.StreamList = new Array;
        this.WebRTCList = null;
        this.HeaderTitle = null;
        this.OnClassInstance.GroupInstance = this;
        this.IntervalMin = null;
        this.intervalIntance = null;
    }
    
    async Init(gid) {
        var _self = this;
        _self.GroupID = gid;
        var dt = await MNClient.ExecDataTable("GRP", "GETGROUPDTL", { "PID": _self.GroupID })
        
         if (dt && dt.length > 0) {
            _self.GroupItem = dt[0];
        }
         else {
             
        }
      
       this.intervalIntance =  RunStopWatch(new Date(_self.GroupItem.col_start_time), _self.GroupItem.col_active_min, (r, isCompleted) => { 
            _self.IntervalMin = r.Min + "분" + r.Sec + "초";
            if (isCompleted) {
                
                _self.OnClassInstance.RemoveGroup();
            }
        });
        

        _self.Hub.on("ReceiveGroup", (senderID, command, param) => {
            var rtc = _self.GetWebRTC(senderID);
            
            if (command == "SDP") {
                rtc.SetSDP( senderID, param, _self.OnClassInstance.MyStream);
            }
            else if (command == "ICE") {
                rtc.SetICE( senderID, param);
            }
        });

        _self.Hub.on("AddGroupUser", (userlist) => {
            
            console.log(userlist)
            if (userlist && userlist.length > 0) {
                for (var i in userlist) {
                    var streamerID = userlist[i];
                    var rtc = _self.GetWebRTC(streamerID);
                    rtc.Run(streamerID, _self.OnClassInstance.MyStream);
                }
            }
        });
        
        _self.Hub.on("RemoveGroupUser", (usr) => {

            
            if (usr) {

                for (var i in _self.StreamList) {
                    var item = _self.StreamList[i];
                    //console.log(item.StreamerID , usr.connectionID);
                    if (item.StreamerID == usr.connectionID) {
                        _self.StreamList.splice(i, 1);
                        break;
                    }
                }
            }
        });
        
        
        
        _self.Hub.send("AddGroupUser", _self.ClassID, _self.GroupID);

    }   
    
    GetWebRTC(id) {
        if (!this.WebRTCList) {
            this.WebRTCList = {};
        }


        var rtn = this.WebRTCList[id];
        if (!rtn) {
            rtn= new MNWebRTC();
            var _self = this;
            rtn.OnAddStream = function (event, senderID) {
                if (event.streams) {
                    _self.BindingStream(id, event.streams[0]);
                }
            }
            rtn.OnRemoveStream = function () {
                console.log("OnRemoveStream");
            }
            rtn.OnICE = function (ice,  senderID) {
                _self.Hub.send("SendGroup",  id, "ICE", ice);
            }
            rtn.OnSDP = function (sdp,  senderID) {
                _self.Hub.send("SendGroup",  id, "SDP", sdp);
            }       
            rtn.ParentID = id;
            this.WebRTCList[id] = rtn;

        }

        return rtn;
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
            var usr = this.OnClassInstance.GetUserInfo(streamerid);
            if (usr) {
                item.StreamerInfo = usr.userInfo;
            }

            
            this.StreamList.push(item);
        }
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
    Dispose() {
        

        
        if (this.WebRTCList) {
            var keys = Object.keys(this.WebRTCList);
            if (keys) {
                for (var i in keys) {
                    
                    this.WebRTCList[keys[i]].CloseAllConnection();
                }
            }
            
        }
        if (this.intervalIntance)
            clearInterval(this.intervalIntance);
        this.intervalIntance = null;
        this.WebRTCList = null;
        this.Hub.send("RemoveGroupUser", this.ClassID, this.GroupID);
        this.OnClassInstance.GroupInstance = null;
        
    }

}