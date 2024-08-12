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
import { Modal } from "components/Modal";
import { useBackendApi } from "contexts/BackendApi";
import { FaBook, FaChartLine, FaTasks } from "react-icons/fa";
import { GLOBAL_PARAMETERS } from "constants/global-parameters";

export const MoveStageModal = ({
  fixedNextStage,
  modalTitle,
  modalMessage,
  successMessage,
  isModalOpen,
  onModalClose,
  record,
  isPlan,
  isReport,
  isLiteratureReview,
  mutate,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { t } = useTranslation();
  const toast = useToast();
  const { action, icon } = getStageItems(isPlan, isReport, isLiteratureReview);

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

const getStageItems = (isPlan, isReport, isLiteratureReview) => {
  const {
    literatureReviews: lrAction,
    plans: planAction,
    reports: reportAction,
  } = useBackendApi();

  let items;
  switch (true) {
    case isPlan:
      items = {
        action: planAction,
        icon: FaTasks,
      };
      break;
    case isReport:
      items = {
        action: reportAction,
        icon: FaChartLine,
      };
      break;
    case isLiteratureReview:
      items = {
        action: lrAction,
        icon: FaBook,
      };
      break;
  }
  return items;
};
