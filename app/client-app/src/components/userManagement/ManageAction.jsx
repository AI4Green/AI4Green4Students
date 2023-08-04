import { useDisclosure } from "@chakra-ui/react";
import { BasicModal } from "components/BasicModal";
import { useEffect, useRef, useState } from "react";
import { ModalDisplayLink } from "./ModalDisplayLink";
import { useUserList } from "api/user";
import { useToast } from "@chakra-ui/react";
import { ModalManageRolesOrInvite } from "./ModalManageRolesOrInvite";
import { actionMenu } from "./actionMenu";
import { ModalDeleteUser } from "./ModalDeleteUser";
import { ModalUpdateUserEmail } from "./ModalUpdateUserEmail";
import { useTranslation } from "react-i18next";

export const ManageAction = ({
  user,
  actionSelected,
  isModalOpen,
  onModalClose,
}) => {
  const { mutate } = useUserList();
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();
  const [generatedLink, setGeneratedLink] = useState();
  const [informUser, setInformUser] = useState(false);
  const ModalDisplayLinkState = useDisclosure();
  const { t } = useTranslation();
  const toast = useToast();

  // toast configured for the User management page
  const displayToast = ({
    position = "top",
    title,
    status,
    duration = "900",
    isClosable = true,
  }) =>
    toast({
      position,
      title,
      status,
      duration,
      isClosable,
    });

  const handleSubmitOrLinkGeneration = async (values) => {
    try {
      setIsLoading(true);
      user && informUser && (user.sendUpdateEmail = informUser); // set email update if available
      // post to the api
      const response = await actionSelected.apiAction({
        values: values,
        user: user,
      });
      setIsLoading(false);
      // check for 200 status (expect link to be available as well)
      if (response && response.status === 200) {
        const parsed = await response?.json(); // parse response body

        parsed[actionSelected?.linkName] &&
          setGeneratedLink(parsed[actionSelected?.linkName]); // update the state if link is available

        displayToast({
          title: `${actionSelected.title} link generated`,
          status: "success",
          duration: 1500,
        });

        ModalDisplayLinkState.onOpen(); // open the modal to display the link
      }

      if (response && response.status === 204) {
        displayToast({
          title: `${actionSelected.title} success`,
          status: "success",
          duration: 1500,
        });
        mutate(); // refresh the user list
        onModalClose();
      }
    } catch (e) {
      switch (e?.response?.status) {
        case 400: {
          const result = await e.response.json();

          let message = t("feedback.error_title");
          if (result.isExistingUser) message = "User already exists";
          else if (result.isExistingEmail) message = "Email already exists";
          else if (result.isNotAllowlisted)
            message = "User not eligible for registration";
          else if (result.IsRolesNotValidOrSelected)
            message = "Roles are not valid or selected";

          setFeedback({ status: "error", message });
          break;
        }
        default:
          setFeedback({
            status: "error",
            message: t("feedback.error_title"),
          });
      }
    }
  };

  const { inviteNew, updateRoles, deleteUser, updateEmail, resendInvite } =
    actionMenu(); // extract action menu
  // Set Modal and action
  // Based on the user selected action, modal and action handler is being set
  // For e.g.for new user invite or existing user roles update
  // ModalManageRolesOrInvite modal is being used but no action has been set.
  // In such case, handleSubmit prop is used as an action handler.
  // See below BasicModal component onAction prop
  const action = () => {
    let modal, onAction;
    switch (actionSelected.name) {
      case inviteNew.name:
      case updateRoles.name:
        modal = (
          <ModalManageRolesOrInvite
            user={user}
            feedback={feedback}
            handleSubmit={handleSubmitOrLinkGeneration}
            manageRoles={actionSelected.name === updateRoles.name}
            formRef={formRef}
          />
        );
        break;

      case deleteUser.name:
        modal = (
          <ModalDeleteUser
            user={user}
            feedback={feedback}
            setInformUser={setInformUser}
            informUser={informUser}
          />
        );
        onAction = handleSubmitOrLinkGeneration;
        break;

      case updateEmail.name:
        modal = (
          <ModalUpdateUserEmail
            user={user}
            feedback={feedback}
            handleSubmit={handleSubmitOrLinkGeneration}
            formRef={formRef}
          />
        );
        break;

      case resendInvite.name: // Do not prompt any modal, just call the handleSubmitOrLinkGeneration function
        break;

      default:
        break;
    }
    return { modal, onAction };
  };

  const formRef = useRef();

  const { modal, onAction } = action();

  useEffect(() => {
    const generateLink = async () => {
      handleSubmitOrLinkGeneration();
    };
    !modal && !generatedLink && generateLink(); // if no modal is available, do prompt action (e.g. resend invite)
  }, [actionSelected.name]);

  return (
    <>
      {modal && !ModalDisplayLinkState.isOpen && (
        <BasicModal
          body={modal} // render modal as per the selected action
          title={actionSelected?.title}
          actionBtnCaption={actionSelected?.btnCaption}
          actionBtnColorScheme={actionSelected?.btnColorScheme}
          actionBtnLeftIcon={actionSelected?.btnIcon}
          isLoading={isLoading}
          onAction={onAction || (() => formRef.current.handleSubmit())}
          isOpen={isModalOpen}
          onClose={() => onModalClose()}
        />
      )}

      <ModalDisplayLink
        displayLink={generatedLink}
        isLoading={isLoading}
        isModalOpen={ModalDisplayLinkState.isOpen}
        onModalClose={() => {
          mutate(); // refresh the user list
          onModalClose();
        }}
        actionSelected={actionSelected}
      />
    </>
  );
};
