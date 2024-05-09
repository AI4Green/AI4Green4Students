import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  sectionsListBySectionType: (sectionTypeId) =>
    `sections/listSectionsBySectionType?sectionTypeId=${sectionTypeId}`,

  file: (sectionId, recordId, fileLocation, fileName) =>
    `sections/file?sectionId=${sectionId}&recordId=${recordId}&fileLocation=${fileLocation}&name=${fileName}`,
};

export const getSectionsApi = ({ api }) => ({
  saveFieldResponses: (formValues) =>
    api.put(`sections/SaveSection`, { body: formValues }),

  downloadSectionFile: (sectionId, recordId, fileLocation, fileName) =>
    api.get(fetchKeys.file(sectionId, recordId, fileLocation, fileName)),
});

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
