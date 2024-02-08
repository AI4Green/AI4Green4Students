import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  commentsByFieldResponse: (fieldResponseId) =>
    `comments?fieldResponse=${fieldResponseId}`,
};

export const getCommentsApi = ({ api }) => ({
  create: (values) =>
    api.post("comments/", {
      json: values,
    }),

  edit: (values, id) =>
    api.put(`comments?id=${id}`, {
      json: values,
    }),

  markAsRead: (id) => api.put(`comments/read?id=${id}`),

  delete: (id) => api.delete(`comments/${id}`),
});

export const useComments = (fieldResponseId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    fieldResponseId ? fetchKeys.commentsByFieldResponse(fieldResponseId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
