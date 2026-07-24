<script setup lang="ts">
withDefaults(defineProps<{ tone?: 'error' | 'success' | 'info' }>(), { tone: 'error' })

const tones: Record<string, string> = {
  error: 'bg-rose-50 text-rose-700 ring-rose-200',
  success: 'bg-emerald-50 text-emerald-700 ring-emerald-200',
  info: 'bg-sky-50 text-sky-700 ring-sky-200',
}
</script>

<template>
  <div class="rounded-md px-4 py-3 text-sm ring-1 ring-inset" :class="tones[tone]" role="alert">
    <slot />
  </div>
</template>
