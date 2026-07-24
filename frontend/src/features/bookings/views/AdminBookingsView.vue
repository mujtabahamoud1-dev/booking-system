<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { storeToRefs } from 'pinia'
import { useI18n } from 'vue-i18n'
import { useBookingsStore } from '../store'
import { useServicesStore } from '@/features/services/store'
import type { Booking } from '../types'
import { apiErrorMessage } from '@/shared/api/client'
import StatusBadge from '../components/StatusBadge.vue'
import AlertMessage from '@/shared/components/AlertMessage.vue'
import LoadingSpinner from '@/shared/components/LoadingSpinner.vue'

const bookings = useBookingsStore()
const servicesStore = useServicesStore()
const { items, loading } = storeToRefs(bookings)
const { t } = useI18n()

const error = ref<string | null>(null)

const serviceNames = computed(() =>
  Object.fromEntries(servicesStore.items.map((s) => [s.id, s.name])),
)

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
  if (!window.confirm(t('bookings.confirmCancel'))) return
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
    <h1 class="mb-6 text-2xl font-bold text-slate-900">{{ t('bookings.allTitle') }}</h1>

    <AlertMessage v-if="error" class="mb-4">{{ error }}</AlertMessage>

    <LoadingSpinner v-if="loading" />

    <div v-else class="overflow-hidden rounded-lg border border-slate-200 bg-white shadow-sm">
      <table class="min-w-full divide-y divide-slate-200 text-sm">
        <thead class="bg-slate-50 text-start text-slate-500">
          <tr>
            <th class="px-4 py-3 font-medium">#</th>
            <th class="px-4 py-3 font-medium">{{ t('common.fields.service') }}</th>
            <th class="px-4 py-3 font-medium">{{ t('common.fields.user') }}</th>
            <th class="px-4 py-3 font-medium">{{ t('common.fields.date') }}</th>
            <th class="px-4 py-3 font-medium">{{ t('common.fields.status') }}</th>
            <th class="px-4 py-3"></th>
          </tr>
        </thead>
        <tbody class="divide-y divide-slate-100">
          <tr v-for="booking in items" :key="booking.id">
            <td class="px-4 py-3 text-slate-400">{{ booking.id }}</td>
            <td class="px-4 py-3 font-medium text-slate-900">
              {{
                serviceNames[booking.serviceId] ??
                t('bookings.unnamedService', { id: booking.serviceId })
              }}
            </td>
            <td class="px-4 py-3 text-slate-600">
              <span dir="ltr" class="inline-block">#{{ booking.userId }}</span>
            </td>
            <td class="px-4 py-3 text-slate-600">
              <span dir="ltr" class="inline-block">{{ booking.bookingDate }}</span>
            </td>
            <td class="px-4 py-3"><StatusBadge :status="booking.status" /></td>
            <td class="px-4 py-3 text-end whitespace-nowrap">
              <button
                v-if="booking.status === 'pending'"
                class="font-medium text-emerald-600 hover:text-emerald-500"
                @click="confirm(booking)"
              >
                {{ t('common.actions.confirm') }}
              </button>
              <button
                v-if="booking.status !== 'cancelled'"
                class="ms-3 font-medium text-rose-600 hover:text-rose-500"
                @click="cancel(booking)"
              >
                {{ t('common.actions.cancel') }}
              </button>
            </td>
          </tr>
          <tr v-if="items.length === 0">
            <td colspan="6" class="px-4 py-8 text-center text-slate-500">
              {{ t('bookings.allEmpty') }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </section>
</template>
