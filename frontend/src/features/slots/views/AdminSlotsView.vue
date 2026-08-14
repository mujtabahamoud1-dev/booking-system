<script setup lang="ts">
import { computed, onMounted, ref, watch } from "vue";
import { storeToRefs } from "pinia";
import { useI18n } from "vue-i18n";
import { useServicesStore } from "@/features/services/store";
import { useSlotsStore } from "../store";
import { DAY_INDEXES, shortTime, type Slot, type UpdateSlotRequest } from "../types";
import { apiErrorMessage } from "@/shared/api/client";
import { useConfirm } from "@/shared/composables/useConfirm";
import SlotForm from "../components/SlotForm.vue";
import WeekTrack from "../components/WeekTrack.vue";
import BaseButton from "@/shared/components/BaseButton.vue";
import BaseModal from "@/shared/components/BaseModal.vue";
import BaseSelect from "@/shared/components/BaseSelect.vue";
import AlertMessage from "@/shared/components/AlertMessage.vue";
import LoadingSpinner from "@/shared/components/LoadingSpinner.vue";
import FilterChips from "@/shared/components/FilterChips.vue";

const servicesStore = useServicesStore();
const slotsStore = useSlotsStore();
const { items: services } = storeToRefs(servicesStore);
const { adminItems: slots, adminCounts: counts, adminLoaded, loading } = storeToRefs(slotsStore);
const { t } = useI18n();
const { confirm } = useConfirm();

// "all" is a review mode: one service at a time answers "when does this run",
// but only every service at once answers "is Thursday covered by anyone".
const selectedServiceId = ref<number | "all" | null>(null);
const dayFilter = ref<number | "all">("all");
const selectedSlotId = ref<number | null>(null);
const showForm = ref(false);
const editing = ref<Slot | null>(null);
const submitting = ref(false);
const error = ref<string | null>(null);

const showingAll = computed(() => selectedServiceId.value === "all");

// Every change of service or day is a query; `slots` is whatever the server last
// sent for the current one and is rendered as-is.
watch([selectedServiceId, dayFilter], () => {
  if (selectedServiceId.value === null) return;
  slotsStore.search({
    serviceId: showingAll.value ? undefined : (selectedServiceId.value as number),
    dayOfWeek: dayFilter.value === "all" ? undefined : (dayFilter.value as number),
  });
});

const serviceNames = computed(() => Object.fromEntries(services.value.map((s) => [s.id, s.name])));

// Counted in the database for the chosen service, ignoring the day filter — so
// the chips keep saying what the other days hold while one is selected.
const dayOptions = computed(() => [
  {
    value: "all" as const,
    label: t("slots.everyDay"),
    count: counts.value.total,
  },
  ...DAY_INDEXES.map((day) => ({
    value: day,
    label: t(`common.days.${day}`),
    count: counts.value.byDay[day] ?? 0,
  })),
]);

// Widened to number[]: DAY_INDEXES is a const tuple, so its own `includes`
// only accepts the literal weekday union, not a slot's plain number.
const visibleDays = computed<number[]>(() =>
  dayFilter.value === "all" ? [...DAY_INDEXES] : [dayFilter.value as number],
);

// The server already narrowed the list, so a selection that no longer appears in
// it simply stops resolving — which is what clears the action bar when a filter
// moves the selected slot out of view.
const selectedSlot = computed(() => slots.value.find((s) => s.id === selectedSlotId.value) ?? null);

const weeklyHours = computed(() => {
  const minutes = slots.value.reduce((sum, slot) => {
    const [sh, sm] = slot.startTime.split(":").map(Number);
    const [eh, em] = slot.endTime.split(":").map(Number);
    return sum + (eh * 60 + em - (sh * 60 + sm));
  }, 0);
  return Math.round((minutes / 60) * 10) / 10;
});

onMounted(async () => {
  await servicesStore.load(true);
  if (services.value.length > 0) selectedServiceId.value = services.value[0].id;
});

// A slot picked under one service has no meaning under another.
watch(selectedServiceId, () => {
  selectedSlotId.value = null;
});

function openCreate(): void {
  editing.value = null;
  error.value = null;
  showForm.value = true;
}

function openEdit(slot: Slot): void {
  editing.value = slot;
  error.value = null;
  showForm.value = true;
}

async function save(payload: UpdateSlotRequest): Promise<void> {
  // An edit belongs to the slot's own service, not the picker — in the
  // all-services view those are not the same thing. Creating still needs a
  // single service chosen, which is why the button is disabled without one.
  const serviceId = editing.value
    ? editing.value.serviceId
    : typeof selectedServiceId.value === "number"
      ? selectedServiceId.value
      : null;
  if (serviceId === null) return;

  submitting.value = true;
  error.value = null;
  try {
    if (editing.value) {
      await slotsStore.update(editing.value.id, serviceId, payload);
    } else {
      await slotsStore.create({ serviceId, ...payload });
    }
    showForm.value = false;
  } catch (e) {
    error.value = apiErrorMessage(e, t("slots.saveFailed"));
  } finally {
    submitting.value = false;
  }
}

async function remove(slot: Slot): Promise<void> {
  const ok = await confirm({
    message: t("slots.confirmDelete"),
    confirmLabel: t("common.actions.delete"),
  });
  if (!ok) return;

  error.value = null;
  try {
    await slotsStore.remove(slot.id, slot.serviceId);
    selectedSlotId.value = null;
  } catch (e) {
    // 409 when the slot already has bookings.
    error.value = apiErrorMessage(e, t("slots.deleteFailed"));
  }
}
</script>

<template>
  <section>
    <header class="mb-8">
      <h1 class="u-display text-3xl text-balance md:text-4xl">
        {{ t("slots.title") }}
      </h1>
      <p class="mt-3 text-ink-soft">{{ t("slots.subtitle") }}</p>
    </header>

    <div class="mb-6 flex flex-wrap items-end justify-between gap-4">
      <div class="w-full sm:max-w-xs">
        <BaseSelect v-model="selectedServiceId" :label="t('common.fields.service')">
          <option v-if="services.length === 0" :value="null">
            {{ t("slots.noServices") }}
          </option>
          <option v-if="services.length > 1" value="all">
            {{ t("slots.allServices") }}
          </option>
          <option v-for="service in services" :key="service.id" :value="service.id">
            {{ service.name }}
          </option>
        </BaseSelect>
      </div>
      <div class="w-full sm:w-auto">
        <BaseButton
          :disabled="selectedServiceId === null || showingAll"
          class="w-full sm:w-auto"
          @click="openCreate"
        >
          {{ t("slots.newSlot") }}
        </BaseButton>
      </div>
    </div>

    <!-- The disabled button above is not self-explaining, so it says why. -->
    <p v-if="showingAll" class="mb-6 text-sm text-ink-faint">
      {{ t("slots.createNeedsService") }}
    </p>

    <!-- The chips survive an empty day on purpose: filtering to a day with no
         cover must not take away the control you need to leave it. -->
    <div v-if="counts.total > 0" class="mb-8">
      <FilterChips
        v-model="dayFilter"
        :options="dayOptions"
        :group-label="t('common.filters.day')"
        size="compact"
      />
    </div>

    <AlertMessage v-if="error" class="mb-6">{{ error }}</AlertMessage>

    <!-- Only the first load takes the page away. Later queries leave the week in
         place and dim it, so changing a filter does not make the screen flicker. -->
    <LoadingSpinner v-if="!adminLoaded" />

    <template v-else>
      <!-- Every weekday is shown, empty ones included: a gap in cover is the
           thing you most need to notice here. -->
      <div
        class="border border-line bg-surface p-4 transition-opacity sm:p-7"
        :class="loading ? 'opacity-60' : ''"
        :aria-busy="loading"
      >
        <div class="mb-6 flex items-baseline justify-between gap-4">
          <h2 class="u-label text-ink-faint">
            {{ showingAll ? t("slots.allServicesCover") : t("slots.weekCover") }}
          </h2>
          <p v-if="slots.length > 0" class="u-data text-xs text-ink-faint">
            {{ t("slots.weeklyHours", { hours: weeklyHours }) }}
          </p>
        </div>

        <!-- Only a service with no cover at all gets the empty notice. Filtering
             to a quiet day still draws the day, labelled "Closed" — the absence
             is the answer you came for, not a missing panel. -->
        <p
          v-if="counts.total === 0"
          class="border border-dashed border-line px-6 py-12 text-center text-sm text-ink-faint"
        >
          {{ t("slots.empty") }}
        </p>
        <WeekTrack
          v-else
          :slots="slots"
          :selected-id="selectedSlotId"
          :group-label="showingAll ? t('slots.allServicesCover') : t('slots.weekCover')"
          :days="visibleDays"
          :service-names="showingAll ? serviceNames : undefined"
          show-empty-days
          @select="selectedSlotId = $event.id"
        />
      </div>

      <!-- Actions attach to the selected bar rather than repeating the whole
           week as a table underneath it. -->
      <div
        v-if="selectedSlot"
        class="mt-4 flex flex-wrap items-center justify-between gap-4 border border-line bg-brand-soft px-4 py-4 sm:px-5"
      >
        <p class="text-sm">
          <span v-if="showingAll" class="me-3 font-medium">
            {{ serviceNames[selectedSlot.serviceId] }}
            <span class="mx-1 text-line" aria-hidden="true">/</span>
          </span>
          <span class="font-medium">{{ t(`common.days.${selectedSlot.dayOfWeek}`) }}</span>
          <span class="u-data ms-3 text-ink-soft">
            <span dir="ltr" class="inline-block"
              >{{ shortTime(selectedSlot.startTime) }}–{{ shortTime(selectedSlot.endTime) }}</span
            >
            <span class="mx-2 text-line" aria-hidden="true">/</span>
            {{ t("slots.capacityOf", { count: selectedSlot.maxBookings }) }}
          </span>
        </p>
        <div class="flex w-full items-center gap-2 sm:w-auto">
          <BaseButton
            variant="secondary"
            class="flex-1 sm:flex-none"
            @click="openEdit(selectedSlot)"
          >
            {{ t("common.actions.edit") }}
          </BaseButton>
          <BaseButton variant="danger" class="flex-1 sm:flex-none" @click="remove(selectedSlot)">
            {{ t("common.actions.delete") }}
          </BaseButton>
        </div>
      </div>
      <p v-else-if="slots.length > 0" class="mt-4 text-sm text-ink-faint">
        {{ t("slots.selectHint") }}
      </p>
    </template>

    <BaseModal
      v-if="showForm"
      :title="editing ? t('slots.editSlot') : t('slots.newSlot')"
      @close="showForm = false"
    >
      <SlotForm :slot="editing" :submitting="submitting" @save="save" @cancel="showForm = false" />
    </BaseModal>
  </section>
</template>
