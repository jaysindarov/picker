<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { moviesApi, booksApi, foodsApi, ratingsApi } from '@/api'
import { useAuthStore } from '@/stores/auth'
import StarRating from '@/components/StarRating.vue'
import CommentSection from '@/components/CommentSection.vue'

const route  = useRoute()
const router = useRouter()
const auth   = useAuthStore()

const item      = ref(null)
const loading   = ref(true)
const myRating  = ref(0)
const ratingBusy= ref(false)
const ratingMsg = ref('')

// CategoryType enum: Food=1, Movie=2, Book=3
const typeMap = { movie: { api: moviesApi, cat: 2 }, book: { api: booksApi, cat: 3 }, food: { api: foodsApi, cat: 1 } }
const typeKey = route.params.type
const cfg     = typeMap[typeKey]

const typeLabel = { movie: '🎬 Movie', book: '📚 Book', food: '🍜 Food' }[typeKey] || ''
const listRoute = { movie: '/movies', book: '/books', food: '/foods' }[typeKey] || '/'

const avgStars = computed(() => {
  const v = item.value?.averageRating
  return v ? Math.round(v) : 0
})

async function loadItem() {
  loading.value = true
  try {
    item.value = (await cfg.api.getById(route.params.id)).data
    try {
      const r = await ratingsApi.getMyRating(route.params.id, cfg.cat)
      myRating.value = r.data.value
    } catch { myRating.value = 0 }
  } finally {
    loading.value = false
  }
}

async function submitRating(val) {
  if (ratingBusy.value) return
  ratingBusy.value = true
  ratingMsg.value  = ''
  try {
    await ratingsApi.upsert({ categoryType: cfg.cat, itemId: route.params.id, value: val })
    myRating.value = val
    ratingMsg.value = '✓ Rating saved!'
    await loadItem()
    setTimeout(() => { ratingMsg.value = '' }, 2000)
  } finally {
    ratingBusy.value = false
  }
}

onMounted(loadItem)
</script>

<template>
  <div class="max-w-4xl mx-auto px-4 sm:px-6 py-8">
    <!-- Back -->
    <button @click="router.push(listRoute)" class="flex items-center gap-2 text-white/40 hover:text-white text-sm mb-6 transition-colors group">
      <span class="group-hover:-translate-x-1 transition-transform">←</span>
      Back to {{ typeKey }}s
    </button>

    <!-- Loading skeleton -->
    <div v-if="loading" class="animate-shimmer">
      <div class="h-72 glass rounded-2xl mb-6" />
      <div class="h-8 bg-white/10 rounded w-3/4 mb-3" />
      <div class="h-4 bg-white/5 rounded w-full mb-2" />
      <div class="h-4 bg-white/5 rounded w-2/3" />
    </div>

    <template v-else-if="item">
      <!-- Hero -->
      <div class="relative glass rounded-2xl overflow-hidden mb-8">
        <div class="absolute inset-0">
          <img v-if="item.imageUrl" :src="item.imageUrl" :alt="item.title" class="w-full h-full object-cover opacity-30 blur-sm scale-105" @error="(e)=>e.target.style.display='none'" />
          <div class="absolute inset-0 bg-gradient-to-r from-black/80 via-black/60 to-transparent" />
        </div>
        <div class="relative flex flex-col sm:flex-row gap-6 p-6 sm:p-8">
          <!-- Poster -->
          <div class="flex-shrink-0 w-36 sm:w-44 mx-auto sm:mx-0">
            <div class="rounded-xl overflow-hidden shadow-2xl aspect-[2/3] bg-white/5">
              <img v-if="item.imageUrl" :src="item.imageUrl" :alt="item.title" class="w-full h-full object-cover" @error="(e)=>e.target.style.display='none'" />
              <div v-else class="w-full h-full flex items-center justify-center text-5xl opacity-30">
                {{ { movie:'🎬', book:'📚', food:'🍜' }[typeKey] }}
              </div>
            </div>
          </div>
          <!-- Info -->
          <div class="flex-1 text-center sm:text-left">
            <div class="flex items-center justify-center sm:justify-start gap-2 mb-3">
              <span class="badge bg-white/15 text-white/70 text-xs">{{ typeLabel }}</span>
              <span v-if="item.genreName || item.cuisineName" class="badge bg-violet-500/20 text-violet-300 text-xs">
                {{ item.genreName || item.cuisineName }}
              </span>
            </div>
            <h1 class="text-3xl sm:text-4xl font-black text-white mb-3 leading-tight">{{ item.title }}</h1>
            <p v-if="item.description" class="text-white/60 leading-relaxed mb-4">{{ item.description }}</p>

            <!-- Rating display -->
            <div v-if="item.averageRating" class="flex items-center gap-3 justify-center sm:justify-start mb-4">
              <div class="flex gap-0.5">
                <span v-for="s in 5" :key="s" class="text-xl" :class="s <= avgStars ? 'text-amber-400' : 'text-white/15'">★</span>
              </div>
              <span class="text-white font-bold">{{ item.averageRating.toFixed(1) }}</span>
              <span class="text-white/40 text-sm">{{ item.totalRatings }} ratings</span>
            </div>

            <!-- User rating -->
            <div class="mt-4">
              <p class="text-xs text-white/40 mb-2 uppercase tracking-wider font-medium">Your rating</p>
              <div class="flex items-center gap-3">
                <StarRating :model-value="myRating" @update:model-value="submitRating" />
                <Transition name="fade">
                  <span v-if="ratingMsg" class="text-xs text-emerald-400">{{ ratingMsg }}</span>
                </Transition>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Comments -->
      <div class="glass rounded-2xl p-6">
        <CommentSection :item-id="item.id" :category-type="cfg.cat" />
      </div>
    </template>

    <div v-else class="text-center py-20">
      <p class="text-6xl mb-4">🔍</p>
      <p class="text-white/40 text-lg">Item not found</p>
    </div>
  </div>
</template>

<style scoped>
.fade-enter-active, .fade-leave-active { transition: opacity 0.3s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
</style>
