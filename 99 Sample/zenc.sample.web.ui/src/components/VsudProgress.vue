<template>
  <div :class="size =='large'? 'progress-lg' : 'progress'">
    <div
      class="progress-bar"
      :class="getClasses(color, variant)"
      :style="{ width: percentage + '%' }"
    ></div>
  </div>
</template>

<script>
export default {
  name: "VsudProgress",
  props: {
    color: {
      type: String,
      default: "success",
    },
    variant: {
      type: String,
      default: "fill",
    },
    percentage: {
      type: Number,
      required: true,
    },
    size:{
      type:String,
      default: ''
    }
  },
  methods: {
    getClasses: (color, variant) => {
      let colorValue;

      if (variant === "gradient") {
        colorValue = `bg-gradient-${color}`;
      } else {
        colorValue = `bg-${color}`;
      }

      return `${colorValue}`;
    },
  },
};
</script>
