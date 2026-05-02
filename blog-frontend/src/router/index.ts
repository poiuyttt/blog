import { createRouter, createWebHistory } from "vue-router";
import HomePage from "../views/HomePage.vue";
import AboutPage from "../views/AboutPage.vue";
import SearchPage from "../views/SearchPage.vue";
import LoginPage from "../views/LoginPage.vue";

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
    path: "/login",
    name: "Login",
    component: LoginPage,
  },
  {
    path: "/article/:id",
    name: "article",
    component: () => import("../views/ArticlePage.vue"),
    meta: { requiresAuth: true },
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

router.beforeEach((to, from, next) => {
  const requiresAuth = to.matched.some((record) => record.meta.requiresAuth);

  const isLoggedIn = !!localStorage.getItem("token");

  if (requiresAuth && !isLoggedIn) {
    next("/login");
  } else {
    next();
  }
});

export default router;
