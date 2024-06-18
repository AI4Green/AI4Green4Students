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
import { useProjectSummaryByStudent } from "api/projects";
import { Modal } from "components/Modal";
import { useBackendApi } from "contexts/BackendApi";
import { FaBook, FaChartLine, FaTasks } from "react-icons/fa";

export const DeleteModal = ({
  isModalOpen,
  onModalClose,
  record,
  isPlan,
  isReport,
  isLiteratureReview,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { mutate } = useProjectSummaryByStudent(record?.project?.id);
  const { t } = useTranslation();
  const toast = useToast();
  const { action, label, icon } = getDeleteItems(
    isPlan,
    isReport,
    isLiteratureReview
  );

  const handleDelete = async () => {
    try {
      setIsLoading(true);
      const response = await action.delete(record.id);
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          position: "top",
          title: `${label} deleted`,
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

  const modalBody = (
    <VStack>
      {feedback && (
        <Alert status={feedback.status}>
          <AlertIcon />
          {feedback.message}
        </Alert>
      )}
      <HStack spacing={5}>
        <Icon as={icon} color={"red.500"} fontSize="5xl" />
        <VStack align="flex-end" flex={1}>
          <Text align="right">
            Are you sure you want to delete this {label}?
          </Text>
          <Text fontWeight="bold">{record.title}</Text>
        </VStack>
      </HStack>
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

/**
 * Get the relevant items based on the section type.
 * @param {bool} isPlan - is the section type a plan
 * @param {bool} isReport - is the section type a report
 * @param {bool} isLiteratureReview - is the section type a literature review
 * @returns - object relevant to the section type.
 * for e.g. if 'isPlan' is true, it will return
 * - planAction, used for deleting a plan
 * - label, used to display the title of the modal
 */
const getDeleteItems = (isPlan, isReport, isLiteratureReview) => {
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
        label: "Plan",
        icon: FaTasks,
      };
      break;
    case isReport:
      items = {
        action: reportAction,
        label: "Report",
        icon: FaChartLine,
      };
      break;
    case isLiteratureReview:
      items = {
        action: lrAction,
        label: "Literature review",
        icon: FaBook,
      };
      break;
  }
  return items;
};
