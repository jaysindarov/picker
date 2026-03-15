<script setup>
import { ref, computed, onMounted } from 'vue'
import { moviesApi, genresApi } from '@/api'
import { useAuthStore } from '@/stores/auth'
import ItemCard from '@/components/ItemCard.vue'
import ItemFormModal from '@/components/ItemFormModal.vue'
import ConfirmModal from '@/components/ConfirmModal.vue'

const auth    = useAuthStore()
const items   = ref([])
const genres  = ref([])
const loading = ref(false)
const search  = ref('')
const filterGenreId = ref('')

const showForm    = ref(false)
const editTarget  = ref(null)
const showConfirm = ref(false)
const deleteTarget= ref(null)

const filtered = computed(() => {
  let list = items.value
  if (filterGenreId.value) list = list.filter(i => i.genreId === filterGenreId.value)
  if (search.value) {
    const q = search.value.toLowerCase()
    list = list.filter(i => i.title.toLowerCase().includes(q) || i.description?.toLowerCase().includes(q))
  }
  return list
})

async function load() {
  loading.value = true
  try {
    const [itemsRes, genresRes] = await Promise.all([moviesApi.getAll(), genresApi.getAll()])
    items.value  = itemsRes.data
    genres.value = genresRes.data
  } finally {
    loading.value = false
  }
}

async function saveItem(payload) {
  if (editTarget.value) {
    await moviesApi.update(editTarget.value.id, payload)
  } else {
    await moviesApi.create(payload)
  }
  showForm.value = false
  editTarget.value = null
  await load()
}

async function confirmDelete() {
  await moviesApi.delete(deleteTarget.value.id)
  showConfirm.value = false
  deleteTarget.value = null
  await load()
}

function openEdit(item)   { editTarget.value = item; showForm.value = true }
function openDelete(item) { deleteTarget.value = item; showConfirm.value = true }

onMounted(load)
</script>

<template>
  <div class="max-w-7xl mx-auto px-4 sm:px-6 py-8">
    <!-- Header -->
    <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-8">
      <div>
        <h1 class="text-2xl font-bold text-white flex items-center gap-2">🎬 Movies</h1>
        <p class="text-white/40 text-sm mt-1">{{ items.length }} titles available</p>
      </div>
      <button v-if="auth.isAdmin" @click="editTarget = null; showForm = true" class="btn-primary flex items-center gap-2">
        <span class="text-lg">+</span> Add movie
      </button>
    </div>

    <!-- Filters -->
    <div class="flex flex-col sm:flex-row gap-3 mb-6">
      <div class="relative flex-1">
        <span class="absolute left-3 top-1/2 -translate-y-1/2 text-white/30">🔍</span>
        <input v-model="search" class="input pl-9" placeholder="Search movies..." />
      </div>
      <select v-model="filterGenreId" class="input sm:w-48">
        <option value="">All genres</option>
        <option v-for="g in genres" :key="g.id" :value="g.id">{{ g.name }}</option>
      </select>
    </div>

    <!-- Grid -->
    <div v-if="loading" class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6 gap-4">
      <div v-for="i in 12" :key="i" class="glass rounded-2xl overflow-hidden animate-shimmer">
        <div class="aspect-[2/3] bg-white/5" />
        <div class="p-3"><div class="h-3 bg-white/10 rounded w-3/4 mb-2" /><div class="h-2 bg-white/5 rounded w-1/2" /></div>
      </div>
    </div>

    <div v-else-if="filtered.length" class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 xl:grid-cols-6 gap-4">
      <div v-for="item in filtered" :key="item.id" class="relative group">
        <ItemCard :item="item" type="movies" />
        <div v-if="auth.isAdmin" class="absolute top-2 left-2 hidden group-hover:flex gap-1 z-10">
          <button @click.stop="openEdit(item)"   class="p-1.5 rounded-lg bg-black/70 backdrop-blur-sm text-xs hover:bg-white/20 transition-colors">✏️</button>
          <button @click.stop="openDelete(item)" class="p-1.5 rounded-lg bg-black/70 backdrop-blur-sm text-xs hover:bg-red-500/40 transition-colors">🗑️</button>
        </div>
      </div>
    </div>

    <div v-else class="text-center py-20">
      <p class="text-6xl mb-4">🎬</p>
      <p class="text-white/40 text-lg">No movies found</p>
      <p v-if="auth.isAdmin" class="text-white/25 text-sm mt-2">Add your first movie to get started</p>
    </div>

    <ItemFormModal
      :show="showForm"
      type="movies"
      :item="editTarget"
      :genres="genres"
      @close="showForm = false"
      @save="saveItem"
    />
    <ConfirmModal
      :show="showConfirm"
      :message="`Delete '${deleteTarget?.title}'? This action cannot be undone.`"
      @confirm="confirmDelete"
      @cancel="showConfirm = false"
    />
  </div>
</template>
