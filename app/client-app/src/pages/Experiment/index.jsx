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
import { useExperimentsList } from "api/experiments";
import { useProjectsList } from "api/projects";
import { DataTable } from "components/dataTable/DataTable";
import { CreateOrEditExperimentModal as NewExperimentModal } from "components/experiment/modal/CreateOrEditExperimentModal";
import { experimentColumns } from "components/experiment/table/experimentColumns";
import { useParams } from "react-router-dom";
import { NotFound } from "pages/error/NotFound";
import { ExperimentLayout } from "components/experiment/ExperimentLayout";
import { useUser } from "contexts/User";
import { EXPERIMENTS_PERMISSIONS } from "constants/site-permissions";

const NewExperiment = ({ project }) => {
  const NewExperimentState = useDisclosure();
  return (
    <>
      <Button
        onClick={NewExperimentState.onOpen}
        colorScheme="green"
        leftIcon={<FaPlus />}
        size="sm"
      >
        <Text fontSize="sm" fontWeight="semibold">
          New Experiment
        </Text>
      </Button>

      <NewExperimentModal
        isModalOpen={NewExperimentState.isOpen}
        onModalClose={NewExperimentState.onClose}
        projectGroup={
          project.projectGroups[0] // first project group as student can only be in one project group
        }
      />
    </>
  );
};

const ExperimentHeader = ({
  project,
  experimentData,
  setSearchValue,
  searchValue,
  isInstructor,
}) => (
  <HStack my={2} w="100%" justifyContent="space-between">
    <VStack align="start">
      <Heading as="h2" size="md" fontWeight="semibold" color="blue.600">
        <Icon as={FaLayerGroup} /> {project?.name}
      </Heading>
      <HStack spacing={2}>
        <Heading as="h2" size="xs" fontWeight="semibold">
          Experiments
        </Heading>
        <Text fontSize="xs" color="gray" fontWeight="semibold">
          ({experimentData?.length})
        </Text>
      </HStack>
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
            placeholder="Search Experiments"
            _placeholder={{ opacity: 1 }}
            onChange={(e) => setSearchValue(e.target.value)}
            value={searchValue || ""}
          />
        </InputGroup>
      </HStack>
      {!isInstructor && <NewExperiment project={project} />}
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
  const { data: experiments } = useExperimentsList(project?.id);

  const experimentData = useMemo(
    () =>
      // filter by projectId (projectId validated above)
      experiments
        ?.filter((experiment) => experiment.projectId.toString() === projectId)
        .map((experiment) => ({
          id: experiment.id,
          title: experiment.title,
          project: project,
          projectGroups: project.projectGroups.find(
            (pg) => pg.id === experiment.projectGroupId
          ),
          studentName: experiment.ownerName,

          subRows: [
            {
              experimentId: experiment.id,
              title: "Overview",
              isOverview: true,
            },
            ...experiment.reactions.map((reaction) => ({
              projectId: project.id,
              experimentId: experiment.id,
              id: reaction.id,
              title: reaction.title,
              status: reaction.status,
              isReaction: true,
            })),
          ],
        })),
    [experiments]
  );

  const isInstructor = user.permissions?.includes(
    EXPERIMENTS_PERMISSIONS.ViewAllExperiments
  );

  return (
    <RequireValidProjectId projects={projects} id={projectId}>
      <ExperimentLayout>
        <ExperimentHeader
          project={project}
          experimentData={experimentData}
          setSearchValue={setSearchValue}
          searchValue={searchValue}
          isInstructor={isInstructor}
        />
        <DataTable
          data={experimentData}
          columns={experimentColumns(isInstructor)}
          globalFilter={searchValue}
        />
      </ExperimentLayout>
    </RequireValidProjectId>
  );
};
