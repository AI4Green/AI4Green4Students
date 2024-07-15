import { useMemo } from "react";
import { SECTION_TYPES as EXPERIMENT_DATA_TYPES } from "constants/section-types";

/**
 * Hook to generate table data from projectSummary and project
 * @param {*} projectSummary - projectSummary object contains plans, reports, and literature review
 * @param {*} project
 * @returns
 */
export const useExperimentTableData = (projectSummary, project) => {
  const projectGroup = project.projectGroups.find(
    (pg) => pg.id === projectSummary.projectGroupId
  );
  const { plans } = projectSummary ?? { plans: [] };
  const { literatureReviews } = projectSummary ?? { literatureReviews: [] };
  const { reports } = projectSummary ?? { reports: [] };

  const tableData = useMemo(
    () => [
      ...literatureReviews.map((literatureReview) => ({
        dataType: EXPERIMENT_DATA_TYPES.LiteratureReview,
        id: literatureReview.id,
        title: `Literature review ${literatureReview.id}`,
        project,
        projectGroup,
        studentName: literatureReview.ownerName,
        status: literatureReview.stage,
        stagePermissions: literatureReview.permissions,
        overviewPath: `/projects/${project.id}/literature-reviews/${literatureReview.id}/overview`,
      })),
      ...plans.map((plan) => ({
        dataType: EXPERIMENT_DATA_TYPES.Plan,
        id: plan.id,
        title: plan?.title || `Plan ${plan.id}`,
        project,
        projectGroup,
        studentName: plan.ownerName,
        status: plan.stage,
        stagePermissions: plan.permissions,
        overviewPath: `/projects/${project.id}/plans/${plan.id}/overview`,
        note: {
          id: plan.noteId,
          overviewPath: `/projects/${project.id}/notes/${plan.noteId}/overview`,
        },

        subRows: [],
      })),
      ...reports.map((report) => ({
        dataType: EXPERIMENT_DATA_TYPES.Report,
        id: report.id,
        title: report?.title || `Report ${report.id}`,
        project,
        projectGroup,
        studentName: report.ownerName,
        status: report.stage,
        stagePermissions: report.permissions,
        overviewPath: `/projects/${project.id}/reports/${report.id}/overview`,
      })),
    ],
    [plans, project]
  );

  return { tableData: tableData ?? [] };
};
