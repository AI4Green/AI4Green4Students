import {
  Box,
  HStack,
  Menu,
  MenuButton,
  MenuItem,
  MenuList,
  useDisclosure,
  Avatar,
  Button,
  useBreakpointValue,
  Divider,
  VStack,
  Text,
} from "@chakra-ui/react";
import { useBackendApi, useUser } from "contexts";
import { useTranslation } from "react-i18next";
import {
  FaSignOutAlt,
  FaSignInAlt,
  FaUserPlus,
  FaHome,
  FaEnvelope,
} from "react-icons/fa";
import { useNavigate, Link } from "react-router-dom";
import { LoadingModal } from "components/core/LoadingModal";
import { forwardRef } from "react";
import { navbarItems } from "config/navbar-items";
import { UpdateUserEmailModal } from "components/admin/user-management/modal/UpdateUserEmailModal";

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
      fontWeight="light"
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

  const {
    isOpen: isOpenChangeEmail,
    onOpen: onOpenChangeEmail,
    onClose: onCloseChangeEmail,
  } = useDisclosure();

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
        <Avatar name={user.fullName} size={avatarSize} />
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
        <Divider />

        <MenuItem onClick={onOpenChangeEmail} icon={<FaEnvelope />}>
          Change Email
        </MenuItem>
        {isOpenChangeEmail && (
          <UpdateUserEmailModal
            isModalOpen={isOpenChangeEmail}
            onModalClose={onCloseChangeEmail}
            user={{ id: user.userId, ...user }}
          />
        )}

        <MenuItem fontSize="sm" isDisabled>
          <VStack fontSize="xxs" align="flex-start">
            <Text>{user.fullName}</Text>
            <Text>{user.email}</Text>
          </VStack>
        </MenuItem>

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
  <NavBarButton leftIcon={<FaSignInAlt />} as={Link} to="/account/login">
    {t("buttons.login")}
  </NavBarButton>
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
      py={6}
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
      <HStack justify="flex-end" flexGrow={1} spacing={1}>
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
