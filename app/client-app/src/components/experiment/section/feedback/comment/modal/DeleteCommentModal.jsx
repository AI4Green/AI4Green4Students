import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Alert, AlertIcon, VStack, Text, useToast } from "@chakra-ui/react";
import { useBackendApi } from "contexts/BackendApi";
import { Modal } from "components/Modal";
import { useSectionForm } from "contexts/SectionForm";
import { GLOBAL_PARAMETERS } from "constants/global-parameters";

export const DeleteCommentModal = ({ isModalOpen, onModalClose, comment }) => {
  const { mutate } = useSectionForm();
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { comments: action } = useBackendApi();
  const { t } = useTranslation();
  const toast = useToast();

  const handleDelete = async () => {
    try {
      setIsLoading(true);
      const response = await action.delete(comment.id);
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          position: "top",
          title: "Comment deleted",
          status: "success",
          duration: GLOBAL_PARAMETERS.ToastDuration,
          isClosable: true,
        });
        await mutate();
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
        <Alert status={feedback.status}>
          <AlertIcon />
          {feedback.message}
        </Alert>
      )}
      <Text>Are you sure you want to delete this comment?</Text>
      <Text fontWeight="bold">{comment.value}</Text>
    </VStack>
  );
  return (
    <Modal
      body={modalBody}
      title="Delete Confirmation"
      actionBtnCaption="Delete"
      actionBtnColorScheme="red"
      onAction={handleDelete}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};
