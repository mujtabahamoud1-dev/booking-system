import { createApp } from "vue";
import { createPinia } from "pinia";
import "./style.css";
import App from "./App.vue";
import { router } from "./router";
import { i18n, initLocale } from "@/shared/i18n";
import { useAuthStore } from "@/features/auth/store";

const app = createApp(App);
app.use(createPinia());
app.use(i18n);

// Set <html lang>/<html dir> from the stored (or browser-detected) locale before
// the first paint so an RTL session never flashes left-to-right.
initLocale();

// Rehydrate the session and wire the API client's refresh hooks before the
// router (and its guards) start evaluating the first navigation.
useAuthStore().init();

app.use(router);
app.mount("#app");
