<template>

  <main class="main-content position-relative max-height-vh-100 h-100 border-radius-lg">
    <!-- nav -->
    <Navbar :class="[navClasses]" v-if="this.$store.state.showNavbar" />
    <div :class="this.$store.state.containerCss" >
      <Breadcrumbs class="col-12 m-2" v-if="this.$store.state.showNavbar">
      </Breadcrumbs>
      <router-view style="min-height:calc(100vh - 246px)" />
      <Footer class="mt-4" v-show="this.$store.state.showFooter"></Footer>

    </div>


  </main>
</template>

<script>
import Tsidenav from "@/views/Common/TSidenav";
import Navbar from "@/views/Common/Navbar.vue";
import Footer from "@/views/Common/Footer.vue";
import Breadcrumbs from '@/views/Common/Breadcrumbs.vue';
import { mapMutations } from "vuex";

export default {
  name: "App",
  components: {
    Tsidenav,
    Navbar,
    Footer, Breadcrumbs
  },
  methods: {
    ...mapMutations(["toggleConfigurator", "navbarMinimize"]),
  },
  computed: {
    navClasses() {
      return {
        "position-sticky blur shadow-blur mt-4 left-auto top-1 z-index-sticky": this
          .$store.state.isNavFixed,
        "position-absolute px-4 mx-0 w-100 z-index-2": this.$store.state
          .isAbsolute,
        "px-0 ": !this.$store.state.isAbsolute,
      };
    },
  },
  created() {
    this.$store.state.isMobile = window.innerWidth < 980;
    window.addEventListener("resize", (e) => { 
      if (window.innerWidth < 980) {
        this.$store.state.isMobile = true;
      }
      else {
        this.$store.state.isMobile = false;
      }
      //console.log(this.$store.state.isMobile)

    });
    
    const userString = localStorage.getItem("user");
    if(userString){
      const userData = JSON.parse(userString);
      //console.log('hello!', userData);
      this.$store.commit('setUserData',userData);
    }
  },
  beforeMount() {
    this.$store.state.isTransparent = "bg-transparent";
  },
};
</script>
