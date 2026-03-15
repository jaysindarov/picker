<script setup>
import { ref, computed } from 'vue'
import { moviesApi, booksApi, foodsApi, genresApi, cuisinesApi } from '@/api'
import { useRouter } from 'vue-router'

const router = useRouter()

const category = ref('movies') // movies | books | foods
const genreId  = ref('')
const cuisineId= ref('')
const genres   = ref([])
const cuisines = ref([])

const picked   = ref(null)
const spinning = ref(false)
const showResult = ref(false)
const particles  = ref([])

const categoryConfig = {
  movies: { label: 'Movie',  icon: '🎬', color: 'from-blue-600 to-violet-600',   shadow: 'shadow-blue-500/30' },
  books:  { label: 'Book',   icon: '📚', color: 'from-emerald-600 to-teal-600',  shadow: 'shadow-emerald-500/30' },
  foods:  { label: 'Food',   icon: '🍜', color: 'from-orange-600 to-pink-600',   shadow: 'shadow-orange-500/30' },
}

const cfg = computed(() => categoryConfig[category.value])

async function loadFilters() {
  const [g, c] = await Promise.all([genresApi.getAll(), cuisinesApi.getAll()])
  genres.value   = g.data
  cuisines.value = c.data
}

async function pickRandom() {
  if (spinning.value) return
  spinning.value  = true
  showResult.value = false
  picked.value    = null

  try {
    let data
    if (category.value === 'movies') {
      data = (await moviesApi.getRandom(genreId.value || undefined)).data
    } else if (category.value === 'books') {
      data = (await booksApi.getRandom(genreId.value || undefined)).data
    } else {
      data = (await foodsApi.getRandom(cuisineId.value || undefined)).data
    }

    // Simulate suspense animation
    await new Promise(r => setTimeout(r, 1800))
    picked.value = data

    // Spawn confetti particles
    particles.value = Array.from({ length: 20 }, (_, i) => ({
      id: i,
      x: Math.random() * 100,
      color: ['#8b5cf6','#ec4899','#f59e0b','#10b981','#3b82f6'][i % 5],
      delay: Math.random() * 0.5,
      size: 6 + Math.random() * 8,
    }))

    showResult.value = true
    setTimeout(() => { particles.value = [] }, 2000)
  } catch (e) {
    picked.value = { title: 'Nothing found', description: 'Try a different category or filter.' }
    showResult.value = true
  } finally {
    spinning.value = false
  }
}

function goToDetail() {
  if (!picked.value?.id) return
  const singular = { movies: 'movie', books: 'book', foods: 'food' }
  router.push(`/item/${singular[category.value]}/${picked.value.id}`)
}

loadFilters()
</script>

<template>
  <div class="min-h-screen flex flex-col items-center justify-center p-4 relative overflow-hidden">
    <!-- BG blobs -->
    <div class="absolute inset-0 pointer-events-none overflow-hidden">
      <div class="absolute top-1/4 left-1/4 w-96 h-96 rounded-full bg-violet-600/10 blur-3xl" />
      <div class="absolute bottom-1/4 right-1/4 w-96 h-96 rounded-full bg-pink-600/10 blur-3xl" />
    </div>

    <!-- Confetti -->
    <TransitionGroup name="confetti">
      <div v-for="p in particles" :key="p.id" class="absolute pointer-events-none"
        :style="{ left: p.x+'%', top: '-20px', '--color': p.color, '--delay': p.delay+'s', '--size': p.size+'px', animation: `fall 1.5s ${p.delay}s ease-in forwards` }">
        <div class="rounded-sm" :style="{ width: 'var(--size)', height: 'var(--size)', background: 'var(--color)' }" />
      </div>
    </TransitionGroup>

    <div class="relative z-10 w-full max-w-lg text-center">
      <h1 class="text-4xl font-black text-white mb-2">🎲 Pick for me!</h1>
      <p class="text-white/40 mb-10">Can't decide? Let fate choose.</p>

      <!-- Category selector -->
      <div class="glass rounded-2xl p-1.5 flex gap-1 mb-6">
        <button v-for="(v, k) in categoryConfig" :key="k"
          @click="category = k; picked = null; showResult = false"
          class="flex-1 py-3 rounded-xl text-sm font-semibold transition-all duration-200"
          :class="category === k
            ? `bg-gradient-to-r ${v.color} text-white shadow-md`
            : 'text-white/50 hover:text-white/80'">
          {{ v.icon }} {{ v.label }}
        </button>
      </div>

      <!-- Filter -->
      <div class="glass rounded-2xl p-4 mb-8">
        <label class="block text-xs text-white/40 mb-2 uppercase tracking-wider font-medium">
          Filter by {{ category === 'foods' ? 'cuisine' : 'genre' }} (optional)
        </label>
        <select v-if="category !== 'foods'" v-model="genreId" class="input">
          <option value="">Any genre</option>
          <option v-for="g in genres" :key="g.id" :value="g.id">{{ g.name }}</option>
        </select>
        <select v-else v-model="cuisineId" class="input">
          <option value="">Any cuisine</option>
          <option v-for="c in cuisines" :key="c.id" :value="c.id">{{ c.name }}</option>
        </select>
      </div>

      <!-- Pick button -->
      <button @click="pickRandom" :disabled="spinning"
        class="relative w-40 h-40 mx-auto rounded-full flex flex-col items-center justify-center cursor-pointer transition-all duration-300 disabled:cursor-not-allowed"
        :class="`bg-gradient-to-br ${cfg.color} shadow-2xl ${cfg.shadow} hover:scale-105 active:scale-95`">
        <div class="absolute inset-0 rounded-full" :class="spinning ? 'animate-spin_slow ring-4 ring-white/20' : ''" />
        <span class="text-5xl" :class="spinning ? 'animate-spin_slow' : ''">{{ spinning ? '✨' : '🎲' }}</span>
        <span class="text-white font-bold text-sm mt-1">{{ spinning ? 'Picking...' : 'Pick!' }}</span>
      </button>

      <!-- Result -->
      <Transition name="result">
        <div v-if="showResult && picked" class="mt-10 glass rounded-2xl overflow-hidden animate-glow">
          <div class="relative">
            <div v-if="picked.imageUrl" class="aspect-video overflow-hidden">
              <img :src="picked.imageUrl" :alt="picked.title" class="w-full h-full object-cover" @error="(e)=>e.target.parentElement.style.display='none'" />
              <div class="absolute inset-0 bg-gradient-to-t from-black/90 via-black/30 to-transparent" />
            </div>
            <div v-else class="aspect-video flex items-center justify-center text-8xl bg-gradient-to-br from-violet-900/50 to-purple-900/50">
              {{ cfg.icon }}
            </div>
            <div class="absolute bottom-0 left-0 right-0 p-6">
              <div class="flex items-center gap-2 mb-2">
                <span class="badge" :class="`bg-gradient-to-r ${cfg.color} text-white`">✨ Your pick!</span>
                <span v-if="picked.genreName || picked.cuisineName" class="badge bg-white/10 text-white/60">
                  {{ picked.genreName || picked.cuisineName }}
                </span>
              </div>
              <h2 class="text-2xl font-black text-white leading-tight">{{ picked.title }}</h2>
              <p v-if="picked.description" class="text-white/60 text-sm mt-2 line-clamp-2">{{ picked.description }}</p>
              <div v-if="picked.averageRating" class="flex items-center gap-2 mt-3">
                <span class="text-amber-400">★</span>
                <span class="text-white font-semibold">{{ picked.averageRating.toFixed(1) }}</span>
                <span class="text-white/40 text-sm">/ 5</span>
              </div>
            </div>
          </div>
          <div class="p-4 flex gap-3">
            <button @click="goToDetail" class="btn-primary flex-1">View details →</button>
            <button @click="pickRandom" class="btn-secondary flex-1">Pick again 🎲</button>
          </div>
        </div>
      </Transition>
    </div>
  </div>
</template>

<style scoped>
.result-enter-active { transition: all 0.5s cubic-bezier(0.34, 1.56, 0.64, 1); }
.result-enter-from   { opacity: 0; transform: translateY(30px) scale(0.9); }

@keyframes fall {
  0%   { transform: translateY(0) rotate(0deg); opacity: 1; }
  100% { transform: translateY(100vh) rotate(360deg); opacity: 0; }
}
</style>
