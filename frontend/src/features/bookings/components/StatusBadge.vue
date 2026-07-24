<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import type { BookingStatus } from '../types'

defineProps<{ status: BookingStatus }>()

const { t } = useI18n()

const styles: Record<BookingStatus, string> = {
  pending: 'bg-amber-50 text-amber-700 ring-amber-200',
  confirmed: 'bg-emerald-50 text-emerald-700 ring-emerald-200',
  cancelled: 'bg-slate-100 text-slate-500 ring-slate-200',
}
</script>

<template>
  <span
    class="inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium capitalize ring-1 ring-inset"
    :class="styles[status]"
  >
    {{ t(`bookings.status.${status}`) }}
  </span>
</template>
