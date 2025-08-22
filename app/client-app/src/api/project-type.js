import { useBackendApi } from "contexts";
import useSWR from "swr";

export const fetchKeys = {
  projectTypesList: "project-types/",
  projectType: (projectTypeId) => `project-types/${projectTypeId}`,
};

export const useProjectTypesList = () => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    fetchKeys.projectTypesList,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useProjectType = (projectTypeId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectTypeId ? fetchKeys.projectType(projectTypeId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
