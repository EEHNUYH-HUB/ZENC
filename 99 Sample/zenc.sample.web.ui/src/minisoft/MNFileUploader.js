import MNClient from '@/minisoft/MNClient'
import {Guid} from '@/minisoft/MNCommon.js'

export default class MNFileUploader {

    constructor(){
        this.ProgressEvent = null;
        this.CompletedEvent = null;
        
        this.BufferSize = (1024 * 1024) * 2; // 2MB씩 split
        
        

    }
    async Upload(file, fileName) {
        var freader = new FileReader();
        var _self = this;
        var totalSize = file.size;
        var staticName = Guid();
        var loadSize = 0;
        var endPosition = 0;
        freader.onload =async function(e){
     
            var result = await MNClient.Run('FileHandler', 'FileUpload', {
                filename: fileName,
                staticName: staticName,
                fileTotalSize: totalSize,
                contents: _self.ArrayBufferToBase64(e.target.result)
            });
          
            loadSize = endPosition;
            if (_self.ProgressEvent)
                _self.ProgressEvent(_self.GetProgressPercent(loadSize, totalSize));
            
            if (result.data.Type == 0) { // COMPLETE
                if (_self.CompletedEvent)
                    _self.CompletedEvent(staticName);
            }
            else {    
                if (loadSize <= totalSize) {
                    
                    endPosition = _self.GetPostion(loadSize, totalSize);
                    freader.readAsArrayBuffer(file.slice(loadSize, endPosition));
                }
            }

        
        };
        endPosition = _self.GetPostion(loadSize, totalSize);
        freader.readAsArrayBuffer(file.slice(loadSize,endPosition));
    }
    GetPostion(loadSize, totalSize) {
        if (loadSize + this.BufferSize > totalSize) {
            
            return totalSize;
        }
        else {
            ;
            return loadSize + this.BufferSize;
        }
    }
    GetProgressPercent(loadSize, totalSize) {
        return loadSize * 100 / totalSize;
    }
    ArrayBufferToBase64(buffer) {
        let binary = '';
        let bytes = new Uint8Array(buffer);
        let len = bytes.byteLength;
        for (let i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    }

    Dispose() {
        this.WebRTC.CloseAllConnection(this.ConnectionID);
    }
}