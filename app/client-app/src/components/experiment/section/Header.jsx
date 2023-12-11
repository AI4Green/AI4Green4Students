import { HStack, Heading, VStack, Icon } from "@chakra-ui/react";
import { FaFlask } from "react-icons/fa";

export const Header = ({ header, subHeader, overview, actionSection }) => (
  <HStack w="full" borderBottomWidth={1}>
    <VStack align="start" my={2}>
      <Heading as="h2" size="md" fontWeight="semibold" color="green.600">
        <Icon as={FaFlask} /> {header}
      </Heading>
      <HStack spacing={2}>
        <Heading as="h2" size="xs" fontWeight="semibold">
          {subHeader}
        </Heading>
      </HStack>
    </VStack>

    <VStack align="end" my={2} flex={1}>
      <Heading as="h2" size="lg" fontWeight="semibold" color="blue.600">
        {overview}
      </Heading>
      {actionSection}
    </VStack>
  </HStack>
);
