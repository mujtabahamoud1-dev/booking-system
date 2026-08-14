<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";
import { DAY_INDEXES, shortTime, type Slot } from "../types";

// The week laid out against a real clock axis: each slot is a bar whose
// position and width are its actual start and end. Replaces a list of
// "Monday 09:00 – 17:00" strings, which made you read seven rows to learn
// something the eye can take in at once.
const props = withDefaults(
  defineProps<{
    slots: Slot[];
    selectedId?: number | null;
    // Admin wants every weekday visible so gaps in cover are obvious; a patient
    // choosing a time only cares about the days that actually have slots.
    showEmptyDays?: boolean;
    selectable?: boolean;
    groupLabel?: string;
    days?: readonly number[];
    serviceNames?: Record<number, string>;
  }>(),
  {
    selectedId: null,
    showEmptyDays: false,
    selectable: true,
    groupLabel: undefined,
    days: () => DAY_INDEXES,
    serviceNames: undefined,
  },
);

const emit = defineEmits<{ select: [slot: Slot] }>();

const { t } = useI18n();

const toMinutes = (time: string): number => {
  const [h, m] = time.split(":").map(Number);
  return h * 60 + m;
};

// The axis is derived from the data rather than fixed at 00:00–24:00, so a
// clinic open 09:00–17:00 fills the width instead of hiding in the middle.
// Padded out to whole hours, and never narrower than six so one short slot
// doesn't render as a full-width bar.
const axis = computed(() => {
  if (props.slots.length === 0) return { from: 8 * 60, to: 18 * 60 };

  let from = Math.floor(Math.min(...props.slots.map((s) => toMinutes(s.startTime))) / 60) * 60;
  let to = Math.ceil(Math.max(...props.slots.map((s) => toMinutes(s.endTime))) / 60) * 60;

  const minSpan = 6 * 60;
  if (to - from < minSpan) {
    const grow = minSpan - (to - from);
    from = Math.max(0, from - Math.floor(grow / 2));
    to = Math.min(24 * 60, from + minSpan);
    from = Math.max(0, to - minSpan);
  }
  return { from, to };
});

const span = computed(() => axis.value.to - axis.value.from);

// Hour ticks thin out on a long axis so the labels never collide.
const ticks = computed(() => {
  const step = span.value > 8 * 60 ? 120 : 60;
  const out: { at: number; label: string }[] = [];
  for (let m = axis.value.from; m <= axis.value.to; m += step) {
    out.push({
      at: ((m - axis.value.from) / span.value) * 100,
      label: hourLabel(m),
    });
  }
  return out;
});

const slotsFor = (day: number): Slot[] =>
  props.slots
    .filter((s) => s.dayOfWeek === day)
    .sort((a, b) => toMinutes(a.startTime) - toMinutes(b.startTime));

// Overlapping slots on the same day are stacked into lanes rather than drawn on
// top of each other — an admin double-booking a window should be able to see it.
// Only the wide layout needs this; the narrow one gives every slot its own row.
function lanesFor(day: number): Slot[][] {
  const lanes: Slot[][] = [];
  for (const slot of slotsFor(day)) {
    const lane = lanes.find((l) => toMinutes(l[l.length - 1].endTime) <= toMinutes(slot.startTime));
    if (lane) lane.push(slot);
    else lanes.push([slot]);
  }
  return lanes;
}

const rows = computed(() =>
  DAY_INDEXES.filter((day) => props.days.includes(day))
    .map((day) => ({ day, slots: slotsFor(day), lanes: lanesFor(day) }))
    .filter((row) => props.showEmptyDays || row.slots.length > 0),
);

// Percentage geometry, clamped so a slot reaching past the axis still reads.
function geometry(slot: Slot): { start: number; width: number } {
  const start = Math.max(0, ((toMinutes(slot.startTime) - axis.value.from) / span.value) * 100);
  const end = Math.min(100, ((toMinutes(slot.endTime) - axis.value.from) / span.value) * 100);
  return { start, width: Math.max(end - start, 0.5) };
}

const hourLabel = (minutes: number): string => String(Math.floor(minutes / 60)).padStart(2, "0");

// Undefined outside the all-services view, where every bar belongs to the one
// service already named on the page.
const serviceName = (slot: Slot): string | undefined => props.serviceNames?.[slot.serviceId];

function label(slot: Slot): string {
  const parts = [
    t(`common.days.${slot.dayOfWeek}`),
    `${shortTime(slot.startTime)}–${shortTime(slot.endTime)}`,
    t("slots.capacityOf", { count: slot.maxBookings }),
  ];
  const name = serviceName(slot);
  if (name) parts.unshift(name);
  return parts.join(", ");
}
</script>

<template>
  <div
    :role="selectable ? 'radiogroup' : undefined"
    :aria-label="selectable ? groupLabel : undefined"
  >
    <!-- Narrow screens: the clock axis cannot be both readable and tappable in
         ~260px, so it stops being the control. Each slot gets a full-width row
         with its hours in plain text, and the bar moves underneath as a rule
         drawn against the same clinic-day window — so two slots are still
         comparable at a glance, which is the whole point of the track. -->
    <div class="sm:hidden">
      <p
        dir="ltr"
        class="u-data mb-3 flex justify-between text-[0.625rem] text-ink-faint"
        aria-hidden="true"
      >
        <span>{{ hourLabel(axis.from) }}</span>
        <span>{{ hourLabel(axis.to) }}</span>
      </p>

      <div v-for="row in rows" :key="row.day" class="border-t border-line/60 py-3">
        <p class="u-label mb-2 text-ink-soft">
          {{ t(`common.days.${row.day}`) }}
        </p>

        <p v-if="row.slots.length === 0" class="text-xs text-ink-faint">
          {{ t("slots.dayClosed") }}
        </p>

        <div v-else class="space-y-2">
          <component
            :is="selectable ? 'button' : 'div'"
            v-for="(slot, i) in row.slots"
            :key="slot.id"
            :type="selectable ? 'button' : undefined"
            :role="selectable ? 'radio' : undefined"
            :aria-checked="selectable ? selectedId === slot.id : undefined"
            :aria-label="label(slot)"
            class="block w-full rounded-sm px-3 py-2.5 text-start transition-colors"
            :class="selectedId === slot.id ? 'bg-brand text-surface' : 'bg-brand-soft text-brand'"
            @click="selectable && emit('select', slot)"
          >
            <span class="flex items-baseline justify-between gap-3">
              <span dir="ltr" class="u-data text-sm font-medium">
                {{ shortTime(slot.startTime) }}–{{ shortTime(slot.endTime) }}
              </span>
              <span class="u-label opacity-75">
                {{ t("slots.capacityOf", { count: slot.maxBookings }) }}
              </span>
            </span>

            <span v-if="serviceName(slot)" class="mt-1 block truncate text-xs opacity-80">
              {{ serviceName(slot) }}
            </span>

            <span
              dir="ltr"
              class="mt-2 block h-1 w-full rounded-full"
              :class="selectedId === slot.id ? 'bg-surface/25' : 'bg-brand/15'"
              aria-hidden="true"
            >
              <span
                class="u-extend block h-full rounded-full"
                :class="selectedId === slot.id ? 'bg-surface' : 'bg-brand'"
                :style="{
                  marginInlineStart: `${geometry(slot).start}%`,
                  width: `${geometry(slot).width}%`,
                  '--i': i + row.day,
                }"
              />
            </span>
          </component>
        </div>
      </div>
    </div>

    <!-- Wide screens: the full clock axis. It reads left-to-right in both
         locales, matching how the codebase already renders time ranges. Day
         names stay with the page. -->
    <div class="hidden sm:grid sm:grid-cols-[7rem_1fr] sm:gap-x-4">
      <div></div>
      <div dir="ltr" class="relative mb-2 h-4">
        <span
          v-for="tick in ticks"
          :key="tick.at"
          class="u-data absolute top-0 -translate-x-1/2 text-[0.625rem] text-ink-faint"
          :style="{ left: `${tick.at}%` }"
          aria-hidden="true"
          >{{ tick.label }}</span
        >
      </div>

      <template v-for="row in rows" :key="row.day">
        <div class="flex items-center border-b border-line/60 py-2">
          <span class="u-label text-ink-soft">{{ t(`common.days.${row.day}`) }}</span>
        </div>

        <!-- Clipped: on a narrow screen a short slot near the end of the axis
             hits the bar's min-width and would otherwise widen the page. -->
        <div dir="ltr" class="relative overflow-hidden border-s border-b border-line/60 py-2">
          <!-- Hour gridlines sit behind the bars. -->
          <span
            v-for="tick in ticks"
            :key="`g-${tick.at}`"
            class="pointer-events-none absolute inset-y-0 w-px bg-line/60"
            :style="{ left: `${tick.at}%` }"
            aria-hidden="true"
          />

          <p
            v-if="row.lanes.length === 0"
            dir="auto"
            class="relative flex h-8 items-center px-2 text-xs text-ink-faint"
          >
            {{ t("slots.dayClosed") }}
          </p>

          <div
            v-for="(lane, laneIndex) in row.lanes"
            :key="laneIndex"
            class="relative h-8"
            :class="laneIndex > 0 ? 'mt-1' : ''"
          >
            <component
              :is="selectable ? 'button' : 'div'"
              v-for="(slot, i) in lane"
              :key="slot.id"
              :type="selectable ? 'button' : undefined"
              :role="selectable ? 'radio' : undefined"
              :aria-checked="selectable ? selectedId === slot.id : undefined"
              :aria-label="label(slot)"
              :title="label(slot)"
              class="u-extend absolute inset-y-0 flex min-w-11 items-center justify-between gap-2 overflow-hidden rounded-sm px-2 text-start transition-colors"
              :class="[
                selectable ? 'cursor-pointer' : '',
                selectedId === slot.id
                  ? 'bg-brand text-surface'
                  : 'bg-brand-soft text-brand hover:bg-brand/20',
              ]"
              :style="{
                left: `${geometry(slot).start}%`,
                width: `${geometry(slot).width}%`,
                '--i': i + row.day,
              }"
              @click="selectable && emit('select', slot)"
            >
              <span class="u-data truncate text-xs font-medium">
                {{ shortTime(slot.startTime) }}–{{ shortTime(slot.endTime) }}
              </span>
              <!-- Across services the bar's own service is what disambiguates
                   it; capacity stays reachable in the tooltip and the label. -->
              <span v-if="serviceName(slot)" class="truncate text-[0.625rem] opacity-80">
                {{ serviceName(slot) }}
              </span>
              <span
                v-else
                class="u-data shrink-0 text-[0.625rem] opacity-70"
                :title="t('slots.capacityOf', { count: slot.maxBookings })"
                >×{{ slot.maxBookings }}</span
              >
            </component>
          </div>
        </div>
      </template>
    </div>
  </div>
</template>
