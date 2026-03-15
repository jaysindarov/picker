<script setup>
import { ref, onMounted } from 'vue'
import { commentsApi } from '@/api'
import { useAuthStore } from '@/stores/auth'

const props = defineProps({
  itemId:       { type: String, required: true },
  categoryType: { type: Number, required: true }, // 1=Food, 2=Movie, 3=Book
})

const auth     = useAuthStore()
const comments = ref([])
const loading  = ref(false)
const newText  = ref('')
const submitting = ref(false)
const editingId  = ref(null)
const editText   = ref('')

async function loadComments() {
  loading.value = true
  try {
    const { data } = await commentsApi.getByItem(props.itemId, props.categoryType)
    comments.value = data
  } finally {
    loading.value = false
  }
}

async function submit() {
  if (!newText.value.trim()) return
  submitting.value = true
  try {
    await commentsApi.create({
      content: newText.value.trim(),
      authorName: auth.user?.displayName || auth.user?.email || 'Anonymous',
      categoryType: props.categoryType,
      itemId: props.itemId,
    })
    newText.value = ''
    await loadComments()
  } finally {
    submitting.value = false
  }
}

function startEdit(comment) {
  editingId.value = comment.id
  editText.value  = comment.content
}

async function saveEdit(comment) {
  await commentsApi.update(comment.id, { content: editText.value })
  editingId.value = null
  await loadComments()
}

async function deleteComment(id) {
  await commentsApi.delete(id)
  await loadComments()
}

function canEdit(comment) {
  return auth.isAdmin || comment.userId === auth.user?.userId
}

function timeAgo(dateStr) {
  const diff = Date.now() - new Date(dateStr).getTime()
  const m = Math.floor(diff / 60000)
  if (m < 1)  return 'just now'
  if (m < 60) return `${m}m ago`
  const h = Math.floor(m / 60)
  if (h < 24) return `${h}h ago`
  return `${Math.floor(h / 24)}d ago`
}

onMounted(loadComments)
</script>

<template>
  <div>
    <h3 class="font-bold text-white mb-4 flex items-center gap-2">
      <span class="text-white/60">💬</span>
      Comments
      <span v-if="comments.length" class="text-sm font-normal text-white/40">({{ comments.length }})</span>
    </h3>

    <!-- Add comment -->
    <div class="glass rounded-2xl p-4 mb-6">
      <textarea
        v-model="newText"
        placeholder="Share your thoughts..."
        rows="3"
        class="input resize-none text-sm mb-3"
        @keydown.ctrl.enter="submit"
      />
      <div class="flex justify-end">
        <button @click="submit" :disabled="submitting || !newText.trim()" class="btn-primary text-sm disabled:opacity-40 disabled:cursor-not-allowed">
          {{ submitting ? 'Posting...' : 'Post comment' }}
        </button>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="loading" class="space-y-3">
      <div v-for="i in 2" :key="i" class="glass rounded-xl p-4 animate-shimmer">
        <div class="h-3 bg-white/10 rounded w-24 mb-2" />
        <div class="h-3 bg-white/10 rounded w-full mb-1" />
        <div class="h-3 bg-white/10 rounded w-3/4" />
      </div>
    </div>

    <!-- Comments list -->
    <div v-else-if="comments.length" class="space-y-3">
      <div v-for="c in comments" :key="c.id" class="glass rounded-xl p-4 group">
        <div class="flex items-start justify-between gap-2 mb-2">
          <div class="flex items-center gap-2">
            <div class="w-7 h-7 rounded-lg bg-gradient-to-br from-violet-500 to-pink-500 flex items-center justify-center text-xs font-bold flex-shrink-0">
              {{ c.authorName[0].toUpperCase() }}
            </div>
            <span class="text-sm font-semibold text-white/90">{{ c.authorName }}</span>
            <span class="text-xs text-white/30">{{ timeAgo(c.createdAt) }}</span>
          </div>
          <div v-if="canEdit(c)" class="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
            <button @click="startEdit(c)" class="p-1 rounded-lg hover:bg-white/10 text-white/40 hover:text-white/80 text-xs transition-colors">✏️</button>
            <button @click="deleteComment(c.id)" class="p-1 rounded-lg hover:bg-red-500/20 text-white/40 hover:text-red-400 text-xs transition-colors">🗑️</button>
          </div>
        </div>

        <div v-if="editingId === c.id">
          <textarea v-model="editText" class="input text-sm resize-none mb-2" rows="2" />
          <div class="flex gap-2">
            <button @click="saveEdit(c)" class="btn-primary text-xs">Save</button>
            <button @click="editingId = null" class="btn-secondary text-xs">Cancel</button>
          </div>
        </div>
        <p v-else class="text-sm text-white/70 leading-relaxed">{{ c.content }}</p>
      </div>
    </div>

    <p v-else class="text-center text-white/30 text-sm py-8">No comments yet. Be the first!</p>
  </div>
</template>
