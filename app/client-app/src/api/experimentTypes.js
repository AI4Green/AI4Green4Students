import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  experimentTypesList: "experimentTypes/",
};

export const getExperimentTypesApi = ({ api }) => ({
  create: ({ values }) =>
    api.post("experimentTypes/", {
      json: values,
    }),

  edit: ({ values, id }) =>
    api.put(`experimentTypes/${id}`, {
      json: values,
    }),

  delete: ({ id }) => api.delete(`experimentTypes/${id}`),
});

export const useExperimentTypesList = () => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    fetchKeys.experimentTypesList,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
