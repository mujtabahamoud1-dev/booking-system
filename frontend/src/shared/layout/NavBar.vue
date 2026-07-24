<script setup lang="ts">
import { computed } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/features/auth/store'
import LanguageSwitcher from '@/shared/components/LanguageSwitcher.vue'

const auth = useAuthStore()
const router = useRouter()
const { t } = useI18n()

// Route targets shown for the current session, in nav order.
const links = computed(() => {
  if (!auth.isAuthenticated) {
    return [{ to: '/', label: t('common.nav.services') }]
  }
  if (auth.isAdmin) {
    return [
      { to: '/admin', label: t('common.nav.dashboard') },
      { to: '/admin/services', label: t('common.nav.services') },
      { to: '/admin/slots', label: t('common.nav.slots') },
      { to: '/admin/bookings', label: t('common.nav.bookings') },
    ]
  }
  return [
    { to: '/', label: t('common.nav.services') },
    { to: '/bookings', label: t('common.nav.myBookings') },
  ]
})

async function logout(): Promise<void> {
  await auth.logout()
  router.push('/login')
}
</script>

<template>
  <header class="border-b border-slate-200 bg-white">
    <nav class="mx-auto flex h-14 max-w-5xl items-center gap-6 px-4">
      <RouterLink to="/" class="flex items-center gap-2 font-semibold text-slate-900">
        <span class="text-lg">📅</span>
        <span>{{ t('common.appName') }}</span>
      </RouterLink>

      <div class="flex items-center gap-1">
        <RouterLink
          v-for="link in links"
          :key="link.to"
          :to="link.to"
          class="rounded-md px-3 py-1.5 text-sm font-medium text-slate-600 hover:bg-slate-100 hover:text-slate-900"
          active-class="bg-slate-100 text-slate-900"
        >
          {{ link.label }}
        </RouterLink>
      </div>

      <div class="ms-auto flex items-center gap-3 text-sm">
        <LanguageSwitcher />

        <template v-if="auth.isAuthenticated">
          <span class="text-slate-500">
            {{ auth.name }}
            <span
              v-if="auth.isAdmin"
              class="ms-1 rounded bg-indigo-50 px-1.5 py-0.5 text-xs font-medium text-indigo-700"
              >{{ t('common.nav.admin') }}</span
            >
          </span>
          <button class="font-medium text-slate-600 hover:text-slate-900" @click="logout">
            {{ t('common.nav.logout') }}
          </button>
        </template>
        <template v-else>
          <RouterLink to="/login" class="font-medium text-slate-600 hover:text-slate-900">{{
            t('common.nav.login')
          }}</RouterLink>
          <RouterLink
            to="/register"
            class="rounded-md bg-indigo-600 px-3 py-1.5 font-semibold text-white hover:bg-indigo-700"
            >{{ t('common.nav.register') }}</RouterLink
          >
        </template>
      </div>
    </nav>
  </header>
</template>
