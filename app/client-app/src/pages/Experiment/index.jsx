import {
  Stack,
  HStack,
  Heading,
  VStack,
  Text,
  Input,
  InputGroup,
  InputLeftElement,
  Button,
  useDisclosure,
} from "@chakra-ui/react";
import { FaPlus, FaSearch } from "react-icons/fa";
import { useMemo, useEffect } from "react";
import { useExperimentsList } from "api/experiments";
import { useProjectsList } from "api/projects";
import { BasicTable } from "components/BasicTable";
import { CreateExperimentModal as NewExperimentModal } from "components/experiment/modal/CreateExperimentModal";
import { ExperimentColumns } from "components/experiment/ExperimentColumns";
import { useNavigate, useParams } from "react-router-dom";

export const Experiment = () => {
  const navigate = useNavigate();
  const { projectId } = useParams();
  const { data: projects } = useProjectsList();

  useEffect(() => {
    const isValidProject = projects.some(
      (project) => project.id.toString() === projectId
    );
    !isValidProject &&
      navigate("/", {
        state: {
          toast: {
            title: "Redirecting as invalid Project Id supplied.",
            status: "error",
            duration: 1500,
            isClosable: true,
          },
        },
      });
  }, []);

  const project = projects.find(
    (project) => project.id.toString() === projectId
  );

  const { data: experiments } = useExperimentsList();

  const experimentData = useMemo(
    () =>
      // filter by projectId (projectId validated above)
      experiments
        ?.filter((experiment) => experiment.projectId.toString() === projectId)
        .map((experiment) => ({
          id: experiment.id,
          title: experiment.title,
          project: experiment.projectName,
        })),
    [experiments]
  );

  const NewExperimentState = useDisclosure();

  const ExperimentHeader = () => (
    <HStack my={2} w="100%" justifyContent="space-between">
      <VStack align="start">
        <Heading as="h2" size="md" fontWeight="semibold" color="blue.600">
          {project?.name}
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
              // TODO: implement search
            />
          </InputGroup>
        </HStack>
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
        {NewExperimentState.isOpen && (
          <NewExperimentModal
            isModalOpen={NewExperimentState.isOpen}
            onModalClose={NewExperimentState.onClose}
            projectGroup={
              project.projectGroups[0] // first project group as student can only be in one project group
            }
          />
        )}
      </HStack>
    </HStack>
  );

  return (
    <Stack align="stretch" w="100%" alignItems="center">
      <VStack
        m={4}
        p={4}
        align="stretch"
        minW={{ base: "95%", md: "85%", lg: "75%", xl: "60%" }}
        spacing={4}
      >
        <VStack
          align="flex-start"
          borderWidth={1}
          px={5}
          py={2}
          borderRadius={7}
          spacing={4}
        >
          <ExperimentHeader />
          <BasicTable data={experimentData} columns={ExperimentColumns} />
        </VStack>
      </VStack>
    </Stack>
  );
};
