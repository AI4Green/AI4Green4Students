import { useState } from "react";
import { useTranslation } from "react-i18next";
import {
  Alert,
  AlertIcon,
  VStack,
  Text,
  useToast,
  HStack,
  Badge,
  Icon,
} from "@chakra-ui/react";
import { Modal } from "components/core/Modal";
import { useBackendApi } from "contexts/BackendApi";
import { GLOBAL_PARAMETERS } from "constants/global-parameters";
import { FaLock } from "react-icons/fa";

export const LockProjectGroupNotesModal = ({
  projectGroup,
  isModalOpen,
  onModalClose,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { notes: noteAction } = useBackendApi();
  const { t } = useTranslation();
  const toast = useToast();

  const handleNotesLocking = async () => {
    try {
      setIsLoading(true);
      const response = await noteAction.lockProjectGroupNotes(projectGroup.id);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          title: `All the notes for project group ${projectGroup.name} has now been locked.`,
          status: "success",
          duration: GLOBAL_PARAMETERS.ToastDuration,
          position: "top",
          isClosable: true,
        });
        onModalClose();
      }
    } catch (e) {
      setFeedback({
        status: "error",
        message: t("feedback.error_title"),
      });
    } finally {
      setIsLoading(false);
    }
  };

  const modalBody = (
    <VStack align="flex-start" spacing={4}>
      {feedback && (
        <Alert status={feedback.status}>
          <AlertIcon />
          {feedback.message}
        </Alert>
      )}
      <Text>
        Are you sure you want to lock all the notes for this project group?
      </Text>

      <HStack borderWidth={1} borderRadius={7} p={2} w="full">
        <Icon as={FaLock} color="yellow.400" fontSize="5xl" />
        <VStack align="stretch" spacing={0}>
          <Text fontWeight="bold" fontSize="xl">
            {projectGroup.name}
          </Text>
          <HStack>
            <Badge colorScheme="green">Project</Badge>
            <Text as="b" fontSize="sm">
              {projectGroup?.project?.name}
            </Text>
          </HStack>
        </VStack>
      </HStack>
    </VStack>
  );

  const resetState = () => {
    setFeedback();
    setIsLoading(false);
  };

  return (
    <Modal
      body={modalBody}
      title="Lock Notes Confirmation"
      actionBtnCaption="Lock"
      actionBtnColorScheme="yellow"
      onAction={handleNotesLocking}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={() => {
        resetState();
        onModalClose();
      }}
    />
  );
};
