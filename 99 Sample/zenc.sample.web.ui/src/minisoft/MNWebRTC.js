
export default class MNWebRTC {
    constructor() {
        this.OfferOption = { offerToReceiveAudio: true, offerToReceiveVideo: true }
        this.ICEServers = [{ url: 'stun:stun.l.google.com:19302' }]
        this.Peers = null
        
        this.OnAddStream = null
        this.OnRemoveStream = null
        this.OnICE = null
        this.OnSDP = null
    }

    async Run (id,strm) {
        var peer = this.GetPeer(id);

        var _self = this;

        if (strm) {
            strm.getTracks().forEach(
                function (track) {
                    peer.addTrack(
                        track,
                        strm
                    );
                }
            );
        }

        var desc =await peer.createOffer(this.OfferOption);   
        await peer.setLocalDescription(desc);
        _self.OnSDP(JSON.stringify({ "sdp": peer.localDescription }), id);
                    
           
    }
    async SetSDP  (id, data, strm) {
        var _self = this;
        var signal = JSON.parse(data);
        var sdp = signal.sdp;
        var peer = this.GetPeer(id);

        if (sdp) {
            await peer.setRemoteDescription(new RTCSessionDescription(sdp));
            if (peer.remoteDescription.type == "offer") {
                if (strm) {
                    
                    strm.getTracks().forEach(
                        function (track) {
                            
                            peer.addTrack(
                                track,
                                strm
                            );
                        }
                    );
                }

                var desc = await peer.createAnswer();
                await peer.setLocalDescription(desc);    
                _self.OnSDP(JSON.stringify({ "sdp": peer.localDescription }), id);

            }

        }
    }
    SetICE (id,data) {
        var signal = JSON.parse(data);
        var ice = signal.ice;
        var peer = this.GetPeer(id);
        if (ice) {
            peer.addIceCandidate(new RTCIceCandidate(ice));
        }

    }
    ReplaceStream(id, strm, beforeStream) {
        var peer = this.GetPeer(id);
        if (peer) {
            this._Replace(peer, strm, beforeStream);
        }
    }
    ReplaceAllStream(strm, beforeStream) {
        var tmpItems = this.GetPeers();
        for (var i in tmpItems) {

            var peer = tmpItems[i];
            this._Replace(peer, strm, beforeStream);
        }
    }
    _Replace(peer, strm, beforeStream) {
        if (peer&& strm&&beforeStream) {
            var beforeVideoTrack = beforeStream.getVideoTracks()[0];
            var sender = peer.getSenders().find(function (s) {
                return s.track.kind == beforeVideoTrack.kind;
            });

            var videoTrackMixer = strm.getVideoTracks()[0];
            sender.replaceTrack(videoTrackMixer);
        }
    }
    CloseConnection  (id) {
        
        var peer = this.Peers[id];

        if (peer) {
            this.OnRemoveStream(id);
            peer.close();
            delete this.Peers[id];
        }
    }
    CloseAllConnection  () {
        var items = this.GetPeers();
        if (items) {
            for (var i in items) {
                items[i].close();
                
            }

            this.Peers = null;
        }
    }

    GetPeer (id) {
        if (!this.Peers) {
            
            this.Peers = {};
        }

        return this.Peers[id] || this._CreateConnection(id);
    }
    GetPeers() {
        var rtn = new Array;
        if (this.Peers) {
            var keys = Object.keys(this.Peers);
            if (keys) {
                for (var i in keys) {
                    rtn.push(this.Peers[keys[i]]);
                }
            }
            
        }
        return rtn;
    }
    _CreateConnection  (id) {
        var peer = new RTCPeerConnection({ iceServers: this.ICEServers });
        var _self = this;

        peer.onicecandidate = function (event) {
            if (event.candidate) {
                _self.OnICE(JSON.stringify({ "ice": event.candidate }), id);
            }
        };
        peer.onstatechange = function () {
            // var states = {
            //     'iceConnectionState': connection.iceConnectionState,
            //     'iceGatheringState': connection.iceGatheringState,
            //     'readyState': connection.readyState,
            //     'signalingState': connection.signalingState
            // };
        };
        peer.addEventListener('track', function (event) {
            _self.OnAddStream(event, id);
        });
        peer.addEventListener('connectionstatechange', event => {
            console.log(peer.connectionState)
            
        });
        peer.oniceconnectionstatechange = function () {

            if (peer.iceConnectionState == 'disconnected') {
                _self.CloseConnection(id);
                
            } else if (peer.iceConnectionState == 'connected') {
                // _self.OnAddStream(event, id);
            }
        };
        this.Peers[id] = peer;
        
        return peer;
    }

    
}
