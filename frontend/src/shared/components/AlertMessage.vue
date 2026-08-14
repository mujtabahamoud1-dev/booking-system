<script setup lang="ts">
withDefaults(defineProps<{ tone?: "error" | "success" | "info" }>(), { tone: "error" });

// A tone-coloured rule on the leading edge carries the signal, so the message
// body stays quiet enough to read.
const tones: Record<string, string> = {
  error: "bg-alert-soft text-ink border-alert",
  success: "bg-brand-soft text-ink border-brand",
  info: "bg-signal-soft text-ink border-signal",
};
</script>

<template>
  <div class="rounded-sm border-s-2 px-4 py-3 text-sm" :class="tones[tone]" role="alert">
    <slot />
  </div>
</template>
