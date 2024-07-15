import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  plansList: (projectId) => `plans?projectId=${projectId}`,

  plan: (planId) => `plans/${planId}`,

  planSectionsList: (planId) => `plans/summary/${planId}`,

  planSection: (planId, sectionId) => `plans/form/${planId}/${sectionId}`,
};

export const getPlansApi = ({ api }) => ({
  create: (values) =>
    api.post("plans/", {
      json: values,
    }),

  delete: (id) => api.delete(`plans/${id}`),

  advanceStage: (id, stageName) =>
    api.post(`plans/${id}/AdvanceStage`, {
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

export const usePlanSectionsList = (planId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    planId ? fetchKeys.planSectionsList(planId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const usePlanSection = (planId, sectionId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    planId && sectionId ? fetchKeys.planSection(planId, sectionId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
