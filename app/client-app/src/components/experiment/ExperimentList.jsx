import {
  HStack,
  Heading,
  Icon,
  VStack,
  Text,
  Button,
  useDisclosure,
} from "@chakra-ui/react";
import { useState } from "react";
import { useProjectsList } from "api/projects";
import { usePlansList } from "api/plans";
import { ExperimentLayout } from "./ExperimentLayout";
import { useIsInstructor } from "./useIsInstructor";
import { useExperimentTableData } from "./table/useExperimentTableData";
import { experimentColumns } from "./table/experimentColumns";
import { CreateOrEditPlanModal } from "./modal/CreateOrEditPlanModal";
import { FaLayerGroup, FaPlus } from "react-icons/fa";
import { NotFound } from "pages/error/NotFound";
import { DataTable } from "components/dataTable/DataTable";
import { DataTableSearchBar } from "components/dataTable/DataTableSearchBar";

export const ExperimentList = ({ projectId, plans }) => {
  const { data: projects } = useProjectsList();
  const [searchValue, setSearchValue] = useState("");
  const project = projects.find((x) => x.id.toString() === projectId);
  const isInstructor = useIsInstructor();
  const { tableData } = useExperimentTableData(plans, project);

  return (
    <WithValidProjectId projectId={projectId} projects={projects}>
      <ExperimentLayout>
        <HStack my={2} w="100%" justifyContent="space-between">
          <ExperimentHeading project={project} />
        </HStack>
        <DataTable
          data={tableData}
          globalFilter={searchValue}
          columns={experimentColumns(isInstructor)}
        >
          <HStack flex={1} justifyContent="flex-start">
            <DataTableSearchBar
              searchValue={searchValue}
              setSearchValue={setSearchValue}
              placeholder="Search"
            />
            {!isInstructor && <NewPlan project={project} />}
          </HStack>
        </DataTable>
      </ExperimentLayout>
    </WithValidProjectId>
  );
};

const WithValidProjectId = ({ projectId, projects, children }) => {
  const isValidProject = projects.some(
    (project) => project.id.toString() === projectId
  );

  return isValidProject ? children : <NotFound />;
};

const ExperimentHeading = ({ project }) => (
  <VStack align="start">
    <Heading as="h2" size="md" fontWeight="semibold" color="blue.600">
      <Icon as={FaLayerGroup} /> {project?.name}
    </Heading>
  </VStack>
);

const NewPlan = ({ project }) => {
  const { data: plans } = usePlansList(project?.id);
  const { isOpen, onOpen, onClose } = useDisclosure();
  return (
    <>
      <Button
        onClick={onOpen}
        colorScheme="green"
        leftIcon={<FaPlus />}
        size="sm"
      >
        <Text fontSize="sm" fontWeight="semibold">
          {plans?.length === 0 ? "Start planning" : "New plan"}
        </Text>
      </Button>

      <CreateOrEditPlanModal
        isModalOpen={isOpen}
        onModalClose={onClose}
        project={project}
      />
    </>
  );
};
