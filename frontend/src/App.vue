<script setup lang="ts">
import { RouterView } from 'vue-router'
import NavBar from '@/shared/layout/NavBar.vue'
</script>

<template>
  <div class="flex min-h-full flex-col">
    <NavBar />
    <main class="mx-auto w-full max-w-5xl flex-1 px-4 py-8">
      <RouterView />
    </main>
  </div>
</template>
