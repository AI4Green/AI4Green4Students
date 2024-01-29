import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  projectsList: "projects/",
  project: (projectId) => `projects/${projectId}`,
  projectSummaryByStudent: (projectId) =>
    `projects/getProjectSummary?projectId=${projectId}`,
  projectSummaryByProjectGroup: (projectGroupId) =>
    `projects/getProjectGroupProjectSummary?projectGroupId=${projectGroupId}`,
};

export const getProjectsApi = ({ api }) => ({
  create: ({ values }) =>
    api.post("projects/", {
      json: values,
    }),

  edit: ({ values, id }) =>
    api.put(`projects/${id}`, {
      json: values,
    }),

  delete: ({ id }) => api.delete(`projects/${id}`),
});

export const useProjectsList = () => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    fetchKeys.projectsList,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useProject = (projectId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectId ? fetchKeys.project(projectId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useProjectSummaryByStudent = (projectId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectId ? fetchKeys.projectSummaryByStudent(projectId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useProjectSummaryByProjectGroup = (projectGroupId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectGroupId
      ? fetchKeys.projectSummaryByProjectGroup(projectGroupId)
      : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
