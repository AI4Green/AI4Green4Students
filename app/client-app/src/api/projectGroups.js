import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  projectGroupsList: "projectgroups/",
  projectGroup: (projectGroup) => `projectgroups/${projectGroup}`,
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

export const useProjectGroup = (projectGroup) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectGroup ? fetchKeys.projectGroup(projectGroup) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
