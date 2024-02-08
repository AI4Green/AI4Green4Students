import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Alert, AlertIcon, VStack, Text, useToast } from "@chakra-ui/react";
import { useBackendApi } from "contexts/BackendApi";
import { useComments } from "api/comment";
import { BasicModal } from "components/BasicModal";

export const DeleteCommentModal = ({
  fieldResponseId,
  isModalOpen,
  onModalClose,
  comment,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { comments: action } = useBackendApi();
  const { mutate } = useComments(fieldResponseId);
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
          duration: 1500,
          isClosable: true,
        });
        mutate();
        onModalClose();
      }
    } catch (e) {
      setFeedback({
        status: "error",
        message: t("feedback.error_title"),
      });
    }
  };

  const Modal = (
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
    <BasicModal
      body={Modal}
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
