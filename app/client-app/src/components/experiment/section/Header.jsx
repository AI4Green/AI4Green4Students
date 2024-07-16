import { HStack, Heading, VStack, Icon, Avatar, Text } from "@chakra-ui/react";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";

export const Header = ({
  header,
  projectName,
  overviewTitle,
  owner,
  icon,
  actionSection,
}) => (
  <HStack w="full" justify="space-between" borderBottomWidth={1} py={4}>
    <VStack gap={2} align="start">
      <Heading as="h2" size="md" fontWeight="semibold" color="gray.700">
        {header}
      </Heading>
      <HStack align="center" gap={2}>
        {owner && (
          <HStack>
            <Avatar name={owner} size="xs" />
            <Text fontSize="sm" fontWeight="semibold" color="gray.700">
              {owner}
            </Text>
          </HStack>
        )}

        <Text fontSize="sm" color="gray.600" fontWeight="semibold">
          <Icon as={TITLE_ICON_COMPONENTS.Project} /> Project - {projectName}
        </Text>
      </HStack>
    </VStack>

    <VStack align="end" gap={2}>
      <HStack align="baseline">
        <Icon as={icon} boxSize="5" color="blue.600" />
        <Heading as="h1" size="lg" fontWeight="semibold" color="blue.600">
          {overviewTitle}
        </Heading>
      </HStack>
      {actionSection}
    </VStack>
  </HStack>
);
