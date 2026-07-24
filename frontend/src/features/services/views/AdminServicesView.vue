<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { storeToRefs } from 'pinia'
import { useI18n } from 'vue-i18n'
import { useServicesStore } from '../store'
import type { Service, UpdateServiceRequest } from '../types'
import { apiErrorMessage } from '@/shared/api/client'
import ServiceForm from '../components/ServiceForm.vue'
import BaseButton from '@/shared/components/BaseButton.vue'
import BaseModal from '@/shared/components/BaseModal.vue'
import AlertMessage from '@/shared/components/AlertMessage.vue'
import LoadingSpinner from '@/shared/components/LoadingSpinner.vue'

const store = useServicesStore()
const { items, loading } = storeToRefs(store)
const { t } = useI18n()

const showForm = ref(false)
const editing = ref<Service | null>(null)
const submitting = ref(false)
const error = ref<string | null>(null)

onMounted(() => store.load(true))

function openCreate(): void {
  editing.value = null
  error.value = null
  showForm.value = true
}

function openEdit(service: Service): void {
  editing.value = service
  error.value = null
  showForm.value = true
}

async function save(payload: UpdateServiceRequest): Promise<void> {
  submitting.value = true
  error.value = null
  try {
    if (editing.value) {
      await store.update(editing.value.id, payload)
    } else {
      await store.create({
        name: payload.name,
        description: payload.description,
        duration: payload.duration,
        price: payload.price,
      })
    }
    showForm.value = false
  } catch (e) {
    error.value = apiErrorMessage(e, t('services.saveFailed'))
  } finally {
    submitting.value = false
  }
}

async function remove(service: Service): Promise<void> {
  if (!confirm(t('services.confirmDelete', { name: service.name }))) return
  error.value = null
  try {
    await store.remove(service.id)
  } catch (e) {
    // 409 when the service has bookings — the API asks you to deactivate instead.
    error.value = apiErrorMessage(e, t('services.deleteFailed'))
  }
}
</script>

<template>
  <section>
    <div class="mb-6 flex items-center justify-between">
      <h1 class="text-2xl font-bold text-slate-900">{{ t('services.adminTitle') }}</h1>
      <BaseButton @click="openCreate">{{ t('services.newService') }}</BaseButton>
    </div>

    <AlertMessage v-if="error" class="mb-4">{{ error }}</AlertMessage>

    <LoadingSpinner v-if="loading" />

    <div v-else class="overflow-hidden rounded-lg border border-slate-200 bg-white shadow-sm">
      <table class="min-w-full divide-y divide-slate-200 text-sm">
        <thead class="bg-slate-50 text-start text-slate-500">
          <tr>
            <th class="px-4 py-3 font-medium">{{ t('common.fields.name') }}</th>
            <th class="px-4 py-3 font-medium">{{ t('common.fields.duration') }}</th>
            <th class="px-4 py-3 font-medium">{{ t('common.fields.price') }}</th>
            <th class="px-4 py-3 font-medium">{{ t('common.fields.status') }}</th>
            <th class="px-4 py-3"></th>
          </tr>
        </thead>
        <tbody class="divide-y divide-slate-100">
          <tr v-for="service in items" :key="service.id">
            <td class="px-4 py-3">
              <div class="font-medium text-slate-900">{{ service.name }}</div>
              <div class="text-slate-400">
                {{ service.description || t('common.emptyValue') }}
              </div>
            </td>
            <td class="px-4 py-3 text-slate-600">
              {{ t('common.minutesShort', { count: service.duration }) }}
            </td>
            <td class="px-4 py-3 text-slate-600">${{ service.price.toFixed(2) }}</td>
            <td class="px-4 py-3">
              <span
                class="inline-flex rounded-full px-2 py-0.5 text-xs font-medium ring-1 ring-inset"
                :class="
                  service.isActive
                    ? 'bg-emerald-50 text-emerald-700 ring-emerald-200'
                    : 'bg-slate-100 text-slate-500 ring-slate-200'
                "
              >
                {{ service.isActive ? t('services.statusActive') : t('services.statusInactive') }}
              </span>
            </td>
            <td class="px-4 py-3 text-end whitespace-nowrap">
              <button
                class="font-medium text-indigo-600 hover:text-indigo-500"
                @click="openEdit(service)"
              >
                {{ t('common.actions.edit') }}
              </button>
              <button
                class="ms-3 font-medium text-rose-600 hover:text-rose-500"
                @click="remove(service)"
              >
                {{ t('common.actions.delete') }}
              </button>
            </td>
          </tr>
          <tr v-if="items.length === 0">
            <td colspan="5" class="px-4 py-8 text-center text-slate-500">
              {{ t('services.adminEmpty') }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <BaseModal
      v-if="showForm"
      :title="editing ? t('services.editService') : t('services.newService')"
      @close="showForm = false"
    >
      <ServiceForm
        :service="editing"
        :submitting="submitting"
        @save="save"
        @cancel="showForm = false"
      />
    </BaseModal>
  </section>
</template>
