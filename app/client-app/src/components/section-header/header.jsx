import { HStack, Heading, VStack, Icon, Avatar, Text } from "@chakra-ui/react";
import { TITLE_ICON_COMPONENTS } from "constants";

export const SectionHeader = ({
  header,
  projectName,
  overviewTitle,
  owner,
  icon,
  actionSection,
}) => (
  <HStack w="full" justify="space-between" borderBottomWidth={1} py={4}>
    <VStack gap={4} align="start">
      {header && (
        <Heading
          as="h2"
          fontSize={{ base: "sm", lg: "md" }}
          fontWeight="normal"
          color="gray.700"
        >
          {header}
        </Heading>
      )}
      <HStack align="center" gap={4}>
        {owner && (
          <HStack>
            <Avatar name={owner} size="xs" />
            <Text
              fontSize={{ base: "xs", md: "sm" }}
              fontWeight="light"
              color="gray.700"
            >
              {owner}
            </Text>
          </HStack>
        )}

        <Text
          fontSize={{ base: "xs", md: "sm" }}
          color="brand.500"
          fontWeight="semibold"
        >
          <Icon as={TITLE_ICON_COMPONENTS.Project} /> Project - {projectName}
        </Text>
      </HStack>
    </VStack>

    <VStack align="end" gap={2}>
      <HStack align="baseline">
        <Icon as={icon} boxSize="5" color="brand.500" />
        <Heading
          as="h1"
          fontSize={{ base: "md", md: "lg", lg: "xl", "2xl": "2xl" }}
          fontWeight="normal"
          color="brand.500"
        >
          {overviewTitle}
        </Heading>
      </HStack>
      {actionSection}
    </VStack>
  </HStack>
);
