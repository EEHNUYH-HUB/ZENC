<template>
  <div class="col-lg-2 col-6 text-center">
    <div class="border-dashed border-1 border-secondary border-radius-md py-3">
      <h6 class=" small mb-0" :class="css">{{ title }}</h6>
      <span v-if="subtitle" class="text-xs  mb-0" >{{ subtitle }}</span>
      <h4 class="font-weight-bolder">
        <span class="text-sm">{{ prefix }}</span>
        {{ "" }}
        {{ count }}
        {{ "" }}
        <span class="text-sm">{{ suffix }}</span>
      </h4>
    </div>
  </div>
</template>
<script>
// import countTo from "vue-count-to/src";

export default {
  name: "NumberCard",
  components: {
    // countTo,
  },
  props: { 
    title: {
      type: String,
      default: "",
    },
    subtitle: {
      type: String,
      default: "",
    },
    count: {
      type: Number,
      default: 0,
    },
    prefix: {
      type: String,
      default: "",
    },
    suffix: {
      type: String,
      default: "",
    },
    css: {
      type: String,
      default: "text-info",
    },
  },
};
</script>
