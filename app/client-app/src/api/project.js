import { useBackendApi } from "contexts";
import useSWR from "swr";

export const fetchKeys = {
  projectsList: "projects/",
  project: (id) => `projects/${id}`,
  projectSummaryByStudent: (id, studentId) =>
    `projects/${id}/summary${studentId ? `?studentId=${studentId}` : ""}`,
  projectInstructors: (id) => `projects/${id}/instructors`,
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

  inviteInstructors: (id, { emails }) =>
    api.post(`projects/${id}/invite-instructors`, {
      json: { emails },
    }),
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

export const useProject = (id) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    id ? fetchKeys.project(id) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useProjectSummaryByStudent = (id, studentId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    id ? fetchKeys.projectSummaryByStudent(id, studentId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useProjectInstructors = (id) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    id ? fetchKeys.projectInstructors(id) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
