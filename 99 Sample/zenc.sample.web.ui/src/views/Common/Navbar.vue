<template>
  <div class="card "
    :class="css? 'container-fluid top-0 position-sticky z-index-sticky top-2':$store.state.isMobile ? 'mb-6' :'mb-7'"
    v-if="$store.state.user">

    <nav class="top-0 navbar navbar-expand-lg position-absolute z-index-3    start-0 end-0 "
      :class="css ? css : $store.state.isMobile ? 'blur' : 'my-3 py-2 blur blur-rounded shadow mx-4 '">
      <div class="container-fluid">
        <router-link :to="{ path: 'DASHBOARD' } ">
          <a class="navbar-brand font-weight-bolder ms-lg-0 ms-3 " :class="css">Onclass -
            Minisoft</a>
        </router-link>
        <button class="shadow-none navbar-toggler ms-2" type="button" data-bs-toggle="collapse"
          data-bs-target="#navigation" aria-controls="navigation" aria-expanded="false" aria-label="Toggle navigation">
          <span class="mt-2 navbar-toggler-icon">
            <span class="navbar-toggler-bar bar1"></span>
            <span class="navbar-toggler-bar bar2"></span>
            <span class="navbar-toggler-bar bar3"></span>
          </span>
        </button>
        <div id="navigation" class="pt-3 pb-2 collapse navbar-collapse w-100 py-lg-0">
          <ul class="mx-auto navbar-nav navbar-nav-hover ">
            <router-link :to="{ path: 'DASHBOARD' } ">
              <li class=" mx-2 nav-item dropdown dropdown-hover">
                <a id="dropdownMenuPages" :class="css" role="button"
                  class="cursor-pointer nav-link ps-2 d-flex justify-content-between align-items-center">
                  대쉬보드
                </a>
              </li>
            </router-link>
            <li class="mx-2 nav-item dropdown dropdown-hover">
              <a id="dropdownMenuPages" role="button" :class="css"
                class="cursor-pointer nav-link ps-2 d-flex justify-content-between align-items-center"
                data-bs-toggle="dropdown" aria-expanded="false">
                강의 리스트
                <img :src="downArrWhite" alt="down-arrow" class="arrow ms-1 d-none" />
                <img :src="downArrBlack" alt="down-arrow" class="arrow ms-1 d-block d-lg-block" />
              </a>
              <div class="p-3 mt-0 dropdown-menu dropdown-menu-animation dropdown-sm border-radius-xl mt-lg-3"
                aria-labelledby=" dropdownMenuPages">
                <div class="row d-none d-lg-block">
                  <div class="px-4 py-2 col-12">
                    <div class="row">
                      <div class="col-12 position-relative">
                        <router-link
                          :to="{ path: 'LECTURELIST', query: { year:item.col_year,semester:item.col_semester }}"
                          v-for="item,index in classSemList" :key="index">
                          <span class="dropdown-item border-radius-md">
                            {{item.col_year}}년 {{ item.col_semester }}학기
                            <template v-if="item.col_status =='ST010102'">(진행중)</template>
                          </span>
                        </router-link>
                      </div>

                    </div>
                  </div>
                </div>

              </div>
            </li>

            <li v-if="$store.state.user.ROLE == 'STUDENT'" class="mx-2 nav-item dropdown dropdown-hover">
              <a id="dropdownMenuPages" role="button" :class="css"
                class="cursor-pointer nav-link ps-2 d-flex justify-content-between align-items-center"
                data-bs-toggle="dropdown" aria-expanded="false">
                추천 과목

              </a>

            </li>
            <li class="mx-2 nav-item dropdown dropdown-hover">
              <a id="dropdownMenuPages" role="button" :class="css"
                class="cursor-pointer nav-link ps-2 d-flex justify-content-between align-items-center"
                data-bs-toggle="dropdown" aria-expanded="false" @click="ManageAuthentication()">
                사용자 관리

              </a>

            </li>
            <li class="mx-2 nav-item dropdown dropdown-hover">
              <a id="dropdownMenuPages" role="button" :class="css"
                class="cursor-pointer nav-link ps-2 d-flex justify-content-between align-items-center"
                data-bs-toggle="dropdown" aria-expanded="false">
                메시지 관리

              </a>

            </li>

          </ul>
          <span class="navbar-brand font-weight-bolder ms-lg-0 ms-3 " :class="css" >{{
            $store.state.user.USER_NAME
            }}
            ({{ $store.state.user.ROLE == 'STUDENT' ?
            $store.state.user.STUDENT_NUMBER : $store.state.user.PROFESSOR_NUMBER}})</span>
          <div class="navbar-brand font-weight-bolder ms-lg-0  ">
            <a href="javascript:;" @click="Logout()" v-if="css"
              class="mb-0 btn btn-sm  me-1  btn-round bg-white ">로그아웃</a>
            <a href="javascript:;" @click="Logout()" v-else
              class="mb-0 btn btn-sm  me-1 bg-gradient-success btn-round bg-white text-white">로그아웃</a>

          </div>
        </div>

      </div>
    </nav>

  </div>

</template>
<script>
import downArrWhite from "@/assets/img/down-arrow-white.svg";
import downArrBlack from "@/assets/img/down-arrow-dark.svg";
import MNClient from '@/minisoft/MNClient'
import { mapMutations, mapActions } from "vuex";
import router from "../../router";


export default {
  name: "tnavbar",
  data() {
    return {
      showMenu: false, downArrWhite,
      downArrBlack,classSemList:Array
    };
  },
  props: ["minNav", "textWhite","css"],
  async created() {
    

    this.classSemList = await MNClient.ExecDataTable("CLASS", "CLASSSEMESTERINFO", null);
    
  },
  methods: {
    ...mapMutations(["navbarMinimize", "toggleConfigurator"]),
    ...mapActions(["toggleSidebarColor"]),

    toggleSidebar() {
      this.toggleSidebarColor("bg-white");
      this.navbarMinimize();
    },
    Logout() {
      this.$store.dispatch('logout');
    },
    ManageAuthentication(){
      router.push({ path: '/MNGAUTH' });
    }
  },
  components: {
    //TBreadcrumbs
  },
  
  updated() {
    // const navbar = document.getElementById("navbarBlur");
    // window.addEventListener("scroll", () => {
    //   if (window.scrollY > 10 && this.$store.state.isNavFixed) {
    //     navbar.classList.add("blur");
    //     navbar.classList.add("position-sticky");
    //     navbar.classList.add("shadow-blur");
    //   } else {
    //     navbar.classList.remove("blur");
    //     navbar.classList.remove("position-sticky");
    //     navbar.classList.remove("shadow-blur");
    //   }
    // });
  },

};
</script>
