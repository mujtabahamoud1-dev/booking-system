<script setup lang="ts">
import { computed, onMounted, ref, watch } from "vue";
import { storeToRefs } from "pinia";
import { useI18n } from "vue-i18n";
import { useBookingsStore } from "../store";
import { useServicesStore } from "@/features/services/store";
import { serviceName as localizedServiceName } from "@/features/services/labels";
import { shortTime } from "@/features/slots/types";
import type { AdminBooking, BookingStatus } from "../types";
import { apiErrorMessage } from "@/shared/api/client";
import { useConfirm } from "@/shared/composables/useConfirm";
import StatusBadge from "../components/StatusBadge.vue";
import AlertMessage from "@/shared/components/AlertMessage.vue";
import LoadingSpinner from "@/shared/components/LoadingSpinner.vue";
import BaseSelect from "@/shared/components/BaseSelect.vue";
import SearchInput from "@/shared/components/SearchInput.vue";
import FilterChips from "@/shared/components/FilterChips.vue";

const bookings = useBookingsStore();
const servicesStore = useServicesStore();
const { adminItems: items, adminCounts: counts, adminLoaded, loading } = storeToRefs(bookings);
const { t, locale } = useI18n();
// Aliased: `confirm` below is the row action that approves a booking.
const { confirm: askConfirm } = useConfirm();

const error = ref<string | null>(null);
const filter = ref<BookingStatus | "all">("all");
const search = ref("");
const dateRange = ref<"all" | "today" | "week" | "upcoming" | "past">("all");

const serviceNames = computed(() =>
  Object.fromEntries(servicesStore.items.map((s) => [s.id, localizedServiceName(s, locale.value)])),
);

const serviceName = (booking: AdminBooking): string =>
  serviceNames.value[booking.serviceId] ?? t("bookings.unnamedService", { id: booking.serviceId });

const bookingHours = (booking: AdminBooking): string =>
  `${shortTime(booking.slotStartTime)}–${shortTime(booking.slotEndTime)}`;

// The named ranges are turned into "yyyy-MM-dd" bounds here, so the API stays a
// plain date-range filter instead of learning what "this week" means. Read once
// at load: a page left open past midnight is not worth a timer.
const isoDate = (d: Date): string =>
  `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}`;

const today = isoDate(new Date());
const weekEnd = isoDate(new Date(Date.now() + 6 * 24 * 60 * 60 * 1000));

const yesterday = isoDate(new Date(Date.now() - 24 * 60 * 60 * 1000));

const dateBounds = computed<{ from?: string; to?: string }>(() => {
  switch (dateRange.value) {
    case "today":
      return { from: today, to: today };
    case "week":
      return { from: today, to: weekEnd };
    case "upcoming":
      return { from: today };
    case "past":
      return { to: yesterday };
    default:
      return {};
  }
});

// Every filter change is a query. `items` is whatever the server last sent for
// the current one, so it is rendered as-is and never narrowed again here.
watch(
  [search, filter, dateRange],
  () =>
    bookings.loadAll({
      search: search.value,
      status: filter.value,
      ...dateBounds.value,
    }),
  { immediate: true },
);

const statusOptions = computed(() =>
  (["all", "pending", "confirmed", "cancelled"] as const).map((key) => ({
    value: key,
    label: key === "all" ? t("bookings.filterAll") : t(`bookings.status.${key}`),
    count: key === "all" ? counts.value.total : counts.value[key],
  })),
);

const dateOptions = computed(() => [
  { value: "all", label: t("bookings.dateAll") },
  { value: "today", label: t("bookings.dateToday") },
  { value: "week", label: t("bookings.dateWeek") },
  { value: "upcoming", label: t("bookings.dateUpcoming") },
  { value: "past", label: t("bookings.datePast") },
]);

const hasFilters = computed(
  () => search.value.trim() !== "" || filter.value !== "all" || dateRange.value !== "all",
);

const emptyMessage = computed(() =>
  hasFilters.value ? t("bookings.filterEmpty") : t("bookings.allEmpty"),
);

onMounted(() => servicesStore.load(true));

async function confirm(booking: AdminBooking): Promise<void> {
  error.value = null;
  try {
    await bookings.confirm(booking.id);
  } catch (e) {
    error.value = apiErrorMessage(e, t("bookings.confirmFailed"));
  }
}

async function cancel(booking: AdminBooking): Promise<void> {
  const ok = await askConfirm({
    message: t("bookings.confirmCancel"),
    confirmLabel: t("common.actions.cancel"),
  });
  if (!ok) return;

  error.value = null;
  try {
    await bookings.cancel(booking.id, true);
  } catch (e) {
    error.value = apiErrorMessage(e, t("bookings.cancelFailed"));
  }
}
</script>

<template>
  <section>
    <header class="mb-8">
      <h1 class="u-display text-3xl text-balance md:text-4xl">
        {{ t("bookings.allTitle") }}
      </h1>
      <p class="mt-3 text-ink-soft">{{ t("bookings.allSubtitle") }}</p>
    </header>

    <AlertMessage v-if="error" class="mb-6">{{ error }}</AlertMessage>

    <div v-if="adminLoaded" class="mb-6 space-y-4">
      <div class="flex flex-col gap-4 sm:flex-row sm:items-end">
        <div class="sm:max-w-sm sm:flex-1">
          <SearchInput
            v-model="search"
            :label="t('bookings.searchLabel')"
            :placeholder="t('bookings.searchPlaceholder')"
            :debounce="300"
            :result-label="
              t('common.search.results', {
                count: items.length,
                total: counts.total,
              })
            "
          />
        </div>
        <div class="sm:w-44">
          <BaseSelect v-model="dateRange" :label="t('common.filters.date')">
            <option v-for="option in dateOptions" :key="option.value" :value="option.value">
              {{ option.label }}
            </option>
          </BaseSelect>
        </div>
      </div>

      <FilterChips
        v-model="filter"
        :options="statusOptions"
        :group-label="t('common.filters.status')"
      />
    </div>

    <!-- Only the first load takes the page away. Later queries leave the rows in
         place and dim them, so typing does not make the screen flicker. -->
    <LoadingSpinner v-if="!adminLoaded" />

    <template v-else>
      <!-- A five-column table in a sideways-scrolling box is a desktop table
           that has been made to fit, not made to work. On a phone each booking
           becomes a record you can read and act on without scrolling at all. -->
      <ul
        class="border-t border-line transition-opacity md:hidden"
        :class="loading ? 'opacity-60' : ''"
        :aria-busy="loading"
      >
        <li v-for="booking in items" :key="booking.id" class="border-b border-line py-4">
          <div class="flex items-start justify-between gap-3">
            <div class="min-w-0">
              <p class="font-medium">{{ booking.patientName }}</p>
              <p class="mt-0.5 truncate text-sm text-ink-soft">
                {{ serviceName(booking) }}
              </p>
              <p dir="ltr" class="u-data mt-1 text-xs text-ink-faint rtl:text-end">
                {{ booking.bookingDate }}
                <span class="mx-1 text-line" aria-hidden="true">/</span>
                <span class="text-ink-soft">{{ bookingHours(booking) }}</span>
                <span class="mx-1 text-line" aria-hidden="true">/</span>
                <span>{{ booking.patientPhone || booking.patientEmail }}</span>
              </p>
            </div>
            <StatusBadge :status="booking.status" />
          </div>

          <div v-if="booking.status !== 'cancelled'" class="mt-1 flex items-center gap-5">
            <button
              v-if="booking.status === 'pending'"
              class="u-action u-label text-brand"
              @click="confirm(booking)"
            >
              {{ t("common.actions.confirm") }}
            </button>
            <button class="u-action u-label text-alert" @click="cancel(booking)">
              {{ t("common.actions.cancel") }}
            </button>
          </div>
        </li>
        <li
          v-if="items.length === 0"
          class="border-b border-line py-14 text-center text-sm text-ink-faint"
        >
          {{ emptyMessage }}
        </li>
      </ul>

      <div
        class="hidden border border-line bg-surface transition-opacity md:block"
        :class="loading ? 'opacity-60' : ''"
        :aria-busy="loading"
      >
        <table class="min-w-full text-sm">
          <thead>
            <tr class="border-b border-line">
              <th class="u-label px-5 py-4 text-start text-ink-faint">
                {{ t("common.fields.dateTime") }}
              </th>
              <th class="u-label px-5 py-4 text-start text-ink-faint">
                {{ t("common.fields.user") }}
              </th>
              <th class="u-label px-5 py-4 text-start text-ink-faint">
                {{ t("common.fields.service") }}
              </th>
              <th class="u-label px-5 py-4 text-start text-ink-faint">
                {{ t("common.fields.status") }}
              </th>
              <th class="px-5 py-4">
                <span class="sr-only">{{ t("common.actions.confirm") }}</span>
              </th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="booking in items"
              :key="booking.id"
              class="border-b border-line last:border-0"
            >
              <td class="u-data px-5 py-4 whitespace-nowrap">
                <span dir="ltr" class="inline-block">{{ booking.bookingDate }}</span>
                <span dir="ltr" class="mt-0.5 block text-xs text-ink-faint">
                  {{ bookingHours(booking) }}
                </span>
              </td>
              <td class="px-5 py-4">
                <div class="font-medium">{{ booking.patientName }}</div>
                <div dir="ltr" class="u-data mt-0.5 text-xs text-ink-faint rtl:text-end">
                  {{ booking.patientPhone || booking.patientEmail }}
                </div>
              </td>
              <td class="px-5 py-4">{{ serviceName(booking) }}</td>
              <td class="px-5 py-4">
                <StatusBadge :status="booking.status" />
              </td>
              <td class="px-5 py-4 text-end whitespace-nowrap">
                <button
                  v-if="booking.status === 'pending'"
                  class="u-action u-label text-brand transition-opacity hover:opacity-70"
                  @click="confirm(booking)"
                >
                  {{ t("common.actions.confirm") }}
                </button>
                <button
                  v-if="booking.status !== 'cancelled'"
                  class="u-action u-label ms-4 text-alert transition-opacity hover:opacity-70"
                  @click="cancel(booking)"
                >
                  {{ t("common.actions.cancel") }}
                </button>
              </td>
            </tr>
            <tr v-if="items.length === 0">
              <td colspan="5" class="px-5 py-16 text-center text-ink-faint">
                {{ emptyMessage }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </template>
  </section>
</template>
