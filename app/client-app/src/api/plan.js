import { useBackendApi } from "contexts";
import useSWR from "swr";

export const fetchKeys = {
  plansList: (projectId) => `plans?projectId=${projectId}`,

  plan: (id) => `plans/${id}`,

  planSectionsList: (id) => `plans/${id}/summary`,

  planSection: (id, sectionId) => `plans/${id}/form/${sectionId}`,
};

export const getPlansApi = ({ api }) => ({
  create: (values) =>
    api.post("plans/", {
      json: values,
    }),

  delete: (id) => api.delete(`plans/${id}`),

  advanceStage: (id, stageName) =>
    api.post(`plans/${id}/advance`, {
      json: { stageName },
    }),

  saveFieldResponses: (formValues) =>
    api.put(`plans/save-form`, { body: formValues }),
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

export const usePlan = (id) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    id ? fetchKeys.plan(id) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const usePlanSectionsList = (id) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    id ? fetchKeys.planSectionsList(id) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const usePlanSection = (id, sectionId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    id && sectionId ? fetchKeys.planSection(id, sectionId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
