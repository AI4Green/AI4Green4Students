import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  literatureReviewsList: (projectId) =>
    `literatureReviews?projectId=${projectId}`,

  literatureReview: (literatureReviewId) =>
    `literatureReviews/${literatureReviewId}`,

  literatureReviewSectionsList: (literatureReviewId) =>
    `literatureReviews/summary/${literatureReviewId}`,

  literatureReviewSection: (literatureReviewId, sectionId) =>
    `literatureReviews/form/${literatureReviewId}/${sectionId}`,
};

export const getLiteratureReviewsApi = ({ api }) => ({
  create: (values) =>
    api.post("literatureReviews/", {
      json: values,
    }),

  delete: (id) => api.delete(`literatureReviews/${id}`),

  advanceStage: (id, stageName) =>
    api.post(`literatureReviews/${id}/AdvanceStage`, {
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

export const useLiteratureReview = (literatureReviewId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    literatureReviewId ? fetchKeys.literatureReview(literatureReviewId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useLiteratureReviewSectionsList = (literatureReviewId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    literatureReviewId
      ? fetchKeys.literatureReviewSectionsList(literatureReviewId)
      : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useLiteratureReviewSection = (literatureReviewId, sectionId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    literatureReviewId && sectionId
      ? fetchKeys.literatureReviewSection(literatureReviewId, sectionId)
      : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
