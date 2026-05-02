import { createRouter, createWebHistory } from "vue-router";
import HomePage from "../views/HomePage.vue";
import AboutPage from "../views/AboutPage.vue";
import SearchPage from "../views/SearchPage.vue";
import ArticlePage from "../views/ArticlePage.vue";

import { useRouter } from "vue-router";

const routes = [
  {
    path: "/",
    name: "Home",
    component: HomePage,
  },
  {
    path: "/about",
    name: "About",
    component: AboutPage,
  },
  {
    path: "/article/:id",
    name: "article",
    component: ArticlePage,
  },
  {
    path: "/search",
    name: "search",
    component: SearchPage,
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
