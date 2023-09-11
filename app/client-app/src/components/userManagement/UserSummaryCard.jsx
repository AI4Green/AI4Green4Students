import {
  Flex,
  Heading,
  HStack,
  Text,
  VStack,
  Spacer,
  Button,
  Popover,
  PopoverTrigger,
  PopoverContent,
  PopoverBody,
  PopoverArrow,
  Badge,
  useDisclosure,
  Box,
} from "@chakra-ui/react";
import {
  FaChevronDown,
  FaCog,
  FaUserAltSlash,
  FaRegEnvelope,
  FaBarcode,
  FaDirections,
} from "react-icons/fa";
import { ActionCard } from "components/ActionCard";
import { USERMANAGEMENT_PERMISSIONS } from "constants/site-permissions";
import { useNavigate } from "react-router-dom";
import { ActionButton } from "components/ActionButton";
import { useUser } from "contexts/User";
import { ManageAction as ManageRoles } from "./ManageAction";
import { ManageAction as DeleteUser } from "./ManageAction";
import { ManageAction as ChangeEmail } from "./ManageAction";
import { ManageAction as ResendInvite } from "./ManageAction";
import { actionMenu } from "./actionMenu";

export const UserSummaryCard = ({ user }) => {
  const ManageRolesState = useDisclosure();
  const DeleteUserState = useDisclosure();
  const ChangeEmailState = useDisclosure();
  const ResendInviteState = useDisclosure();

  const navigate = useNavigate();

  const getNameInitials = () => {
    // get name initials
    const splitName = user.fullName.trim().split(" "); // split name into an array
    let initials = ""; // empty initials
    splitName.forEach((name) => {
      initials = `${initials}${name.charAt(0).toUpperCase()}`; // append first char to 'initials'
    });
    return initials;
  };

  const UserRole = () => {
    // Display user role or roles Popover (if more than one role)
    // Expects an array of roles
    if (user.roles && user.roles.length > 1) {
      // more than one role, display as drop down
      return (
        <Box>
          <Popover trigger="hover">
            <PopoverTrigger>
              <Button
                borderBottom="1px"
                borderColor="gray.400"
                color="gray.500"
                variant="ghost"
                size="sm"
                rightIcon={<FaChevronDown />}
              >
                Roles
              </Button>
            </PopoverTrigger>
            <PopoverContent color="cyan.600" maxW="85px">
              <PopoverArrow />
              <PopoverBody>
                {user.roles.map((role, index) => (
                  <Text key={index} fontSize="sm" align="center" padding={0.3}>
                    <Badge colorScheme="cyan" key={index} margin={0.5}>
                      {role}
                    </Badge>
                  </Text>
                ))}
              </PopoverBody>
            </PopoverContent>
          </Popover>
        </Box>
      );
    }
    // if only one role
    return (
      user.roles && (
        <Badge colorScheme="cyan">
          <Text fontSize="sm">{user.roles[0]}</Text>
        </Badge>
      )
    );
  };

  const UserSummaryCardActions = () => (
    <HStack justifyContent="end" spacing={7}>
      <UserRole />
      <ActionButton actions={actions} />
      {ManageRolesState.isOpen && (
        <ManageRoles
          isModalOpen={ManageRolesState.isOpen}
          onModalClose={ManageRolesState.onClose}
          user={user}
          actionSelected={actionMenu().updateRoles}
        />
      )}
      {DeleteUserState.isOpen && (
        <DeleteUser
          isModalOpen={DeleteUserState.isOpen}
          onModalClose={DeleteUserState.onClose}
          user={user}
          actionSelected={actionMenu().deleteUser}
        />
      )}
      {ChangeEmailState.isOpen && (
        <ChangeEmail
          isModalOpen={ChangeEmailState.isOpen}
          onModalClose={ChangeEmailState.onClose}
          user={user}
          actionSelected={actionMenu().updateEmail}
        />
      )}
      {ResendInviteState.isOpen && (
        <ResendInvite
          isModalOpen={ResendInviteState.isOpen}
          onModalClose={ResendInviteState.onClose}
          user={user}
          actionSelected={actionMenu().resendInvite}
        />
      )}
    </HStack>
  );

  const { user: userLoggedIn } = useUser();
  const actions = {
    manageRoles: {
      isEligible: () => {
        return userLoggedIn.permissions.includes(
          USERMANAGEMENT_PERMISSIONS.EditUsers
        );
      },
      icon: <FaCog />,
      label: "Manage Roles",
      onClick: ManageRolesState.onOpen,
    },
    changeEmail: {
      isEligible: () => {
        return (
          // hide change email option for unconfirmed users
          // If email change required for unconfirmed users, delete the user and send invite to new email
          user.emailConfirmed &&
          userLoggedIn.permissions.includes(
            USERMANAGEMENT_PERMISSIONS.EditUsers
          )
        );
      },
      icon: <FaRegEnvelope />,
      label: "Change Email",
      onClick: ChangeEmailState.onOpen,
    },
    chnagePassword: {
      isEligible: () => {
        return;
      },
      icon: <FaBarcode />,
      label: "Change Password",
      onClick: () => navigate(``),
    },
    delete: {
      isEligible: () => {
        return (
          // hide delete option from deleting itself
          user.email !== userLoggedIn.email &&
          userLoggedIn.permissions.includes(
            USERMANAGEMENT_PERMISSIONS.DeleteUsers
          )
        );
      },
      icon: <FaUserAltSlash />,
      label: "Delete",
      onClick: DeleteUserState.onOpen,
    },
    resendInvite: {
      isEligible: () => {
        return (
          // hide resend invite option for confirmed users
          !user.emailConfirmed &&
          [
            USERMANAGEMENT_PERMISSIONS.InviteStudents,
            USERMANAGEMENT_PERMISSIONS.InviteInstructors,
            USERMANAGEMENT_PERMISSIONS.InviteUsers,
          ].every((x) => userLoggedIn.permissions.includes(x))
        );
      },
      icon: <FaDirections />,
      label: "Resend Invite/Confirmation",
      onClick: ResendInviteState.onOpen,
    },
  };

  return (
    <ActionCard
      title={
        <Flex alignItems={"center"} gap={3}>
          <Heading
            as="h4"
            size="md"
            display="flex"
            justifyContent="center"
            alignItems="center"
            bg="gray.200"
            color="blue.900"
            borderColor="gray.300"
            borderWidth={1}
            borderRadius="50%"
            h="40px"
            w="40px"
          >
            {getNameInitials()}
          </Heading>
          <VStack alignItems="flex-start" spacing={0.2}>
            <Heading as="h4" size="md">
              {user.fullName}
            </Heading>
            <Text color="gray.600" fontSize="sm">
              {user.email}
            </Text>
          </VStack>
          <Spacer />
          <VStack spacing={0.2} align="flex-end" padding={1.5} marginBottom={2}>
            {!user.emailConfirmed && <Text fontSize="2xl">⚠️</Text>}
          </VStack>
        </Flex>
      }
      href="#" // TODO: Open user details page ??
    >
      <UserSummaryCardActions />
    </ActionCard>
  );
};
