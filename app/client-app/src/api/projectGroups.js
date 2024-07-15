import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  projectGroupsList: (projectId) => `projectgroups/project/${projectId}`,

  projectGroup: (projectGroupId) => `projectgroups/${projectGroupId}`,

  projectGroupSummarySection: (projectGroupId) =>
    `projectgroups/form/${projectGroupId}`,
};

export const getProjectGroupsApi = ({ api }) => ({
  create: ({ values }) =>
    api.post("projectgroups/", {
      json: values,
    }),

  edit: ({ values, id }) =>
    api.put(`projectgroups/${id}`, {
      json: values,
    }),

  delete: ({ id }) => api.delete(`projectgroups/${id}`),

  inviteStudents: ({ values, id }) =>
    api.put(`projectgroups/${id}/invite-students`, {
      json: values,
    }),

  removeStudent: ({ values, id }) =>
    api.put(`projectgroups/${id}/remove-student`, {
      json: values,
    }),

  saveFieldResponses: (formValues) =>
    api.put(`projectgroups/save-form`, { body: formValues }),
});

export const useProjectGroupsList = (projectId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectId ? fetchKeys.projectGroupsList(projectId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useProjectGroup = (projectGroupId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectGroupId ? fetchKeys.projectGroup(projectGroupId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useProjectGroupSummarySection = (projectGroupId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectGroupId
      ? fetchKeys.projectGroupSummarySection(projectGroupId)
      : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
