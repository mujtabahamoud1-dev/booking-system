<script setup lang="ts">
// The chip row the bookings page already used, lifted out so slots and services
// get the same control rather than three near-identical button rows.
//
// Counts stay on the chips deliberately: an admin should be able to see there
// are four pending bookings without first filtering to pending.
withDefaults(
  defineProps<{
    // `value` is compared by identity, so callers can use strings or numbers.
    options: { value: string | number; label: string; count?: number }[];
    groupLabel: string;
    size?: "default" | "compact";
  }>(),
  { size: "default" },
);

const model = defineModel<string | number>({ required: true });
</script>

<template>
  <div class="flex flex-wrap gap-2" role="group" :aria-label="groupLabel">
    <button
      v-for="option in options"
      :key="option.value"
      type="button"
      class="u-action u-label gap-2 rounded-sm transition-colors"
      :class="[
        size === 'compact' ? 'px-2.5 py-1.5' : 'px-3 py-2',
        model === option.value
          ? 'bg-brand text-surface'
          : 'bg-surface text-ink-soft ring-1 ring-inset ring-line hover:text-ink',
      ]"
      :aria-pressed="model === option.value"
      @click="model = option.value"
    >
      {{ option.label }}
      <span v-if="option.count !== undefined" class="u-data opacity-65">{{ option.count }}</span>
    </button>
  </div>
</template>
