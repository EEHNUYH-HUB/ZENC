<template>
    <div :class="IsActive ? 'btn-primary' : ''">

        <svg width="20" height="20" viewBox="0 0 60 60" :fill="IsActive ? 'white' : 'black'"
            xmlns="http://www.w3.org/2000/svg">
            <rect width="60" height="60" :fill="IsActive ? '#cb0c9f' : 'white'" />
            <path
                d="M46 37.8842C46 39.4578 46.9226 40.8852 48.3574 41.5314L57.1787 45.5043C58.5022 46.1004 60 45.1323 60 43.6807L60 16.0942C60 14.6427 58.5022 13.6746 57.1787 14.2707L48.3574 18.2436C46.9226 18.8898 46 20.3172 46 21.8908V37.8842Z" />
            <path fill-rule="evenodd" clip-rule="evenodd"
                d="M8 13C3.58172 13 0 16.5817 0 21V39C0 43.4183 3.58172 47 8 47H35C39.4183 47 43 43.4183 43 39V17C43 14.7909 41.2091 13 39 13H8ZM9.5 27C11.9853 27 14 24.9853 14 22.5C14 20.0147 11.9853 18 9.5 18C7.01472 18 5 20.0147 5 22.5C5 24.9853 7.01472 27 9.5 27Z" />
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
