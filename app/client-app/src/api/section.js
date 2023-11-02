import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  sectionsList: (projectId, experimentId) =>
    `sections/${projectId}/${experimentId}`, // get experiment plan sections of the project and other information based on experimentId
};

export const useSectionsList = (projectId, experimentId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectId && experimentId
      ? fetchKeys.sectionsList(projectId, experimentId)
      : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
