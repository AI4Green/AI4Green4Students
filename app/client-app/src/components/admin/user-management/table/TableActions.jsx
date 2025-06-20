import { useDisclosure } from "@chakra-ui/react";
import {
  FaCog,
  FaDirections,
  FaRegEnvelope,
  FaUserAltSlash,
} from "react-icons/fa";
import { ActionButton } from "components/core/ActionButton";
import { USERMANAGEMENT_PERMISSIONS } from "constants";
import { useUser } from "contexts";
import {
  ManageRolesOrInviteModal,
  DeleteUserModal,
  ResendInviteModal,
  UpdateUserEmailModal,
} from "../modal";

export const TableActions = ({ user }) => {
  const {
    EditUsers,
    DeleteUsers,
    InviteStudents,
    InviteInstructors,
    InviteUsers,
  } = USERMANAGEMENT_PERMISSIONS;
  const { emailConfirmed, email } = user;
  const { user: userLoggedIn } = useUser();
  const {
    isOpen: isOpenManageRoles,
    onOpen: onOpenManageRoles,
    onClose: onCloseManageRoles,
  } = useDisclosure();

  const {
    isOpen: isOpenDeleteUser,
    onOpen: onOpenDeleteUser,
    onClose: onCloseDeleteUser,
  } = useDisclosure();
  const {
    isOpen: isOpenChangeEmail,
    onOpen: onOpenChangeEmail,
    onClose: onCloseChangeEmail,
  } = useDisclosure();
  const {
    isOpen: isOpenResendInvite,
    onOpen: onOpenResendInvite,
    onClose: onCloseResendInvite,
  } = useDisclosure();

  const actions = {
    manageRoles: {
      isEligible: () => {
        return userLoggedIn.permissions.includes(EditUsers);
      },
      icon: <FaCog />,
      label: "Manage Roles",
      onClick: onOpenManageRoles,
    },
    changeEmail: {
      isEligible: () => {
        return (
          // hide change email option for unconfirmed users
          // If email change required for unconfirmed users, delete the user and send invite to new email
          emailConfirmed && userLoggedIn.permissions.includes(EditUsers)
        );
      },
      icon: <FaRegEnvelope />,
      label: "Change Email",
      onClick: onOpenChangeEmail,
    },
    delete: {
      isEligible: () => {
        return (
          // hide delete option from deleting itself
          email !== userLoggedIn.email &&
          userLoggedIn.permissions.includes(DeleteUsers)
        );
      },
      icon: <FaUserAltSlash />,
      label: "Delete",
      onClick: onOpenDeleteUser,
    },
    resendInvite: {
      isEligible: () => {
        return (
          // hide resend invite option for confirmed users
          !emailConfirmed &&
          [InviteStudents, InviteInstructors, InviteUsers].every((x) =>
            userLoggedIn.permissions.includes(x)
          )
        );
      },
      icon: <FaDirections />,
      label: "Resend Invite/Confirmation",
      onClick: onOpenResendInvite,
    },
  };
  return (
    <>
      <ActionButton
        actions={actions}
        size="xs"
        colorScheme="blue"
        variant="outline"
      />
      {isOpenManageRoles && (
        <ManageRolesOrInviteModal
          isModalOpen={isOpenManageRoles}
          onModalClose={onCloseManageRoles}
          user={user}
          manageRoles
        />
      )}
      {isOpenDeleteUser && (
        <DeleteUserModal
          isModalOpen={isOpenDeleteUser}
          onModalClose={onCloseDeleteUser}
          user={user}
        />
      )}
      {isOpenChangeEmail && (
        <UpdateUserEmailModal
          isModalOpen={isOpenChangeEmail}
          onModalClose={onCloseChangeEmail}
          user={user}
        />
      )}
      {isOpenResendInvite && (
        <ResendInviteModal onModalClose={onCloseResendInvite} user={user} />
      )}
    </>
  );
};
