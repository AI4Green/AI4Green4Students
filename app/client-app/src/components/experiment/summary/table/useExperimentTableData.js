import { useMemo } from "react";

/**
 * Hook to generate table data from projectSummary and project
 * @param {*} projectSummary - projectSummary object contains plans, reports, and literature review
 * @param {*} project
 * @returns
 */
export const useExperimentTableData = (projectSummary, project) => {
  const {
    sectionTypes: {
      planSectionTypeId,
      noteSectionTypeId,
      literatureReviewSectionTypeId: lrSectionTypeId,
    },
  } = project;
  const projectGroup = project.projectGroups.find(
    (pg) => pg.id === projectSummary.projectGroupId
  );
  const { plans } = projectSummary ?? { plans: [] };
  const { literatureReviews } = projectSummary ?? { literatureReviews: [] };

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
        overviewPath: `/project/${project.id}/project-group/${projectGroup.id}/section-type/${lrSectionTypeId}/literature-review/${literatureReview.id}/overview`,
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
        overviewPath: `/project/${project.id}/project-group/${projectGroup.id}/section-type/${planSectionTypeId}/plan/${plan.id}/overview`,
        note: {
          id: plan.noteId,
          overviewPath: `/project/${project.id}/project-group/${projectGroup.id}/section-type/${noteSectionTypeId}/note/${plan.noteId}/overview`,
        },

        subRows: [],
      })),
    ],
    [plans, project]
  );

  return { tableData: tableData ?? [] };
};

export const EXPERIMENT_DATA_TYPES = {
  LiteratureReview: "LiteratureReview",
  Plan: "Plan",
  Report: "Report",
};
