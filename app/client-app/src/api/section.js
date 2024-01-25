import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  planSectionsList: (planId, sectionTypeId) =>
    `sections/listPlanSectionSummaries?planId=${planId}&sectionTypeId=${sectionTypeId}`,

  planSection: (planId, sectionId) =>
    `sections/getPlanSectionForm?planId=${planId}&sectionId=${sectionId}`,
};

export const usePlanSectionsList = (planId, sectionTypeId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    planId && sectionTypeId
      ? fetchKeys.planSectionsList(planId, sectionTypeId)
      : null,
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
