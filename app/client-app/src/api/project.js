import { useBackendApi } from "contexts";
import useSWR from "swr";

export const fetchKeys = {
  projectsList: "projects/",
  project: (id) => `projects/${id}`,
  projectSummaryByStudent: (id, studentId) =>
    `projects/${id}/summary${studentId ? `?studentId=${studentId}` : ""}`,
  projectInstructors: (id) => `projects/${id}/instructors`,
  validateProjectInstructor: (id) => `projects/${id}/validate-instructor`,
};

export const getProjectsApi = ({ api }) => ({
  create: (values) =>
    api.post("projects/", {
      json: values,
    }),

  edit: (id, values) =>
    api.put(`projects/${id}`, {
      json: values,
    }),

  delete: (id) => api.delete(`projects/${id}`),

  inviteInstructors: (id, { emails }) =>
    api.post(`projects/${id}/invite-instructors`, {
      json: { emails },
    }),

  removeInstructor: (id, instructorId) =>
    api.post(`projects/${id}/remove-instructor`, {
      json: { id: instructorId },
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

export const useIsProjectInstructor = (projectId) => {
  const { api } = useBackendApi();

  return useSWR(
    projectId ? fetchKeys.validateProjectInstructor(projectId) : null,
    async () => {
      try {
        await api.post(fetchKeys.validateProjectInstructor(projectId));
        return true;
      } catch {
        return false;
      }
    }
  );
};
