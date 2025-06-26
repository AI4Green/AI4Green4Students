import { useBackendApi } from "contexts";
import useSWR from "swr";

export const fetchKeys = {
  commentsByFieldResponse: (fieldResponseId) =>
    `comments/field-response/${fieldResponseId}`,
};

export const getCommentsApi = ({ api, apiFetcher }) => ({
  create: (values) =>
    api.post("comments/", {
      json: values,
    }),

  edit: (values, id) =>
    api.put(`comments/${id}`, {
      json: values,
    }),

  markAsRead: (id) => api.put(`comments/${id}/read`),

  setApprovalStatus: (fieldResponseId, isApproved) =>
    api.put(
      `comments/field-response/${fieldResponseId}?isApproved=${isApproved}`
    ),

  getCommentLogs: (fieldResponseId) =>
    apiFetcher(fetchKeys.commentsByFieldResponse(fieldResponseId)),

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
