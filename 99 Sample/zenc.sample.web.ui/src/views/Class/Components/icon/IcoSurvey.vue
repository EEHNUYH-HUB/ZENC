<template>
    <div :class="IsActive ? 'btn-info' : ''">

        <svg width="20" height="20" viewBox="0 0 60 60" :fill="IsActive ? 'white' : 'black'"
            xmlns="http://www.w3.org/2000/svg">
            <rect width="60" height="60" :fill="IsActive ? '#17c1e8' : 'white'" />
            <path
                d="M35.8617 40.084L52.9526 22.9922C53.6278 22.3461 54.5291 21.99 55.4636 22.0002C56.3981 22.0104 57.2915 22.3861 57.9524 23.0468C58.6134 23.7076 58.9893 24.6008 58.9998 25.5353C59.0102 26.4697 58.6544 27.3712 58.0085 28.0465L40.9148 45.1383C40.5249 45.5283 40.0282 45.7941 39.4874 45.9023L34 47L35.0978 41.5113C35.2059 40.9705 35.4718 40.4739 35.8617 40.084Z"
                :stroke="BackgroundColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            <path fill-rule="evenodd" clip-rule="evenodd"
                d="M10 14C10 10.6863 12.6863 8 16 8H42C45.3137 8 48 10.6863 48 14V27.945L35.8617 40.084C35.5986 40.3471 35.3919 40.6589 35.2521 41H17.5C16.6716 41 16 41.6716 16 42.5C16 43.3284 16.6716 44 17.5 44H34.6L34 47L39.4874 45.9023C40.0282 45.7941 40.5249 45.5283 40.9148 45.1383L48 38.0539V49C48 52.3137 45.3137 55 42 55H16C12.6863 55 10 52.3137 10 49V14ZM16 20.5C16 19.6716 16.6716 19 17.5 19H40.5C41.3284 19 42 19.6716 42 20.5C42 21.3284 41.3284 22 40.5 22H17.5C16.6716 22 16 21.3284 16 20.5ZM17.5 30C16.6716 30 16 30.6716 16 31.5C16 32.3284 16.6716 33 17.5 33H40.5C41.3284 33 42 32.3284 42 31.5C42 30.6716 41.3284 30 40.5 30H17.5Z" />
            <path d="M50.6667 25.3333L55.6667 30.3333" :stroke="BackgroundColor" stroke-width="2" />
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