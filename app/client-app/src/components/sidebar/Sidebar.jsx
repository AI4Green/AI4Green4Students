import {
  Text,
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
import { FaUserCog, FaPencilRuler, FaBars } from "react-icons/fa";
import {
  USERMANAGEMENT_PERMISSIONS,
  REGISTRATION_RULES_PERMISSIONS,
} from "constants/site-permissions";
import { useUser } from "contexts/User";
import { SidebarButton } from "./SidebarItem";
import { NavBar } from "components/NavBar";

export const Sidebar = ({ children }) => {
  const { t } = useTranslation();
  const { user } = useUser();

  const BrandLink = () => (
    <Link to="/">
      <Text
        fontSize={{ base: "md", sm: "lg", md: "xl", lg: "2xl" }}
        letterSpacing="tighter"
      >
        {t("buttons.brand")}
      </Text>
    </Link>
  );

  const sidebarMenuItems = [
    {
      name: t("adminMenu.menuList.userManagement"),
      path: "/admin/usermanagement",
      icon: FaUserCog,
      permission: USERMANAGEMENT_PERMISSIONS,
    },
    {
      name: t("adminMenu.menuList.registrationRule"),
      path: "/admin/registrationrule",
      icon: FaPencilRuler,
      permission: REGISTRATION_RULES_PERMISSIONS,
      featureFlag: "settings",
    },
  ];

  // filter items that the user has permission to access or where no permission is required
  const validItems = sidebarMenuItems.filter(
    (item) =>
      !item.permission ||
      Object.values(item.permission).every((x) =>
        user?.permissions?.includes(x)
      )
  );

  // If there are no valid items, don't render the sidebar
  return validItems.length > 0 ? (
    <Grid
      templateColumns={{
        base: "0px minmax(0, 1fr)",
      }}
    >
      <Box
        position="sticky"
        top="0"
        zIndex="30"
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
      <VStack overflow="auto" w="100%">
        <Grid templateRows="auto 1fr" minHeight="100%" minW="100%">
          <NavBar />
          {children}
        </Grid>
      </VStack>
    </Grid>
  ) : (
    <VStack overflow="auto" w="100%">
      <Grid templateRows="auto 1fr" minHeight="100%" minW="100%">
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
