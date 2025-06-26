import { useBackendApi } from "contexts";
import useSWR from "swr";

export const fetchKeys = {
  literatureReviewsList: (projectId) =>
    `literatureReviews?projectId=${projectId}`,

  literatureReview: (id) => `literatureReviews/${id}`,

  literatureReviewSectionsList: (id) => `literatureReviews/${id}/summary`,

  literatureReviewSection: (id, sectionId) =>
    `literatureReviews/${id}/form/${sectionId}`,
};

export const getLiteratureReviewsApi = ({ api }) => ({
  create: (values) =>
    api.post("literatureReviews/", {
      json: values,
    }),

  delete: (id) => api.delete(`literatureReviews/${id}`),

  advanceStage: (id, stageName) =>
    api.post(`literatureReviews/${id}/advance`, {
      json: { stageName },
    }),

  saveFieldResponses: (formValues) =>
    api.put(`literatureReviews/save-form`, { body: formValues }),
});

export const useLiteratureReviewsList = (projectId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectId ? fetchKeys.literatureReviewsList(projectId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useLiteratureReview = (id) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    id ? fetchKeys.literatureReview(id) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useLiteratureReviewSectionsList = (id) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    id ? fetchKeys.literatureReviewSectionsList(id) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useLiteratureReviewSection = (id, sectionId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    id && sectionId ? fetchKeys.literatureReviewSection(id, sectionId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
