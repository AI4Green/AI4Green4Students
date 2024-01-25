import {
  HStack,
  Heading,
  VStack,
  Text,
  Input,
  InputGroup,
  InputLeftElement,
  Button,
  useDisclosure,
  Icon,
} from "@chakra-ui/react";
import { FaPlus, FaSearch, FaLayerGroup } from "react-icons/fa";
import { useMemo, useState } from "react";
import { usePlansList } from "api/plans";
import { useProjectsList } from "api/projects";
import { DataTable } from "components/dataTable/DataTable";
import { CreateOrEditPlanModal } from "components/experiment/modal/CreateOrEditPlanModal";
import { experimentColumns } from "components/experiment/table/experimentColumns";
import { useParams } from "react-router-dom";
import { NotFound } from "pages/error/NotFound";
import { ExperimentLayout } from "components/experiment/ExperimentLayout";
import { useUser } from "contexts/User";
import { EXPERIMENTS_PERMISSIONS } from "constants/site-permissions";

const NewPlan = ({ project, plansNumber }) => {
  const NewPlanState = useDisclosure();
  return (
    <>
      <Button
        onClick={NewPlanState.onOpen}
        colorScheme="green"
        leftIcon={<FaPlus />}
        size="sm"
      >
        <Text fontSize="sm" fontWeight="semibold">
          {plansNumber === 0 ? "Start planning" : "New plan"}
        </Text>
      </Button>

      <CreateOrEditPlanModal
        isModalOpen={NewPlanState.isOpen}
        onModalClose={NewPlanState.onClose}
        project={project}
      />
    </>
  );
};

const ExperimentHeader = ({
  project,
  setSearchValue,
  searchValue,
  isInstructor,
  plansNumber,
}) => (
  <HStack my={2} w="100%" justifyContent="space-between">
    <VStack align="start">
      <Heading as="h2" size="md" fontWeight="semibold" color="blue.600">
        <Icon as={FaLayerGroup} /> {project?.name}
      </Heading>
    </VStack>

    <HStack flex={1} justifyContent="flex-end">
      <HStack>
        <InputGroup>
          <InputLeftElement pointerEvents="none" height="100%">
            <FaSearch color="gray" />
          </InputLeftElement>
          <Input
            variant="outline"
            borderColor="gray.400"
            size="sm"
            borderRadius={6}
            placeholder="Search plans"
            _placeholder={{ opacity: 1 }}
            onChange={(e) => setSearchValue(e.target.value)}
            value={searchValue || ""}
          />
        </InputGroup>
      </HStack>
      {!isInstructor && <NewPlan project={project} plansNumber={plansNumber} />}
    </HStack>
  </HStack>
);

const RequireValidProjectId = ({ projects, id, children }) => {
  const isValidProject = projects.some(
    (project) => project.id.toString() === id // project id captured from useParams()
  );
  return isValidProject ? children : <NotFound />;
};

export const Experiment = () => {
  const { user } = useUser();
  const { projectId } = useParams();
  const { data: projects } = useProjectsList();
  const [searchValue, setSearchValue] = useState("");
  const project = projects.find(
    (project) => project.id.toString() === projectId
  );
  const { data: plans } = usePlansList(project?.id);

  const tableData = useMemo(
    () =>
      // filter by projectId (projectId validated above)
      plans
        ?.filter((plan) => plan.projectId.toString() === projectId)
        .map((plan) => ({
          id: plan.id,
          title: `Plan ${plan.id}`,
          project: project,
          projectGroups: project.projectGroups.find(
            (pg) => pg.id === plan.projectGroupId
          ),
          studentName: plan.ownerName,

          subRows: [], // TODO: add subrows for plan's report. Each plan will have one report.
        })),
    [plans]
  );

  const isInstructor = user.permissions?.includes(
    EXPERIMENTS_PERMISSIONS.ViewAllExperiments
  );

  return (
    <RequireValidProjectId projects={projects} id={projectId}>
      <ExperimentLayout>
        <ExperimentHeader
          project={project}
          setSearchValue={setSearchValue}
          searchValue={searchValue}
          isInstructor={isInstructor}
          plansNumber={plans?.length ?? 0}
        />
        <DataTable
          data={tableData}
          columns={experimentColumns(isInstructor)}
          globalFilter={searchValue}
        />
      </ExperimentLayout>
    </RequireValidProjectId>
  );
};
