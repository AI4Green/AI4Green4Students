import { useParams } from "react-router-dom";
import { Summary } from ".";
import { useProjectSummaryByStudent } from "api/projects";
import { useMemo } from "react";
import { SECTION_TYPES as EXPERIMENT_DATA_TYPES } from "constants/section-types";

export const StudentExperimentList = () => {
  const { projectId, studentId } = useParams();
  const { tableData, summary } = useSummaryData(projectId, studentId);

  return (
    <Summary
      projectSummary={summary}
      tableData={tableData}
      studentId={studentId}
    />
  );
};

/**
 * Hook to get the project summary.
 * @param {*} projectId
 * @returns {Object} - tableData and summary
 * - tableData: array of plan objects containing the data for the table
 * - summary: project summary
 */
const useSummaryData = (projectId, studentId) => {
  const { data: summary } = useProjectSummaryByStudent(projectId, studentId);

  const { plans, project, projectGroup } = summary;

  const tableData = useMemo(
    () =>
      plans.map((plan) => ({
        dataType: EXPERIMENT_DATA_TYPES.Plan,
        id: plan.id,
        title: plan?.title || `Plan ${plan.id}`,
        project,
        projectGroup,
        ownerId: plan.ownerId,
        ownerName: plan.ownerName,
        stage: plan.stage,
        permissions: plan.permissions,
        targetPath: `/projects/${project.id}/plans/${plan.id}/overview`,

        note: {
          id: plan.noteId,
          targetPath: `/projects/${project.id}/notes/${plan.noteId}/overview`,
        },
      })),
    [plans]
  );
  return {
    tableData: tableData ?? [],
    summary: {
      ...summary,
      project: {
        ...summary.project,
        projectGroup: projectGroup,
      },
      reports: summary.reports.map((report) => ({
        ...report,
        overviewPath: `/projects/${project.id}/reports/${report.id}/overview`,
        project,
      })),
      literatureReviews: summary.literatureReviews.map((literatureReview) => ({
        ...literatureReview,
        overviewPath: `/projects/${project.id}/literature-reviews/${literatureReview.id}/overview`,
        project,
      })),
    },
  };
};
