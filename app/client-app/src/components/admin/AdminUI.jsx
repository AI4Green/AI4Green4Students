import { VStack, Heading, Icon } from "@chakra-ui/react";

export const AdminHeading = ({ title, icon }) => (
  <VStack align="flex-start" w="full" my={2}>
    <Heading
      fontSize="2xl"
      as="h2"
      size="md"
      fontWeight="semibold"
      color="blue.600"
    >
      <Icon as={icon} /> {title}
    </Heading>
  </VStack>
);
