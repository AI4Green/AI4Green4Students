import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  note: (noteId) => `notes?noteId=${noteId}`,

  noteSection: (noteId, sectionId) => `notes/form/${noteId}/${sectionId}`,
};

export const getNotesApi = ({ api }) => ({
  saveFieldResponses: (formValues) =>
    api.put(`notes/save-form`, { body: formValues }),
});

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

export const useNoteSection = (noteId, sectionId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    noteId && sectionId ? fetchKeys.noteSection(noteId, sectionId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
