<template>
    <nav
        class="top-0 navbar navbar-expand-lg position-absolute z-index-3  my-3 start-0 end-0 shadow-none blur-rounded  bg-white">
        <div class="container-fluid">
            <div>
                <Breadcrumbs>
                </Breadcrumbs>

                <p class="text-sm mb-0 mt-2">
                    {{ OnClassInstance.ClassItem.curriculumanme }} <b> {{ OnClassInstance.ClassItem.proname
                    }}</b>
                </p>

                <p class="text-sm">
                    {{ GetTime(OnClassInstance.ClassItem.col_start_date) }} ~
                    {{ GetTime(OnClassInstance.ClassItem.col_end_date) }}
                </p>
            </div>
            <div v-if="OnClassInstance.GroupInstance && OnClassInstance.GroupInstance.GroupItem">
                <h6>
                    {{ OnClassInstance.GroupInstance.GroupItem.col_classgroup_name }} ( {{
                    OnClassInstance.GroupInstance.IntervalMin }} )
                </h6>
            </div>

            <button class="shadow-none navbar-toggler ms-2" type="button" data-bs-toggle="collapse"
                data-bs-target="#navigation" aria-controls="navigation" aria-expanded="false"
                aria-label="Toggle navigation">
                <span class="mt-2 navbar-toggler-icon">
                    <span class="navbar-toggler-bar bar1"></span>
                    <span class="navbar-toggler-bar bar2"></span>
                    <span class="navbar-toggler-bar bar3"></span>
                </span>
            </button>
            <div id="navigation" class=" p-3 pt-0 pb-0 collapse navbar-collapse  justify-content-end">
                <ul class=" navbar-nav navbar-nav-hover ">
                    <li>
                        <IcoAudio Text="오디오" @click="OnClassInstance.MuteAudio()" class="nav-item col mb-0 btn ms-1"
                            v-if="OnClassInstance.IsVisibleAudio"
                            :IsActive="OnClassInstance.IsAudio && OnClassInstance.IsStart">
                        </IcoAudio>
                    </li>
                    <li>
                        <IcoVideo @click="OnClassInstance.MuteVideo()" class=" nav-item col mb-0  btn ms-1"
                            v-if="OnClassInstance.IsVisibleVideo" Text="비디오"
                            :IsActive="OnClassInstance.IsVideo && OnClassInstance.IsStart">
                        </IcoVideo>
                    </li>
                    <li>
                        <IcoRecord @click="OnClassInstance.RunRecord()" class=" nav-item col mb-0  btn ms-3"
                            v-if="OnClassInstance.IsVisibleRecord" Text="녹화"
                            :IsActive="OnClassInstance.IsRunRecord && OnClassInstance.IsStart ">
                        </IcoRecord>
                    </li>
                    <li>
                        <IcoWhiteboard @click="OnClassInstance.ShowWhiteBoard()" class="nav-item col mb-0  btn  ms-3 "
                            v-if="OnClassInstance.IsVisibleWhiteboard" Text="판서"
                            :IsActive="OnClassInstance.IsShowWhiteBoard && OnClassInstance.IsStart ">
                        </IcoWhiteboard>
                    </li>
                    <li>
                        <IcoShare @click="OnClassInstance.SharedScreen()" class="nav-item col mb-0  btn ms-1"
                            v-if="OnClassInstance.IsVisibleShare" Text="공유"
                            :IsActive="OnClassInstance.IsShowShared && OnClassInstance.IsStart ">
                        </IcoShare>
                    </li>
                    <li>
                        <IcoGroup @click="OnClassInstance.ShowGroupSetting()" class="nav-item col mb-0  btn ms-3 "
                            v-if="OnClassInstance.IsVisibleGroup" Text="그룹"
                            :IsActive="OnClassInstance.IsShowGroupSetting && OnClassInstance.IsStart ">
                        </IcoGroup>
                    </li>
                    <li>
                        <IcoSurvey @click="OnClassInstance.ShowQuizList()"
                            :IsActive="OnClassInstance.IsShowQuiz && OnClassInstance.IsStart "
                            class="nav-item col mb-0  btn  ms-3 " v-if="OnClassInstance.IsVisibleQuiz" Text="퀴즈/설문">
                        </IcoSurvey>
                    </li>
                    <li>
                        <IcoFileList @click="OnClassInstance.ShowFileListComment()" class="nav-item col mb-0  btn ms-1 "
                            Text="강의자료" :IsActive="OnClassInstance.IsVisibleFileList">
                        </IcoFileList>
                    </li>
                    <li>
                        <IcoAttitudeOn @click="OnClassInstance.RunAttitude()" class="nav-item col mb-0  btn  ms-3 "
                            v-if="OnClassInstance.IsVisibleAttitude" Text="태도"
                            :IsActive="OnClassInstance.IsAttitude && OnClassInstance.IsStart">
                        </IcoAttitudeOn>
                    </li>

                    <li>
                        <IcoComment @click="OnClassInstance.RequestComment()" class="nav-item col mb-0  btn ms-3 "
                            v-if="OnClassInstance.IsVisibleComment" Text="발언요청"
                            :IsActive="OnClassInstance.IsRqtComment && OnClassInstance.IsStart">
                        </IcoComment>
                    </li>

                    <li>
                        <IcoComment @click="OnClassInstance.RequestGroupComment()" class="nav-item col mb-0  btn ms-3 "
                            v-if="OnClassInstance.IsVisibleGroupComment" Text="참여요청"
                            :IsActive="OnClassInstance.IsRqtGroupComment && OnClassInstance.IsStart">
                        </IcoComment>
                    </li>
                    <li>
                        <IcoClose @click="OnClassInstance.RemoveGroup()" class="nav-item col mb-0  btn ms-3 "
                            v-if="OnClassInstance.IsVisibleClose" Text="나가기">
                        </IcoClose>
                    </li>
                    <li>
                        <IcoStart @click="OnClassInstance.Start()" class="nav-item col mb-0  btn ms-3 "
                            v-if="OnClassInstance.IsVisibleStart && OnClassInstance.IsStart" Text="멈춤" IsActive="true">
                        </IcoStart>
                    </li>
                    <li>
                        <IcoStop @click="OnClassInstance.Start()" class="nav-item col mb-0  btn ms-3 "
                            v-if="OnClassInstance.IsVisibleStart && !OnClassInstance.IsStart" Text="시작">
                        </IcoStop>
                    </li>
                </ul>
            </div>

        </div>
    </nav>
</template>

<script setup>
import { defineProps ,onMounted} from 'vue'
import { GetTime } from "@/minisoft/MNCommon"
import { useStore } from "vuex";
import IcoAttitudeOff from "@/views/Class/Components/icon/IcoAttitudeOff.vue"
import IcoAttitudeOn from "@/views/Class/Components/icon/IcoAttitudeOn.vue"
import IcoAudio from "@/views/Class/Components/icon/IcoAudio.vue"
import IcoGroup from "@/views/Class/Components/icon/IcoGroup.vue"
import IcoQuiz from "@/views/Class/Components/icon/IcoQuiz.vue"
import IcoRecord from "@/views/Class/Components/icon/IcoRecord.vue"
import IcoShare from "@/views/Class/Components/icon/IcoShare.vue"
import IcoSurvey from "@/views/Class/Components/icon/IcoSurvey.vue"
import IcoVideo from "@/views/Class/Components/icon/IcoVideo.vue"
import IcoStart from "@/views/Class/Components/icon/IcoStart.vue"
import IcoStop from "@/views/Class/Components/icon/IcoStop.vue"
import IcoWhiteboard from "@/views/Class/Components/icon/IcoWhiteboard.vue"
import IcoComment from '@/views/Class/Components/icon/IcoComment.vue'
import IcoClose from '@/views/Class/Components/icon/IcoClose.vue'
import IcoFileList from '@/views/Class/Components/icon/IcoFileList.vue'

import Breadcrumbs from '@/views/Common/Breadcrumbs.vue';



const store = useStore();
const Props = defineProps({
    OnClassInstance: { type: Object },
    GroupID: { type: Number }
})

onMounted(() => { 
    Props.OnClassInstance.OnLoadData = () => { 
        store.state.navigatorArray = new Array;
        store.state.navigatorArray.push({ Path: 'DASHBOARD', DisplayName: "대쉬보드" })
        store.state.navigatorArray.push({ Path: 'LECTURELIST', Query: { id: Props.OnClassInstance.ClassItem.pk_class_schedule_id, year: Props.OnClassInstance.ClassItem.col_year, semester: Props.OnClassInstance.ClassItem.col_semester }, DisplayName: Props.OnClassInstance.ClassItem.col_year + "년 " + Props.OnClassInstance.ClassItem.col_semester + "학기" })
        store.state.navigatorArray.push({ Path: 'CLASSROOM', Query: { id: Props.OnClassInstance.ClassItem.pk_class_id }, DisplayName: Props.OnClassInstance.ClassItem.subjectname })
        store.state.currentName = Props.OnClassInstance.ClassItem.col_few_weeks + "주차 온라인 강의";


    }
    
})

const test = () => {
    alert("준비중");
};

</script>
