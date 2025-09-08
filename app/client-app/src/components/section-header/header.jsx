import { Avatar, Heading, HStack, Text, VStack } from "@chakra-ui/react";
import { Badge } from "components/core/Badge";
import { TITLE_ICON_COMPONENTS } from "constants";

export const SectionHeader = ({ header, project, owner, action }) => (
  <HStack w="full" justify="space-between" borderBottomWidth={1} py={4}>
    <VStack spacing={4} align="start">
      <HStack spacing={6}>
        {header.title && (
          <Heading
            as="h2"
            fontSize={{ base: "sm", lg: "md" }}
            fontWeight="normal"
            color="gray.700"
          >
            {header.title}
          </Heading>
        )}
        <HStack spacing={4}>
          <Badge
            label={header.type}
            colorScheme="green"
            leftIcon={header.icon}
            fontSize="xxs"
          />
          <Badge
            label={project.name}
            leftIcon={TITLE_ICON_COMPONENTS.Project}
            colorScheme="brand"
            variant="outline"
            fontSize="xxs"
          />
        </HStack>
      </HStack>
      <HStack align="center" gap={4}>
        {owner && (
          <HStack>
            <Avatar name={owner.name} size="xs" />
            <Text
              fontSize={{ base: "xs", md: "sm" }}
              fontWeight="light"
              color="gray.700"
            >
              {owner.name}
            </Text>
          </HStack>
        )}
      </HStack>
    </VStack>

    <VStack align="end">{action}</VStack>
  </HStack>
);
