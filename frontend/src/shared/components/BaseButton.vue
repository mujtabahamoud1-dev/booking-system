<script setup lang="ts">
withDefaults(
  defineProps<{
    variant?: 'primary' | 'secondary' | 'danger' | 'ghost'
    type?: 'button' | 'submit'
    disabled?: boolean
    loading?: boolean
  }>(),
  { variant: 'primary', type: 'button', disabled: false, loading: false },
)

// Red is reserved for destruction, so `danger` is the only variant that gets it
// and `primary` never has to compete with it for attention.
const styles: Record<string, string> = {
  primary: 'bg-brand text-surface hover:bg-brand-deep',
  secondary: 'bg-surface text-ink ring-1 ring-inset ring-line hover:bg-ground',
  danger: 'bg-alert text-surface hover:bg-alert/90',
  ghost: 'text-ink-soft hover:bg-ground hover:text-ink',
}
</script>

<template>
  <button
    :type="type"
    :disabled="disabled || loading"
    class="u-action u-label gap-2 rounded-sm px-4 py-2.5 transition-colors disabled:cursor-not-allowed disabled:opacity-45"
    :class="styles[variant]"
  >
    <svg
      v-if="loading"
      class="h-3.5 w-3.5 animate-spin"
      viewBox="0 0 24 24"
      fill="none"
      aria-hidden="true"
    >
      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z" />
    </svg>
    <slot />
  </button>
</template>
