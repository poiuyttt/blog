import { ref } from "vue";
import { defineStore } from "pinia";

export const useSearchStore = defineStore("search", () => {
  const keyword = ref<string>("");
  const page = ref<number>(1);

  function setKeyword(newKeyword: string): void {
    keyword.value = newKeyword;
  }

  function setPage(newPage: number): void {
    page.value = newPage;
  }

  function reset(): void {
    keyword.value = "";
    page.value = 1;
  }

  return {
    keyword,
    page,
    setKeyword,
    setPage,
    reset,
  };
});
