import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import ElementPlus from 'element-plus';
import 'element-plus/lib/theme-chalk/index.css';
import { URL } from './global'

const app = createApp(App)

app.config.globalProperties.$URL=URL

app.use(router)
app.use(ElementPlus)
app.mount('#app')
