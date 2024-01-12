import { Stack, VStack } from "@chakra-ui/react";

export const ExperimentLayout = ({ children }) => (
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
