import { Grid, GridItem, Heading, Stack, VStack } from "@chakra-ui/react";
import { Outlet } from "react-router-dom";
import { Sidebar } from "components/core/nav";
import { useLocationStateToast } from "helpers/hooks/useLocationStateToast";
import { Footer } from "components/Footer";

export const DefaultLayout = ({ toastDefaults = { position: "top" } }) => {
  useLocationStateToast(toastDefaults);

  return (
    <Grid templateRows="1fr auto" height="100vh">
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
  <Stack align="center" minW="full">
    <VStack
      p={4}
      w={{ base: "full", xl: "90%", "2xl": "70%" }}
      spacing={4}
      align="stretch"
      borderWidth={1}
      borderRadius={7}
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
