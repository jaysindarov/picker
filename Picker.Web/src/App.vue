<script setup>
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import AppNavbar from '@/components/AppNavbar.vue'

const auth = useAuthStore()
onMounted(() => auth.fetchMe())
</script>

<template>
  <div class="page-bg min-h-screen">
    <AppNavbar v-if="auth.isAuthenticated" />
    <RouterView />
  </div>
</template>
