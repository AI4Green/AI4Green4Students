import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  experimentsList: "experiments/",
  experiment: (experimentId) => `experiments/${experimentId}`,
  downloadFile: (experimentId, fileName) =>
    `/api/experiments/${experimentId}/download?fileName=${fileName}`,
};

export const getExperimentsApi = ({ api }) => ({
  create: ({ values }) =>
    api.post("experiments/", {
      json: values,
    }),

  edit: ({ id, ...values }) => {
    const form = new FormData();

    for (const [k, v] of Object.entries(values)) {
      if (Array.isArray(v)) {
        for (let i = 0; i < v.length; i++) {
          form.append(`${k}[]`, v[i]);
        }
      } else form.append(k, v);
    }

    return api.put(`experiments/${id}`, {
      body: form,
    });
  },

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
