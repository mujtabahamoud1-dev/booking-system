<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  label: string
  // Kept for assistive tech when a surrounding legend already names the field,
  // so the label is not read out twice.
  hideLabel?: boolean
  type?: string
  placeholder?: string
  required?: boolean
  error?: string | null
  min?: string | number
  step?: string | number
}>()

// Attributes like `autocomplete` were landing on the wrapping <label> instead of
// the control; forward them to the input explicitly.
defineOptions({ inheritAttrs: false })

// Two-way binding via defineModel; accepts string or number for number inputs.
const model = defineModel<string | number | null>()

// Measured values are set in the mono face wherever they appear, including
// while you are typing them.
const isMeasured = computed(() =>
  ['date', 'time', 'number', 'datetime-local'].includes(props.type ?? 'text'),
)
</script>

<template>
  <label class="block">
    <span class="u-label mb-1.5 block text-ink-soft" :class="hideLabel ? 'sr-only' : ''">
      {{ label }}
      <span v-if="required" class="text-alert" aria-hidden="true">*</span>
    </span>
    <input
      v-bind="$attrs"
      v-model="model"
      :type="type ?? 'text'"
      :placeholder="placeholder"
      :required="required"
      :min="min"
      :step="step"
      :aria-invalid="error ? true : undefined"
      class="block w-full rounded-sm border-0 bg-surface px-3 py-3 text-base text-ink ring-1 ring-inset transition-shadow placeholder:text-ink-faint focus:ring-2 focus:ring-inset focus:ring-brand sm:py-2.5 sm:text-sm"
      :class="[error ? 'ring-alert' : 'ring-line', isMeasured ? 'u-data' : '']"
    />
    <span v-if="error" class="mt-1.5 block text-xs text-alert">{{ error }}</span>
  </label>
</template>
