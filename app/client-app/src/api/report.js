import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  reportsList: (projectId) => `reports?projectId=${projectId}`,

  report: (reportId) => `reports/${reportId}`,

  reportSectionsList: (reportId) => `reports/summary/${reportId}`,

  reportSection: (reportId, sectionId) =>
    `reports/form/${reportId}/${sectionId}`,

  reportExport: (reportId) => `reports/${reportId}/GenerateExport`,
};

export const getReportsApi = ({ api }) => ({
  create: (values) =>
    api.post("reports/", {
      json: values,
    }),

  delete: (id) => api.delete(`reports/${id}`),

  advanceStage: (id, stageName) =>
    api.post(`reports/${id}/AdvanceStage`, {
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

export const useReport = (reportId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    reportId ? fetchKeys.report(reportId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useReportSectionsList = (reportId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    reportId ? fetchKeys.reportSectionsList(reportId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

export const useReportSection = (reportId, sectionId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    reportId && sectionId ? fetchKeys.reportSection(reportId, sectionId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
