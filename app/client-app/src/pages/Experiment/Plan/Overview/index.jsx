import {
  HStack,
  Heading,
  VStack,
  Text,
  Icon,
  IconButton,
  Avatar,
} from "@chakra-ui/react";
import { FaCheckCircle, FaEdit } from "react-icons/fa";
import { useExperiment } from "api/experiments";
import { Link, useParams } from "react-router-dom";
import { NotFound } from "pages/error/NotFound";
import { Layout } from "components/experiment/Layout";
import { NotificationBadge } from "components/NotificationBadge";
import { Header } from "components/experiment/section/Header";

const Section = ({ section, experimentId, index }) => {
  const { id, name, approved, comments } = section;

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
      <Text>{index + 1}</Text>
      <VStack align="start" spacing={0.2}>
        <Heading as="h4" size="md">
          {name}
        </Heading>
      </VStack>

      <HStack justifyContent="flex-end" flex={1}>
        {comments >= 1 && !approved ? (
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

export const Overview = ({ sections, header, subHeader, overviewTitle }) => {
  const { experimentId } = useParams();
  const { data: experiment } = useExperiment(experimentId);

  if (!experiment) return <NotFound />;

  const ExperimentAuthor = () => (
    <HStack pb={2}>
      <Avatar name={experiment?.ownerName} size="sm" />
      <Text fontSize="md" color="gray.600">
        {experiment?.ownerName}
      </Text>
    </HStack>
  );

  return (
    <Layout>
      <Header
        header={header}
        subHeader={subHeader}
        overviewTitle={overviewTitle}
        actionSection={<ExperimentAuthor />}
      />
      <VStack w="lg">
        {sections && sections.length >= 1 ? (
          sections
            .sort((a, b) => a.sortOrder - b.sortOrder)
            .map((section, index) => (
              <Section
                key={section.id}
                section={section}
                experimentId={experimentId}
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
