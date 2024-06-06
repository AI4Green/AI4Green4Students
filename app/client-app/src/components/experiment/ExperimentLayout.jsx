import { Stack, VStack } from "@chakra-ui/react";

export const ExperimentLayout = ({ children }) => (
  <Stack align="center">
    <VStack
      m={4}
      p={4}
      minW={{ base: "full", md: "95%", lg: "80%", xl: "70%" }}
      spacing={4}
      borderWidth={1}
      px={5}
      py={2}
      borderRadius={7}
    >
      {children}
    </VStack>
  </Stack>
);
