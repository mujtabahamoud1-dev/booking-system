<script setup lang="ts">
import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import type { Service, UpdateServiceRequest } from '../types'
import BaseInput from '@/shared/components/BaseInput.vue'
import BaseButton from '@/shared/components/BaseButton.vue'

const { t } = useI18n()

// `service` is null when creating. On save the parent decides create vs update;
// isActive is only meaningful for updates but always carried in the payload.
const props = defineProps<{ service: Service | null; submitting: boolean }>()
const emit = defineEmits<{ save: [payload: UpdateServiceRequest]; cancel: [] }>()

const name = ref(props.service?.name ?? '')
const description = ref(props.service?.description ?? '')
const duration = ref<number>(props.service?.duration ?? 30)
const price = ref<number>(props.service?.price ?? 0)
const isActive = ref(props.service?.isActive ?? true)

function submit(): void {
  emit('save', {
    name: name.value.trim(),
    description: description.value.trim() || null,
    duration: Number(duration.value),
    price: Number(price.value),
    isActive: isActive.value,
  })
}
</script>

<template>
  <form class="space-y-4" @submit.prevent="submit">
    <BaseInput v-model="name" :label="t('common.fields.name')" required />
    <BaseInput
      v-model="description"
      :label="t('common.fields.description')"
      :placeholder="t('common.fields.optional')"
    />
    <div class="grid grid-cols-2 gap-4">
      <BaseInput
        v-model="duration"
        :label="t('services.durationField')"
        type="number"
        min="1"
        required
      />
      <BaseInput
        v-model="price"
        :label="t('common.fields.price')"
        type="number"
        min="0"
        step="0.01"
        required
      />
    </div>
    <label v-if="service" class="flex items-center gap-2 text-sm text-slate-700">
      <input v-model="isActive" type="checkbox" class="h-4 w-4 rounded border-slate-300" />
      {{ t('common.fields.active') }}
    </label>
    <div class="flex justify-end gap-2 pt-2">
      <BaseButton variant="secondary" @click="emit('cancel')">{{
        t('common.actions.cancel')
      }}</BaseButton>
      <BaseButton type="submit" :loading="submitting">
        {{ service ? t('common.actions.saveChanges') : t('common.actions.create') }}
      </BaseButton>
    </div>
  </form>
</template>
