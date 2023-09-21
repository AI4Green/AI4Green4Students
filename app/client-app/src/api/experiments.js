import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  experimentsList: "experiments/",
  experiment: (experimentId) => `experiments/${experimentId}`,
};

export const getExperimentsApi = ({ api }) => ({
  create: ({ values }) =>
    api.post("experiments/", {
      json: values,
    }),

  edit: ({ values, id }) =>
    api.put(`experiments/${id}`, {
      json: values,
    }),

  delete: ({ id }) => api.delete(`experiments/${id}`),
});

export const useExperimentsList = () => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    fetchKeys.experimentsList,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useExperiment = (experimentId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    experimentId ? fetchKeys.experiment(experimentId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
