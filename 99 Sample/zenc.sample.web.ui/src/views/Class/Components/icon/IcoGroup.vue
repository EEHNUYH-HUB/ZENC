<template>
    <div :class="IsActive ? 'btn-primary' : ''">

        <svg width="20" height="20" viewBox="0 0 60 60" :fill="IsActive ? 'white' : 'black'"
            xmlns="http://www.w3.org/2000/svg">
            <rect width="60" height="60" :fill="IsActive ? '#cb0c9f' : 'white'" />
            <circle cx="18.9643" cy="29.0357" r="8.03571" />
            <circle cx="41.9206" cy="30.0695" r="8.03571" />
            <circle cx="31" cy="14" r="7" />
            <path
                d="M1 50.4173C1 44.6038 5.71279 39.891 11.5263 39.891H23.807C29.6205 39.891 34.3333 44.6038 34.3333 50.4173C34.3333 52.9088 32.3136 54.9286 29.8221 54.9286H5.51128C3.01977 54.9286 1 52.9088 1 50.4173Z" />
            <path fill-rule="evenodd" clip-rule="evenodd"
                d="M35.604 54.9286H53.3346C55.3589 54.9286 57 53.2875 57 51.2632C57 46.5397 53.1708 42.7105 48.4474 42.7105L36.7549 42.7105C38.8197 44.6324 40.1111 47.374 40.1111 50.4173C40.1111 52.9074 38.0936 54.9263 35.604 54.9286Z" />
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