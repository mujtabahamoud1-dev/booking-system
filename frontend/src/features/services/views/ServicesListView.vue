<script setup lang="ts">
import { onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useI18n } from 'vue-i18n'
import { useServicesStore } from '../store'
import { useAuthStore } from '@/features/auth/store'
import LoadingSpinner from '@/shared/components/LoadingSpinner.vue'

const store = useServicesStore()
const auth = useAuthStore()
const { t } = useI18n()
const { items, loading } = storeToRefs(store)

onMounted(() => store.load())
</script>

<template>
  <section>
    <div class="mb-6">
      <h1 class="text-2xl font-bold text-slate-900">{{ t('services.title') }}</h1>
      <p class="mt-1 text-slate-500">{{ t('services.subtitle') }}</p>
    </div>

    <LoadingSpinner v-if="loading" />

    <p v-else-if="items.length === 0" class="rounded-lg bg-white p-8 text-center text-slate-500">
      {{ t('services.empty') }}
    </p>

    <div v-else class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      <article
        v-for="service in items"
        :key="service.id"
        class="flex flex-col rounded-lg border border-slate-200 bg-white p-5 shadow-sm"
      >
        <h2 class="text-lg font-semibold text-slate-900">{{ service.name }}</h2>
        <p class="mt-1 flex-1 text-sm text-slate-500">
          {{ service.description || t('services.noDescription') }}
        </p>
        <dl class="mt-4 flex items-center justify-between text-sm">
          <div>
            <dt class="text-slate-400">{{ t('common.fields.duration') }}</dt>
            <dd class="font-medium text-slate-700">
              {{ t('common.minutesShort', { count: service.duration }) }}
            </dd>
          </div>
          <div class="text-end">
            <dt class="text-slate-400">{{ t('common.fields.price') }}</dt>
            <dd class="font-medium text-slate-700">${{ service.price.toFixed(2) }}</dd>
          </div>
        </dl>
        <RouterLink
          v-if="!auth.isAdmin"
          :to="`/book/${service.id}`"
          class="mt-4 inline-flex justify-center rounded-md bg-indigo-600 px-3.5 py-2 text-sm font-semibold text-white shadow-sm hover:bg-indigo-700"
        >
          {{ t('services.bookNow') }}
        </RouterLink>
      </article>
    </div>
  </section>
</template>
