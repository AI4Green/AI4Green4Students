import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  sectionsList: (experimentId) =>
    `sections/listSectionSummaries?experimentId=${experimentId}`, // get experiment plan sections of the project and other information based on experimentId
  section: (sectionId, experimentId) =>
    `sections/getSection?sectionId=${sectionId}?experimentId=${experimentId}`, // get section information for a given sectionId
};

export const useSectionsList = (experimentId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    experimentId ? fetchKeys.sectionsList(experimentId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useSection = (sectionId, experimentId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    sectionId && experimentId
      ? fetchKeys.section(sectionId, experimentId)
      : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
