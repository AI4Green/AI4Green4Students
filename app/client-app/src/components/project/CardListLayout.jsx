import { Stack } from "@chakra-ui/react";

export const CardListLayout = ({ children }) => (
  <Stack
    alignItems="flex-start"
    w={{ base: "95%", md: "85%", lg: "75%", xl: "60%" }}
    align="stretch"
    p={4}
    spacing={3}
  >
    {children}
  </Stack>
);
