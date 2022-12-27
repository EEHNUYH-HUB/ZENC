<template>
  <div
    id="sidenavCard"
    class="shadow-none card card-background card-background-mask-secondary"
  >
    <div
      class="full-background"
      :style="{ 'background-image': 'url(' + imgWhiteCurved + ')' }"
    ></div>
    <div class="p-3 card-body text-start w-100">
      <div
        class="mb-3 text-center bg-white shadow icon icon-shape icon-sm d-flex align-items-center justify-content-center border-radius-md"
      >
        <i
          id="sidenavCardIcon"
          class="top-0 text-lg text-dark text-gradient"
          aria-hidden="true"
          :class="icon"
        ></i>
      </div>
     
      <div class="docs-info">
        <h6 class="mb-0 text-white up">{{ textPrimary }}</h6>
        <p class="text-xs font-weight-bold">{{ textSecondary }}</p>
        <router-link
              :to="{ name: 'Login' }"
              class="px-0 nav-link font-weight-bold"
              :class="this.$store.state.isAbsolute ? this.$store.state.isAbsolute : 'text-body'"
            >
        <a  target="_blank" class="mb-0 btn btn-white btn-sm w-100" @click="Logout()">
          {{ label }}
        </a>
            </router-link>
        
      </div>
    </div>
  </div>
</template>
<script>
import imgWhiteCurved from "../../../assets/img/curved-images/white-curved.jpeg";
import {useStore} from 'vuex';
export default {
  name: "SidenavCard",
  props: {
    textPrimary: {
      type: String,
      default: "",
    },
    textSecondary: {
      type: String,
      default: "",
    },
    label: {
      type: String,
      default: "",
    },
    icon: {
      type: String,
      default: "",
    },
  },
  data() {
    return {
      imgWhiteCurved,
    };
  },
  setup(){
    const store = useStore();
    function Logout(){
      store.dispatch('logout');
    }

    return {
      Logout
    }
  }
};
</script>
