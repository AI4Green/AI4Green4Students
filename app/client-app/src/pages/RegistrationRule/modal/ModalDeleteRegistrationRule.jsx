import { Alert, AlertIcon, VStack, Text, useToast } from "@chakra-ui/react";
import { useRegistrationRulesList } from "api/registrationRules";
import { Modal } from "components/Modal";
import { useState } from "react";
import { useBackendApi } from "contexts/BackendApi";
import { useTranslation } from "react-i18next";

export const ModalDeleteRegistrationRule = ({
  registrationRule,
  isModalOpen,
  onModalClose,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();
  const { registrationRules } = useBackendApi();
  const { mutate } = useRegistrationRulesList();
  const { t } = useTranslation();

  const toast = useToast();
  // toast config
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

  const handleDelete = async () => {
    try {
      setIsLoading(true);

      const response = await registrationRules.delete({
        id: registrationRule.id,
      });

      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        displayToast({
          title: "Registration rule deleted",
          status: "success",
          duration: 1500,
        });
        mutate(); // refresh the user list
        onModalClose();
      }
    } catch (e) {
      setFeedback({
        status: "error",
        message: t("feedback.error_title"),
      });
    }
  };

  const modalBody = (
    <VStack>
      {feedback && (
        <Alert status="error">
          <AlertIcon />
          {feedback}
        </Alert>
      )}
      <Text>Are you sure you want to delete this rule with value:</Text>
      <Text fontWeight="bold">{registrationRule.value}</Text>
    </VStack>
  );
  return (
    <Modal
      body={modalBody} // render modal as per the selected action
      title="Delete Registration Rule confirmation"
      actionBtnCaption="Delete"
      actionBtnColorScheme="red"
      onAction={handleDelete}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};
