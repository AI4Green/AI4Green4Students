import { useBackendApi } from "contexts";
import useSWR from "swr";

export const fetchKeys = {
  field: (fieldId) => `fields?fieldId=${fieldId}`, // get field information for a given fieldId.

  fieldByName: (projectId, sectionType, fieldName) =>
    `fields/${projectId}/${sectionType}/${fieldName}`,
};

export const getFieldsApi = ({ apiFetcher }) => ({
  getFieldByName: async (projectId, sectionType, fieldName) =>
    apiFetcher(fetchKeys.fieldByName(projectId, sectionType, fieldName)),
});

export const useSectionField = (fieldId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    fieldId ? fetchKeys.field(fieldId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
