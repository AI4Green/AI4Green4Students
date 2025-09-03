import { Grid, GridItem, Heading, Stack, VStack } from "@chakra-ui/react";
import { Footer } from "components/core/footer";
import { Sidebar } from "components/core/nav";
import { useLocationStateToast } from "helpers/hooks";
import { Outlet } from "react-router-dom";

export const DefaultLayout = ({ toastDefaults = { position: "top" } }) => {
  useLocationStateToast(toastDefaults);

  return (
    <Grid templateRows="1fr auto" minHeight="100vh" fontWeight="light">
      <Sidebar>
        <Outlet />
      </Sidebar>
      <GridItem>
        <Footer />
      </GridItem>
    </Grid>
  );
};

export const DefaultContentLayout = ({ children }) => (
  <Stack align="center" minW="full" pb={8}>
    <VStack
      p={4}
      w={{ base: "full", xl: "90%", "2xl": "70%" }}
      spacing={4}
      align="stretch"
      borderWidth={0.5}
      borderRadius={4}
    >
      {children}
    </VStack>
  </Stack>
);

export const DefaultContentHeader = ({ icon, header }) => (
  <VStack align="start">
    <Heading as="h1" size="md" color="brand.500">
      {icon} {header}
    </Heading>
  </VStack>
);
