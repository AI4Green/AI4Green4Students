import {
  Box,
  HStack,
  Menu,
  MenuButton,
  MenuItem,
  MenuList,
  Text,
  useDisclosure,
  Avatar,
  Button,
  useBreakpointValue,
} from "@chakra-ui/react";
import { useBackendApi } from "contexts/BackendApi";
import { useUser } from "contexts/User";
import { useTranslation } from "react-i18next";
import { FaSignOutAlt, FaSignInAlt, FaUserPlus, FaHome } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import { LoadingModal } from "components/core/LoadingModal";
import { forwardRef } from "react";
import { Link } from "react-router-dom";
import { navbarItems } from "config/navbar-items";

const NavBarButton = forwardRef(function NavBarButton({ children, ...p }, ref) {
  const size = useBreakpointValue({ base: "xs", md: "sm" });
  return (
    <Button
      leftIcon={<FaHome />}
      ref={ref}
      size={size}
      borderRadius={8}
      variant="ghost"
      {...p}
    >
      {children}
    </Button>
  );
});

const BusyModal = ({ isOpen, onClose, verb }) => (
  <LoadingModal isOpen={isOpen} verb={verb} onClose={onClose} />
);

const LoggedInMenu = ({ user, onLogout }) => {
  const { t } = useTranslation();
  const avatarSize = useBreakpointValue({ base: "xs", md: "sm" });
  const isFullMenu = useBreakpointValue({ base: false, xl: true });

  return (
    <Menu>
      <MenuButton
        py={4}
        as={Button}
        variant="ghost"
        aria-label={`User menu for user: ${user.fullName}`}
        _focus={{ boxShadow: "outline" }}
        _hover={{ backgroundColor: "none", color: "blue.500" }}
      >
        <HStack>
          <Text fontSize={{ base: "xs", md: "sm" }} fontWeight="medium">
            {user.fullName}
          </Text>
          <Avatar name={user.fullName} size={avatarSize} />
        </HStack>
      </MenuButton>
      <MenuList color="gray.800" fontSize={{ base: "xs", md: "sm" }}>
        {!isFullMenu &&
          navbarItems.map((item) => (
            <MenuItem
              key={item.label}
              as={Link}
              to={item.href}
              icon={<item.icon />}
              _focus={{ backgroundColor: "gray.100" }}
            >
              {item.label}
            </MenuItem>
          ))}
        <MenuItem
          onClick={onLogout}
          icon={<FaSignOutAlt />}
          color="red.600"
          _hover={{ backgroundColor: "red.100" }}
        >
          {t("buttons.logout")}
        </MenuItem>
      </MenuList>
    </Menu>
  );
};

const LoggedOutButtons = ({ t }) => (
  <>
    <NavBarButton leftIcon={<FaSignInAlt />} as={Link} to="/account/login">
      {t("buttons.login")}
    </NavBarButton>
    <NavBarButton leftIcon={<FaUserPlus />} as={Link} to="/account/register">
      {t("buttons.register")}
    </NavBarButton>
  </>
);

const UserMenu = () => {
  const { user, signOut } = useUser();
  const { t } = useTranslation();
  const navigate = useNavigate();
  const {
    account: { logout },
  } = useBackendApi();

  const busyModalState = useDisclosure();

  const handleLogoutClick = async () => {
    busyModalState.onOpen();

    await logout();
    signOut();
    navigate("/account/login", {
      state: {
        toast: {
          title: t("logout.feedback.success"),
          status: "success",
          duration: 2500,
          isClosable: true,
        },
      },
    });

    busyModalState.onClose();
  };

  return (
    <>
      {user ? (
        <Box>
          <LoggedInMenu user={user} onLogout={handleLogoutClick} />
        </Box>
      ) : (
        <LoggedOutButtons t={t} />
      )}
      <BusyModal
        isOpen={busyModalState.isOpen}
        onClose={busyModalState.onClose}
        verb={t("logout.feedback.busy")}
      />
    </>
  );
};

export const NavBar = ({ brand }) => {
  const isFullMenu = useBreakpointValue({ base: false, xl: true });
  return (
    <HStack
      px={4}
      py={4}
      mb={8}
      flexGrow={1}
      borderBottom="1px solid"
      borderColor="gray.300"
      position="sticky"
      top={0}
      zIndex={50}
      backdropFilter="blur(10px)"
    >
      {brand}
      <HStack justify="flex-end" flexGrow={1} spacing={2}>
        <NavBarButton as={Link} to="/home" leftIcon={<FaHome />}>
          Home
        </NavBarButton>
        {isFullMenu &&
          navbarItems.map((item) => (
            <NavBarButton
              key={item.label}
              as={Link}
              to={item.href}
              leftIcon={<item.icon />}
            >
              {item.label}
            </NavBarButton>
          ))}
        <UserMenu />
      </HStack>
    </HStack>
  );
};
