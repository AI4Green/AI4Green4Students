import { Button, HStack, Icon, Text } from "@chakra-ui/react";
import { useProjectsList } from "api";
import { DataTable, DataTableGlobalFilter } from "components/core/data-table";
import { CreateOrEditProjectModal } from "components/project/modal";
import { columns } from "components/project/table";
import { STAGES, TITLE_ICON_COMPONENTS } from "constants";
import { useCanManageProject, useIsInstructor } from "helpers/hooks";
import { DefaultContentHeader, DefaultContentLayout } from "layouts/default";
import { useMemo, useState } from "react";
import { FaPlus } from "react-icons/fa";
import { Link, useSearchParams } from "react-router-dom";

export const ProjectList = () => {
  const { tableData } = useProjectTableData();
  const [searchValue, setSearchValue] = useState("");

  return (
    <DefaultContentLayout>
      <HStack my={2} w="100%" justifyContent="space-between">
        <DefaultContentHeader
          header="Projects"
          icon={<Icon as={TITLE_ICON_COMPONENTS.Project} />}
        />
      </HStack>
      <DataTable data={tableData} globalFilter={searchValue} columns={columns}>
        <HStack flex={1} justifyContent="flex-start">
          <DataTableGlobalFilter
            searchValue={searchValue}
            setSearchValue={setSearchValue}
            placeholder="Search"
          />
          {useCanManageProject() && <NewProject />}
        </HStack>
      </DataTable>
    </DefaultContentLayout>
  );
};

const NewProject = () => {
  const [searchParams] = useSearchParams();
  const action = searchParams.get("action");

  return (
    <>
      <Button
        as={Link}
        to="/projects?action=new"
        colorScheme="green"
        leftIcon={<FaPlus />}
        size="sm"
      >
        <Text fontSize="sm" fontWeight="semibold">
          New project
        </Text>
      </Button>

      {action === "new" && <CreateOrEditProjectModal />}
    </>
  );
};

/**
 * Hook to get the table data for listing projects.
 * @returns {Object} - Object containing the table data
 */
const useProjectTableData = () => {
  const { data: projects } = useProjectsList();
  const isInstructor = useIsInstructor();
  const tableData = useMemo(
    () =>
      projects?.map((project) => ({
        id: project.id,
        name: project.name,
        status: project.status || STAGES.OnGoing,
        projectType: project.projectType,
        targetPath: isInstructor
          ? `/projects/${project.id}/project-groups` // for instructors
          : `/projects/${project.id}`, // for students
      })),
    [isInstructor, projects]
  );
  return { tableData: tableData ?? [] };
};
