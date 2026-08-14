<script setup lang="ts">
import { onBeforeUnmount, onMounted, ref, useId } from "vue";
import { useI18n } from "vue-i18n";

defineProps<{ title: string }>();
const emit = defineEmits<{ close: [] }>();

const { t } = useI18n();

const panel = ref<HTMLElement | null>(null);
const titleId = useId();

function onKeydown(event: KeyboardEvent): void {
  if (event.key === "Escape") emit("close");
}

// Escape closes, the panel takes focus on open, and the page behind it stops
// scrolling — the parts a dialog needs to not feel broken.
onMounted(() => {
  document.addEventListener("keydown", onKeydown);
  document.body.style.overflow = "hidden";
  panel.value?.focus();
});

onBeforeUnmount(() => {
  document.removeEventListener("keydown", onKeydown);
  document.body.style.overflow = "";
});
</script>

<template>
  <!-- A phone puts the keyboard over the bottom half of the screen, so a
       centred panel ends up half-covered. Below sm it docks to the bottom edge
       as a sheet instead, within thumb reach and above the keyboard. -->
  <div
    class="fixed inset-0 z-50 flex items-end justify-center bg-ink/45 backdrop-blur-[2px] sm:items-center sm:p-4"
    @click.self="emit('close')"
  >
    <div
      ref="panel"
      role="dialog"
      aria-modal="true"
      :aria-labelledby="titleId"
      tabindex="-1"
      class="u-safe-bottom max-h-[88dvh] w-full overflow-y-auto rounded-t-lg border border-line bg-surface px-5 pt-5 outline-none sm:max-h-[85dvh] sm:max-w-md sm:rounded-sm sm:p-6"
    >
      <div class="mb-5 flex items-start justify-between gap-4 border-b border-line pb-4">
        <h2 :id="titleId" class="u-display text-xl">{{ title }}</h2>
        <button
          class="-me-1 -mt-1 rounded-sm p-1 text-ink-faint transition-colors hover:text-ink"
          :aria-label="t('common.actions.close')"
          @click="emit('close')"
        >
          <svg class="h-4 w-4" viewBox="0 0 16 16" fill="none" aria-hidden="true">
            <path
              d="M3 3l10 10M13 3L3 13"
              stroke="currentColor"
              stroke-width="1.5"
              stroke-linecap="round"
            />
          </svg>
        </button>
      </div>
      <slot />
    </div>
  </div>
</template>
