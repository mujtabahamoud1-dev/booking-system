import { createRouter, createWebHistory, type RouteRecordRaw } from "vue-router";
import { useAuthStore } from "@/features/auth/store";

// Route meta drives the global guard below:
//   requiresAuth  – must be logged in
//   requiresAdmin – must be logged in as admin
//   guestOnly     – only for logged-out visitors (login/register)
const routes: RouteRecordRaw[] = [
  {
    path: "/",
    name: "services",
    component: () => import("@/features/services/views/ServicesListView.vue"),
  },
  {
    path: "/login",
    name: "login",
    component: () => import("@/features/auth/views/LoginView.vue"),
    meta: { guestOnly: true },
  },
  {
    path: "/register",
    name: "register",
    component: () => import("@/features/auth/views/RegisterView.vue"),
    meta: { guestOnly: true },
  },
  {
    path: "/book/:serviceId",
    name: "book",
    component: () => import("@/features/bookings/views/BookServiceView.vue"),
    meta: { requiresAuth: true },
    props: true,
  },
  {
    path: "/bookings",
    name: "my-bookings",
    component: () => import("@/features/bookings/views/MyBookingsView.vue"),
    meta: { requiresAuth: true },
  },
  {
    path: "/admin",
    name: "admin-dashboard",
    component: () => import("@/features/admin/views/AdminDashboardView.vue"),
    meta: { requiresAdmin: true },
  },
  {
    path: "/admin/services",
    name: "admin-services",
    component: () => import("@/features/services/views/AdminServicesView.vue"),
    meta: { requiresAdmin: true },
  },
  {
    path: "/admin/slots",
    name: "admin-slots",
    component: () => import("@/features/slots/views/AdminSlotsView.vue"),
    meta: { requiresAdmin: true },
  },
  {
    path: "/admin/bookings",
    name: "admin-bookings",
    component: () => import("@/features/bookings/views/AdminBookingsView.vue"),
    meta: { requiresAdmin: true },
  },
  { path: "/:pathMatch(.*)*", redirect: "/" },
];

export const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to) => {
  const auth = useAuthStore();

  if ((to.meta.requiresAuth || to.meta.requiresAdmin) && !auth.isAuthenticated) {
    return { name: "login", query: { redirect: to.fullPath } };
  }
  if (to.meta.requiresAdmin && !auth.isAdmin) {
    return { name: "services" };
  }
  if (to.meta.guestOnly && auth.isAuthenticated) {
    return auth.isAdmin ? { name: "admin-dashboard" } : { name: "services" };
  }
  return true;
});
