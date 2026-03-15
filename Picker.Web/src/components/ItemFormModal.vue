<script setup>
import { ref, watch, computed } from 'vue'

const props = defineProps({
  show:     { type: Boolean, default: false },
  type:     { type: String, required: true }, // 'movies' | 'books' | 'foods'
  item:     { type: Object, default: null },   // null = create mode
  genres:   { type: Array, default: () => [] },
  cuisines: { type: Array, default: () => [] },
})
const emit = defineEmits(['close', 'save'])

const form = ref({ title: '', description: '', imageUrl: '', genreId: '', cuisineId: '' })

watch(() => props.show, (v) => {
  if (v) {
    if (props.item) {
      form.value = {
        title:       props.item.title || '',
        description: props.item.description || '',
        imageUrl:    props.item.imageUrl || '',
        genreId:     props.item.genreId || '',
        cuisineId:   props.item.cuisineId || '',
      }
    } else {
      form.value = { title: '', description: '', imageUrl: '', genreId: '', cuisineId: '' }
    }
  }
})

const isFood = computed(() => props.type === 'foods')
const title  = computed(() => props.item ? `Edit ${props.type.slice(0,-1)}` : `Add ${props.type.slice(0,-1)}`)

function save() {
  const payload = {
    title:       form.value.title,
    description: form.value.description,
    imageUrl:    form.value.imageUrl,
  }
  if (isFood.value) payload.cuisineId = form.value.cuisineId
  else              payload.genreId   = form.value.genreId
  emit('save', payload)
}
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="show" class="fixed inset-0 z-50 flex items-center justify-center p-4">
        <div @click="$emit('close')" class="absolute inset-0 bg-black/70 backdrop-blur-sm" />
        <div class="relative glass-dark rounded-2xl p-6 w-full max-w-lg shadow-2xl">
          <div class="flex items-center justify-between mb-6">
            <h2 class="text-lg font-bold text-white capitalize">{{ title }}</h2>
            <button @click="$emit('close')" class="p-2 rounded-xl hover:bg-white/10 text-white/40 hover:text-white transition-colors">✕</button>
          </div>

          <div class="space-y-4">
            <div>
              <label class="block text-xs text-white/50 mb-1.5 font-medium uppercase tracking-wider">Title *</label>
              <input v-model="form.title" class="input" placeholder="Enter title..." />
            </div>
            <div>
              <label class="block text-xs text-white/50 mb-1.5 font-medium uppercase tracking-wider">Description</label>
              <textarea v-model="form.description" class="input resize-none" rows="3" placeholder="Brief description..." />
            </div>
            <div>
              <label class="block text-xs text-white/50 mb-1.5 font-medium uppercase tracking-wider">Image URL</label>
              <input v-model="form.imageUrl" class="input" placeholder="https://..." />
            </div>
            <div v-if="!isFood">
              <label class="block text-xs text-white/50 mb-1.5 font-medium uppercase tracking-wider">Genre *</label>
              <select v-model="form.genreId" class="input">
                <option value="">— Select genre —</option>
                <option v-for="g in genres" :key="g.id" :value="g.id">{{ g.name }}</option>
              </select>
            </div>
            <div v-else>
              <label class="block text-xs text-white/50 mb-1.5 font-medium uppercase tracking-wider">Cuisine *</label>
              <select v-model="form.cuisineId" class="input">
                <option value="">— Select cuisine —</option>
                <option v-for="c in cuisines" :key="c.id" :value="c.id">{{ c.name }}</option>
              </select>
            </div>
          </div>

          <div class="flex justify-end gap-3 mt-6">
            <button @click="$emit('close')" class="btn-secondary">Cancel</button>
            <button @click="save" :disabled="!form.title || (!isFood && !form.genreId) || (isFood && !form.cuisineId)"
              class="btn-primary disabled:opacity-40 disabled:cursor-not-allowed">
              {{ item ? 'Save changes' : 'Create' }}
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.modal-enter-active, .modal-leave-active { transition: all 0.2s ease; }
.modal-enter-from, .modal-leave-to { opacity: 0; transform: scale(0.95); }
</style>
