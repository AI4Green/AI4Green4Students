import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  note: (noteId) => `notes?noteId=${noteId}`,
};

export const useNote = (noteId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    noteId ? fetchKeys.note(noteId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
