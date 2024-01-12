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
import { LoadingModal } from "./LoadingModal";
import { forwardRef } from "react";
import { Link } from "react-router-dom";

const NavBarButton = forwardRef(function NavBarButton({ children, ...p }, ref) {
  const size = useBreakpointValue({ base: "xs", sm: "sm", md: "md" });
  return (
    <Button
      leftIcon={<FaHome />}
      ref={ref}
      size={size}
      borderRadius={8}
      variant="ghost"
      _focus={{}}
      _hover={{ bg: "gray.100" }}
      _active={{ bg: "gray.300" }}
      {...p}
    >
      {children}
    </Button>
  );
});

const UserMenu = () => {
  const { user, signOut } = useUser();
  const { t } = useTranslation();
  const navigate = useNavigate();
  const {
    account: { logout },
  } = useBackendApi();

  const busyModalState = useDisclosure();
  const busyModal = (
    <LoadingModal
      isOpen={busyModalState.isOpen}
      verb={t("logout.feedback.busy")}
    />
  );

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

  const avatarSize = useBreakpointValue({ base: "xs", md: "sm" });

  return user ? (
    <Box>
      <Menu>
        <MenuButton py={2}>
          <HStack>
            <Text
              fontSize={{ base: "xs", sm: "sm", md: "md" }}
              fontWeight="semibold"
            >
              {user.fullName}
            </Text>
            <Avatar name={user.fullName} size={avatarSize} />
          </HStack>
        </MenuButton>
        <MenuList color="gray.800">
          <MenuItem
            onClick={handleLogoutClick}
            icon={<FaSignOutAlt />}
            color="red.600"
            _hover={{ backgroundColor: "red.100" }} // Change font color on hover
          >
            {t("buttons.logout")}
          </MenuItem>
        </MenuList>
      </Menu>
    </Box>
  ) : (
    <>
      <NavBarButton leftIcon={<FaSignInAlt />} as={Link} to="/account/login">
        {t("buttons.login")}
      </NavBarButton>

      <NavBarButton leftIcon={<FaUserPlus />} as={Link} to="/account/register">
        {t("buttons.register")}
      </NavBarButton>
      {busyModal}
    </>
  );
};

export const NavBar = ({ brand }) => {
  const { user } = useUser();
  return (
    <HStack px={4} py={2} flexGrow={1}>
      {brand}
      <HStack justify="flex-end" flexGrow={1} spacing={2}>
        {user && (
          <>
            <NavBarButton as={Link} to="/home">
              Home
            </NavBarButton>
          </>
        )}
        <UserMenu />
      </HStack>
    </HStack>
  );
};
