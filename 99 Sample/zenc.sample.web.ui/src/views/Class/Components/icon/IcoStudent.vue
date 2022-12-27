<template>
    <div :class="IsActive ? 'btn-primary' : ''">

        <svg width="20" height="20" viewBox="0 0 60 60" :fill="IsActive ? 'white' : 'black'"
            xmlns="http://www.w3.org/2000/svg">
            <rect width="60" height="60" :fill="IsActive ? '#cb0c9f' : 'white'" />
            <circle cx="30.5" cy="19" r="13" />
            <path
                d="M5 50.5755C5 41.5878 12.2859 34.3019 21.2736 34.3019H38.7264C47.7141 34.3019 55 41.5878 55 50.5755C55 53.5714 52.5714 56 49.5755 56H10.4245C7.42864 56 5 53.5714 5 50.5755Z" />
        </svg>
        <p class="text-sm mb-0">{{Text}}</p>
    </div>
</template>
<script setup>
import { defineProps } from 'vue'

const Props = defineProps({
    
    IsActive: { type: Boolean },
    Text: { type: String }
})
</script>