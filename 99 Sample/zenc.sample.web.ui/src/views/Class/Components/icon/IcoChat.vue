<template>
    <div :class="IsActive ? 'btn-primary' : ''">

        <svg width="20" height="20" viewBox="0 0 60 60" :fill="IsActive ? 'white' : 'black'"
            xmlns="http://www.w3.org/2000/svg">
            <rect width="60" height="60" :fill="IsActive ? '#cb0c9f' : 'white'" />
            <path fill-rule="evenodd" clip-rule="evenodd"
                d="M4.68318 16.155C3.55871 18.447 3.55871 21.4274 3.55871 27.388C3.55871 33.8906 3.55871 37.1418 4.78541 39.6422C5.95632 42.0289 7.88661 43.9592 10.2733 45.1301C10.9738 45.4737 11.7332 45.7211 12.6065 45.8992L13.4038 52.1483C13.5521 53.3111 14.917 53.862 15.8309 53.1281L24.263 46.3568H36.748C42.9868 46.3568 46.1061 46.3568 48.5561 45.3083C51.6292 43.9931 54.0776 41.5447 55.3928 38.4715C56.4413 36.0216 56.4413 32.9023 56.4413 26.6635C56.4413 21.3846 56.4413 18.7451 55.5541 16.6721C54.4413 14.0718 52.3695 12 49.7692 10.8872C47.6962 10 45.0567 10 39.7778 10H20.9467C14.9861 10 12.0057 10 9.71373 11.1245C7.52595 12.1978 5.75652 13.9672 4.68318 16.155Z" />
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
