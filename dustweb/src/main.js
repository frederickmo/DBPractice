import { createApp } from 'vue'
import App from './App.vue'
import store from './store'
import router from './router'
import element from "@/plugins/element";

const app = createApp(App)

element(app);
app.use(router).use(store).mount('#app')

