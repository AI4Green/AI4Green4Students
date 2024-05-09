import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  projectGroupsList: "projectgroups/",

  projectGroup: (projectGroupId) => `projectgroups/${projectGroupId}`,

  projectGroupSummarySection: (projectGroupId, sectionTypeId) =>
    `projectgroups/form/${projectGroupId}/${sectionTypeId}`,
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

export const useProjectGroupsList = () => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    fetchKeys.projectGroupsList,
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

export const useProjectGroupSummarySection = (
  projectGroupId,
  sectionTypeId
) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectGroupId && sectionTypeId
      ? fetchKeys.projectGroupSummarySection(projectGroupId, sectionTypeId)
      : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
