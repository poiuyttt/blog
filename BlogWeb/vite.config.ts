import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";

export default defineConfig({
  plugins: [vue()],
  build: {
    // outDir：打包输出目录（默认 dist）
    outDir: "dist",
    // minify：压缩方式，默认 'esbuild'，速度最快
    minify: "esbuild",
    // rollupOptions：底层打包工具 Rollup 的配置
    rollupOptions: {
      output: {
        // manualChunks：将 node_modules 中的依赖单独打包
        // 作用：减少主包大小，加快页面加载速度
        // 将 element-plus 单独拆包
        manualChunks(id: string) {
          if (id.includes("element-plus")) {
            return "element-plus";
          }
          if (
            id.includes("vue") ||
            id.includes("vue-router") ||
            id.includes("pinia")
          ) {
            return "vue-vendor";
          }
        },
      },
    },
  },
});
