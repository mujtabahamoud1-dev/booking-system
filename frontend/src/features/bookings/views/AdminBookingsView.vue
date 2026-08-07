<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { storeToRefs } from 'pinia'
import { useI18n } from 'vue-i18n'
import { useBookingsStore } from '../store'
import { useServicesStore } from '@/features/services/store'
import type { Booking, BookingStatus } from '../types'
import { apiErrorMessage } from '@/shared/api/client'
import { useConfirm } from '@/shared/composables/useConfirm'
import StatusBadge from '../components/StatusBadge.vue'
import AlertMessage from '@/shared/components/AlertMessage.vue'
import LoadingSpinner from '@/shared/components/LoadingSpinner.vue'

const bookings = useBookingsStore()
const servicesStore = useServicesStore()
const { items, loading } = storeToRefs(bookings)
const { t } = useI18n()
// Aliased: `confirm` below is the row action that approves a booking.
const { confirm: askConfirm } = useConfirm()

const error = ref<string | null>(null)
const filter = ref<BookingStatus | 'all'>('all')

const serviceNames = computed(() =>
  Object.fromEntries(servicesStore.items.map((s) => [s.id, s.name])),
)

const filters: (BookingStatus | 'all')[] = ['all', 'pending', 'confirmed', 'cancelled']

const counts = computed(() => ({
  all: items.value.length,
  pending: items.value.filter((b) => b.status === 'pending').length,
  confirmed: items.value.filter((b) => b.status === 'confirmed').length,
  cancelled: items.value.filter((b) => b.status === 'cancelled').length,
}))

// Filtering runs over what is already loaded — no extra request for a view the
// page can compute itself.
const visible = computed(() => {
  const rows = filter.value === 'all' ? items.value : items.value.filter((b) => b.status === filter.value)
  return [...rows].sort((a, b) => a.bookingDate.localeCompare(b.bookingDate))
})

const filterLabel = (key: BookingStatus | 'all'): string =>
  key === 'all' ? t('bookings.filterAll') : t(`bookings.status.${key}`)

onMounted(() => {
  bookings.loadAll()
  servicesStore.load(true)
})

async function confirm(booking: Booking): Promise<void> {
  error.value = null
  try {
    await bookings.confirm(booking.id)
  } catch (e) {
    error.value = apiErrorMessage(e, t('bookings.confirmFailed'))
  }
}

async function cancel(booking: Booking): Promise<void> {
  const ok = await askConfirm({
    message: t('bookings.confirmCancel'),
    confirmLabel: t('common.actions.cancel'),
  })
  if (!ok) return

  error.value = null
  try {
    await bookings.cancel(booking.id)
  } catch (e) {
    error.value = apiErrorMessage(e, t('bookings.cancelFailed'))
  }
}
</script>

<template>
  <section>
    <header class="mb-8">
      <h1 class="u-display text-3xl text-balance md:text-4xl">{{ t('bookings.allTitle') }}</h1>
      <p class="mt-3 text-ink-soft">{{ t('bookings.allSubtitle') }}</p>
    </header>

    <AlertMessage v-if="error" class="mb-6">{{ error }}</AlertMessage>

    <LoadingSpinner v-if="loading" />

    <template v-else>
      <div class="mb-6 flex flex-wrap gap-2" role="group" :aria-label="t('common.fields.status')">
        <button
          v-for="key in filters"
          :key="key"
          type="button"
          class="u-action u-label gap-2 rounded-sm px-3 py-2 transition-colors"
          :class="
            filter === key
              ? 'bg-brand text-surface'
              : 'bg-surface text-ink-soft ring-1 ring-inset ring-line hover:text-ink'
          "
          :aria-pressed="filter === key"
          @click="filter = key"
        >
          {{ filterLabel(key) }}
          <span class="u-data opacity-65">{{ counts[key] }}</span>
        </button>
      </div>

      <!-- A five-column table in a sideways-scrolling box is a desktop table
           that has been made to fit, not made to work. On a phone each booking
           becomes a record you can read and act on without scrolling at all. -->
      <ul class="border-t border-line md:hidden">
        <li v-for="booking in visible" :key="booking.id" class="border-b border-line py-4">
          <div class="flex items-start justify-between gap-3">
            <div class="min-w-0">
              <p class="font-medium">
                {{
                  serviceNames[booking.serviceId] ??
                  t('bookings.unnamedService', { id: booking.serviceId })
                }}
              </p>
              <p dir="ltr" class="u-data mt-1 text-xs text-ink-faint rtl:text-end">
                {{ booking.bookingDate }}
                <span class="mx-1 text-line" aria-hidden="true">/</span>
                <span :title="t('common.fields.user')">#{{ booking.userId }}</span>
              </p>
            </div>
            <StatusBadge :status="booking.status" />
          </div>

          <div v-if="booking.status !== 'cancelled'" class="mt-1 flex items-center gap-5">
            <button
              v-if="booking.status === 'pending'"
              class="u-action u-label text-brand"
              @click="confirm(booking)"
            >
              {{ t('common.actions.confirm') }}
            </button>
            <button class="u-action u-label text-alert" @click="cancel(booking)">
              {{ t('common.actions.cancel') }}
            </button>
          </div>
        </li>
        <li v-if="visible.length === 0" class="border-b border-line py-14 text-center text-sm text-ink-faint">
          {{ items.length === 0 ? t('bookings.allEmpty') : t('bookings.filterEmpty') }}
        </li>
      </ul>

      <div class="hidden border border-line bg-surface md:block">
        <table class="min-w-full text-sm">
          <thead>
            <tr class="border-b border-line">
              <th class="u-label px-5 py-4 text-start text-ink-faint">
                {{ t('common.fields.date') }}
              </th>
              <th class="u-label px-5 py-4 text-start text-ink-faint">
                {{ t('common.fields.service') }}
              </th>
              <th class="u-label px-5 py-4 text-start text-ink-faint">
                {{ t('common.fields.user') }}
              </th>
              <th class="u-label px-5 py-4 text-start text-ink-faint">
                {{ t('common.fields.status') }}
              </th>
              <th class="px-5 py-4">
                <span class="sr-only">{{ t('common.actions.confirm') }}</span>
              </th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="booking in visible" :key="booking.id" class="border-b border-line last:border-0">
              <td class="u-data px-5 py-4">
                <span dir="ltr" class="inline-block">{{ booking.bookingDate }}</span>
              </td>
              <td class="px-5 py-4 font-medium">
                {{
                  serviceNames[booking.serviceId] ??
                  t('bookings.unnamedService', { id: booking.serviceId })
                }}
              </td>
              <td class="u-data px-5 py-4 text-ink-soft">
                <span dir="ltr" class="inline-block">#{{ booking.userId }}</span>
              </td>
              <td class="px-5 py-4"><StatusBadge :status="booking.status" /></td>
              <td class="px-5 py-4 text-end whitespace-nowrap">
                <button
                  v-if="booking.status === 'pending'"
                  class="u-action u-label text-brand transition-opacity hover:opacity-70"
                  @click="confirm(booking)"
                >
                  {{ t('common.actions.confirm') }}
                </button>
                <button
                  v-if="booking.status !== 'cancelled'"
                  class="u-action u-label ms-4 text-alert transition-opacity hover:opacity-70"
                  @click="cancel(booking)"
                >
                  {{ t('common.actions.cancel') }}
                </button>
              </td>
            </tr>
            <tr v-if="visible.length === 0">
              <td colspan="5" class="px-5 py-16 text-center text-ink-faint">
                {{ items.length === 0 ? t('bookings.allEmpty') : t('bookings.filterEmpty') }}
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </template>
  </section>
</template>
