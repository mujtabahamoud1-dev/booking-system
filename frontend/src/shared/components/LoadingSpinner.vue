<script setup lang="ts">
import { useI18n } from 'vue-i18n'

defineProps<{ label?: string }>()

const { t } = useI18n()
</script>

<template>
  <div class="flex items-center justify-center gap-3 py-12 text-slate-500">
    <svg class="h-5 w-5 animate-spin" viewBox="0 0 24 24" fill="none" aria-hidden="true">
      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z" />
    </svg>
    <span class="text-sm">{{ label ?? t('common.loading') }}</span>
  </div>
</template>
