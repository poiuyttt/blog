import { ref, computed } from "vue";
import { defineStore } from "pinia";

export const useCounterStore = defineStore(
  "counter",
  () => {
    const count = ref(0);

    const doubleCount = computed(() => count.value * 2);

    function increment(): void {
      count.value++;
    }

    function decrement(): void {
      count.value--;
    }

    function reset(): void {
      count.value = 0;
    }

    return {
      count,
      doubleCount,
      increment,
      decrement,
      reset,
    };
  },
  {
    persist: {
      key: "counter-store",
      paths: ["count"],
    },
  },
);
