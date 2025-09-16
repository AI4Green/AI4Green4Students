import {
  Button,
  Grid,
  GridItem,
  Icon,
  Stack,
  Text,
  VStack,
} from "@chakra-ui/react";
import { Badge } from "components/core/Badge";
import { Footer } from "components/core/footer";
import { Sidebar } from "components/core/nav";
import { useLocationStateToast } from "helpers/hooks";
import { IoIosAddCircleOutline } from "react-icons/io";
import { Link, Outlet } from "react-router-dom";

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
  <Badge
    label={header}
    leftIcon={icon}
    colorScheme="brand"
    variant="outline"
    fontSize="xs"
  />
);

export const NewButton = ({
  icon = IoIosAddCircleOutline,
  label = "New",
  onClick,
  to,
}) => (
  <Button
    as={onClick ? undefined : Link}
    onClick={onClick}
    to={to}
    colorScheme="gray"
    leftIcon={<Icon as={icon} fontSize={14} color="green" />}
    size="xs"
    borderRadius={10}
    variant="outline"
    px={4}
    py={3.5}
  >
    <Text
      fontSize={{ base: "xs", md: "sm" }}
      fontWeight="normal"
      color="gray.700"
    >
      {label}
    </Text>
  </Button>
);
