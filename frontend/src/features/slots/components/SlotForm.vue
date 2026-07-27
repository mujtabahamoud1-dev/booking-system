<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { DAY_INDEXES, shortTime, type Slot, type UpdateSlotRequest } from '../types'
import BaseInput from '@/shared/components/BaseInput.vue'
import BaseSelect from '@/shared/components/BaseSelect.vue'
import BaseButton from '@/shared/components/BaseButton.vue'

const props = defineProps<{ slot: Slot | null; submitting: boolean }>()
const emit = defineEmits<{ save: [payload: UpdateSlotRequest]; cancel: [] }>()

const { t } = useI18n()

const dayOfWeek = ref<number>(props.slot?.dayOfWeek ?? 1)
// <input type="time"> works in "HH:mm"; the API wants "HH:mm:ss".
const startTime = ref(props.slot ? shortTime(props.slot.startTime) : '09:00')
const endTime = ref(props.slot ? shortTime(props.slot.endTime) : '17:00')
const maxBookings = ref<number>(props.slot?.maxBookings ?? 1)

function submit(): void {
  emit('save', {
    dayOfWeek: Number(dayOfWeek.value),
    startTime: `${startTime.value}:00`,
    endTime: `${endTime.value}:00`,
    maxBookings: Number(maxBookings.value),
  })
}
</script>

<template>
  <form class="space-y-5" @submit.prevent="submit">
    <BaseSelect v-model="dayOfWeek" :label="t('slots.dayOfWeek')">
      <option v-for="index in DAY_INDEXES" :key="index" :value="index">
        {{ t(`common.days.${index}`) }}
      </option>
    </BaseSelect>

    <div class="grid grid-cols-2 gap-4">
      <BaseInput v-model="startTime" :label="t('slots.startTime')" type="time" required />
      <BaseInput v-model="endTime" :label="t('slots.endTime')" type="time" required />
    </div>

    <BaseInput
      v-model="maxBookings"
      :label="t('slots.maxBookings')"
      type="number"
      min="1"
      required
    />

    <div class="flex justify-end gap-2 border-t border-line pt-5">
      <BaseButton variant="secondary" @click="emit('cancel')">
        {{ t('common.actions.cancel') }}
      </BaseButton>
      <BaseButton type="submit" :loading="submitting">
        {{ slot ? t('common.actions.saveChanges') : t('common.actions.create') }}
      </BaseButton>
    </div>
  </form>
</template>
