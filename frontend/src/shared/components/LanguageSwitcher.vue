<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { LOCALES, setLocale, type Locale } from "@/shared/i18n";

const { locale, t } = useI18n();

// A segmented control rather than a toggle: both languages are visible, so you
// can see which one is active instead of inferring it from the button label.
const shortLabels: Record<Locale, string> = { en: "EN", ar: "ع" };
const fullLabels: Record<Locale, string> = { en: "common.english", ar: "common.arabic" };
</script>

<template>
  <div
    class="flex items-center gap-px rounded-sm bg-ground p-px"
    role="group"
    :aria-label="t('common.language')"
  >
    <button
      v-for="option in LOCALES"
      :key="option"
      type="button"
      class="u-label rounded-[3px] px-3 py-2 transition-colors sm:px-2 sm:py-1"
      :class="
        locale === option
          ? 'bg-surface text-ink shadow-[0_1px_2px_rgba(20,35,29,0.08)]'
          : 'text-ink-faint hover:text-ink'
      "
      :aria-pressed="locale === option"
      :lang="option"
      @click="setLocale(option)"
    >
      <span aria-hidden="true">{{ shortLabels[option] }}</span>
      <span class="sr-only">{{ t(fullLabels[option]) }}</span>
    </button>
  </div>
</template>
