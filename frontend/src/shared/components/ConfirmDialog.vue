<script setup lang="ts">
import { useI18n } from 'vue-i18n'
import BaseModal from './BaseModal.vue'
import BaseButton from './BaseButton.vue'
import { useConfirmHost } from '../composables/useConfirm'

const { t } = useI18n()
const { request } = useConfirmHost()
</script>

<template>
  <!-- Mounted once, in App.vue. Nothing renders until someone calls confirm(),
       and BaseModal is remounted per question, so its focus and scroll-lock
       setup runs each time. -->
  <BaseModal
    v-if="request"
    :title="request.title ?? t('common.confirmDialog.title')"
    @close="request?.settle(false)"
  >
    <p class="text-ink-soft">{{ request.message }}</p>

    <div class="mt-6 flex justify-end gap-2 border-t border-line pt-5">
      <BaseButton variant="secondary" @click="request?.settle(false)">
        {{ request.dismissLabel ?? t('common.confirmDialog.dismiss') }}
      </BaseButton>
      <BaseButton :variant="request.variant ?? 'danger'" @click="request?.settle(true)">
        {{ request.confirmLabel ?? t('common.actions.confirm') }}
      </BaseButton>
    </div>
  </BaseModal>
</template>
