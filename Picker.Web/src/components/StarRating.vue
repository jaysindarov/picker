<script setup>
import { ref } from 'vue'

const props = defineProps({
  modelValue: { type: Number, default: 0 },
  readonly:   { type: Boolean, default: false },
  size:       { type: String, default: 'md' },
})
const emit = defineEmits(['update:modelValue'])

const hovered = ref(0)
const sizeClass = { sm: 'text-lg', md: 'text-2xl', lg: 'text-3xl' }[props.size] || 'text-2xl'

function select(val) {
  if (!props.readonly) emit('update:modelValue', val)
}
</script>

<template>
  <div class="flex items-center gap-0.5">
    <button
      v-for="star in 5" :key="star"
      @click="select(star)"
      @mouseenter="!readonly && (hovered = star)"
      @mouseleave="hovered = 0"
      :disabled="readonly"
      class="transition-transform duration-100"
      :class="[sizeClass, readonly ? 'cursor-default' : 'cursor-pointer hover:scale-110']"
    >
      <span :class="(hovered || modelValue) >= star ? 'text-amber-400' : 'text-white/20'">★</span>
    </button>
    <span v-if="!readonly && modelValue" class="ml-2 text-sm text-white/40">{{ modelValue }}/5</span>
  </div>
</template>
