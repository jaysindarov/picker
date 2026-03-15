<script setup>
import { useRouter } from 'vue-router'

const props = defineProps({
  item:     { type: Object, required: true },
  type:     { type: String, required: true }, // 'movies' | 'books' | 'foods'
  featured: { type: Boolean, default: false },
})

const router = useRouter()

const typeConfig = {
  movies: { color: 'blue',   icon: '🎬', accent: 'from-blue-500/20 to-blue-600/10',   badge: 'bg-blue-500/20 text-blue-400',   border: 'hover:border-blue-500/30' },
  books:  { color: 'emerald',icon: '📚', accent: 'from-emerald-500/20 to-emerald-600/10', badge: 'bg-emerald-500/20 text-emerald-400', border: 'hover:border-emerald-500/30' },
  foods:  { color: 'orange', icon: '🍜', accent: 'from-orange-500/20 to-orange-600/10', badge: 'bg-orange-500/20 text-orange-400', border: 'hover:border-orange-500/30' },
}

const cfg = typeConfig[props.type] || typeConfig.movies

function navigate() {
  const singular = { movies: 'movie', books: 'book', foods: 'food' }
  router.push(`/item/${singular[props.type]}/${props.item.id}`)
}

function rating(val) {
  if (!val) return '—'
  return val.toFixed(1)
}
</script>

<template>
  <div
    @click="navigate"
    class="card cursor-pointer overflow-hidden group animate-pop"
    :class="[cfg.border, featured ? 'ring-2 ring-violet-500/50 shadow-lg shadow-violet-500/20' : '']"
  >
    <!-- Image -->
    <div class="relative aspect-[2/3] overflow-hidden bg-white/5">
      <img v-if="item.imageUrl" :src="item.imageUrl" :alt="item.title"
        class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
        @error="(e) => e.target.style.display='none'" />
      <div v-else class="w-full h-full flex items-center justify-center">
        <span class="text-5xl opacity-30">{{ cfg.icon }}</span>
      </div>
      <!-- Gradient overlay -->
      <div class="absolute inset-0 bg-gradient-to-t from-black/80 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300" />
      <!-- Featured badge -->
      <div v-if="featured" class="absolute top-2 left-2 px-2 py-0.5 rounded-full text-xs font-bold bg-violet-600 text-white animate-glow">
        ✨ Picked!
      </div>
      <!-- Rating badge -->
      <div v-if="item.averageRating" class="absolute top-2 right-2 flex items-center gap-1 px-2 py-1 rounded-lg bg-black/60 backdrop-blur-sm">
        <span class="text-amber-400 text-xs">★</span>
        <span class="text-xs font-bold text-white">{{ rating(item.averageRating) }}</span>
      </div>
    </div>

    <!-- Content -->
    <div class="p-4">
      <div class="flex items-start justify-between gap-2 mb-2">
        <h3 class="font-semibold text-white text-sm leading-tight line-clamp-2 group-hover:text-violet-300 transition-colors">
          {{ item.title }}
        </h3>
      </div>
      <!-- Genre / Cuisine tag -->
      <span v-if="item.genreName || item.cuisineName" class="badge text-xs" :class="cfg.badge">
        {{ item.genreName || item.cuisineName }}
      </span>
      <!-- Description preview -->
      <p v-if="item.description" class="mt-2 text-xs text-white/40 line-clamp-2 leading-relaxed">
        {{ item.description }}
      </p>
      <!-- Rating row -->
      <div class="mt-3 flex items-center gap-3 text-xs text-white/40">
        <span v-if="item.averageRating" class="flex items-center gap-1">
          <span class="text-amber-400">★★★★★</span>
          <span class="font-medium text-white/60">{{ rating(item.averageRating) }}</span>
        </span>
        <span v-if="item.totalRatings">{{ item.totalRatings }} ratings</span>
        <span v-if="item.comments?.length">{{ item.comments.length }} comments</span>
      </div>
    </div>
  </div>
</template>
