<script setup lang="ts">
defineProps<{
  label: string
  type?: string
  placeholder?: string
  required?: boolean
  error?: string | null
  min?: string | number
  step?: string | number
}>()

// Two-way binding via defineModel; accepts string or number for number inputs.
const model = defineModel<string | number | null>()
</script>

<template>
  <label class="block">
    <span class="mb-1 block text-sm font-medium text-slate-700">
      {{ label }}
      <span v-if="required" class="text-rose-500">*</span>
    </span>
    <input
      v-model="model"
      :type="type ?? 'text'"
      :placeholder="placeholder"
      :required="required"
      :min="min"
      :step="step"
      class="block w-full rounded-md border-0 px-3 py-2 text-slate-900 ring-1 ring-inset ring-slate-300 placeholder:text-slate-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm"
      :class="error ? 'ring-rose-400 focus:ring-rose-500' : ''"
    />
    <span v-if="error" class="mt-1 block text-sm text-rose-600">{{ error }}</span>
  </label>
</template>
