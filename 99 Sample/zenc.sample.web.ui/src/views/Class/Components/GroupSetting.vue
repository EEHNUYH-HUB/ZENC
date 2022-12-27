<template>

    <div class="text-sm">
        <div class="row text-sm" v-show=" RequestGroupID <= 0">
            <div class="col col-2 m-0  card" @drop="OnAppendDrop(null)" @dragover="OnDragOver"
                v-if="OnClassInstance.ModeType != 'GROUPSTANDBY'">
                <p class="user-item text-sm m-0">미등록 참여자 : {{ UnRegUserList.length }}</p>


                <div v-for="user, index in UnRegUserList" :key="index" @dragstart="OnDragUser(user)" draggable="true"
                    class="card  mb-2 bg-light">
                    <div class="row">
                        <img class="col col-auto thimg" :src="user.ImgUrl" />
                        <span class="col" style="margin-top:20px">{{ user.userInfo.USER_NAME
                        }}({{ user.userInfo.STUDENT_NUMBER }})</span>
                    </div>
                </div>
                <hr class="horizontal dark mt-2 mb-2" />
                <div class="col text-center ">
                    <select class="form-control mt-2 mb-2 " @change="OnChangedRandom" v-model="RandomValue">
                        <option value="0">직접 지정</option>
                        <option value="1">1그룹 랜덤</option>
                        <option value="2">2그룹 랜덤</option>
                        <option value="3">3그룹 랜덤</option>
                        <option value="4">4그룹 랜덤</option>
                        <option value="5">5그룹 랜덤</option>
                        <option value="6">6그룹 랜덤</option>
                        <option value="7">7그룹 랜덤</option>
                    </select>
                    <input type="text" class="form-control mt-2 mb-2" v-model="ActiveMin"
                        placeholder="활동시간(분)을 입력하세요." />
                    <button class="btn btn-primary mt-2 m-2" @click="Run()">그룹시작</button>
                </div>
            </div>


            <div class="col col-10 px-4 row " @drop="OnCreateDrop" @dragover="OnDragOver">
                <span v-if="!GroupList || GroupList.length == 0" class="text-center mt-4">드래그 하여 그룹을 생성
                    하세요.</span>
                <div class="card col m-2  grp " v-for="grp, index in GroupList" :key="index"
                    :class="grp.isRqtComment ? 'bg-primary text-light' : 'bg-light '" @drop="OnAppendDrop(grp)"
                    @dragover="OnDragOver">

                    <span class="user-item " :class="!grp.isEnd?'':'text-danger'">{{ grp.GroupName }} 참여자 : {{ grp.UserList.length }}
                        {{grp.isEnd?"(종료)":""}}</span>

                    <div v-for=" user, ui in grp.UserList" :key="ui" class="card  mb-2"
                        :class="grp.isRqtComment ? 'bg-light text-dark' :''" draggable="true"
                        @dragstart="OnDragUser(user)">
                        <div class="row">
                            <img class=" col col-auto thimg" :src="user.ImgUrl" />
                            <span class="col" style="margin-top:18px">{{ user.userInfo.USER_NAME
                            }}({{
                                user.userInfo.STUDENT_NUMBER
                                }})</span>
                        </div>
                    </div>


                </div>

            </div>
        </div>
        <GroupRoom :OnClassInstance="OnClassInstance" :GroupID="RequestGroupID" v-if="RequestGroupID > 0"></GroupRoom>
    </div>

</template>

<script setup>
import { ref, defineExpose, defineProps,onMounted } from 'vue';
import GroupRoom from "@/views/Class/Components/GroupRoom.vue";
// import MNClient from '@/minisoft/MNClient.js'
import { Guid, GetRandom ,GetTime} from '@/minisoft/MNCommon.js'

import Swal from 'sweetalert2'

const Props = defineProps({
    OnClassInstance: { type: Object }
    //, OnCompleted: { type: Function }
})

const UnRegUserList = ref(Array);
const RandomValue = ref(0);
const ActiveMin = ref("");

const GroupList = ref(null);
const DragStartItem = ref(null);
const RequestGroupID = ref(-1);


onMounted(() => { 
    Clear();
    Props.OnClassInstance.OnChangeUserList = (usr,type) => { 

        if (type == "NEW") {
            UnRegUserList.value.push(usr);
        }
        else {
            var isFind = true;
            for (var i in UnRegUserList.value) {
                if (UnRegUserList.value[i].connectionID == usr.connectionID)
                {
                    UnRegUserList.value.splice(i, 1);
                    isFind = false;
                    break;
                }
            }
            if (isFind) {
                for (var i in GroupList.value) {
                    if (isFind) {
                        var grp = GroupList.value[i];
                        if (grp) {
                            for (var j in grp.UserList) {
                                if (grp.UserList[j].connectionID == usr.connectionID) {
                                    grp.UserList.splice(j, 1)

                                    isFind = false;
                                    break;

                                }
                            }
                        }
                    }
                }
            }
        }
    }
})

const Run =async () => {

    var cnt = UnRegUserList.value.length;

    if (ActiveMin.value && cnt == 0) {
        Props.OnClassInstance.OnRqtGroupComment = (groupID, title,msg) => { 
            for (var i in GroupList.value) {
                var grp = GroupList.value[i];

                if (grp.GroupName == title) {
                    grp.ID = groupID;
                    grp.isRqtComment = true;

                    var item = new Object;
                    item.UnqID = groupID;
                    item.Msg = msg;
                    item.Title = title;
                    item.MsgDate = GetTime(new Date);
                    item.OnClick = (isOK) => {
                        if (isOK) {
                            Props.OnClassInstance.SetMode( "GROUP");
                            RequestGroupID.value = grp.ID;
                        }
                        Props.OnClassInstance.CancelGroupComment(grp.ID);
                    }
                    Props.OnClassInstance.NotifyItems.push(item);
                    break;
                }
            }     
        };
        Props.OnClassInstance.OnCancelGroupComment = (groupID) => {
            
            for (var i in GroupList.value) {
                var grp = GroupList.value[i];
                
                if (grp.ID == groupID) {
                    grp.isRqtComment = false;
                    break;
                }

            }     

            Props.OnClassInstance.RemoveAlert(groupID);
        };
        Props.OnClassInstance.OnRemoveGroup = () => { 

            RequestGroupID.value = -1;

        };
        Props.OnClassInstance.OnResponseEndGroup =async (groupID) => {


            await Props.OnClassInstance.UpdateEndTime(groupID);
            
            for (var i in GroupList.value) {
                var grp = GroupList.value[i];
                console.log(grp.ID , groupID)
                if (grp.ID == groupID) {
                    grp.isEnd = true;
                    break;
                }

            }     
            var cnt = 0;
            for (var i in GroupList.value) {
                var grp = GroupList.value[i];

                if (grp.isEnd) {
                    cnt += 1;
                }

            }     

            if (cnt == GroupList.value.length) {
                Props.OnClassInstance.IsShowGroupSetting = false;
                Props.OnClassInstance.SetMode("DEFAULT");
            }
        }

        var result = await Props.OnClassInstance.StartGroup(GroupList.value, ActiveMin.value);;
         for (var i in GroupList.value) {
            var grp = GroupList.value[i];
            grp.ID = result[grp.GroupName];
        }   


        console.log(GroupList.value);
    }
    else {

        if (cnt  > 0) {
            Swal.fire({
                text: '그룹 지정 안 된 학생이 있습니다.',
                icon: 'info'
            });

        }
        else {
            Swal.fire({
                text: '활동시간을 설정해주시길 바랍니다.',
                icon: 'info'
            });
        }

    }
}
const Clear = () => {

    
    UnRegUserList.value = new Array;
    for (var i in Props.OnClassInstance.UserList) {
        var sourceUsr = Props.OnClassInstance.UserList[i];
        sourceUsr.ParentID = null;
        if (!sourceUsr.isClassOwner) {
            UnRegUserList.value.push(sourceUsr);
        }
    }

    ActiveMin.value = null;
    GroupList.value = null;
    DragStartItem.value = null;
    
}
const OnCreateDrop = (e) => {
    if (DragStartItem.value) {
        if (!GroupList.value) {
            GroupList.value = new Array;
        }
        var grp = new Object;
        grp.ID = Guid();
        grp.GroupName = "";
        grp.UserList = new Array;
        GroupList.value.push(grp);
        OnAppendDrop(grp);
    }
}
const OnDragUser = (user) => {
    DragStartItem.value = user;
}
const OnDragOver = (e) => {

    e.preventDefault();
}
const OnAppendDrop = (grp) => {

    
    if ( DragStartItem.value.ParentID && DragStartItem.value.ParentID == grp) return;

    RemoverUserInGroup(DragStartItem.value);



    for (var i in UnRegUserList.value) {
        if (UnRegUserList.value[i] == DragStartItem.value) {
            UnRegUserList.value.splice(i, 1);
            break;
        }

    }

    if (grp) {
        grp.UserList.push(DragStartItem.value)
        DragStartItem.value.ParentID = grp.ID;
        
    }
    else {

        UnRegUserList.value.push(DragStartItem.value);
        DragStartItem.value.ParentID = null;
    }
    DragStartItem.value = null;

    RenameGroup();
}
const RemoverUserInGroup = (targetUser) => {

    var parentGroup = GetGroup(targetUser.ParentID);
    if (parentGroup) {
        

        for (var i in parentGroup.UserList) {
            var user = parentGroup.UserList[i];
            if (user) {
                if (user.connectionID == targetUser.connectionID) {
                    parentGroup.UserList.splice(i, 1);
                    RemoveGroup(targetUser.ParentID);
                    targetUser.ParentID = null;

                    break;
                }
            }
        }
    }
}
const GetGroup = (id) => {
    for(var i in GroupList.value){
        var grp = GroupList.value[i];
        if (grp.ID == id)
            return grp;
    }
}
const RemoveGroup = (id) => {
    var group = GetGroup(id);
    if (!group.UserList || group.UserList.length == 0) {
        for (var i in GroupList.value) {
            var tmp = GroupList.value[i];
            if (tmp == group) {
                GroupList.value.splice(i, 1);
                break;
            }
        }
    }
}

const RenameGroup = () => {
    var grpNames = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'L', 'O', 'P', 'Q', 'R', 'S', 'T', 'u']
    for (var i in GroupList.value) {
        var tmp = GroupList.value[i];
        tmp.GroupName = "그룹 " + grpNames[i];
    }

}


const OnChangedRandom = () => {
    Clear();

    GroupList.value = new Array;



    for (var i = 0; i < RandomValue.value; i++) {
        var grp = new Object;
        grp.ID = Guid();
        grp.GroupName = "";
        grp.UserList = new Array;
        GroupList.value.push(grp);
    }
    RenameGroup();


    var tmpUserList = new Array;
    for (var i in UnRegUserList.value) {
        tmpUserList.push(UnRegUserList.value[i])
    }
    var randomUserIndexs = GetRandom(tmpUserList.length)
    var randomGroupIndexs = GetRandom(RandomValue.value)
    
    for (var i in randomUserIndexs) {
        
        DragStartItem.value = tmpUserList[randomUserIndexs[i]];
        if (DragStartItem.value) {
            var grpIndex = randomGroupIndexs[i % RandomValue.value];
            var grp = GroupList.value[grpIndex];

            OnAppendDrop(grp);
        }
    }


}
// defineExpose({
//     Show
// });
</script>

<style scoped>


.grp {
    max-width: 250px;
}

.user-item {
    height: 40px;
    line-height: 40px
}

.vertical-text {
    display: "table-cell";
    vertical-align: "middle";
    width: 20px;
}


.thimg {
    margin-left: 10px;
    margin-top: 10px;
    margin-bottom: 10px;
    margin-right: 0px;
    width: 40%
}

.public-btn {
    width: 140px;
}

/* Scrollbar */
::-webkit-scrollbar {
    width: .45rem;
}

::-webkit-scrollbar-thumb {
    background-color: rgba(27, 27, 27, .4);
    border-radius: 3px;
}

::-webkit-scrollbar-track {
    background: transparent;
}
</style>