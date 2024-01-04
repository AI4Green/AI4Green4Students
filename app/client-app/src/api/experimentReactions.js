import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  experimentReactionsList: (experimentId) =>
    `experimentreactions?experimentId=${experimentId}`, // get reactions list for the given experiment
  experimentReaction: (id) => `experimentreactions/${id}`, // get reaction information
};

export const getExperimentReactionsApi = ({ api }) => ({
  create: ({ values }) =>
    api.post("experimentreactions/", {
      json: values,
    }),

  edit: ({ id, values }) =>
    api.put(`experimentreactions/${id}`, {
      json: values,
    }),

  delete: ({ id }) => api.delete(`experimentreactions/${id}`),
});

export const useExperimentReactionsList = (experimentId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    experimentId ? fetchKeys.experimentReactionsList(experimentId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useExperimentReaction = (id) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    id ? fetchKeys.experimentReaction(id) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
