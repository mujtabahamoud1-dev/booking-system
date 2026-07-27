<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import type { BookingStatus } from '../types'

defineProps<{ status: BookingStatus }>()

const { t } = useI18n()

// Amber and green are the yellow and green resistance bands; cancelled drops out
// of the colour system entirely, which is the point.
const styles: Record<BookingStatus, string> = {
  pending: 'bg-signal-soft text-signal-ink',
  confirmed: 'bg-brand-soft text-brand',
  cancelled: 'bg-ground text-ink-faint',
}

const dots: Record<BookingStatus, string> = {
  pending: 'bg-signal',
  confirmed: 'bg-brand',
  cancelled: 'bg-ink-faint',
}
</script>

<template>
  <span class="u-label inline-flex items-center gap-1.5 rounded-sm px-2 py-1" :class="styles[status]">
    <span class="h-1.5 w-1.5 rounded-full" :class="dots[status]" aria-hidden="true" />
    {{ t(`bookings.status.${status}`) }}
  </span>
</template>
