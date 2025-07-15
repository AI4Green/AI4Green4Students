import { useBackendApi } from "contexts";
import useSWR from "swr";

export const fetchKeys = {
  notesList: (projectId) => `notes?projectId=${projectId}`,

  note: (id) => `notes/${id}`,

  noteSection: (id, sectionId) => `notes/${id}/form/${sectionId}`,

  noteFieldResponse: (id, fieldId) => `notes/${id}/field-response/${fieldId}`,

  requestFeedback: (id) => `notes/${id}/request-feedback`,

  completeFeedback: (id) => `notes/${id}/complete-feedback`,

  noteFeedback: (id) => `notes/${id}/feedback`,
};

export const getNotesApi = ({ api, apiFetcher }) => ({
  saveFieldResponses: (formValues) =>
    api.put(`notes/save-form`, { body: formValues }),

  getNotesList: async (projectId) => apiFetcher(fetchKeys.notesList(projectId)),

  getNoteFieldResponse: async (id, fieldId) =>
    apiFetcher(fetchKeys.noteFieldResponse(id, fieldId)),

  lockProjectGroupNotes: (projectGroupId) =>
    api.post(`notes/lock-notes/${projectGroupId}`),

  advanceStage: (id, stageName) =>
    api.post(`notes/${id}/advance`, {
      json: { stageName },
    }),

  requestFeedback: async (id) => api.post(fetchKeys.requestFeedback(id)),

  completeFeedback: async (id, feedback) =>
    api.post(fetchKeys.completeFeedback(id), {
      json: {
        value: feedback,
      },
    }),
});

export const useNote = (id) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    id ? fetchKeys.note(id) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useNoteFeedback = (id) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    id ? fetchKeys.noteFeedback(id) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    {
      suspense: true,
    }
  );
};

export const useNoteSection = (id, sectionId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    id && sectionId ? fetchKeys.noteSection(id, sectionId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
