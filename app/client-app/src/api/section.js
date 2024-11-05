import { useBackendApi } from "contexts";
import useSWR from "swr";

export const fetchKeys = {
  sectionsListByProject: (projectId) =>
    `sections/listSectionsByProject?projectId=${projectId}`,

  sectionsListBySectionType: (projectId, sectionType) =>
    `sections/listSectionsBySectionType?projectId=${projectId}&sectionType=${sectionType}`,

  file: (sectionId, recordId, fileLocation, fileName) =>
    `sections/file?sectionId=${sectionId}&recordId=${recordId}&fileLocation=${fileLocation}&name=${fileName}`,
};

export const getSectionsApi = ({ api }) => ({
  saveFieldResponses: (formValues) =>
    api.put(`sections/SaveSection`, { body: formValues }),

  downloadSectionFile: (sectionId, recordId, fileLocation, fileName) =>
    api.get(fetchKeys.file(sectionId, recordId, fileLocation, fileName)),
});

export const useSectionsListByProject = (projectId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectId ? fetchKeys.sectionsListByProject(projectId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useSectionsListBySectionType = (projectId, sectionType) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectId && sectionType
      ? fetchKeys.sectionsListBySectionType(projectId, sectionType)
      : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
