/// <reference types="vite/client" />

declare module '*.vue' {
    import type { DefineComponent } from 'vue'
    const component: DefineComponent<{}, {}, any>
    export default component
  }
  declare module 'element-plus/dist/locale/zh-tw.mjs' {
    const zhTw: any
    export default zhTw
  }