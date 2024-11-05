import { Button, HStack, Icon, Text, useDisclosure } from "@chakra-ui/react";
import { useProjectsList } from "api";
import { DataTable, DataTableGlobalFilter } from "components/core/data-table";
import { useIsInstructor, useCanManageProject } from "helpers/hooks";
import { CreateOrEditProjectModal } from "components/project/modal";
import { columns } from "components/project/table";
import { TITLE_ICON_COMPONENTS, STAGES } from "constants";
import {
  DefaultContentHeader,
  DefaultContentLayout,
} from "layouts/DefaultLayout";
import { useMemo, useState } from "react";
import { FaPlus } from "react-icons/fa";

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
      <DataTable
        data={tableData}
        globalFilter={searchValue}
        columns={columns(useCanManageProject())}
      >
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
  const { isOpen, onOpen, onClose } = useDisclosure();
  return (
    <>
      <Button
        onClick={onOpen}
        colorScheme="green"
        leftIcon={<FaPlus />}
        size="sm"
        isDisabled
      >
        <Text fontSize="sm" fontWeight="semibold">
          New project
        </Text>
      </Button>

      <CreateOrEditProjectModal isModalOpen={isOpen} onModalClose={onClose} />
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
        targetPath: isInstructor
          ? `/projects/${project.id}/project-groups` // for instructors
          : `/projects/${project.id}`, // for students
      })),
    [projects]
  );
  return { tableData: tableData ?? [] };
};
