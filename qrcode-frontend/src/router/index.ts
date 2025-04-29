import { createRouter, createWebHistory } from 'vue-router'
import AdminView from '../views/AdminView.vue'
import CheckInView from '../views/CheckInView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',           // 添加根路徑路由
      redirect: '/admin'   // 將根路徑重定向到管理頁面
    },
    {
      path: '/admin',
      name: 'admin',
      component: AdminView
    },
    {
      path: '/checkin',
      name: 'checkin',
      component: CheckInView
    },
    {
      path: '/:pathMatch(.*)*',  // 捕獲所有未匹配的路由
      redirect: '/admin'         // 重定向到管理頁面
    }
  ]
})

export default router