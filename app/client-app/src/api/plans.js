import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  plansList: (projectId) => `plans?projectId=${projectId}`,
  plan: (planId) => `plans/${planId}`,
};

export const getPlansApi = ({ api }) => ({
  create: ({ values }) =>
    api.post("plans/", {
      json: values,
    }),

  edit: ({ id, values }) =>
    api.put(`plans/${id}`, {
      json: values,
    }),

  delete: ({ id }) => api.delete(`plans/${id}`),
});

export const usePlansList = (projectId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectId ? fetchKeys.plansList(projectId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const usePlan = (planId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    planId ? fetchKeys.plan(planId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
