import { useMemo } from "react";

export const useProjectManagementTableData = (projects, projectGroups) => {
  const tableData = useMemo(
    () =>
      projects?.map((project) => {
        const relatedGroups = projectGroups?.filter(
          (relatedGroup) => relatedGroup.projectId === project.id
        ); // get related project groups

        return {
          id: project.id,
          name: project.name,
          projectGroupNumber: relatedGroups.length ?? 0,
          startDate: project.startDate,
          planningDeadline: project.planningDeadline,
          experimentDeadline: project.experimentDeadline,

          subRows:
            relatedGroups?.map((group) => ({
              id: group.id,
              name: group.name,
              studentNumber: group.students.length,

              subRows: group.students.map((student) => ({
                studentId: student.id,
                name: student.name,
                studentEmail: student.email,
              })),
            })) || [], // return empty if no related groups
        };
      }),
    [projects, projectGroups]
  );

  return { tableData: tableData ?? [] };
};
