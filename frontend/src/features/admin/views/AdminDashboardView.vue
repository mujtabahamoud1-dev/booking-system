<script setup lang="ts">
import { computed, onMounted } from "vue";
import { RouterLink } from "vue-router";
import { storeToRefs } from "pinia";
import { useI18n } from "vue-i18n";
import { useServicesStore } from "@/features/services/store";
import { useBookingsStore } from "@/features/bookings/store";
import { useAuthStore } from "@/features/auth/store";
import LoadingSpinner from "@/shared/components/LoadingSpinner.vue";

const servicesStore = useServicesStore();
const bookingsStore = useBookingsStore();
const auth = useAuthStore();
const { items: services } = storeToRefs(servicesStore);
const { items: bookings, loading } = storeToRefs(bookingsStore);
const { t } = useI18n();

onMounted(() => {
  servicesStore.load(true);
  bookingsStore.loadAll();
});

const stats = computed(() => ({
  services: services.value.length,
  active: services.value.filter((s) => s.isActive).length,
  total: bookings.value.length,
  pending: bookings.value.filter((b) => b.status === "pending").length,
  confirmed: bookings.value.filter((b) => b.status === "confirmed").length,
  cancelled: bookings.value.filter((b) => b.status === "cancelled").length,
}));

const cards = computed(() => [
  {
    key: "services",
    label: t("admin.cards.services"),
    value: stats.value.services,
    sub: t("admin.cards.servicesSub", { count: stats.value.active }),
  },
  {
    key: "bookings",
    label: t("admin.cards.bookings"),
    value: stats.value.total,
    sub: t("admin.cards.bookingsSub"),
  },
  {
    key: "pending",
    label: t("admin.cards.pending"),
    value: stats.value.pending,
    sub: t("admin.cards.pendingSub"),
  },
  {
    key: "confirmed",
    label: t("admin.cards.confirmed"),
    value: stats.value.confirmed,
    sub: t("admin.cards.confirmedSub"),
  },
]);

const shortcuts = computed(() => [
  { to: "/admin/services", label: t("admin.shortcuts.services") },
  { to: "/admin/slots", label: t("admin.shortcuts.slots") },
  { to: "/admin/bookings", label: t("admin.shortcuts.bookings") },
]);
</script>

<template>
  <section>
    <h1 class="text-2xl font-bold text-slate-900">
      {{ t("admin.welcome", { name: auth.name }) }}
    </h1>
    <p class="mt-1 text-slate-500">{{ t("admin.overview") }}</p>

    <LoadingSpinner v-if="loading" />

    <template v-else>
      <div class="mt-6 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <div
          v-for="card in cards"
          :key="card.key"
          class="rounded-lg border border-slate-200 bg-white p-5 shadow-sm"
        >
          <div class="text-sm text-slate-500">{{ card.label }}</div>
          <div class="mt-1 text-3xl font-bold text-slate-900">
            {{ card.value }}
          </div>
          <div class="mt-1 text-xs text-slate-400">{{ card.sub }}</div>
        </div>
      </div>

      <div class="mt-8 flex flex-wrap gap-3">
        <RouterLink
          v-for="shortcut in shortcuts"
          :key="shortcut.to"
          :to="shortcut.to"
          class="rounded-md bg-white px-4 py-2 text-sm font-medium text-slate-700 shadow-sm ring-1 ring-slate-200 hover:bg-slate-50"
        >
          {{ shortcut.label }}
          <!-- The arrow points "forward", so it mirrors with the writing direction. -->
          <span class="inline-block rtl:rotate-180" aria-hidden="true">→</span>
        </RouterLink>
      </div>
    </template>
  </section>
</template>
