import { useState } from "react";
import { useTranslation } from "react-i18next";
import {
  Alert,
  AlertIcon,
  VStack,
  Text,
  useToast,
  HStack,
  Icon,
} from "@chakra-ui/react";
import { Modal } from "components/core/Modal";
import { useBackendApi } from "contexts";
import {
  GLOBAL_PARAMETERS,
  SECTION_TYPES,
  TITLE_ICON_COMPONENTS,
} from "constants";

export const MoveStageModal = ({
  fixedNextStage,
  modalTitle,
  modalMessage,
  successMessage,
  isModalOpen,
  onModalClose,
  record,
  sectionType,
  mutate,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { t } = useTranslation();
  const toast = useToast();
  const { action, icon } = getStageItems(sectionType);

  const handleAdvanceStage = async () => {
    try {
      setIsLoading(true);
      const response = await action.advanceStage(record.id, fixedNextStage);
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          position: "top",
          title: successMessage,
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
      <HStack spacing={5}>
        <Icon as={icon} color={"green.500"} fontSize="5xl" />
        <VStack align="flex-end" flex={1}>
          <Text align="right">{modalMessage}</Text>
          <Text fontWeight="bold">{record.title}</Text>
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
      title={modalTitle}
      actionBtnColorScheme="green"
      onAction={handleAdvanceStage}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={() => {
        resetState();
        onModalClose();
      }}
    />
  );
};

const getStageItems = (sectionType) => {
  const {
    literatureReviews: lrAction,
    plans: planAction,
    notes: noteAction,
    reports: reportAction,
  } = useBackendApi();

  let items;
  switch (sectionType) {
    case SECTION_TYPES.Plan:
      items = {
        action: planAction,
        icon: TITLE_ICON_COMPONENTS.Plan,
      };
      break;
    case SECTION_TYPES.Report:
      items = {
        action: reportAction,
        icon: TITLE_ICON_COMPONENTS.Report,
      };
      break;
    case SECTION_TYPES.LiteratureReview:
      items = {
        action: lrAction,
        icon: TITLE_ICON_COMPONENTS.LiteratureReview,
      };
      break;
    case SECTION_TYPES.Note:
      items = {
        action: noteAction,
        icon: TITLE_ICON_COMPONENTS.Note,
      };
      break;
  }
  return items;
};
