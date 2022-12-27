<template>
    <div :class="IsActive ? 'btn-primary' : ''">


        <svg width="20" height="20" viewBox="0 0 60 60" :fill="IsActive ? 'white' : 'black'"
            xmlns="http://www.w3.org/2000/svg">
            <rect width="60" height="60" :fill="IsActive ? '#cb0c9f' : 'white'" />
            <path fill-rule="evenodd" clip-rule="evenodd"
                d="M30 55C43.8071 55 55 43.8071 55 30C55 16.1929 43.8071 5 30 5C16.1929 5 5 16.1929 5 30C5 43.8071 16.1929 55 30 55ZM21 22C18.7909 22 17 23.7909 17 26V27C17 29.2091 18.7909 31 21 31C23.2091 31 25 29.2091 25 27V26C25 23.7909 23.2091 22 21 22ZM35 26C35 23.7909 36.7909 22 39 22C41.2091 22 43 23.7909 43 26V27C43 29.2091 41.2091 31 39 31C36.7909 31 35 29.2091 35 27V26ZM27 38C25.8954 38 25 38.8954 25 40C25 41.1046 25.8954 42 27 42H33C34.1046 42 35 41.1046 35 40C35 38.8954 34.1046 38 33 38H27Z" />
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
