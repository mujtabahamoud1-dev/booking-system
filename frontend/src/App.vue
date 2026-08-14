<script setup lang="ts">
import { RouterView } from "vue-router";
import { useI18n } from "vue-i18n";
import NavBar from "@/shared/layout/NavBar.vue";
import ConfirmDialog from "@/shared/components/ConfirmDialog.vue";

const { t } = useI18n();
</script>

<template>
  <div class="flex min-h-dvh flex-col">
    <NavBar />

    <main class="u-gutter mx-auto w-full max-w-6xl flex-1 py-8 md:py-14">
      <RouterView />
    </main>

    <footer class="border-t border-line">
      <div
        class="u-gutter u-safe-bottom mx-auto flex max-w-6xl flex-wrap items-center justify-between gap-x-3 gap-y-1.5 pt-6 text-xs text-ink-faint"
      >
        <span>{{ t("common.brand.name") }} · {{ t("common.brand.discipline") }}</span>
        <span>{{ t("common.brand.note") }}</span>
      </div>
    </footer>

    <!-- One host for every useConfirm() caller in the app. -->
    <ConfirmDialog />
  </div>
</template>
