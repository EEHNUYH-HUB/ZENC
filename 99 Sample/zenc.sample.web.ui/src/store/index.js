
import { createStore } from "vuex";
import bootstrap from "bootstrap/dist/js/bootstrap.min.js";
import axios from "axios";
import router from '@/router';

export default createStore({
  state: {
    hideConfigButton: false,
    isPinned: true,
    showConfig: false,
    isTransparent: "",
    isRTL: false,
    color: "",
    isNavFixed: false,
    isAbsolute: false,
    showNavs: true,
    showSidenav: true,
    showNavbar: true,
    showFooter: true,
    showMain: true,
    navbarFixed:
      "position-sticky blur shadow-blur left-auto top-1 z-index-sticky px-0 mx-4",
    absolute: "position-absolute px-4 mx-0 w-100 z-index-2",
    bootstrap,
    user: null,
    navigatorArray: new Array,
    currentName: "",
    containerCss: 'container p-0',
    isMobile: false,
    addNaviation:(r)=>{
      navigatorArray.push(r)
    }
  },
  mutations: {
    setUserData(state, userData){
      state.user = userData;
      localStorage.setItem("user", JSON.stringify(userData));
      axios.defaults.headers['Authorization'] = `Bearer ${userData.token}`;
    },
    clearUserData(state){
      state.user = null;
      localStorage.removeItem("user");
      axios.defaults.headers['Authorization'] = null;
    },
    toggleConfigurator(state) {
      state.showConfig = !state.showConfig;
    },
    navbarMinimize(state) {
      const sidenav_show = document.querySelector(".g-sidenav-show");
      if (sidenav_show.classList.contains("g-sidenav-hidden")) {
        sidenav_show.classList.remove("g-sidenav-hidden");
        sidenav_show.classList.add("g-sidenav-pinned");
        state.isPinned = true;
      } else {
        sidenav_show.classList.add("g-sidenav-hidden");
        sidenav_show.classList.remove("g-sidenav-pinned");
        state.isPinned = false;
      }
    },
    sidebarType(state, payload) {
      state.isTransparent = payload;
    },
    cardBackground(state, payload) {
      state.color = payload;
    },
    navbarFixed(state) {
      if (state.isNavFixed === false) {
        state.isNavFixed = true;
      } else {
        state.isNavFixed = false;
      }
    },
    toggleEveryDisplay(state) {
      state.showNavbar = !state.showNavbar;
      state.showSidenav = !state.showSidenav;
      state.showFooter = !state.showFooter;

      if (state.showNavbar) {
        state.containerCss = "container p-0";
      }
      else {
        state.containerCss = "container-fluid bg-white";
      }

    },
    toggleHideConfig(state) {
      state.hideConfigButton = !state.hideConfigButton;
    }
    
  },
  actions: {
    toggleSidebarColor({ commit }, payload) {
      commit("sidebarType", payload);
    },
    setCardBackground({ commit }, payload) {
      commit("cardBackground", payload);
    },
    logout({commit}){
      commit("clearUserData");
      router.push('/login');
    }
  },
  getters: {
    isLogin(state){
      return state.username !== '';
    }
  },
});
