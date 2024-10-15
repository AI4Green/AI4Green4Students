import { HStack, Icon, Stack } from "@chakra-ui/react";
import { DataTable } from "components/dataTable/DataTable";
import { DataTableSearchBar } from "components/dataTable/DataTableSearchBar";
import { DefaultContentLayout } from "layouts/DefaultLayout";
import { useMemo, useState } from "react";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { useParams } from "react-router-dom";
import { Breadcrumbs } from "components/Breadcrumbs";
import { buildProjectPath } from "routes/Project";
import { useProjectGroup } from "api/projectGroups";
import { projectGroupStudentColumns } from "components/project/table/projectGroupStudentColumns";
import { useUser } from "contexts/User";
import { ProjectGroup } from "pages/experiment/summary";
import { DefaultContentHeader } from "layouts/DefaultLayout";

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
        columns={projectGroupStudentColumns}
      >
        <HStack flex={1} justifyContent="flex-start">
          <DataTableSearchBar
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

    [projectGroup]
  );
  return { tableData: tableData ?? [], projectGroup };
};
