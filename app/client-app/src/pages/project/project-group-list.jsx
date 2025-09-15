import { Button, HStack, Icon, Text } from "@chakra-ui/react";
import { useProject, useProjectGroupsList } from "api";
import { Breadcrumbs } from "components/core/breadcrumbs";
import { DataTable, DataTableGlobalFilter } from "components/core/data-table";
import { CreateOrEditProjectGroupModal } from "components/project-group/modal";
import { columns } from "components/project-group/table";
import {
  PROJECTMANAGEMENT_PERMISSIONS,
  TITLE_ICON_COMPONENTS,
} from "constants";
import { useUser } from "contexts";
import { DefaultContentHeader, DefaultContentLayout } from "layouts/default";
import { useMemo, useState } from "react";
import { FaPlus } from "react-icons/fa";
import { useParams, useSearchParams } from "react-router-dom";
import { buildProjectPath } from "routes/project";

export const ProjectGroupList = () => {
  const { user } = useUser();
  const { projectId } = useParams();
  const { data: project } = useProject(projectId);
  const { tableData } = useProjectGroupTableData(projectId, project);
  const [searchValue, setSearchValue] = useState("");

  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: project?.name,
    },
  ];

  return (
    <DefaultContentLayout>
      <Breadcrumbs items={breadcrumbItems} />
      <HStack my={2} w="100%" justifyContent="space-between">
        <DefaultContentHeader
          header="Project Groups and
          Students"
          icon={<Icon as={TITLE_ICON_COMPONENTS.ProjectGroup} />}
        />
      </HStack>
      <DataTable data={tableData} globalFilter={searchValue} columns={columns}>
        <HStack flex={1} justifyContent="flex-start">
          <DataTableGlobalFilter
            searchValue={searchValue}
            setSearchValue={setSearchValue}
            placeholder="Search"
          />
          {user.permissions?.includes(
            PROJECTMANAGEMENT_PERMISSIONS.CreateProjects
          ) && <NewProjectGroup />}
        </HStack>
      </DataTable>
    </DefaultContentLayout>
  );
};

const NewProjectGroup = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const action = searchParams.get("action");
  return (
    <>
      <Button
        onClick={() => setSearchParams({ action: "new" })}
        colorScheme="green"
        leftIcon={<FaPlus />}
        size="sm"
      >
        <Text fontSize={{ base: "xs", md: "sm" }} fontWeight="semibold">
          New Project Group
        </Text>
      </Button>

      {action === "new" && <CreateOrEditProjectGroupModal />}
    </>
  );
};

/**
 * Hook to get the table data for listing project groups.
 * @returns {Object} - Object containing the table data
 */
const useProjectGroupTableData = (projectId) => {
  const { data: projectGroups } = useProjectGroupsList(projectId);
  const tableData = useMemo(
    () =>
      projectGroups?.map((pg) => ({
        id: pg.id,
        name: pg.name,
        startDate: pg.startDate,
        planningDeadline: pg.planningDeadline,
        experimentDeadline: pg.experimentDeadline,
        project: pg.project,
        subRows: pg.students.map((student) => ({
          targetPath: buildProjectPath(projectId, pg.id, student.id),
          id: student.id,
          name: student.name,
          email: student.email,
        })),
      })),
    [projectGroups, projectId]
  );
  return { tableData: tableData ?? [] };
};
