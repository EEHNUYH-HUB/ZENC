import adapter  from "webrtc-adapter";
import {Subject} from "@microsoft/signalr"
import MNClient from '@/minisoft/MNClient'

export default class MNSnapshot{

    constructor(classID,hubConnection){
        
        this.timer = 3000;
        this.imgSize = 80;
        this.InterverHandle = null;
        this.SignalRSubject = null;
        this.gContext = null;
        
        this.canvas = null;
        
        this.VideoStream = null;
        this.VideoTrack = null;
        this.VideoImageCpature = null;

        this.UseSnapshot = false;
        this.Img = null;
        this.cnt = 0;
        this.ClassID = classID;
        this.HubConnection = hubConnection;
    }

    async RunUsingSignalR(stream) {
        if (!this.UseSnapshot) return;

        this.VideoStream = stream;
        this.SignalRSubject = new Subject();
        this.HubConnection.send("WriteSnapshot", this.ClassID, this.SignalRSubject);
        
        this.LoadMedia();

        this.InterverHandle = setInterval(async ()=>{
            var wImg=await this.GrabFrame();
            
            if (wImg != null) {
//                 var data = this.GetDataURL(wImg);
//                 let chunks = data.match(/.{1,25000}/g);
// console.log(chunks)
                this.SignalRSubject.next(this.GetDataURL(wImg));
            }

        }, this.timer);


    }
   

    StopUsingSignalR(){
        this.Stop()();
        this.SignalRSubject.complete();
        this.SignalRSubject = null;

    }

  

    GetSnapShotImgUrl(classID, connectionID) {
        return process.env.VUE_APP_API_URL
            + "api/Image/SingleImageHandler?classid="
            + classID + "&connectionid="
            + connectionID;
    }
   
    
    Stop() {
        if(this.InterverHandle)
            clearInterval(this.InterverHandle);
        if (this.Img)
            this.Img.src = "";
        if (this.Video) {

            this.InterverHandle = null;
            this.VideoTrack = null
            this.VideoImageCpature = null;
            this.VideoStream = null;
        }
    }

    LoadMedia() {
        
        if (!this.UseSnapshot) return;

        this.canvas =document.createElement("canvas");
        this.gContext = this.canvas.getContext('2d');
        
        
        this.VideoTrack = this.VideoStream.getVideoTracks()[0];
        
        this.VideoImageCpature =new ImageCapture(this.VideoTrack);
    }

    async GrabFrame() {
        var P=new Promise((R)=>{this.VideoImageCpature.grabFrame().then((vImg) => {R(vImg);}).catch(() => {R(null);});        });
        return P;    
    }

    

    GetDataURL(img){

        var defalutLen = this.imgSize;
        var cW = 0;
        var cH = 0;
        if (img.width > img.height) {
            cW = defalutLen;
            cH = img.height * defalutLen / img.width;
        }
        else {
            cW = img.width * defalutLen / img.height;
            cH = defalutLen;
        }

        // var cW = img.width;
        // var cH = img.height;


        this.canvas.width = cW;
        this.canvas.height = cH;

        this.gContext.drawImage(img, 0, 0, cW, cH);

        var dataURL = this.canvas.toDataURL("image/jpg");
        //console.log(dataURL)

        return dataURL.replace(/^data:image\/(png|jpg);base64,/, "");
    }
}
