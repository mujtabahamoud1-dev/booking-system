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
  <div class="mx-auto max-w-sm py-6">
    <header class="mb-8">
      <p class="u-label mb-3 text-brand">{{ t('common.brand.name') }}</p>
      <h1 class="u-display text-3xl">{{ t('auth.createAccount') }}</h1>
      <p class="mt-3 text-sm text-ink-soft">{{ t('auth.createAccountHelp') }}</p>
    </header>

    <form class="space-y-5" @submit.prevent="submit">
      <AlertMessage v-if="error">{{ error }}</AlertMessage>
      <BaseInput
        v-model="name"
        :label="t('common.fields.name')"
        required
        autocomplete="name"
      />
      <BaseInput
        v-model="email"
        :label="t('common.fields.email')"
        type="email"
        required
        autocomplete="email"
      />
      <BaseInput
        v-model="password"
        :label="t('common.fields.password')"
        type="password"
        required
        autocomplete="new-password"
      />
      <BaseInput
        v-model="phone"
        :label="t('common.fields.phone')"
        type="tel"
        autocomplete="tel"
        :placeholder="t('common.fields.optional')"
      />
      <BaseButton type="submit" :loading="submitting" class="w-full">
        {{ t('auth.register') }}
      </BaseButton>
    </form>

    <p class="mt-8 border-t border-line pt-6 text-sm text-ink-soft">
      {{ t('auth.haveAccount') }}
      <RouterLink to="/login" class="font-medium text-brand underline underline-offset-4">
        {{ t('auth.signIn') }}
      </RouterLink>
    </p>
  </div>
</template>
