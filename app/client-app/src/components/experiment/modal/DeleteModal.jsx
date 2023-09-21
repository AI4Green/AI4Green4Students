import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Alert, AlertIcon, VStack, Text, useToast } from "@chakra-ui/react";
import { useExperimentsList } from "api/experiments";
import { BasicModal } from "components/BasicModal";
import { useBackendApi } from "contexts/BackendApi";

export const DeleteModal = ({ experiment, isModalOpen, onModalClose }) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { experiments: action } = useBackendApi();
  const { mutate: mutate } = useExperimentsList();
  const { t } = useTranslation();
  const toast = useToast();

  const handleDelete = async () => {
    try {
      setIsLoading(true);
      const response = await action.delete({
        id: experiment.id,
      });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          position: "top",
          title: "Experiment deleted",
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
        <Alert status="error">
          <AlertIcon />
          {feedback}
        </Alert>
      )}
      <Text>Are you sure you want to delete this experiment:</Text>
      <Text fontWeight="bold">{experiment.title}</Text>
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
