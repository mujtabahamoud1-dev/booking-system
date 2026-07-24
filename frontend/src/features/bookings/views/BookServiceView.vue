<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { servicesApi } from '@/features/services/api'
import type { Service } from '@/features/services/types'
import { slotsApi } from '@/features/slots/api'
import { shortTime, type Slot } from '@/features/slots/types'
import { useBookingsStore } from '../store'
import { apiErrorMessage } from '@/shared/api/client'
import BaseInput from '@/shared/components/BaseInput.vue'
import BaseButton from '@/shared/components/BaseButton.vue'
import AlertMessage from '@/shared/components/AlertMessage.vue'
import LoadingSpinner from '@/shared/components/LoadingSpinner.vue'

const props = defineProps<{ serviceId: string }>()
const router = useRouter()
const bookings = useBookingsStore()
const { t } = useI18n()

const service = ref<Service | null>(null)
const slots = ref<Slot[]>([])
const loading = ref(true)

const selectedSlotId = ref<number | null>(null)
const bookingDate = ref('')
const notes = ref('')
const error = ref<string | null>(null)
const submitting = ref(false)

const id = Number(props.serviceId)

const selectedSlot = computed(() => slots.value.find((s) => s.id === selectedSlotId.value) ?? null)

// The API rejects a date whose weekday differs from the slot's; catch it early.
const dayMismatch = computed(() => {
  if (!selectedSlot.value || !bookingDate.value) return false
  const weekday = new Date(`${bookingDate.value}T00:00:00`).getDay()
  return weekday !== selectedSlot.value.dayOfWeek
})

const dateError = computed(() =>
  dayMismatch.value && selectedSlot.value
    ? t('bookings.pickDay', { day: t(`common.days.${selectedSlot.value.dayOfWeek}`) })
    : null,
)

const today = new Date().toISOString().slice(0, 10)

onMounted(async () => {
  try {
    ;[service.value, slots.value] = await Promise.all([
      servicesApi.getById(id),
      slotsApi.getForService(id),
    ])
  } catch (e) {
    error.value = apiErrorMessage(e, t('bookings.loadFailed'))
  } finally {
    loading.value = false
  }
})

async function submit(): Promise<void> {
  if (!selectedSlotId.value) {
    error.value = t('bookings.slotRequired')
    return
  }
  if (dayMismatch.value) {
    error.value = t('bookings.dayMismatch')
    return
  }
  error.value = null
  submitting.value = true
  try {
    await bookings.create({
      serviceId: id,
      slotId: selectedSlotId.value,
      bookingDate: bookingDate.value,
      notes: notes.value || null,
    })
    router.push('/bookings')
  } catch (e) {
    error.value = apiErrorMessage(e, t('bookings.createFailed'))
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <section class="mx-auto max-w-lg">
    <LoadingSpinner v-if="loading" />

    <template v-else-if="service">
      <h1 class="text-2xl font-bold text-slate-900">
        {{ t('bookings.bookTitle', { name: service.name }) }}
      </h1>
      <p class="mt-1 text-slate-500">
        {{ t('common.minutesShort', { count: service.duration }) }} · ${{
          service.price.toFixed(2)
        }}
      </p>

      <form class="mt-6 space-y-5" @submit.prevent="submit">
        <AlertMessage v-if="error">{{ error }}</AlertMessage>

        <div>
          <span class="mb-2 block text-sm font-medium text-slate-700">{{
            t('bookings.chooseSlot')
          }}</span>
          <p v-if="slots.length === 0" class="text-sm text-slate-500">
            {{ t('bookings.noSlots') }}
          </p>
          <div class="space-y-2">
            <label
              v-for="slot in slots"
              :key="slot.id"
              class="flex cursor-pointer items-center gap-3 rounded-md border p-3 text-sm"
              :class="
                selectedSlotId === slot.id
                  ? 'border-indigo-500 bg-indigo-50'
                  : 'border-slate-200 hover:bg-slate-50'
              "
            >
              <input v-model="selectedSlotId" type="radio" :value="slot.id" class="h-4 w-4" />
              <span class="font-medium text-slate-800">{{
                t(`common.days.${slot.dayOfWeek}`)
              }}</span>
              <!-- Clock ranges stay left-to-right even in an RTL page. -->
              <span dir="ltr" class="text-slate-500"
                >{{ shortTime(slot.startTime) }} – {{ shortTime(slot.endTime) }}</span
              >
            </label>
          </div>
        </div>

        <BaseInput
          v-model="bookingDate"
          :label="t('common.fields.date')"
          type="date"
          :min="today"
          required
          :error="dateError"
        />

        <BaseInput
          v-model="notes"
          :label="t('common.fields.notes')"
          :placeholder="t('common.fields.optional')"
        />

        <BaseButton
          type="submit"
          :loading="submitting"
          :disabled="slots.length === 0"
          class="w-full"
        >
          {{ t('bookings.confirmBooking') }}
        </BaseButton>
      </form>
    </template>

    <AlertMessage v-else>{{ error ?? t('bookings.serviceNotFound') }}</AlertMessage>
  </section>
</template>
