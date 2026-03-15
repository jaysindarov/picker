<script setup>
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth   = useAuthStore()
const router = useRouter()
const route  = useRoute()
const menuOpen = ref(false)

const navLinks = [
  { to: '/movies', label: 'Movies',  icon: '🎬' },
  { to: '/books',  label: 'Books',   icon: '📚' },
  { to: '/foods',  label: 'Foods',   icon: '🍜' },
  { to: '/pick',   label: 'Pick for me!', icon: '🎲', highlight: true },
]

function logout() {
  auth.logout()
  router.push('/auth')
}
</script>

<template>
  <nav class="glass-dark sticky top-0 z-50 border-b border-white/[0.06]">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 flex items-center justify-between h-16">
      <!-- Logo -->
      <RouterLink to="/movies" class="flex items-center gap-2 group">
        <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-violet-500 to-purple-700 flex items-center justify-center text-sm font-black group-hover:scale-105 transition-transform">P</div>
        <span class="font-bold text-lg tracking-tight text-white">Picker</span>
      </RouterLink>

      <!-- Desktop nav -->
      <div class="hidden md:flex items-center gap-1">
        <RouterLink
          v-for="link in navLinks" :key="link.to" :to="link.to"
          class="flex items-center gap-1.5 px-4 py-2 rounded-xl text-sm font-medium transition-all duration-200"
          :class="link.highlight
            ? 'bg-gradient-to-r from-violet-600/30 to-purple-600/30 border border-violet-500/30 text-violet-300 hover:from-violet-600/50 hover:to-purple-600/50'
            : route.path === link.to
              ? 'bg-white/10 text-white'
              : 'text-white/60 hover:text-white hover:bg-white/5'"
        >
          <span>{{ link.icon }}</span>{{ link.label }}
        </RouterLink>
        <RouterLink v-if="auth.isAdmin" to="/admin"
          class="flex items-center gap-1.5 px-4 py-2 rounded-xl text-sm font-medium transition-all duration-200 text-amber-400/80 hover:text-amber-400 hover:bg-amber-500/10"
          :class="route.path === '/admin' ? 'bg-amber-500/10 text-amber-400' : ''">
          ⚙️ Admin
        </RouterLink>
      </div>

      <!-- User menu -->
      <div class="relative">
        <button @click="menuOpen = !menuOpen" class="flex items-center gap-2 px-3 py-1.5 rounded-xl glass hover:bg-white/10 transition-all duration-200">
          <div class="w-7 h-7 rounded-lg bg-gradient-to-br from-violet-500 to-pink-500 flex items-center justify-center text-xs font-bold">
            {{ (auth.user?.displayName || auth.user?.email || 'U')[0].toUpperCase() }}
          </div>
          <span class="hidden sm:block text-sm text-white/80 max-w-[120px] truncate">{{ auth.user?.displayName || auth.user?.email }}</span>
          <svg class="w-3.5 h-3.5 text-white/40 transition-transform" :class="menuOpen ? 'rotate-180' : ''" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/></svg>
        </button>

        <Transition name="dropdown">
          <div v-if="menuOpen" @click.outside="menuOpen = false" class="absolute right-0 top-12 w-52 glass-dark rounded-2xl border border-white/10 shadow-xl overflow-hidden z-50">
            <div class="px-4 py-3 border-b border-white/10">
              <p class="text-xs text-white/40 font-medium uppercase tracking-wider">Signed in as</p>
              <p class="text-sm text-white font-medium truncate">{{ auth.user?.email }}</p>
              <span class="mt-1 inline-block text-xs px-2 py-0.5 rounded-md font-semibold"
                :class="auth.isAdmin ? 'bg-amber-500/20 text-amber-400' : 'bg-violet-500/20 text-violet-400'">
                {{ auth.user?.role }}
              </span>
            </div>
            <button @click="logout(); menuOpen = false"
              class="w-full text-left px-4 py-3 text-sm text-red-400 hover:bg-red-500/10 transition-colors flex items-center gap-2">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"/></svg>
              Sign out
            </button>
          </div>
        </Transition>
      </div>
    </div>

    <!-- Mobile nav -->
    <div class="md:hidden flex items-center gap-1 px-4 pb-3 overflow-x-auto">
      <RouterLink v-for="link in navLinks" :key="link.to" :to="link.to"
        class="flex-shrink-0 flex items-center gap-1 px-3 py-1.5 rounded-lg text-xs font-medium transition-all"
        :class="link.highlight
          ? 'bg-violet-600/30 text-violet-300 border border-violet-500/30'
          : route.path === link.to ? 'bg-white/10 text-white' : 'text-white/50 hover:text-white'">
        {{ link.icon }} {{ link.label }}
      </RouterLink>
      <RouterLink v-if="auth.isAdmin" to="/admin"
        class="flex-shrink-0 flex items-center gap-1 px-3 py-1.5 rounded-lg text-xs font-medium text-amber-400/80">
        ⚙️ Admin
      </RouterLink>
    </div>
  </nav>
</template>

<style scoped>
.dropdown-enter-active, .dropdown-leave-active { transition: all 0.15s ease; }
.dropdown-enter-from, .dropdown-leave-to { opacity: 0; transform: translateY(-6px) scale(0.97); }
</style>
