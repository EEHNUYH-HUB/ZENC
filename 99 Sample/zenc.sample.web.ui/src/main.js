import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import store from "./store";
import { VueCookies } from 'vue-cookies'
import "./assets/css/nucleo-icons.css";
import "./assets/css/nucleo-svg.css";
import SoftUIDashboard from "./soft-ui-dashboard";

const app = createApp(App)
app.use(router)
app.use(VueCookies)
app.use(store)
app.use(SoftUIDashboard)
app.mount('#app')

