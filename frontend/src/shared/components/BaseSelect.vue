<script setup lang="ts">
defineProps<{ label: string; hideLabel?: boolean }>()

defineOptions({ inheritAttrs: false })

// Options are passed as a slot so callers keep control of their own <option>s.
const model = defineModel<string | number | null>()
</script>

<template>
  <label class="block">
    <span class="u-label mb-1.5 block text-ink-soft" :class="hideLabel ? 'sr-only' : ''">
      {{ label }}
    </span>
    <div class="relative">
      <select
        v-bind="$attrs"
        v-model="model"
        class="block w-full appearance-none rounded-sm border-0 bg-surface py-3 pe-9 ps-3 text-base text-ink ring-1 ring-inset ring-line transition-shadow focus:ring-2 focus:ring-inset focus:ring-brand sm:py-2.5 sm:text-sm"
      >
        <slot />
      </select>
      <svg
        class="pointer-events-none absolute inset-y-0 end-3 my-auto h-3 w-3 text-ink-faint"
        viewBox="0 0 12 12"
        fill="none"
        aria-hidden="true"
      >
        <path d="M2.5 4.5L6 8l3.5-3.5" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" />
      </svg>
    </div>
  </label>
</template>
