import { useParams } from "react-router-dom";
import { Summary } from ".";
import { useProjectSummaryByStudent } from "api/projects";
import { useMemo } from "react";
import { SECTION_TYPES as EXPERIMENT_DATA_TYPES } from "constants/section-types";
import { buildOverviewPath } from "routes/Project";

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
  const { LiteratureReview, Plan, Note, Report } = EXPERIMENT_DATA_TYPES;
  const { data: summary } = useProjectSummaryByStudent(projectId, studentId);
  const { plans, project, projectGroup } = summary;

  const tableData = useMemo(
    () =>
      plans.map((plan) => ({
        dataType: Plan,
        id: plan.id,
        title: plan?.title || `Plan ${plan.id}`,
        project,
        projectGroup,
        ownerId: plan.ownerId,
        ownerName: plan.ownerName,
        stage: plan.stage,
        permissions: plan.permissions,
        targetPath: buildOverviewPath(
          Plan,
          project.id,
          projectGroup.id,
          plan.id
        ),

        note: {
          id: plan.note.id,
          stage: plan.note.stage,
          permissions: plan.note.permissions,
          targetPath: buildOverviewPath(
            Note,
            project.id,
            projectGroup.id,
            plan.note.id
          ),
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
        projectGroup,
      },
      reports: summary.reports.map((report) => ({
        ...report,
        overviewPath: buildOverviewPath(
          Report,
          project.id,
          projectGroup.id,
          report.id
        ),
        project,
      })),
      literatureReviews: summary.literatureReviews.map((literatureReview) => ({
        ...literatureReview,
        overviewPath: buildOverviewPath(
          LiteratureReview,
          project.id,
          projectGroup.id,
          literatureReview.id
        ),
        project,
      })),
    },
  };
};
