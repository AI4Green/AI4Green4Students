import { useMemo } from "react";
import { EXPERIMENT_DATA_TYPES } from "./experiment-data-types";

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
  const { plans } = projectSummary ?? { plans: [] };
  const { literatureReviews } = projectSummary ?? { literatureReviews: [] };

  const tableData = useMemo(
    () => [
      ...literatureReviews.map((literatureReview) => ({
        dataType: EXPERIMENT_DATA_TYPES.LiteratureReview,
        id: literatureReview.id,
        title: `Literature review ${literatureReview.id}`,
        project: project,
        projectGroup: project.projectGroups.find(
          (pg) => pg.id === projectSummary.projectGroupId
        ),
        studentName: literatureReview.ownerName,
        status: literatureReview.stage,
        stagePermissions: literatureReview.permissions,
        overviewPath: `/project/${lrSectionTypeId}/literatureReview-overview/${literatureReview.id}`, // path to literature review overview page
      })),
      ...plans.map((plan) => ({
        dataType: EXPERIMENT_DATA_TYPES.Plan,
        id: plan.id,
        title: plan?.title || `Plan ${plan.id}`,
        project: project,
        projectGroup: project.projectGroups.find(
          (pg) => pg.id === projectSummary.projectGroupId
        ),
        studentName: plan.ownerName,
        status: plan.stage,
        stagePermissions: plan.permissions,
        overviewPath: `/project/${planSectionTypeId}/plan-overview/${plan.id}`,
        note: {
          id: plan.noteId,
          overviewPath: `/project/${noteSectionTypeId}/note-overview/${plan.noteId}`,
        },

        /**
         * TODO: add subrows for plan's report. Each plan will have one report.
         * Expecting report to be included in the plan object.
         **/

        subRows: [],
      })),
    ],
    [plans, project]
  );

  return { tableData: tableData ?? [] };
};
