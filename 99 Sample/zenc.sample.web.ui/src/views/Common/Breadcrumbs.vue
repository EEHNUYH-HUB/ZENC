<template>
  <nav aria-label="breadcrumb card">
    <ol class="px-0 pt-1 pb-0 mb-0 bg-transparent breadcrumb me-sm-6">
      <li class="text-sm breadcrumb-item" :class="textWhite" v-for="item,index in this.$store.state.navigatorArray"
        :key="index">
        <router-link :to="{ path:item.Path,query:item.Query}">
          <a :class="textWhite" class="opacity-8" href="#">{{ item.DisplayName }}</a>
        </router-link>
      </li>
    </ol>
    <h6 class="mb-0 font-weight-bolder" :class="textWhite ? 'text-white' : ''">
      {{this.$store.state.currentName}}
    </h6>
  </nav>


</template>

<script>
// import { useRoute } from 'vue-router';
// import router from '@/router';
export default {
  name: "tbreadcrumbs",
  props: {
    currentPage: {
      required: true,
    },
    textWhite: {
      type: String,
    },
  },
};
</script>
