import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  field: (fieldId) => `fields?fieldId=${fieldId}`, // get field information for a given fieldId. useful for getting field options
};

export const useField = (fieldId) => {
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
