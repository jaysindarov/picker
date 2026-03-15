<script setup>
defineProps({
  show:    { type: Boolean, default: false },
  message: { type: String, default: 'Are you sure?' },
})
defineEmits(['confirm', 'cancel'])
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div v-if="show" class="fixed inset-0 z-50 flex items-center justify-center p-4">
        <div @click="$emit('cancel')" class="absolute inset-0 bg-black/70 backdrop-blur-sm" />
        <div class="relative glass-dark rounded-2xl p-6 w-full max-w-sm shadow-2xl text-center">
          <div class="text-4xl mb-3">🗑️</div>
          <h3 class="text-lg font-bold text-white mb-2">Delete item?</h3>
          <p class="text-sm text-white/50 mb-6">{{ message }}</p>
          <div class="flex gap-3 justify-center">
            <button @click="$emit('cancel')" class="btn-secondary flex-1">Cancel</button>
            <button @click="$emit('confirm')" class="btn-danger flex-1 !px-0">Delete</button>
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
