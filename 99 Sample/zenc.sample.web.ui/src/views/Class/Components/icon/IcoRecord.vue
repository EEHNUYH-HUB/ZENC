<template>
    <div :class="IsActive ? '' : ''">

        <svg width="20" height="20" viewBox="0 0 60 60" :fill="IsActive ? 'red' : 'black'"
            xmlns="http://www.w3.org/2000/svg">
            <rect width="60" height="60" :fill="IsActive ? 'white' : 'white'" />
            <circle cx="9.5" cy="22.5" r="4.5" :fill="IsActive ? 'white' : 'white'" />
            <circle cx="30" cy="30" r="19.5" />
            <path fill-rule="evenodd" clip-rule="evenodd"
                d="M30 52.88C42.6363 52.88 52.88 42.6363 52.88 30C52.88 17.3637 42.6363 7.12 30 7.12C17.3637 7.12 7.12 17.3637 7.12 30C7.12 42.6363 17.3637 52.88 30 52.88ZM30 56C44.3594 56 56 44.3594 56 30C56 15.6406 44.3594 4 30 4C15.6406 4 4 15.6406 4 30C4 44.3594 15.6406 56 30 56Z" />
        </svg>
        <p class="text-sm mb-0 " :class="IsActive ?'text-danger':''">{{Text}}</p>
    </div>
</template>
<script setup>
import { defineProps } from 'vue'

const Props = defineProps({
    
    IsActive: { type: Boolean },
    Text: { type: String }
})
</script>