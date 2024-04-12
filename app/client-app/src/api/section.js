import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  sectionsListBySectionType: (sectionTypeId) =>
    `sections/listSectionsBySectionType?sectionTypeId=${sectionTypeId}`,

  file: (sectionId, recordId, fileLocation, fileName) =>
    `sections/file?sectionId=${sectionId}&recordId=${recordId}&fileLocation=${fileLocation}&name=${fileName}`,

  planSectionsList: (planId, sectionTypeId) =>
    `sections/listPlanSectionSummaries?planId=${planId}&sectionTypeId=${sectionTypeId}`,

  planSection: (planId, sectionId) =>
    `sections/getPlanSectionForm?planId=${planId}&sectionId=${sectionId}`,

  noteSection: (noteId, sectionId) =>
    `sections/getNoteSectionForm?noteId=${noteId}&sectionId=${sectionId}`,

  literatureReviewSectionsList: (literatureReviewId, sectionTypeId) =>
    `sections/listLiteratureReviewSectionSummaries?literatureReviewId=${literatureReviewId}&sectionTypeId=${sectionTypeId}`,

  literatureReviewSection: (literatureReviewId, sectionId) =>
    `sections/getLiteratureReviewSectionForm?literatureReviewId=${literatureReviewId}&sectionId=${sectionId}`,

  projectGroupSummarySection: (projectGroupId, sectionTypeId) =>
    `sections/GetProjectGroupSectionForm?projectGroupId=${projectGroupId}&sectionTypeId=${sectionTypeId}`,
};

export const getSectionsApi = ({ api }) => ({
  saveFieldResponses: (formValues) =>
    api.put(`sections/SaveSection`, { body: formValues }),

  downloadSectionFile: (sectionId, recordId, fileLocation, fileName) =>
    api.get(fetchKeys.file(sectionId, recordId, fileLocation, fileName)),
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

export const useSectionsListBySectionType = (sectionTypeId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    sectionTypeId ? fetchKeys.sectionsListBySectionType(sectionTypeId) : null,
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

export const useNoteSection = (noteId, sectionId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    noteId && sectionId ? fetchKeys.noteSection(noteId, sectionId) : null,
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

export const useProjectGroupSummarySection = (
  projectGroupId,
  sectionTypeId
) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectGroupId && sectionTypeId
      ? fetchKeys.projectGroupSummarySection(projectGroupId, sectionTypeId)
      : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
