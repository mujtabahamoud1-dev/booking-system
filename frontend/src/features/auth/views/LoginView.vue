<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter, RouterLink } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '../store'
import { apiErrorMessage } from '@/shared/api/client'
import BaseInput from '@/shared/components/BaseInput.vue'
import BaseButton from '@/shared/components/BaseButton.vue'
import AlertMessage from '@/shared/components/AlertMessage.vue'

const auth = useAuthStore()
const route = useRoute()
const router = useRouter()
const { t } = useI18n()

const email = ref('')
const password = ref('')
const error = ref<string | null>(null)
const submitting = ref(false)

async function submit(): Promise<void> {
  error.value = null
  submitting.value = true
  try {
    await auth.login({ email: email.value, password: password.value })
    const redirect = route.query.redirect as string | undefined
    router.push(redirect ?? (auth.isAdmin ? '/admin' : '/'))
  } catch (e) {
    error.value = apiErrorMessage(e, t('auth.loginFailed'))
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div class="mx-auto max-w-sm py-6">
    <header class="mb-8">
      <p class="u-label mb-3 text-brand">{{ t('common.brand.name') }}</p>
      <h1 class="u-display text-3xl">{{ t('auth.signIn') }}</h1>
      <p class="mt-3 text-sm text-ink-soft">{{ t('auth.signInHelp') }}</p>
    </header>

    <form class="space-y-5" @submit.prevent="submit">
      <AlertMessage v-if="error">{{ error }}</AlertMessage>
      <BaseInput
        v-model="email"
        :label="t('common.fields.email')"
        type="email"
        required
        autocomplete="username"
      />
      <BaseInput
        v-model="password"
        :label="t('common.fields.password')"
        type="password"
        required
        autocomplete="current-password"
      />
      <BaseButton type="submit" :loading="submitting" class="w-full">
        {{ t('auth.signIn') }}
      </BaseButton>
    </form>

    <p class="mt-8 border-t border-line pt-6 text-sm text-ink-soft">
      {{ t('auth.noAccount') }}
      <RouterLink to="/register" class="font-medium text-brand underline underline-offset-4">
        {{ t('auth.register') }}
      </RouterLink>
    </p>
  </div>
</template>
