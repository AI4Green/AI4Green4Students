import { useBackendApi } from "contexts/BackendApi";
import { FaBan } from "react-icons/fa";

export const actionMenu = () => {
  const { users, account } = useBackendApi();
  const options = {
    inviteNew: {
      // for handling user invite
      name: "userInviteNew",
      title: "User invite", // application for modal title and display text
      apiAction: async ({ values }) => account.invite(values), // triggers api call
      btnCaption: "Invite", // caption for action/primary button
      linkName: "activationLink", // checks for the key 'activationLink' to extract activation link
    },
    updateRoles: {
      // for handling user roles update
      name: "userUpdateRoles",
      title: "User roles update",
      apiAction: async ({ values, user }) =>
        users.setUserRoles({ id: user.id, values: values }),
      btnCaption: "Update Roles",
    },
    deleteUser: {
      // for handling user delete
      name: "userDelete",
      title: "User delete",
      apiAction: async (user) => users.delete(user),
      btnCaption: "Delete User",
      btnColorScheme: "red",
      btnIcon: FaBan(),
    },
    updateEmail: {
      // for handling user email change request
      name: "userUpdateEmail",
      title: "Email Change",
      apiAction: async ({ values, user }) =>
        users.setUserEmail({ id: user.id, values: values }),
      btnCaption: "Change Email",
      linkName: "emailChangeLink", // checks for the key 'emailChangeLink' to extract email change link
    },
    resendInvite: {
      // for handling user roles update
      name: "userResendInvite",
      title: "Resend Invite",
      apiAction: async ({ user }) => account.resendInvite(user.id),
      linkName: "activationLink", // checks for the key 'activationLink' to extract activation link
    },
  };
  return options;
};
