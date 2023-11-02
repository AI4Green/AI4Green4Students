import { Layout } from "components/experiment/Layout";
import {
  HStack,
  Heading,
  VStack,
  Text,
  Icon,
  IconButton,
} from "@chakra-ui/react";
import { FaFlask, FaCheckCircle, FaEdit } from "react-icons/fa";

import { useSectionsList } from "api/section";
import { useExperiment } from "api/experiments";
import { Link, useParams } from "react-router-dom";
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

export const Overview = () => {
  const { experimentId } = useParams();
  const { data: experiment } = useExperiment(experimentId);
  const { data: sections } = useSectionsList(experiment.projectId); // get sections related to the project

  const Section = ({ section }) => {
    const { name, sortOrder, approved, comments } = section;
    return (
      <HStack
        w="full"
        borderBottomWidth={1}
        py={1}
        px={5}
        borderRadius={5}
        gap={2}
        _hover={{
          bg: "gray.50",
        }}
      >
        <Text>{sortOrder}</Text>
        <VStack align="start" spacing={0.2}>
          <Heading as="h4" size="md">
            {name}
          </Heading>
        </VStack>

        <HStack justifyContent="flex-end" flex={1}>
          <Link
            to={`/experiments/project/${experiment.projectId}/experiment/${experiment.id}/plansection/${section.id}`} // Path to the experiment plan section
          >
            {comments >= 1 ? (
              <NotificationBadge count={comments > 9 ? "9+" : comments} />
            ) : approved ? (
              <IconButton
                isRound
                variant="ghost"
                aria-label="Approved"
                size="lg"
                icon={<Icon as={FaCheckCircle} color="green.500" boxSize={5} />}
              />
            ) : (
              <IconButton
                isRound
                variant="ghost"
                aria-label="Incomplete/Unapproved"
                size="lg"
                icon={<Icon as={FaEdit} boxSize={5} color="gray.600" />}
              />
            )}
          </Link>
        </HStack>
      </HStack>
    );
  };

  return (
    <Layout>
      <Header
        experimentTitle={experiment.title}
        projectName={experiment.projectName}
        header="Experiment Overview"
      />
      <VStack w="lg">
        {sections?.length === 0 ? (
          <Text fontSize="lg">No sections available</Text>
        ) : (
          sections
            .sort((a, b) => a.sortOrder - b.sortOrder)
            .map((section) => (
              <Section
                key={section.id}
                section={section}
                projectId={experiment.projectId}
              />
            ))
        )}
      </VStack>
    </Layout>
  );
};
