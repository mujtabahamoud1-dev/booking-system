<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { RouterLink } from "vue-router";
import { storeToRefs } from "pinia";
import { useI18n } from "vue-i18n";
import { useBookingsStore } from "../store";
import { useServicesStore } from "@/features/services/store";
import { serviceName } from "@/features/services/labels";
import { shortTime } from "@/features/slots/types";
import type { Booking } from "../types";
import { apiErrorMessage } from "@/shared/api/client";
import { useConfirm } from "@/shared/composables/useConfirm";
import StatusBadge from "../components/StatusBadge.vue";
import AlertMessage from "@/shared/components/AlertMessage.vue";
import LoadingSpinner from "@/shared/components/LoadingSpinner.vue";

const bookings = useBookingsStore();
const servicesStore = useServicesStore();
const { items, loading } = storeToRefs(bookings);
const { t, locale } = useI18n();
const { confirm } = useConfirm();

const error = ref<string | null>(null);

// serviceId -> name in the reader's language.
const serviceNames = computed(() =>
  Object.fromEntries(servicesStore.items.map((s) => [s.id, serviceName(s, locale.value)])),
);

// Soonest first — the next appointment is the one you came here to check.
const ordered = computed(() =>
  [...items.value].sort((a, b) => a.bookingDate.localeCompare(b.bookingDate)),
);

const dayFormat = computed(() => new Intl.DateTimeFormat(locale.value, { day: "numeric" }));
const monthFormat = computed(() => new Intl.DateTimeFormat(locale.value, { month: "short" }));
const weekdayFormat = computed(() => new Intl.DateTimeFormat(locale.value, { weekday: "long" }));

const asDate = (iso: string): Date => new Date(`${iso}T00:00:00`);

onMounted(() => {
  bookings.loadMine();
  servicesStore.load();
});

async function cancel(booking: Booking): Promise<void> {
  const ok = await confirm({
    message: t("bookings.confirmCancel"),
    confirmLabel: t("common.actions.cancel"),
  });
  if (!ok) return;

  error.value = null;
  try {
    await bookings.cancel(booking.id);
  } catch (e) {
    error.value = apiErrorMessage(e, t("bookings.cancelFailed"));
  }
}
</script>

<template>
  <section>
    <header class="mb-8 sm:mb-10">
      <h1 class="u-display text-3xl text-balance md:text-4xl">{{ t("bookings.mineTitle") }}</h1>
      <p class="mt-3 text-ink-soft">{{ t("bookings.mineSubtitle") }}</p>
    </header>

    <AlertMessage v-if="error" class="mb-6">{{ error }}</AlertMessage>

    <LoadingSpinner v-if="loading" />

    <div
      v-else-if="items.length === 0"
      class="border border-dashed border-line px-6 py-16 text-center"
    >
      <p class="text-sm text-ink-faint">{{ t("bookings.mineEmpty") }}</p>
      <RouterLink
        to="/"
        class="u-label mt-5 inline-block rounded-sm bg-brand px-4 py-2.5 text-surface transition-colors hover:bg-brand-deep"
      >
        {{ t("bookings.browseServices") }}
      </RouterLink>
    </div>

    <!-- A record list rather than a table: the date leads, because that is what
         you are scanning for, and it survives a narrow screen. -->
    <ul v-else class="border-t border-line">
      <li
        v-for="(booking, i) in ordered"
        :key="booking.id"
        class="u-rise flex flex-wrap items-center gap-x-6 gap-y-2 border-b border-line py-4 sm:py-5"
        :class="booking.status === 'cancelled' ? 'opacity-55' : ''"
        :style="{ '--i': i }"
      >
        <div class="w-14 shrink-0 text-center">
          <div class="u-data text-2xl leading-none font-medium">
            {{ dayFormat.format(asDate(booking.bookingDate)) }}
          </div>
          <div class="u-label mt-1 text-ink-faint">
            {{ monthFormat.format(asDate(booking.bookingDate)) }}
          </div>
        </div>

        <div class="min-w-32 flex-1">
          <p class="font-medium">
            {{
              serviceNames[booking.serviceId] ??
              t("bookings.unnamedService", { id: booking.serviceId })
            }}
          </p>
          <p class="mt-0.5 flex flex-wrap items-baseline gap-x-2 text-sm text-ink-faint">
            <span>{{ weekdayFormat.format(asDate(booking.bookingDate)) }}</span>
            <span class="text-line" aria-hidden="true">/</span>
            <span dir="ltr" class="u-data text-ink-soft">
              {{ shortTime(booking.slotStartTime) }}–{{ shortTime(booking.slotEndTime) }}
            </span>
          </p>
          <p v-if="booking.notes" class="mt-1 text-sm text-ink-faint">{{ booking.notes }}</p>
        </div>

        <!-- On a phone the status and the action drop to their own line, indented
             to sit under the appointment they belong to rather than under the
             date column. -->
        <div
          class="flex w-full items-center justify-between gap-4 ps-20 sm:w-auto sm:justify-start sm:ps-0"
        >
          <StatusBadge :status="booking.status" />

          <button
            v-if="booking.status !== 'cancelled'"
            class="u-action u-label text-alert transition-opacity hover:opacity-70"
            @click="cancel(booking)"
          >
            {{ t("common.actions.cancel") }}
          </button>
        </div>
      </li>
    </ul>
  </section>
</template>
