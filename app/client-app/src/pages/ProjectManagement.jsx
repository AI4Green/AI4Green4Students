import { HStack, Button, useDisclosure, Text } from "@chakra-ui/react";
import { useState } from "react";
import { useProjectsList } from "api/projects";
import { useProjectGroupsList } from "api/projectGroups";
import { DataTable } from "components/dataTable/DataTable";
import { useProjectManagementTableData } from "components/projectManagement/table/useProjectManagementTableData";
import { projectManagementColumns } from "components/projectManagement/table/projectManagementColumns";
import { DataTableSearchBar } from "components/dataTable/DataTableSearchBar";
import { FaPlus, FaProjectDiagram } from "react-icons/fa";
import { AdminLayout, AdminHeading } from "components/admin/AdminUI";
import { CreateOrEditProjectModal } from "components/projectManagement/modal/CreateOrEditProjectModal";

export const ProjectManagement = () => {
  const { data: projectGroups } = useProjectGroupsList();
  const { data: projects } = useProjectsList();
  const [searchValue, setSearchValue] = useState("");
  const { tableData } = useProjectManagementTableData(projects, projectGroups);

  return (
    <AdminLayout>
      <AdminHeading title="Project Management" icon={FaProjectDiagram} />
      <DataTable
        data={tableData}
        columns={projectManagementColumns}
        globalFilter={searchValue}
      >
        <HStack flex={1} justify="flex-start">
          <DataTableSearchBar
            searchValue={searchValue}
            setSearchValue={setSearchValue}
            placeholder="Search"
          />
          <NewProject />
        </HStack>
      </DataTable>
    </AdminLayout>
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
