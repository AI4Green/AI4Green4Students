import { Button, HStack, Icon, Text, useDisclosure } from "@chakra-ui/react";
import { useProjectGroupsList } from "api/projectGroups";
import { useProject } from "api/projects";
import { DataTable } from "components/dataTable/DataTable";
import { DataTableSearchBar } from "components/dataTable/DataTableSearchBar";
import { CreateOrEditProjectGroupModal } from "components/project/modal/CreateOrEditProjectGroupModal";
import { projectGroupColumns } from "components/project/table/projectGroupColumns";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { DefaultContentLayout } from "layouts/DefaultLayout";
import { useMemo, useState } from "react";
import { FaPlus } from "react-icons/fa";
import { useParams } from "react-router-dom";
import { useCanManageProjects } from "./ProjectList";
import { Breadcrumbs } from "components/Breadcrumbs";
import { buildProjectPath } from "routes/Project";
import { DefaultContentHeader } from "layouts/DefaultLayout";

export const ProjectGroupList = () => {
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
      <DataTable
        data={tableData}
        globalFilter={searchValue}
        columns={projectGroupColumns}
      >
        <HStack flex={1} justifyContent="flex-start">
          <DataTableSearchBar
            searchValue={searchValue}
            setSearchValue={setSearchValue}
            placeholder="Search"
          />
          {useCanManageProjects() && <NewProjectGroup project={project} />}
        </HStack>
      </DataTable>
    </DefaultContentLayout>
  );
};

const NewProjectGroup = ({ project }) => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  return (
    <>
      <Button
        onClick={onOpen}
        colorScheme="green"
        leftIcon={<FaPlus />}
        size="sm"
      >
        <Text fontSize={{ base: "xs", md: "sm" }} fontWeight="semibold">
          New Project Group
        </Text>
      </Button>

      <CreateOrEditProjectGroupModal
        isModalOpen={isOpen}
        onModalClose={onClose}
        project={project}
      />
    </>
  );
};

/**
 * Hook to get the table data for listing project groups.
 * @returns {Object} - Object containing the table data
 */
const useProjectGroupTableData = (projectId, project) => {
  const { data: projectGroups } = useProjectGroupsList(projectId);
  const tableData = useMemo(
    () =>
      projectGroups?.map((pg) => ({
        id: pg.id,
        name: pg.name,
        project,
        subRows: pg.students.map((student) => ({
          targetPath: buildProjectPath(projectId, pg.id, student.id),
          studentId: student.id,
          name: student.name,
          studentEmail: student.email,
        })),
      })),
    [projectGroups]
  );
  return { tableData: tableData ?? [] };
};
