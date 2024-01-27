import { useMemo } from "react";

/**
 * Hook to generate table data from plans and project
 * @param {*} plans
 * @param {*} project
 * @returns
 */

export const useExperimentTableData = (plans, project) => {
  const tableData = useMemo(
    () =>
      plans?.map((plan) => ({
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
    [plans, project]
  );

  return { tableData: tableData ?? [] };
};
