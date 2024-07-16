import {
  Button,
  Heading,
  HStack,
  Icon,
  Text,
  useDisclosure,
  VStack,
} from "@chakra-ui/react";
import { useProjectsList } from "api/projects";
import { DataTable } from "components/dataTable/DataTable";
import { DataTableSearchBar } from "components/dataTable/DataTableSearchBar";
import { CreateOrEditProjectModal } from "components/project/modal/CreateOrEditProjectModal";
import { projectColumns } from "components/project/table/projectColumns";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { PROJECTMANAGEMENT_PERMISSIONS } from "constants/site-permissions";
import { STAGES } from "constants/stages";
import { useUser } from "contexts/User";
import { DefaultContentLayout } from "layouts/DefaultLayout";
import { useMemo, useState } from "react";
import { FaPlus } from "react-icons/fa";

export const ProjectList = () => {
  const { tableData } = useProjectTableData();
  const [searchValue, setSearchValue] = useState("");

  return (
    <DefaultContentLayout>
      <HStack my={2} w="100%" justifyContent="space-between">
        <VStack align="start">
          <Heading as="h2" size="md" fontWeight="semibold" color="blue.600">
            <Icon as={TITLE_ICON_COMPONENTS.Project} /> Projects
          </Heading>
        </VStack>
      </HStack>
      <DataTable
        data={tableData}
        globalFilter={searchValue}
        columns={projectColumns(useCanManageProjects())}
      >
        <HStack flex={1} justifyContent="flex-start">
          <DataTableSearchBar
            searchValue={searchValue}
            setSearchValue={setSearchValue}
            placeholder="Search"
          />
          {useCanManageProjects() && <NewProject />}
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
  const canManageProjects = useCanManageProjects();
  const tableData = useMemo(
    () =>
      projects?.map((project) => ({
        id: project.id,
        name: project.name,
        status: project.status || STAGES.OnGoing,
        targetPath: canManageProjects
          ? `/projects/${project.id}/project-groups` // for instructors
          : `/projects/${project.id}`, // for students
      })),
    [projects]
  );
  return { tableData: tableData ?? [] };
};

/**
 * Hook to check if the user can manage projects.
 * @returns {boolean}
 */
export const useCanManageProjects = () => {
  const permissions = Object.values(PROJECTMANAGEMENT_PERMISSIONS);
  const { user } = useUser();
  return permissions.every((permission) =>
    user?.permissions?.includes(permission)
  );
};
