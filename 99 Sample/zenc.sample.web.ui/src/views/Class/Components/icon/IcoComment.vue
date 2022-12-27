<template>
    <div :class="IsActive ? 'btn-primary' : ''">

        <svg width="20" height="20" viewBox="0 0 60 60" :fill="IsActive ? 'white' : 'black'"
            xmlns="http://www.w3.org/2000/svg">
            <rect width="60" height="60" :fill="IsActive ? '#cb0c9f' : 'white'" />
            <path
                d="M53.4006 19.7357C53.4006 17.4558 51.5483 15.6076 49.2633 15.6076C46.9783 15.6076 45.126 17.4558 45.126 19.7357V11.2755C45.126 8.99556 43.2737 7.14734 40.9887 7.14734C38.7037 7.14734 36.8514 8.99556 36.8514 11.2755V6.12819C36.8514 3.84823 34.9991 2 32.7141 2C30.4291 2 28.5768 3.84823 28.5768 6.12819V11.2755C28.5768 8.99556 26.7245 7.14734 24.4395 7.14734C22.1545 7.14734 20.3022 8.99556 20.3022 11.2755V34.6463L14.3919 27.4185C12.8017 25.4739 9.96417 25.1119 7.93517 26.5946C5.90876 28.0755 5.39591 30.8784 6.76811 32.9778L16.4968 47.8642C16.4968 47.8642 20.8056 54.2405 26.5763 55.9761C32.3478 57.7117 46.1819 58.5003 51.7957 47.6148C51.7957 47.6148 53.3765 44.8799 53.3377 40.1737V36.9899C53.3791 36.7559 53.4023 36.5151 53.4023 36.2683V19.7357H53.4006Z"
                :stroke="BackgroundColor" stroke-width="2" stroke-miterlimit="10" stroke-linejoin="round" />
            <path d="M28.5768 11.2755V28.1297" :stroke="BackgroundColor" stroke-width="2" stroke-miterlimit="10"
                stroke-linecap="round" stroke-linejoin="round" />
            <path d="M36.8514 28.1297V11.2755" :stroke="BackgroundColor" stroke-width="2" stroke-miterlimit="10"
                stroke-linecap="round" stroke-linejoin="round" />
            <path d="M45.126 11.2755V28.1297" :stroke="BackgroundColor" stroke-width="2" stroke-miterlimit="10"
                stroke-linecap="round" stroke-linejoin="round" />
        </svg>
        <p class="text-sm mb-0">{{ Text }}</p>
    </div>
</template>
<script setup>
import { defineProps } from 'vue'

const Props = defineProps({
    
    IsActive: { type: Boolean },
    Text:{type:String}
})
</script>
