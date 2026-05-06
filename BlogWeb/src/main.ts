import { createApp } from "vue";
import { createPinia } from "pinia";
import piniaPluginPersistedstate from "pinia-plugin-persistedstate";
import ElementPlus from "element-plus";
import "element-plus/dist/index.css";
import App from "./App.vue";
import router from "./router";

import VMdEditor from "@kangc/v-md-editor";
import githubTheme from "@kangc/v-md-editor/lib/theme/github";
import lineNumberPlugin from "@kangc/v-md-editor/lib/plugins/line-number/index";
import hljs from "highlight.js";

import "@kangc/v-md-editor/lib/style/base-editor.css";
import "@kangc/v-md-editor/lib/theme/style/github.css";
import "@kangc/v-md-editor/lib/style/preview.css";
import "./style.css";

const pinia = createPinia();
pinia.use(piniaPluginPersistedstate);

VMdEditor.use(githubTheme, { Hljs: hljs });
VMdEditor.use(
  lineNumberPlugin.default ? lineNumberPlugin.default() : lineNumberPlugin(),
);

const app = createApp(App);
app.use(pinia);
app.use(router);
app.use(ElementPlus);
app.use(VMdEditor);

app.mount("#app");
