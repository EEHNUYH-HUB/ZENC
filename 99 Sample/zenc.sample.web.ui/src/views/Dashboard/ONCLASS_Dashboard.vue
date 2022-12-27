<template>


    <div class="col-12 ">
        <ProClassToDay v-if="isProfessor" />
        <StdClassToDay v-if="isStudent" />
    </div>
    <div class="col-12 mt-4">
        <ProClassList v-if="isProfessor" />
        <StdClassList v-if="isStudent" />
    </div>
    <div class="col-12 mt-4">
        <Calendar id="classCalendar" />
    </div>
    <div class="col-12 mt-4">
        <ProClassStatus v-if="isProfessor" />
        <StdClassStatus v-if="isStudent" />
    </div>

    
</template>

<script setup>


import downArrWhite from "@/assets/img/down-arrow-white.svg";
import downArrBlack from "@/assets/img/down-arrow-dark.svg";
import { computed, onMounted, onUnmounted,ref } from 'vue'
import { useStore } from 'vuex'
import Swal from 'sweetalert2'
import ProClassToDay from '@/views/Dashboard/Components/ProClassToDay.vue'
import StdClassToDay from '@/views/Dashboard/Components/StdClassToDay.vue'
import ProClassStatus from '@/views/Dashboard/Components/ProClassStatus.vue'
import StdClassStatus from '@/views/Dashboard/Components/StdClassStatus.vue'
import ProClassList from '@/views/Dashboard/Components/ProClassList.vue';
import StdClassList from '@/views/Dashboard/Components/StdClassList.vue';
import Calendar from '@/views/Dashboard/Components/Calendar.vue';

let isStudent = ref(false);
let isProfessor = ref(false);

const store = useStore();

onMounted(() => {
     
    store.state.navigatorArray = new Array;
    store.state.currentName = "";
    const user = store.state.user;
    if (!user) {
        store.dispatch('logout');
        
        return;
    }

    if (user.ROLE == 'STUDENT') {
        isStudent.value = true;
    } else if (user.ROLE == 'PROFESSOR') {
        isProfessor.value = true;
    }
})
onUnmounted(() => {
    
    
})
</script>
