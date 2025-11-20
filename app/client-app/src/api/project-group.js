import { useBackendApi } from "contexts";
import useSWR from "swr";

export const fetchKeys = {
  projectGroupsList: (projectId) => `project-groups/project/${projectId}`,

  projectGroup: (id) => `project-groups/${id}`,

  projectGroupSummarySection: (id) => `project-groups/${id}/form`,
};

export const getProjectGroupsApi = ({ api }) => ({
  create: ({ values }) =>
    api.post("project-groups/", {
      json: values,
    }),

  edit: ({ values, id }) =>
    api.put(`project-groups/${id}`, {
      json: values,
    }),

  delete: ({ id }) => api.delete(`project-groups/${id}`),

  inviteStudents: ({ values, id }) =>
    api.post(`project-groups/${id}/invite-students`, {
      json: values,
    }),

  removeStudent: ({ values, id }) =>
    api.put(`project-groups/${id}/remove-student`, {
      json: values,
    }),

  saveFieldResponses: (formValues) =>
    api.put(`project-groups/save-form`, { body: formValues }),
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
