import { useBackendApi } from "contexts";
import useSWR from "swr";

export const fetchKeys = {
  reportsList: (projectId) => `reports?projectId=${projectId}`,

  report: (id) => `reports/${id}`,

  reportSectionsList: (id) => `reports/${id}/summary`,

  reportSection: (id, sectionId) => `reports/${id}/form/${sectionId}`,

  reportExport: (reportId) => `reports/${reportId}/GenerateExport`,
};

export const getReportsApi = ({ api }) => ({
  create: (values) =>
    api.post("reports/", {
      json: values,
    }),

  delete: (id) => api.delete(`reports/${id}`),

  advanceStage: (id, stageName) =>
    api.post(`reports/${id}/advance`, {
      json: { stageName },
    }),

  saveFieldResponses: (formValues) =>
    api.put(`reports/save-form`, { body: formValues }),

  downloadReportExport: async (reportId) =>
    await api.get(fetchKeys.reportExport(reportId)),
});

export const useReportsList = (projectId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    projectId ? fetchKeys.reportsList(projectId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useReport = (id) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    id ? fetchKeys.report(id) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useReportSectionsList = (id) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    id ? fetchKeys.reportSectionsList(id) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useReportSection = (id, sectionId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    id && sectionId ? fetchKeys.reportSection(id, sectionId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
