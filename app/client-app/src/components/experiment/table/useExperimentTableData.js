import { useMemo } from "react";
import { EXPERIMENT_DATA_TYPES } from "./experiment-data-types";

/**
 * Hook to generate table data from projectSummary and project
 * @param {*} projectSummary - projectSummary object contains plans, reports, and literature review
 * @param {*} project
 * @returns
 */

export const useExperimentTableData = (projectSummary, project) => {
  const { plans } = projectSummary ?? { plans: [] };
  const tableData = useMemo(
    () => [
      {
        dataType: EXPERIMENT_DATA_TYPES.LiteratureReview,
        title: "Literature review placeholder", // TODO: replace with actual literature review data here.
      },
      ...plans.map((plan) => ({
        dataType: EXPERIMENT_DATA_TYPES.Plan,
        id: plan.id,
        title: `Plan ${plan.id}`,
        project: project,
        projectGroups: project.projectGroups.find(
          (pg) => pg.id === plan.projectGroupId
        ),
        studentName: plan.ownerName,
        status: plan.stage,

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
