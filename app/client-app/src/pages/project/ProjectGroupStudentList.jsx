import {
  Button,
  Heading,
  HStack,
  Icon,
  Text,
  VStack,
  Stack,
} from "@chakra-ui/react";
import { DataTable } from "components/dataTable/DataTable";
import { DataTableSearchBar } from "components/dataTable/DataTableSearchBar";
import { DefaultContentLayout } from "layouts/DefaultLayout";
import { useMemo, useState } from "react";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";

import { useParams, useNavigate } from "react-router-dom";
import { FaUsers } from "react-icons/fa";
import { Breadcrumbs } from "components/Breadcrumbs";
import { buildProjectPath } from "routes/Project";
import { useProjectGroup } from "api/projectGroups";
import { projectGroupStudentColumns } from "components/project/table/projectGroupStudentColumns";

export const ProjectGroupStudentList = () => {
  const { projectId, projectGroupId } = useParams();
  const [searchValue, setSearchValue] = useState("");
  const navigate = useNavigate();
  const { tableData, projectGroup } = usePGStudentTableData(projectGroupId);

  const breadcrumbItems = [
    { label: "Home", href: "/" },
    { label: "Projects", href: "/projects" },
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
          <VStack align="start">
            <Heading as="h2" size="md" fontWeight="semibold" color="blue.600">
              <Icon as={TITLE_ICON_COMPONENTS.Students} /> Project Group
              Students
            </Heading>
          </VStack>
          <Button
            onClick={() =>
              navigate(
                `/projects/${projectId}/project-groups/${projectGroupId}/activities`
              )
            }
            colorScheme="gray"
            leftIcon={<FaUsers />}
            size="sm"
            variant="outline"
          >
            <Text fontSize="sm" fontWeight="semibold">
              Project Group Activities
            </Text>
          </Button>
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
  const tableData = useMemo(
    () =>
      projectGroup.students.map((student) => ({
        targetPath: buildProjectPath(projectGroup.projectId, false, student.id),
        studentId: student.id,
        name: student.name,
        studentEmail: student.email,
      })),

    [projectGroup]
  );
  return { tableData: tableData ?? [], projectGroup };
};
