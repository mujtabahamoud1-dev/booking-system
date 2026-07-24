<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, RouterLink } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '../store'
import { apiErrorMessage } from '@/shared/api/client'
import BaseInput from '@/shared/components/BaseInput.vue'
import BaseButton from '@/shared/components/BaseButton.vue'
import AlertMessage from '@/shared/components/AlertMessage.vue'

const auth = useAuthStore()
const router = useRouter()
const { t } = useI18n()

const name = ref('')
const email = ref('')
const password = ref('')
const phone = ref('')
const error = ref<string | null>(null)
const submitting = ref(false)

async function submit(): Promise<void> {
  error.value = null
  submitting.value = true
  try {
    await auth.register({
      name: name.value,
      email: email.value,
      password: password.value,
      phone: phone.value || null,
    })
    // Registration logs the client in; they land on the services list.
    router.push('/')
  } catch (e) {
    error.value = apiErrorMessage(e, t('auth.registerFailed'))
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div class="mx-auto max-w-sm">
    <h1 class="mb-6 text-2xl font-bold text-slate-900">{{ t('auth.createAccount') }}</h1>
    <form class="space-y-4" @submit.prevent="submit">
      <AlertMessage v-if="error">{{ error }}</AlertMessage>
      <BaseInput v-model="name" :label="t('common.fields.name')" required />
      <BaseInput v-model="email" :label="t('common.fields.email')" type="email" required />
      <BaseInput v-model="password" :label="t('common.fields.password')" type="password" required />
      <BaseInput
        v-model="phone"
        :label="t('common.fields.phone')"
        :placeholder="t('common.fields.optional')"
      />
      <BaseButton type="submit" :loading="submitting" class="w-full">{{
        t('auth.register')
      }}</BaseButton>
    </form>
    <p class="mt-4 text-center text-sm text-slate-500">
      {{ t('auth.haveAccount') }}
      <RouterLink to="/login" class="font-medium text-indigo-600 hover:text-indigo-500">{{
        t('auth.signIn')
      }}</RouterLink>
    </p>
  </div>
</template>
