import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
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
