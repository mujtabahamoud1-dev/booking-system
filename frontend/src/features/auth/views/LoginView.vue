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
  <div class="mx-auto max-w-sm">
    <h1 class="mb-6 text-2xl font-bold text-slate-900">{{ t('auth.signIn') }}</h1>
    <form class="space-y-4" @submit.prevent="submit">
      <AlertMessage v-if="error">{{ error }}</AlertMessage>
      <BaseInput
        v-model="email"
        :label="t('common.fields.email')"
        type="email"
        required
        autocomplete="username"
      />
      <BaseInput v-model="password" :label="t('common.fields.password')" type="password" required />
      <BaseButton type="submit" :loading="submitting" class="w-full">{{
        t('auth.signIn')
      }}</BaseButton>
    </form>
    <p class="mt-4 text-center text-sm text-slate-500">
      {{ t('auth.noAccount') }}
      <RouterLink to="/register" class="font-medium text-indigo-600 hover:text-indigo-500">{{
        t('auth.register')
      }}</RouterLink>
    </p>
  </div>
</template>
