import adapter  from "webrtc-adapter";
import {Subject} from "@microsoft/signalr"
import MNClient from '@/minisoft/MNClient'
import {  Guid } from "@/minisoft/MNCommon"
export default class MNAttitude{

    constructor(classID,hubConnection){
        
        this.timer = 1000 * 60;
        
        this.imgSize = 80;
        this.InterverHandle = null;
        
        this.gContext = null;
        
        this.canvas = null;
        
        
        this.VideoTrack = null;
        this.VideoImageCpature = null;
        this.ClassID = classID;
        this.HubConnection = hubConnection;
        this.CompleteCheckAttendance = null;
        this.HubConnection.on("CheckFace", (r) => {
             if (this.CompleteCheckAttendance) {
                 this.CompleteCheckAttendance(r);
                 
            }
        });
        
    }

    async Run(stream) {
        var _self = this;
        
        this.InterverHandle = setInterval(async () => {
            
            _self.WriteAttitude(stream);
                
        }, this.timer);
    }
   
    async WriteAttitude(stream) {

        if (!this.VideoTrack) {
            this.LoadMedia(stream);
        }
        var subject = new Subject();
        this.HubConnection.send("WriteAttitude", this.ClassID, Guid(),subject);
        
        var data = this.GetDataURL(await this.GrabFrame());
        let chunks = data.match(/.{1,25000}/g);
        
        chunks.forEach((chunk) => {
            subject.next(chunk);
        });
        subject.complete();
        
        subject = null;
    }
    async CheckFace(stream,isAttendance,complete) {

        if (!this.VideoTrack) {
            this.LoadMedia(stream);
        }
        var subject = new Subject();
        this.CompleteCheckAttendance = complete;
        this.HubConnection.send("CheckFace", this.ClassID, Guid(),new Date(),isAttendance,subject);
        
        var data = this.GetDataURL(await this.GrabFrame());
        let chunks = data.match(/.{1,25000}/g);
        
        chunks.forEach((chunk) => {
            subject.next(chunk);
        });
        subject.complete();
        
        subject = null;
    }
    
    Stop() {
        if(this.InterverHandle)
            clearInterval(this.InterverHandle);

        this.InterverHandle = null;
        this.VideoTrack = null
        this.VideoImageCpature = null;
    
    }

    LoadMedia(stream) {
        
        this.canvas =document.createElement("canvas");
        this.gContext = this.canvas.getContext('2d');        
        this.VideoTrack = stream.getVideoTracks()[0];        
        this.VideoImageCpature =new ImageCapture(this.VideoTrack);
    }

    async GrabFrame() {
        var P=new Promise((R)=>{this.VideoImageCpature.grabFrame().then((vImg) => {R(vImg);}).catch(() => {R(null);});        });
        return P;    
    }

    

    GetDataURL(img){
       
         var cW = img.width;
         var cH = img.height;


        this.canvas.width = cW;
        this.canvas.height = cH;

        this.gContext.drawImage(img, 0, 0, cW, cH);

        var dataURL = this.canvas.toDataURL("image/jpg");
        
        return dataURL.replace(/^data:image\/(png|jpg);base64,/, "");
    }
}
