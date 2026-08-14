<script setup lang="ts">
import { useI18n } from "vue-i18n";

defineProps<{ label?: string }>();

const { t } = useI18n();
</script>

<template>
  <!-- An indeterminate bar rather than a spinner: it echoes the week track, so
       loading looks like part of the same system. -->
  <div class="flex flex-col items-center gap-3 py-14" role="status">
    <div class="track h-1 w-40 overflow-hidden rounded-full bg-line" aria-hidden="true">
      <div class="bar h-full w-1/3 rounded-full bg-brand"></div>
    </div>
    <span class="u-label text-ink-faint">{{ label ?? t("common.loading") }}</span>
  </div>
</template>

<style scoped>
@keyframes sweep {
  0% {
    transform: translateX(-100%);
  }
  100% {
    transform: translateX(300%);
  }
}

.bar {
  animation: sweep 1.1s cubic-bezier(0.4, 0, 0.2, 1) infinite;
}

@media (prefers-reduced-motion: reduce) {
  .bar {
    animation: none;
    width: 100%;
    opacity: 0.5;
  }
}
</style>
