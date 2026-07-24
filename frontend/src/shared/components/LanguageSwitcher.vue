<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { setLocale, type Locale } from '@/shared/i18n'

const { locale, t } = useI18n()

// Two locales, so the control is a toggle to "the other one".
const next = computed<Locale>(() => (locale.value === 'ar' ? 'en' : 'ar'))
const label = computed(() => (next.value === 'ar' ? t('common.arabic') : t('common.english')))
</script>

<template>
  <button
    type="button"
    class="rounded-md px-2 py-1 text-sm font-medium text-slate-600 hover:bg-slate-100 hover:text-slate-900"
    :title="t('common.language')"
    :aria-label="t('common.language')"
    @click="setLocale(next)"
  >
    <span aria-hidden="true">🌐</span> {{ label }}
  </button>
</template>
