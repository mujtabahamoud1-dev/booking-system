<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue'
import { storeToRefs } from 'pinia'
import { useI18n } from 'vue-i18n'
import { useServicesStore } from '@/features/services/store'
import { useSlotsStore } from '../store'
import { shortTime, type Slot, type UpdateSlotRequest } from '../types'
import { apiErrorMessage } from '@/shared/api/client'
import { useConfirm } from '@/shared/composables/useConfirm'
import SlotForm from '../components/SlotForm.vue'
import WeekTrack from '../components/WeekTrack.vue'
import BaseButton from '@/shared/components/BaseButton.vue'
import BaseModal from '@/shared/components/BaseModal.vue'
import BaseSelect from '@/shared/components/BaseSelect.vue'
import AlertMessage from '@/shared/components/AlertMessage.vue'
import LoadingSpinner from '@/shared/components/LoadingSpinner.vue'

const servicesStore = useServicesStore()
const slotsStore = useSlotsStore()
const { items: services } = storeToRefs(servicesStore)
const { loading } = storeToRefs(slotsStore)
const { t } = useI18n()
const { confirm } = useConfirm()

const selectedServiceId = ref<number | null>(null)
const selectedSlotId = ref<number | null>(null)
const showForm = ref(false)
const editing = ref<Slot | null>(null)
const submitting = ref(false)
const error = ref<string | null>(null)

const slots = computed(() =>
  selectedServiceId.value ? slotsStore.forService(selectedServiceId.value) : [],
)

const selectedSlot = computed(() => slots.value.find((s) => s.id === selectedSlotId.value) ?? null)

const weeklyHours = computed(() => {
  const minutes = slots.value.reduce((sum, slot) => {
    const [sh, sm] = slot.startTime.split(':').map(Number)
    const [eh, em] = slot.endTime.split(':').map(Number)
    return sum + (eh * 60 + em - (sh * 60 + sm))
  }, 0)
  return Math.round((minutes / 60) * 10) / 10
})

onMounted(async () => {
  await servicesStore.load(true)
  if (services.value.length > 0) selectedServiceId.value = services.value[0].id
})

// Reload slots whenever the chosen service changes.
watch(selectedServiceId, (id) => {
  selectedSlotId.value = null
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
  const ok = await confirm({
    message: t('slots.confirmDelete'),
    confirmLabel: t('common.actions.delete'),
  })
  if (!ok) return

  error.value = null
  try {
    await slotsStore.remove(slot.id, selectedServiceId.value)
    selectedSlotId.value = null
  } catch (e) {
    // 409 when the slot already has bookings.
    error.value = apiErrorMessage(e, t('slots.deleteFailed'))
  }
}
</script>

<template>
  <section>
    <header class="mb-8">
      <h1 class="u-display text-3xl text-balance md:text-4xl">{{ t('slots.title') }}</h1>
      <p class="mt-3 text-ink-soft">{{ t('slots.subtitle') }}</p>
    </header>

    <div class="mb-8 flex flex-wrap items-end justify-between gap-4">
      <div class="w-full sm:max-w-xs">
        <BaseSelect v-model="selectedServiceId" :label="t('common.fields.service')">
          <option v-if="services.length === 0" :value="null">{{ t('slots.noServices') }}</option>
          <option v-for="service in services" :key="service.id" :value="service.id">
            {{ service.name }}
          </option>
        </BaseSelect>
      </div>
      <BaseButton :disabled="!selectedServiceId" class="w-full sm:w-auto" @click="openCreate">
        {{ t('slots.newSlot') }}
      </BaseButton>
    </div>

    <AlertMessage v-if="error" class="mb-6">{{ error }}</AlertMessage>

    <LoadingSpinner v-if="loading" />

    <template v-else>
      <!-- Every weekday is shown, empty ones included: a gap in cover is the
           thing you most need to notice here. -->
      <div class="border border-line bg-surface p-4 sm:p-7">
        <div class="mb-6 flex items-baseline justify-between gap-4">
          <h2 class="u-label text-ink-faint">{{ t('slots.weekCover') }}</h2>
          <p v-if="slots.length > 0" class="u-data text-xs text-ink-faint">
            {{ t('slots.weeklyHours', { hours: weeklyHours }) }}
          </p>
        </div>

        <p
          v-if="slots.length === 0"
          class="border border-dashed border-line px-6 py-12 text-center text-sm text-ink-faint"
        >
          {{ t('slots.empty') }}
        </p>
        <WeekTrack
          v-else
          :slots="slots"
          :selected-id="selectedSlotId"
          :group-label="t('slots.weekCover')"
          show-empty-days
          @select="selectedSlotId = $event.id"
        />
      </div>

      <!-- Actions attach to the selected bar rather than repeating the whole
           week as a table underneath it. -->
      <div
        v-if="selectedSlot"
        class="mt-4 flex flex-wrap items-center justify-between gap-4 border border-line bg-brand-soft px-4 py-4 sm:px-5"
      >
        <p class="text-sm">
          <span class="font-medium">{{ t(`common.days.${selectedSlot.dayOfWeek}`) }}</span>
          <span class="u-data ms-3 text-ink-soft">
            <span dir="ltr" class="inline-block"
              >{{ shortTime(selectedSlot.startTime) }}–{{ shortTime(selectedSlot.endTime) }}</span
            >
            <span class="mx-2 text-line" aria-hidden="true">/</span>
            {{ t('slots.capacityOf', { count: selectedSlot.maxBookings }) }}
          </span>
        </p>
        <div class="flex w-full items-center gap-2 sm:w-auto">
          <BaseButton variant="secondary" class="flex-1 sm:flex-none" @click="openEdit(selectedSlot)">
            {{ t('common.actions.edit') }}
          </BaseButton>
          <BaseButton variant="danger" class="flex-1 sm:flex-none" @click="remove(selectedSlot)">
            {{ t('common.actions.delete') }}
          </BaseButton>
        </div>
      </div>
      <p v-else-if="slots.length > 0" class="mt-4 text-sm text-ink-faint">
        {{ t('slots.selectHint') }}
      </p>
    </template>

    <BaseModal
      v-if="showForm"
      :title="editing ? t('slots.editSlot') : t('slots.newSlot')"
      @close="showForm = false"
    >
      <SlotForm :slot="editing" :submitting="submitting" @save="save" @cancel="showForm = false" />
    </BaseModal>
  </section>
</template>
