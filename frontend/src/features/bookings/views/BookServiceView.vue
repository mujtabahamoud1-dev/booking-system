<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { RouterLink } from "vue-router";
import { useI18n } from "vue-i18n";
import { servicesApi } from "@/features/services/api";
import type { Service } from "@/features/services/types";
import { useServiceLabels } from "@/features/services/labels";
import { slotsApi } from "@/features/slots/api";
import { shortTime, type Slot } from "@/features/slots/types";
import { useBookingsStore } from "../store";
import type { Booking } from "../types";
import { apiErrorMessage } from "@/shared/api/client";
import StatusBadge from "../components/StatusBadge.vue";
import WeekTrack from "@/features/slots/components/WeekTrack.vue";
import BaseInput from "@/shared/components/BaseInput.vue";
import BaseButton from "@/shared/components/BaseButton.vue";
import AlertMessage from "@/shared/components/AlertMessage.vue";
import LoadingSpinner from "@/shared/components/LoadingSpinner.vue";

const props = defineProps<{ serviceId: string }>();
const bookings = useBookingsStore();
const { t, locale } = useI18n();
const { serviceName } = useServiceLabels();

const service = ref<Service | null>(null);
const slots = ref<Slot[]>([]);
const loading = ref(true);

// The created booking; shown in place of the form.
const submitted = ref<Booking | null>(null);

const selectedSlotId = ref<number | null>(null);
const bookingDate = ref("");
const notes = ref("");
const error = ref<string | null>(null);
const submitting = ref(false);

const id = Number(props.serviceId);

const selectedSlot = computed(() => slots.value.find((s) => s.id === selectedSlotId.value) ?? null);

// toISOString() reports UTC, which rolls the date over for anyone east or west
// of it. Dates here are calendar dates, so format them locally.
const toISODate = (d: Date): string =>
  `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}`;

const today = toISODate(new Date());

// The API rejects a date whose weekday differs from the slot's; catch it early.
const dayMismatch = computed(() => {
  if (!selectedSlot.value || !bookingDate.value) return false;
  const weekday = new Date(`${bookingDate.value}T00:00:00`).getDay();
  return weekday !== selectedSlot.value.dayOfWeek;
});

const dateError = computed(() =>
  dayMismatch.value && selectedSlot.value
    ? t("bookings.pickDay", { day: t(`common.days.${selectedSlot.value.dayOfWeek}`) })
    : null,
);

// Once a slot is chosen its weekday is fixed, so the next six occurrences of
// that weekday cover almost every booking as one tap — and can't be wrong.
// The date field stays available underneath for anything further out.
const upcomingDates = computed(() => {
  if (!selectedSlot.value) return [];
  const out: string[] = [];
  const cursor = new Date();
  cursor.setHours(0, 0, 0, 0);
  while (out.length < 6) {
    if (cursor.getDay() === selectedSlot.value.dayOfWeek) out.push(toISODate(cursor));
    cursor.setDate(cursor.getDate() + 1);
  }
  return out;
});

const dateFormat = computed(
  () => new Intl.DateTimeFormat(locale.value, { day: "numeric", month: "short" }),
);

const formatDate = (iso: string): string => dateFormat.value.format(new Date(`${iso}T00:00:00`));

const canSubmit = computed(
  () => selectedSlotId.value !== null && bookingDate.value !== "" && !dayMismatch.value,
);

onMounted(async () => {
  try {
    [service.value, slots.value] = await Promise.all([
      servicesApi.getById(id),
      slotsApi.getForService(id),
    ]);
  } catch (e) {
    error.value = apiErrorMessage(e, t("bookings.loadFailed"));
  } finally {
    loading.value = false;
  }
});

function selectSlot(slot: Slot): void {
  selectedSlotId.value = slot.id;
  // A date already picked for a different weekday would now be invalid.
  if (dayMismatch.value) bookingDate.value = "";
}

async function submit(): Promise<void> {
  if (!selectedSlotId.value) {
    error.value = t("bookings.slotRequired");
    return;
  }
  if (dayMismatch.value) {
    error.value = t("bookings.dayMismatch");
    return;
  }
  error.value = null;
  submitting.value = true;
  try {
    submitted.value = await bookings.create({
      serviceId: id,
      slotId: selectedSlotId.value,
      bookingDate: bookingDate.value,
      notes: notes.value || null,
    });
    window.scrollTo({ top: 0 });
  } catch (e) {
    error.value = apiErrorMessage(e, t("bookings.createFailed"));
  } finally {
    submitting.value = false;
  }
}

function reset(): void {
  submitted.value = null;
  selectedSlotId.value = null;
  bookingDate.value = "";
  notes.value = "";
  error.value = null;
}

const submittedWeekday = computed(() =>
  submitted.value
    ? new Intl.DateTimeFormat(locale.value, { weekday: "long" }).format(
        new Date(`${submitted.value.bookingDate}T00:00:00`),
      )
    : "",
);
</script>

<template>
  <section>
    <LoadingSpinner v-if="loading" />

    <template v-else-if="service">
      <RouterLink
        to="/"
        class="u-action u-label mb-4 gap-2 text-ink-faint transition-colors hover:text-ink sm:mb-6"
      >
        <span class="inline-block rtl:rotate-180" aria-hidden="true">←</span>
        {{ t("bookings.backToServices") }}
      </RouterLink>

      <header class="mb-8 border-b border-line pb-6 sm:mb-10 sm:pb-8">
        <h1 class="u-display text-3xl text-balance md:text-4xl">{{ serviceName(service) }}</h1>
        <p class="u-data mt-3 text-sm text-ink-soft">
          {{ t("common.minutesShort", { count: service.duration }) }}
          <span class="mx-2 text-line" aria-hidden="true">/</span>
          <!-- Currency is written prefix-first, so it stays LTR like the clock. -->
          <span dir="ltr" class="inline-block">${{ service.price.toFixed(2) }}</span>
        </p>
      </header>

      <div v-if="submitted">
        <p class="u-label mb-4 text-brand">{{ t("bookings.sentEyebrow") }}</p>
        <h2 class="u-display text-2xl text-balance md:text-3xl">{{ t("bookings.sentTitle") }}</h2>

        <div class="mt-7 border border-line bg-surface p-5 sm:p-6">
          <p class="u-label text-ink-faint">{{ t("bookings.sentWhen") }}</p>
          <p class="mt-2 flex flex-wrap items-baseline gap-x-2 text-lg">
            <span>{{ submittedWeekday }}</span>
            <span class="u-data">{{ formatDate(submitted.bookingDate) }}</span>
            <span class="text-line" aria-hidden="true">/</span>
            <span dir="ltr" class="u-data inline-block">
              {{ shortTime(submitted.slotStartTime) }}–{{ shortTime(submitted.slotEndTime) }}
            </span>
          </p>
          <div class="mt-5 border-t border-line pt-4">
            <StatusBadge :status="submitted.status" />
          </div>
        </div>

        <p class="mt-6 max-w-prose text-sm leading-relaxed text-ink-soft">
          {{ t("bookings.sentBody") }}
        </p>

        <div class="mt-8 flex flex-wrap gap-3">
          <RouterLink
            to="/bookings"
            class="u-action u-label rounded-sm bg-brand px-4 py-2.5 text-surface transition-colors hover:bg-brand-deep"
          >
            {{ t("bookings.viewBookings") }}
          </RouterLink>
          <button
            type="button"
            class="u-action u-label rounded-sm px-4 py-2.5 text-ink ring-1 ring-line ring-inset transition-colors hover:bg-brand-soft"
            @click="reset"
          >
            {{ t("bookings.bookAnother") }}
          </button>
        </div>
      </div>

      <form v-else class="space-y-10" @submit.prevent="submit">
        <AlertMessage v-if="error">{{ error }}</AlertMessage>

        <!-- Step one: the week's cover, drawn against a clock. -->
        <fieldset>
          <legend class="u-label mb-1 text-brand">{{ t("bookings.stepTime") }}</legend>
          <p class="mb-6 text-sm text-ink-soft">{{ t("bookings.stepTimeHelp") }}</p>

          <p
            v-if="slots.length === 0"
            class="border border-dashed border-line px-6 py-12 text-center text-sm text-ink-faint"
          >
            {{ t("bookings.noSlots") }}
          </p>
          <WeekTrack
            v-else
            :slots="slots"
            :selected-id="selectedSlotId"
            :group-label="t('bookings.chooseSlot')"
            @select="selectSlot"
          />
        </fieldset>

        <!-- Step two appears only once a slot fixes the weekday. -->
        <fieldset v-if="selectedSlot">
          <legend class="u-label mb-1 text-brand">{{ t("bookings.stepDate") }}</legend>
          <p class="mb-6 text-sm text-ink-soft">
            {{ t("bookings.stepDateHelp", { day: t(`common.days.${selectedSlot.dayOfWeek}`) }) }}
          </p>

          <div class="flex flex-wrap gap-2" role="radiogroup" :aria-label="t('bookings.stepDate')">
            <button
              v-for="date in upcomingDates"
              :key="date"
              type="button"
              role="radio"
              :aria-checked="bookingDate === date"
              class="u-action u-data rounded-sm px-3.5 py-2 text-sm transition-colors"
              :class="
                bookingDate === date
                  ? 'bg-brand text-surface'
                  : 'bg-surface text-ink ring-1 ring-inset ring-line hover:bg-brand-soft'
              "
              @click="bookingDate = date"
            >
              {{ formatDate(date) }}
            </button>
          </div>

          <details class="mt-5">
            <!-- Padded rather than given `u-action`, which would switch the
                 summary off `display: list-item` and take its marker with it. -->
            <summary class="u-label cursor-pointer py-3 text-ink-faint hover:text-ink">
              {{ t("bookings.otherDate") }}
            </summary>
            <div class="mt-2 max-w-full sm:max-w-56">
              <BaseInput
                v-model="bookingDate"
                :label="t('common.fields.date')"
                type="date"
                :min="today"
                :error="dateError"
              />
            </div>
          </details>
        </fieldset>

        <fieldset v-if="selectedSlot" class="max-w-lg">
          <legend class="u-label mb-1 text-brand">{{ t("bookings.stepNotes") }}</legend>
          <p class="mb-4 text-sm text-ink-soft">{{ t("bookings.stepNotesHelp") }}</p>
          <BaseInput
            v-model="notes"
            :label="t('common.fields.notes')"
            hide-label
            :placeholder="t('bookings.notesPlaceholder')"
          />
        </fieldset>

        <!-- What you are about to book, spelled out before you commit. On a
             phone this is the one control the whole page exists for, and the
             form is long enough to scroll it away, so it docks to the bottom
             edge instead of waiting at the end. -->
        <div
          class="border-t border-line pt-4 pb-[max(1rem,env(safe-area-inset-bottom))] sm:static sm:bg-transparent sm:pt-8 sm:pb-0 sm:backdrop-blur-none"
          :class="selectedSlot ? 'u-bleed sticky bottom-0 bg-ground/95 backdrop-blur-md' : ''"
        >
          <p v-if="canSubmit && selectedSlot" class="mb-3 text-sm text-ink-soft sm:mb-5">
            {{ t("bookings.summaryLead") }}
            <span class="u-data text-ink">
              {{ formatDate(bookingDate) }},
              <span dir="ltr" class="inline-block"
                >{{ shortTime(selectedSlot.startTime) }}–{{ shortTime(selectedSlot.endTime) }}</span
              >
            </span>
          </p>
          <BaseButton
            type="submit"
            :loading="submitting"
            :disabled="!canSubmit"
            class="w-full sm:w-auto"
          >
            {{ t("bookings.confirmBooking") }}
          </BaseButton>
        </div>
      </form>
    </template>

    <AlertMessage v-else>{{ error ?? t("bookings.serviceNotFound") }}</AlertMessage>
  </section>
</template>
