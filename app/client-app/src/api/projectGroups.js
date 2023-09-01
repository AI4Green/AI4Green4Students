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
