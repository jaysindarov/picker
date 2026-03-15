<script setup>
import { ref, onMounted } from 'vue'
import { genresApi, cuisinesApi, adminApi } from '@/api'

const tab = ref('users') // users | genres | cuisines
const tabs = [
  { key: 'users',    label: 'Users',    icon: '👥' },
  { key: 'genres',   label: 'Genres',   icon: '🏷️' },
  { key: 'cuisines', label: 'Cuisines', icon: '🌍' },
]

// Users
const users    = ref([])
const usersLoading = ref(false)

async function loadUsers() {
  usersLoading.value = true
  users.value = (await adminApi.getUsers()).data
  usersLoading.value = false
}

async function assignRole(userId, role) {
  await adminApi.assignRole(userId, role)
  await loadUsers()
}

// Genres
const genres      = ref([])
const newGenre    = ref('')
const editGenreId = ref(null)
const editGenreName= ref('')

async function loadGenres() {
  genres.value = (await genresApi.getAll()).data
}
async function addGenre() {
  if (!newGenre.value.trim()) return
  await genresApi.create({ name: newGenre.value.trim() })
  newGenre.value = ''
  await loadGenres()
}
async function saveGenre(g) {
  await genresApi.update(g.id, { name: editGenreName.value })
  editGenreId.value = null
  await loadGenres()
}
async function deleteGenre(id) {
  await genresApi.delete(id)
  await loadGenres()
}
function startEditGenre(g) { editGenreId.value = g.id; editGenreName.value = g.name }

// Cuisines
const cuisines       = ref([])
const newCuisine     = ref('')
const editCuisineId  = ref(null)
const editCuisineName= ref('')

async function loadCuisines() {
  cuisines.value = (await cuisinesApi.getAll()).data
}
async function addCuisine() {
  if (!newCuisine.value.trim()) return
  await cuisinesApi.create({ name: newCuisine.value.trim() })
  newCuisine.value = ''
  await loadCuisines()
}
async function saveCuisine(c) {
  await cuisinesApi.update(c.id, { name: editCuisineName.value })
  editCuisineId.value = null
  await loadCuisines()
}
async function deleteCuisine(id) {
  await cuisinesApi.delete(id)
  await loadCuisines()
}
function startEditCuisine(c) { editCuisineId.value = c.id; editCuisineName.value = c.name }

function switchTab(key) {
  tab.value = key
  if (key === 'users' && !users.value.length)    loadUsers()
  if (key === 'genres' && !genres.value.length)   loadGenres()
  if (key === 'cuisines' && !cuisines.value.length) loadCuisines()
}

onMounted(() => { loadUsers(); loadGenres(); loadCuisines() })
</script>

<template>
  <div class="max-w-4xl mx-auto px-4 sm:px-6 py-8">
    <div class="mb-8">
      <h1 class="text-2xl font-bold text-white flex items-center gap-2">⚙️ Admin Panel</h1>
      <p class="text-white/40 text-sm mt-1">Manage users, genres, and cuisines</p>
    </div>

    <!-- Tabs -->
    <div class="glass rounded-2xl p-1.5 flex gap-1 mb-6 w-fit">
      <button v-for="t in tabs" :key="t.key" @click="switchTab(t.key)"
        class="px-5 py-2 rounded-xl text-sm font-semibold transition-all duration-200"
        :class="tab === t.key ? 'bg-white/10 text-white' : 'text-white/40 hover:text-white/70'">
        {{ t.icon }} {{ t.label }}
      </button>
    </div>

    <!-- USERS -->
    <div v-if="tab === 'users'">
      <div v-if="usersLoading" class="space-y-3">
        <div v-for="i in 4" :key="i" class="glass rounded-xl p-4 animate-shimmer flex gap-4">
          <div class="w-10 h-10 rounded-xl bg-white/10 flex-shrink-0" />
          <div class="flex-1"><div class="h-3 bg-white/10 rounded w-40 mb-2" /><div class="h-2 bg-white/5 rounded w-24" /></div>
        </div>
      </div>
      <div v-else class="space-y-3">
        <div v-for="u in users" :key="u.id" class="glass rounded-xl p-4 flex items-center gap-4">
          <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-violet-500 to-pink-500 flex items-center justify-center text-sm font-bold flex-shrink-0">
            {{ (u.displayName || u.email)[0].toUpperCase() }}
          </div>
          <div class="flex-1 min-w-0">
            <p class="text-sm font-semibold text-white truncate">{{ u.displayName || u.email }}</p>
            <p class="text-xs text-white/40 truncate">{{ u.email }}</p>
          </div>
          <div class="flex items-center gap-2 flex-shrink-0">
            <span class="badge text-xs" :class="u.role === 'Admin' ? 'bg-amber-500/20 text-amber-400' : 'bg-violet-500/20 text-violet-400'">{{ u.role }}</span>
            <button v-if="u.role !== 'Admin'"   @click="assignRole(u.id, 'Admin')" class="btn-secondary text-xs !px-3 !py-1.5">Make Admin</button>
            <button v-if="u.role !== 'User'"    @click="assignRole(u.id, 'User')"  class="btn-danger  text-xs !px-3 !py-1.5">Remove Admin</button>
          </div>
        </div>
        <p v-if="!users.length" class="text-center text-white/30 py-8">No users found</p>
      </div>
    </div>

    <!-- GENRES -->
    <div v-if="tab === 'genres'">
      <!-- Add new -->
      <div class="glass rounded-xl p-4 flex gap-3 mb-4">
        <input v-model="newGenre" class="input flex-1" placeholder="New genre name..." @keydown.enter="addGenre" />
        <button @click="addGenre" :disabled="!newGenre.trim()" class="btn-primary !px-4 disabled:opacity-40">Add</button>
      </div>
      <div class="space-y-2">
        <div v-for="g in genres" :key="g.id" class="glass rounded-xl p-3 flex items-center gap-3 group">
          <span class="text-sm text-white/50">🏷️</span>
          <div class="flex-1">
            <input v-if="editGenreId === g.id" v-model="editGenreName" class="input text-sm py-1.5" @keydown.enter="saveGenre(g)" @keydown.escape="editGenreId=null" />
            <span v-else class="text-sm text-white font-medium">{{ g.name }}</span>
          </div>
          <div class="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
            <template v-if="editGenreId === g.id">
              <button @click="saveGenre(g)"    class="btn-primary text-xs !px-3 !py-1.5">Save</button>
              <button @click="editGenreId=null" class="btn-secondary text-xs !px-3 !py-1.5">Cancel</button>
            </template>
            <template v-else>
              <button @click="startEditGenre(g)" class="p-1.5 rounded-lg hover:bg-white/10 text-white/40 hover:text-white text-sm transition-colors">✏️</button>
              <button @click="deleteGenre(g.id)"  class="p-1.5 rounded-lg hover:bg-red-500/20 text-white/40 hover:text-red-400 text-sm transition-colors">🗑️</button>
            </template>
          </div>
        </div>
        <p v-if="!genres.length" class="text-center text-white/30 py-8">No genres yet</p>
      </div>
    </div>

    <!-- CUISINES -->
    <div v-if="tab === 'cuisines'">
      <div class="glass rounded-xl p-4 flex gap-3 mb-4">
        <input v-model="newCuisine" class="input flex-1" placeholder="New cuisine name..." @keydown.enter="addCuisine" />
        <button @click="addCuisine" :disabled="!newCuisine.trim()" class="btn-primary !px-4 disabled:opacity-40">Add</button>
      </div>
      <div class="space-y-2">
        <div v-for="c in cuisines" :key="c.id" class="glass rounded-xl p-3 flex items-center gap-3 group">
          <span class="text-sm text-white/50">🌍</span>
          <div class="flex-1">
            <input v-if="editCuisineId === c.id" v-model="editCuisineName" class="input text-sm py-1.5" @keydown.enter="saveCuisine(c)" @keydown.escape="editCuisineId=null" />
            <span v-else class="text-sm text-white font-medium">{{ c.name }}</span>
          </div>
          <div class="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
            <template v-if="editCuisineId === c.id">
              <button @click="saveCuisine(c)"      class="btn-primary text-xs !px-3 !py-1.5">Save</button>
              <button @click="editCuisineId=null"   class="btn-secondary text-xs !px-3 !py-1.5">Cancel</button>
            </template>
            <template v-else>
              <button @click="startEditCuisine(c)"  class="p-1.5 rounded-lg hover:bg-white/10 text-white/40 hover:text-white text-sm transition-colors">✏️</button>
              <button @click="deleteCuisine(c.id)"  class="p-1.5 rounded-lg hover:bg-red-500/20 text-white/40 hover:text-red-400 text-sm transition-colors">🗑️</button>
            </template>
          </div>
        </div>
        <p v-if="!cuisines.length" class="text-center text-white/30 py-8">No cuisines yet</p>
      </div>
    </div>
  </div>
</template>
