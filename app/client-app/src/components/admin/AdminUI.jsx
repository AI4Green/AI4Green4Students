import { Stack, VStack, Heading, Icon } from "@chakra-ui/react";

export const AdminLayout = ({ children }) => (
  <Stack align="stretch" w="100%" alignItems="center">
    <VStack
      m={4}
      p={4}
      align="stretch"
      minW={{ base: "full", md: "95%", lg: "80%", xl: "70%" }}
      spacing={4}
    >
      <VStack borderWidth={1} px={5} py={2} borderRadius={7} spacing={4}>
        {children}
      </VStack>
    </VStack>
  </Stack>
);

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
