import {
  Alert,
  AlertIcon,
  VStack,
  Text,
  Checkbox,
  useToast,
  HStack,
  Icon,
} from "@chakra-ui/react";
import { useUserList } from "api/user";
import { Modal } from "components/Modal";
import { useBackendApi } from "contexts/BackendApi";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { FaExclamationTriangle } from "react-icons/fa";
import { errorMessage } from "../error-message";
import { GLOBAL_PARAMETERS } from "constants/global-parameters";

export const DeleteUserModal = ({ user, onModalClose, isModalOpen }) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();
  const [informUser, setInformUser] = useState(false);

  const toast = useToast();
  const { t } = useTranslation();

  const { mutate } = useUserList();
  const { users } = useBackendApi();

  const handleDelete = async () => {
    try {
      setIsLoading(true);
      user && informUser && (user.sendUpdateEmail = informUser); // set email update if available
      const response = await users.delete(user);
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          title: `User ${user.name || user.email} deleted`,
          status: "success",
          duration: GLOBAL_PARAMETERS.ToastDuration,
          isClosable: true,
          position: "top",
        });
        mutate();
        onModalClose();
      }
    } catch (e) {
      const { messageError } = await errorMessage(e, t);
      setFeedback({
        status: "error",
        message: messageError,
      });
    }
  };

  const modalBody = (
    <HStack>
      <Icon as={FaExclamationTriangle} color="red.500" fontSize="5xl" />
      <VStack align="flex-end" flex={1}>
        {feedback && (
          <Alert status={feedback.status}>
            <AlertIcon />
            {feedback.message}
          </Alert>
        )}
        <Text>Are you sure you want to delete this user?</Text>
        <Text fontWeight="bold">{user.fullName || user.email}</Text>
        <Checkbox
          onChange={() => {
            setInformUser(!informUser);
          }}
          isChecked={informUser}
          isDisabled={!user.emailConfirmed}
        >
          <Text fontSize="sm">Update user about their account deletion?</Text>
        </Checkbox>
      </VStack>
    </HStack>
  );

  return (
    <Modal
      body={modalBody}
      title="Delete User confirmation"
      actionBtnCaption="Delete"
      actionBtnColorScheme="red"
      onAction={handleDelete}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};
