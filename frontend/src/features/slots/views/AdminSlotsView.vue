<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { storeToRefs } from 'pinia'
import { useI18n } from 'vue-i18n'
import { useServicesStore } from '@/features/services/store'
import { useSlotsStore } from '../store'
import { shortTime, type Slot, type UpdateSlotRequest } from '../types'
import { apiErrorMessage } from '@/shared/api/client'
import SlotForm from '../components/SlotForm.vue'
import BaseButton from '@/shared/components/BaseButton.vue'
import BaseModal from '@/shared/components/BaseModal.vue'
import AlertMessage from '@/shared/components/AlertMessage.vue'
import LoadingSpinner from '@/shared/components/LoadingSpinner.vue'

const servicesStore = useServicesStore()
const slotsStore = useSlotsStore()
const { items: services } = storeToRefs(servicesStore)
const { loading } = storeToRefs(slotsStore)
const { t } = useI18n()

const selectedServiceId = ref<number | null>(null)
const showForm = ref(false)
const editing = ref<Slot | null>(null)
const submitting = ref(false)
const error = ref<string | null>(null)

const slots = computed(() =>
  selectedServiceId.value ? slotsStore.forService(selectedServiceId.value) : [],
)

onMounted(async () => {
  await servicesStore.load(true)
  if (services.value.length > 0) selectedServiceId.value = services.value[0].id
})

// Reload slots whenever the chosen service changes.
watch(selectedServiceId, (id) => {
  if (id) slotsStore.load(id)
})

function openCreate(): void {
  editing.value = null
  error.value = null
  showForm.value = true
}

function openEdit(slot: Slot): void {
  editing.value = slot
  error.value = null
  showForm.value = true
}

async function save(payload: UpdateSlotRequest): Promise<void> {
  if (!selectedServiceId.value) return
  submitting.value = true
  error.value = null
  try {
    if (editing.value) {
      await slotsStore.update(editing.value.id, selectedServiceId.value, payload)
    } else {
      await slotsStore.create({ serviceId: selectedServiceId.value, ...payload })
    }
    showForm.value = false
  } catch (e) {
    error.value = apiErrorMessage(e, t('slots.saveFailed'))
  } finally {
    submitting.value = false
  }
}

async function remove(slot: Slot): Promise<void> {
  if (!selectedServiceId.value) return
  if (!confirm(t('slots.confirmDelete'))) return
  error.value = null
  try {
    await slotsStore.remove(slot.id, selectedServiceId.value)
  } catch (e) {
    // 409 when the slot already has bookings.
    error.value = apiErrorMessage(e, t('slots.deleteFailed'))
  }
}
</script>

<template>
  <section>
    <div class="mb-6 flex flex-wrap items-center justify-between gap-4">
      <h1 class="text-2xl font-bold text-slate-900">{{ t('slots.title') }}</h1>
      <BaseButton :disabled="!selectedServiceId" @click="openCreate">{{
        t('slots.newSlot')
      }}</BaseButton>
    </div>

    <label class="mb-6 block max-w-xs">
      <span class="mb-1 block text-sm font-medium text-slate-700">{{
        t('common.fields.service')
      }}</span>
      <select
        v-model="selectedServiceId"
        class="block w-full rounded-md border-0 px-3 py-2 text-slate-900 ring-1 ring-inset ring-slate-300 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm"
      >
        <option v-if="services.length === 0" :value="null">{{ t('slots.noServices') }}</option>
        <option v-for="service in services" :key="service.id" :value="service.id">
          {{ service.name }}
        </option>
      </select>
    </label>

    <AlertMessage v-if="error" class="mb-4">{{ error }}</AlertMessage>

    <LoadingSpinner v-if="loading" />

    <div v-else class="overflow-hidden rounded-lg border border-slate-200 bg-white shadow-sm">
      <table class="min-w-full divide-y divide-slate-200 text-sm">
        <thead class="bg-slate-50 text-start text-slate-500">
          <tr>
            <th class="px-4 py-3 font-medium">{{ t('slots.day') }}</th>
            <th class="px-4 py-3 font-medium">{{ t('slots.time') }}</th>
            <th class="px-4 py-3 font-medium">{{ t('slots.maxBookings') }}</th>
            <th class="px-4 py-3"></th>
          </tr>
        </thead>
        <tbody class="divide-y divide-slate-100">
          <tr v-for="slot in slots" :key="slot.id">
            <td class="px-4 py-3 font-medium text-slate-900">
              {{ t(`common.days.${slot.dayOfWeek}`) }}
            </td>
            <!-- Clock ranges stay left-to-right even in an RTL page. -->
            <td class="px-4 py-3 text-slate-600">
              <span dir="ltr" class="inline-block">
                {{ shortTime(slot.startTime) }} – {{ shortTime(slot.endTime) }}
              </span>
            </td>
            <td class="px-4 py-3 text-slate-600">{{ slot.maxBookings }}</td>
            <td class="px-4 py-3 text-end whitespace-nowrap">
              <button
                class="font-medium text-indigo-600 hover:text-indigo-500"
                @click="openEdit(slot)"
              >
                {{ t('common.actions.edit') }}
              </button>
              <button
                class="ms-3 font-medium text-rose-600 hover:text-rose-500"
                @click="remove(slot)"
              >
                {{ t('common.actions.delete') }}
              </button>
            </td>
          </tr>
          <tr v-if="slots.length === 0">
            <td colspan="4" class="px-4 py-8 text-center text-slate-500">
              {{ t('slots.empty') }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <BaseModal
      v-if="showForm"
      :title="editing ? t('slots.editSlot') : t('slots.newSlot')"
      @close="showForm = false"
    >
      <SlotForm :slot="editing" :submitting="submitting" @save="save" @cancel="showForm = false" />
    </BaseModal>
  </section>
</template>
