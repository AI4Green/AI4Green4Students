import {
  HStack,
  Heading,
  VStack,
  Text,
  Icon,
  IconButton,
} from "@chakra-ui/react";
import { FaCheckCircle, FaEdit } from "react-icons/fa";
import { useExperimentsList } from "api/experiments";
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

export const Overview = ({ sections, header, subHeader, overview }) => {
  const { experimentId } = useParams();
  const { data: experiments } = useExperimentsList();
  const isValidExperimentId = experiments.some(
    (item) => item.id.toString() === experimentId
  );

  if (!isValidExperimentId) return <NotFound />;

  return (
    <Layout>
      <Header header={header} subHeader={subHeader} overview={overview} />
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
