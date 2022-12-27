<template>
    <div class="radios  row">
        <div class=" col-lg-2"  v-for="(userItem, index) in OnClassInstance.UserList" :key="index"
            v-show="!userItem.isClassOwner">
            <div class="p-1  border-radius-lg" :class="userItem.isRqtComment ?'bg-primary':''">
                <div class="card m-1 ">
                    <div class="card-header p-3 pb-0  ">
                        <p class="text-sm mb-0  ">
                            {{ userItem.userInfo.USER_NAME }} {{ userItem.userInfo.STUDENT_NUMBER}}
                            <!-- ({{
                            userItem.userInfo.USER_EMAIL}}) -->
                        </p>
                    </div>
                    <div class="card-body border-radius-lg p-3 row ">
                        <img :src="userItem.ImgUrl" class="col" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script setup>
import {  defineProps } from "vue"

 const props = defineProps({ 
     OnClassInstance: Object

 });



</script>
<style scoped>


</style>
