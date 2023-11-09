import {
  HStack,
  Heading,
  VStack,
  Text,
  Icon,
  IconButton,
} from "@chakra-ui/react";
import { FaFlask, FaCheckCircle, FaEdit } from "react-icons/fa";
import { Suspense } from "react";
import { useSectionsList } from "api/section";
import { useExperiment, useExperimentsList } from "api/experiments";
import { Link, useParams } from "react-router-dom";
import { NotFound } from "pages/error/NotFound";
import { BusyPage } from "components/Busy";
import { Layout } from "components/experiment/Layout";
import { NotificationBadge } from "components/NotificationBadge";

export const Header = ({ experimentTitle, projectName, header }) => (
  <HStack w="full" borderBottomWidth={1}>
    <VStack align="start" my={2}>
      <Heading as="h2" size="md" fontWeight="semibold" color="green.600">
        <Icon as={FaFlask} /> {experimentTitle}
      </Heading>
      <HStack spacing={2}>
        <Heading as="h2" size="xs" fontWeight="semibold">
          {projectName}
        </Heading>
      </HStack>
    </VStack>
    <VStack align="end" my={2} flex={1}>
      <Heading as="h2" size="lg" fontWeight="semibold" color="blue.600">
        {header}
      </Heading>
    </VStack>
  </HStack>
);

const Section = ({ section, experimentId, index }) => {
  const { id, name, sortOrder, approved, comments } = section;
  return (
    <HStack
      w="full"
      borderBottomWidth={1}
      p={2}
      borderRadius={5}
      gap={2}
      _hover={{
        bg: "gray.50",
      }}
    >
      <Text>{sortOrder || index + 1}</Text>
      <VStack align="start" spacing={0.2}>
        <Heading as="h4" size="md">
          {name}
        </Heading>
      </VStack>

      <HStack justifyContent="flex-end" flex={1}>
        {comments >= 1 && !approved ? (
          <IconButton
            as={Link}
            to={`/experiments/${experimentId}/plansection/${id}`}
            isRound
            variant="ghost"
            aria-label="Notification"
            size="lg"
            icon={<NotificationBadge count={comments > 9 ? "9+" : comments} />}
          />
        ) : approved ? (
          <IconButton
            isRound
            variant="ghost"
            aria-label="Incomplete/Unapproved"
            size="lg"
            icon={<Icon as={FaCheckCircle} color="green.500" boxSize={5} />}
          />
        ) : (
          <IconButton
            as={Link}
            to={`/experiments/${experimentId}/plansection/${id}`}
            isRound
            variant="ghost"
            aria-label="Incomplete/Unapproved"
            size="lg"
            icon={<Icon as={FaEdit} boxSize={5} color="gray.600" />}
          />
        )}
      </HStack>
    </HStack>
  );
};

const ExperimentOverview = ({ experimentId }) => {
  const { data: experiments } = useExperimentsList();
  const isValidExperimentId = experiments.some(
    (item) => item.id.toString() === experimentId
  );

  if (!isValidExperimentId) return <NotFound />;

  const { data: experiment } = useExperiment(experimentId);
  const { data: sections } = useSectionsList(
    experiment.projectId,
    experimentId
  ); // get sections related to the project
  return (
    <Layout>
      <Header
        experimentTitle={experiment.title}
        projectName={experiment.projectName}
        header="Experiment Overview"
      />
      <VStack w="lg">
        {sections && sections.length >= 1 ? (
          sections
            .sort((a, b) => a.sortOrder - b.sortOrder)
            .map((section, index) => (
              <Section
                key={section.id}
                section={section}
                projectId={experiment.projectId}
                experimentId={experiment.id}
                index={index}
              />
            ))
        ) : (
          <Text fontSize="lg">No sections available</Text>
        )}
      </VStack>
    </Layout>
  );
};

export const Overview = () => {
  const { experimentId } = useParams();
  return (
    <Suspense fallback={<BusyPage />}>
      <ExperimentOverview experimentId={experimentId} />
    </Suspense>
  );
};
