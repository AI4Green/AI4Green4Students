import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  literatureReviewsList: (projectId) =>
    `literatureReviews?projectId=${projectId}`,
  literatureReviewsListByProjectGroup: (projectGroupId) =>
    `literatureReviews/listLiteratureReviews?projectGroupId=${projectGroupId}`,
  literatureReview: (literatureReviewId) =>
    `literatureReviews/${literatureReviewId}`,
};

export const getLiteratureReviewsApi = ({ api }) => ({
  create: (values) =>
    api.post("literatureReviews/", {
      json: values,
    }),

  delete: (id) => api.delete(`literatureReviews/${id}`),
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
