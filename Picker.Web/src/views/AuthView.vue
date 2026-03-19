<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const auth   = useAuthStore()
const router = useRouter()

const mode      = ref('login') // 'login' | 'register'
const email     = ref('')
const password  = ref('')
const username  = ref('')
const firstName = ref('')
const lastName  = ref('')
const error     = ref('')
const loading   = ref(false)

async function submit() {
  error.value   = ''
  loading.value = true
  try {
    if (mode.value === 'login') {
      await auth.login(username.value, password.value)
    } else {
      await auth.register(email.value, password.value, username.value, firstName.value, lastName.value)
    }
    router.push('/movies')
  } catch (e) {
    error.value = e.response?.data?.error || e.response?.data || 'Something went wrong.'
  } finally {
    loading.value = false
  }
}

function switchMode() {
  mode.value = mode.value === 'login' ? 'register' : 'login'
  error.value = ''
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center p-4 page-bg">
    <!-- Background decoration -->
    <div class="absolute inset-0 overflow-hidden pointer-events-none">
      <div class="absolute -top-40 -left-40 w-96 h-96 rounded-full bg-violet-600/10 blur-3xl" />
      <div class="absolute -bottom-40 -right-40 w-96 h-96 rounded-full bg-purple-600/10 blur-3xl" />
    </div>

    <div class="relative w-full max-w-md">
      <!-- Logo -->
      <div class="text-center mb-8">
        <div class="inline-flex items-center justify-center w-16 h-16 rounded-2xl bg-gradient-to-br from-violet-500 to-purple-700 text-3xl font-black mb-4 animate-float shadow-lg shadow-violet-500/30">P</div>
        <h1 class="text-3xl font-bold text-white">Picker</h1>
        <p class="text-white/40 mt-1 text-sm">Discover what to watch, read or eat</p>
      </div>

      <!-- Card -->
      <div class="glass rounded-2xl p-8 shadow-2xl">
        <!-- Tabs -->
        <div class="flex rounded-xl bg-white/5 p-1 mb-6">
          <button @click="switchMode" v-if="mode !== 'login'"
            class="flex-1 py-2 text-sm font-medium rounded-lg text-white/50 hover:text-white/80 transition-colors">
            Sign In
          </button>
          <button v-else class="flex-1 py-2 text-sm font-semibold rounded-lg bg-white/10 text-white transition-colors">
            Sign In
          </button>
          <button @click="switchMode" v-if="mode !== 'register'"
            class="flex-1 py-2 text-sm font-medium rounded-lg text-white/50 hover:text-white/80 transition-colors">
            Create Account
          </button>
          <button v-else class="flex-1 py-2 text-sm font-semibold rounded-lg bg-white/10 text-white transition-colors">
            Create Account
          </button>
        </div>

        <form @submit.prevent="submit" class="space-y-4">
          <div v-if="mode === 'register'" class="grid grid-cols-2 gap-3">
            <div>
              <label class="block text-xs text-white/50 mb-1.5 font-medium">First name</label>
              <input v-model="firstName" type="text" class="input" placeholder="Jane" required />
            </div>
            <div>
              <label class="block text-xs text-white/50 mb-1.5 font-medium">Last name</label>
              <input v-model="lastName" type="text" class="input" placeholder="Doe" required />
            </div>
          </div>
          <div v-if="mode === 'register'">
            <label class="block text-xs text-white/50 mb-1.5 font-medium">Email address</label>
            <input v-model="email" type="email" class="input" placeholder="you@example.com" required />
          </div>
          <div>
            <label class="block text-xs text-white/50 mb-1.5 font-medium">Username</label>
            <input v-model="username" type="text" class="input" placeholder="janedoe" required />
          </div>
          <div>
            <label class="block text-xs text-white/50 mb-1.5 font-medium">Password</label>
            <input v-model="password" type="password" class="input" placeholder="••••••••" required minlength="6" />
          </div>

          <Transition name="slide">
            <div v-if="error" class="bg-red-500/10 border border-red-500/20 rounded-xl px-4 py-3 text-sm text-red-400">
              {{ error }}
            </div>
          </Transition>

          <button type="submit" :disabled="loading" class="btn-primary w-full py-3 disabled:opacity-50 disabled:cursor-not-allowed mt-2">
            <span v-if="loading">{{ mode === 'login' ? 'Signing in...' : 'Creating account...' }}</span>
            <span v-else>{{ mode === 'login' ? 'Sign in' : 'Create account' }}</span>
          </button>
        </form>
      </div>

      <!-- Demo hint -->
      <p class="text-center text-white/25 text-xs mt-4">
        🎬 Movies · 📚 Books · 🍜 Foods · 🎲 Random picks
      </p>
    </div>
  </div>
</template>

<style scoped>
.slide-enter-active, .slide-leave-active { transition: all 0.2s ease; }
.slide-enter-from, .slide-leave-to { opacity: 0; transform: translateY(-6px); }
</style>
