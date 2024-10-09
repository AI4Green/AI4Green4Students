import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  notesList: (projectId) => `notes?projectId=${projectId}`,

  note: (noteId) => `notes/${noteId}`,

  noteSection: (noteId, sectionId) => `notes/form/${noteId}/${sectionId}`,

  noteFieldResponse: (noteId, fieldId) =>
    `notes/field-response/${noteId}/${fieldId}`,

  requestFeedback: (noteId) => `notes/${noteId}/request-feedback`,

  completeFeedback: (noteId) => `notes/${noteId}/complete-feedback`,
};

export const getNotesApi = ({ api, apiFetcher }) => ({
  saveFieldResponses: (formValues) =>
    api.put(`notes/save-form`, { body: formValues }),

  getNotesList: async (projectId) => apiFetcher(fetchKeys.notesList(projectId)),

  getNoteFieldResponse: async (noteId, fieldId) =>
    apiFetcher(fetchKeys.noteFieldResponse(noteId, fieldId)),

  lockProjectGroupNotes: (projectGroupId) =>
    api.post(`notes/lock-notes/${projectGroupId}`),

  advanceStage: (id, stageName) =>
    api.post(`notes/${id}/AdvanceStage`, {
      json: { stageName },
    }),

  requestFeedback: async (noteId) =>
    api.post(fetchKeys.requestFeedback(noteId)),

  completeFeedback: async (noteId) =>
    api.post(fetchKeys.completeFeedback(noteId)),
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
