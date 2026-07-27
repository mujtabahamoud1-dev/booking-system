<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { RouterLink } from 'vue-router'
import { storeToRefs } from 'pinia'
import { useI18n } from 'vue-i18n'
import { useServicesStore } from '@/features/services/store'
import { useBookingsStore } from '@/features/bookings/store'
import { useAuthStore } from '@/features/auth/store'
import LoadingSpinner from '@/shared/components/LoadingSpinner.vue'

const servicesStore = useServicesStore()
const bookingsStore = useBookingsStore()
const auth = useAuthStore()
const { items: services } = storeToRefs(servicesStore)
const { items: bookings, loading } = storeToRefs(bookingsStore)
const { t } = useI18n()

onMounted(() => {
  servicesStore.load(true)
  bookingsStore.loadAll()
})

const stats = computed(() => ({
  services: services.value.length,
  active: services.value.filter((s) => s.isActive).length,
  total: bookings.value.length,
  pending: bookings.value.filter((b) => b.status === 'pending').length,
  confirmed: bookings.value.filter((b) => b.status === 'confirmed').length,
  cancelled: bookings.value.filter((b) => b.status === 'cancelled').length,
}))

// The counts are reference, not the headline — what matters on opening this page
// is whether anything is waiting on you.
const metrics = computed(() => [
  { key: 'services', label: t('admin.metrics.services'), value: stats.value.services },
  { key: 'active', label: t('admin.metrics.active'), value: stats.value.active },
  { key: 'total', label: t('admin.metrics.total'), value: stats.value.total },
  { key: 'confirmed', label: t('admin.metrics.confirmed'), value: stats.value.confirmed },
  { key: 'cancelled', label: t('admin.metrics.cancelled'), value: stats.value.cancelled },
])

const shortcuts = computed(() => [
  { to: '/admin/services', label: t('admin.shortcuts.services'), hint: t('admin.shortcuts.servicesHint') },
  { to: '/admin/slots', label: t('admin.shortcuts.slots'), hint: t('admin.shortcuts.slotsHint') },
  { to: '/admin/bookings', label: t('admin.shortcuts.bookings'), hint: t('admin.shortcuts.bookingsHint') },
])
</script>

<template>
  <section>
    <header class="mb-8 sm:mb-10">
      <h1 class="u-display text-3xl text-balance md:text-4xl">
        {{ t('admin.welcome', { name: auth.name }) }}
      </h1>
    </header>

    <LoadingSpinner v-if="loading" />

    <template v-else>
      <!-- The one thing that might need doing, stated plainly and linked. -->
      <RouterLink
        v-if="stats.pending > 0"
        to="/admin/bookings"
        class="group flex items-center gap-4 border-s-2 border-signal bg-signal-soft p-5 transition-colors hover:bg-signal-soft/70 sm:gap-5 sm:p-6"
      >
        <span class="u-data text-4xl leading-none font-medium text-signal-ink">
          {{ stats.pending }}
        </span>
        <span>
          <span class="block font-medium">{{ t('admin.pendingLead') }}</span>
          <span class="u-label mt-1 block text-signal-ink">
            {{ t('admin.pendingAction') }}
            <span class="inline-block transition-transform group-hover:translate-x-0.5 rtl:rotate-180" aria-hidden="true">→</span>
          </span>
        </span>
      </RouterLink>

      <!-- The clear state is the quiet one; it should not take the same weight
           as a queue that needs working through. -->
      <p v-else class="border-s-2 border-brand bg-brand-soft px-5 py-4 text-sm sm:px-6">
        {{ t('admin.pendingClear') }}
      </p>

      <!-- Reference figures, set as a rule-separated row rather than a grid of
           boxes so they stay subordinate to the queue above. -->
      <dl class="mt-10 grid grid-cols-2 border-s border-t border-line sm:grid-cols-3 lg:grid-cols-5">
        <div
          v-for="metric in metrics"
          :key="metric.key"
          class="border-e border-b border-line bg-surface px-4 py-3.5 sm:px-5 sm:py-4"
        >
          <dt class="u-label text-ink-faint">{{ metric.label }}</dt>
          <dd class="u-data mt-1.5 text-2xl font-medium">{{ metric.value }}</dd>
        </div>
      </dl>

      <nav class="mt-10">
        <h2 class="u-label mb-4 text-ink-faint">{{ t('admin.manage') }}</h2>
        <ul class="border-t border-line">
          <li v-for="shortcut in shortcuts" :key="shortcut.to">
            <RouterLink
              :to="shortcut.to"
              class="group flex items-center justify-between gap-4 border-b border-line py-4 transition-colors hover:bg-surface"
            >
              <span>
                <span class="block font-medium">{{ shortcut.label }}</span>
                <span class="mt-0.5 block text-sm text-ink-faint">{{ shortcut.hint }}</span>
              </span>
              <span
                class="text-ink-faint transition-transform group-hover:translate-x-0.5 rtl:rotate-180"
                aria-hidden="true"
                >→</span
              >
            </RouterLink>
          </li>
        </ul>
      </nav>
    </template>
  </section>
</template>
