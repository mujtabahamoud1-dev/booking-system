<script setup lang="ts">
import { computed } from "vue";
import { RouterLink, useRouter } from "vue-router";
import { useI18n } from "vue-i18n";
import { useAuthStore } from "@/features/auth/store";
import LanguageSwitcher from "@/shared/components/LanguageSwitcher.vue";

const auth = useAuthStore();
const router = useRouter();
const { t } = useI18n();

// Route targets shown for the current session, in nav order.
const links = computed(() => {
  if (!auth.isAuthenticated) {
    return [{ to: "/", label: t("common.nav.services") }];
  }
  if (auth.isAdmin) {
    return [
      { to: "/admin", label: t("common.nav.dashboard") },
      { to: "/admin/services", label: t("common.nav.services") },
      { to: "/admin/slots", label: t("common.nav.slots") },
      { to: "/admin/bookings", label: t("common.nav.bookings") },
    ];
  }
  return [
    { to: "/", label: t("common.nav.services") },
    { to: "/bookings", label: t("common.nav.myBookings") },
  ];
});

async function logout(): Promise<void> {
  await auth.logout();
  router.push("/login");
}
</script>

<template>
  <!-- Sticky is worth a fixed strip of a desktop window; on a phone it would eat
       a tenth of the screen for the whole scroll, so the header travels with the
       page there and the in-page back links carry the wayfinding. -->
  <header class="z-40 border-b border-line bg-surface/90 backdrop-blur-md md:sticky md:top-0">
    <div
      class="u-gutter mx-auto flex max-w-6xl flex-wrap items-center gap-x-8 pt-1.5 pb-0 lg:h-18 lg:flex-nowrap lg:py-0"
    >
      <RouterLink to="/" class="order-1 flex items-baseline gap-2.5 py-2 whitespace-nowrap">
        <span class="u-display text-lg text-brand">{{ t("common.brand.name") }}</span>
        <span class="u-label hidden text-ink-faint sm:inline">{{
          t("common.brand.discipline")
        }}</span>
      </RouterLink>

      <!-- Four destinations at most, and every one of them fits: the links take
           their own row rather than scrolling out of sight or collapsing into a
           menu that hides how small this app is. They only join the masthead
           row at `lg`, because an admin's four links plus the account controls
           need about a thousand pixels before one row stops being a squeeze. -->
      <nav class="order-3 w-full lg:order-2 lg:w-auto">
        <ul class="flex flex-wrap items-center gap-x-6">
          <li v-for="link in links" :key="link.to">
            <RouterLink
              :to="link.to"
              class="u-label flex min-h-11 items-center border-b-2 border-transparent whitespace-nowrap text-ink-faint transition-colors hover:text-ink lg:min-h-0 lg:py-6"
              active-class="border-brand text-ink"
            >
              {{ link.label }}
            </RouterLink>
          </li>
        </ul>
      </nav>

      <div class="order-2 ms-auto flex items-center gap-3 lg:order-3 lg:gap-4">
        <LanguageSwitcher />

        <template v-if="auth.isAuthenticated">
          <span class="hidden items-baseline gap-2 text-sm text-ink-soft sm:flex">
            {{ auth.name }}
            <span
              v-if="auth.isAdmin"
              class="u-label rounded-sm bg-brand-soft px-1.5 py-0.5 text-brand"
            >
              {{ t("common.nav.admin") }}
            </span>
          </span>
          <button
            class="u-action u-label text-ink-faint transition-colors hover:text-ink"
            @click="logout"
          >
            {{ t("common.nav.logout") }}
          </button>
        </template>
        <template v-else>
          <RouterLink
            to="/login"
            class="u-action u-label text-ink-faint transition-colors hover:text-ink"
          >
            {{ t("common.nav.login") }}
          </RouterLink>
          <RouterLink
            to="/register"
            class="u-action u-label rounded-sm bg-brand px-3.5 py-2 text-surface transition-colors hover:bg-brand-deep"
          >
            {{ t("common.nav.register") }}
          </RouterLink>
        </template>
      </div>
    </div>
  </header>
</template>
