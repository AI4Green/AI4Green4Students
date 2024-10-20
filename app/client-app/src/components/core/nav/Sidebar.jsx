import {
  Heading,
  Box,
  VStack,
  Grid,
  useDisclosure,
  Drawer,
  DrawerOverlay,
  DrawerContent,
  DrawerCloseButton,
  DrawerBody,
  IconButton,
} from "@chakra-ui/react";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { FaBars } from "react-icons/fa";
import { useUser } from "contexts/User";
import { NavBar, SidebarButton } from "components/core/nav";
import { getSidebarItems } from "config/sidebar-items";

export const Sidebar = ({ children }) => {
  const { t } = useTranslation();
  const { user } = useUser();

  const BrandLink = () => (
    <Link to="/">
      <Heading
        as="h1"
        fontSize={{ base: "md", sm: "lg", md: "xl", lg: "2xl" }}
        fontWeight="light"
        letterSpacing="tighter"
        color="brand.500"
      >
        {t("buttons.brand")}
      </Heading>
    </Link>
  );

  // filter items that the user has permission to access or where no permission is required
  const validItems = getSidebarItems(t).filter(
    (item) =>
      !item.permission ||
      Object.values(item.permission).every((x) =>
        user?.permissions?.includes(x)
      )
  );

  // If there are no valid items, don't render the sidebar
  return validItems.length > 0 ? (
    <VStack overflow="auto" w="100%">
      <Grid templateRows="auto 1fr" minW="100%">
        <NavBar
          brand={
            <>
              <Box
                display="none"
                h="screen"
                w="full"
                flexShrink={0}
                borderRightWidth={1}
                borderRightColor="gray.200"
              >
                <Box h="full" pt={4} pb={6} lg={{ pt: 4, pb: 8 }}>
                  <SidebarContent brand={<BrandLink />} items={validItems} />
                </Box>
              </Box>
              <SideMenuDrawer items={validItems} brand={<BrandLink />} />
            </>
          }
        />
        {children}
      </Grid>
    </VStack>
  ) : (
    <VStack overflow="auto" w="100%">
      <Grid templateRows="auto 1fr" minW="100%">
        <NavBar brand={<BrandLink />} />
        {children}
      </Grid>
    </VStack>
  );
};

const SideMenuDrawer = ({ items, brand }) => {
  // only applicable to small screens
  const DrawerState = useDisclosure();
  return (
    <Box ml={1} mt={1}>
      <IconButton
        icon={<FaBars />}
        size="lg"
        onClick={DrawerState.onOpen}
        variant="ghost"
        aria-label="Open sidebar menu"
      />
      <Drawer
        size="xs"
        isOpen={DrawerState.isOpen}
        onClose={DrawerState.onClose}
        placement="left"
      >
        <DrawerOverlay />
        <DrawerContent>
          <DrawerCloseButton aria-label="Close sidebar menu" />
          <DrawerBody>
            <SidebarContent
              items={items}
              brand={brand}
              onClose={DrawerState.onClose}
            />
          </DrawerBody>
        </DrawerContent>
      </Drawer>
    </Box>
  );
};

const SidebarContent = ({ brand, items, onClose }) => (
  <VStack spacing={5} align="flex-start" px={4} as="aside">
    {brand}

    {items?.map((item, i) => (
      <SidebarButton item={item} key={i} onClose={onClose} />
    ))}
  </VStack>
);
