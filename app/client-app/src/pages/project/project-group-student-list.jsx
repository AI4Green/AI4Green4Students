import { HStack, Icon, Stack } from "@chakra-ui/react";
import { useProjectGroup } from "api";
import { Breadcrumbs } from "components/core/breadcrumbs";
import { DataTable, DataTableGlobalFilter } from "components/core/data-table";
import { ProjectGroup } from "components/project-group/project-group";
import { studentColumns } from "components/project-group/table";
import { TITLE_ICON_COMPONENTS } from "constants";
import { useUser } from "contexts";
import { DefaultContentHeader, DefaultContentLayout } from "layouts/default";
import { useMemo, useState } from "react";
import { useParams } from "react-router-dom";
import { buildProjectPath } from "routes/project";

export const ProjectGroupStudentList = () => {
  const { projectId, projectGroupId } = useParams();
  const [searchValue, setSearchValue] = useState("");
  const { tableData, projectGroup } = usePGStudentTableData(projectGroupId);

  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: projectGroup?.projectName,
      href: buildProjectPath(projectId),
    },
    {
      label: projectGroup?.name,
    },
  ];

  return (
    <DefaultContentLayout>
      <Stack spacing={4} mb={4}>
        <Breadcrumbs items={breadcrumbItems} />
        <HStack w="100%" justifyContent="space-between" align="center">
          <DefaultContentHeader
            header="Project Group Students"
            icon={<Icon as={TITLE_ICON_COMPONENTS.Students} />}
          />
          <ProjectGroup
            projectId={projectId}
            projectGroupId={projectGroupId}
            isViewingActivities={true}
          />
        </HStack>
      </Stack>
      <DataTable
        data={tableData}
        globalFilter={searchValue}
        columns={studentColumns}
      >
        <HStack flex={1} justifyContent="flex-start">
          <DataTableGlobalFilter
            searchValue={searchValue}
            setSearchValue={setSearchValue}
            placeholder="Search"
          />
        </HStack>
      </DataTable>
    </DefaultContentLayout>
  );
};

/**
 * Hook to get the table data for listing project groups.
 * @returns {Object} - Object containing the table data
 */
const usePGStudentTableData = (projectGroupId) => {
  const { data: projectGroup } = useProjectGroup(projectGroupId);
  const { user } = useUser();
  const tableData = useMemo(
    () =>
      projectGroup.students
        .filter((student) => student.id != user.userId) // filter out the current user
        .map((student) => ({
          targetPath: buildProjectPath(
            projectGroup.projectId,
            projectGroup.id,
            student.id
          ),
          studentId: student.id,
          name: student.name,
          studentEmail: student.email,
        })),

    [projectGroup, user.userId]
  );
  return { tableData: tableData ?? [], projectGroup };
};
