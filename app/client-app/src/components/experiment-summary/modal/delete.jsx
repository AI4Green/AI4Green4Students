import {
  Alert,
  AlertIcon,
  HStack,
  Icon,
  Text,
  useToast,
  VStack,
} from "@chakra-ui/react";
import { useProjectSummaryByStudent } from "api";
import { Modal } from "components/core/modal";
import { GLOBAL_PARAMETERS, SECTION_TYPES } from "constants";
import { useBackendApi } from "contexts";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { FaBook, FaChartLine, FaTasks } from "react-icons/fa";

export const DeleteModal = ({
  isModalOpen,
  onModalClose,
  record,
  sectionType,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { mutate } = useProjectSummaryByStudent(record?.project?.id);
  const { t } = useTranslation();
  const toast = useToast();

  const { literatureReviews, plans, reports } = useBackendApi();
  const { action, label, icon } = getDeleteItems(sectionType, {
    literatureReviews,
    plans,
    reports,
  });

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
          duration: GLOBAL_PARAMETERS.ToastDuration,
          isClosable: true,
        });
        await mutate();
        onModalClose();
      }
    } catch {
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
          <Text fontWeight="bold">{record?.title}</Text>
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
      title="Delete Confirmation"
      actionBtnCaption="Delete"
      actionBtnColorScheme="red"
      onAction={handleDelete}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={() => {
        resetState();
        onModalClose();
      }}
    />
  );
};

/**
 * Get the relevant items based on the section type.
 * @param {string} sectionType - section type
 * @returns - object relevant to the section type.
 * for e.g. if section type is 'Plan', it will return
 * - planAction, used for deleting a plan
 * - label, used to display the title of the modal
 */
const getDeleteItems = (sectionType, apis) => {
  const { literatureReviews, plans, reports } = apis;

  let items;
  switch (sectionType) {
    case SECTION_TYPES.Plan:
      items = {
        action: plans,
        label: "Plan",
        icon: FaTasks,
      };
      break;
    case SECTION_TYPES.Report:
      items = {
        action: reports,
        label: "Report",
        icon: FaChartLine,
      };
      break;
    case SECTION_TYPES.LiteratureReview:
      items = {
        action: literatureReviews,
        label: "Literature review",
        icon: FaBook,
      };
      break;
  }
  return items;
};
