import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  sectionsListBySectionType: (sectionTypeId) =>
    `sections/ListBySectionType?sectionTypeId=${sectionTypeId}`,

  planSectionsList: (planId, sectionTypeId) =>
    `sections/listPlanSectionSummaries?planId=${planId}&sectionTypeId=${sectionTypeId}`,

  planSection: (planId, sectionId) =>
    `sections/getPlanSectionForm?planId=${planId}&sectionId=${sectionId}`,

  literatureReviewSectionsList: (literatureReviewId, sectionTypeId) =>
    `sections/listLiteratureReviewSectionSummaries?literatureReviewId=${literatureReviewId}&sectionTypeId=${sectionTypeId}`,

  literatureReviewSection: (literatureReviewId, sectionId) =>
    `sections/getLiteratureReviewSectionForm?literatureReviewId=${literatureReviewId}&sectionId=${sectionId}`,
};

export const getSectionsApi = ({ api }) => ({
  saveFieldResponses: (formValues) =>
    api.put(`sections/SaveSection`, { body: formValues }),
});

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

export const useLiteratureReviewSectionsList = (
  literatureReviewId,
  sectionTypeId
) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    literatureReviewId && sectionTypeId
      ? fetchKeys.literatureReviewSectionsList(
          literatureReviewId,
          sectionTypeId
        )
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
