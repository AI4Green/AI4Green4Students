import { useDisclosure } from "@chakra-ui/react";
import { ActionButton } from "components/core/ActionButton";
import { FaEye, FaPaperPlane, FaRegComment } from "react-icons/fa";
import {
  OverviewModal,
  CreateNoteFeedbackModal,
  ViewNoteFeedbackModal,
} from "./modal";
import { useState } from "react";
import { STAGES, STATUS_ICON_COMPONENTS, SECTION_TYPES } from "constants";

export const StudentAction = ({ sections, record, sectionType }) => {
  const {
    isOpen: isViewSummaryOpen,
    onOpen: onViewSummaryOpen,
    onClose: onViewSummaryClose,
  } = useDisclosure();

  const {
    isOpen: isCreateNoteFeedbackOpen,
    onOpen: onCreateNoteFeedbackOpen,
    onClose: onCreateNoteFeedbackClose,
  } = useDisclosure();

  const {
    isOpen: isViewNoteFeedbackOpen,
    onOpen: onViewNoteFeedbackOpen,
    onClose: onViewNoteFeedbackClose,
  } = useDisclosure();

  const [modalActionProps, setModalActionProps] = useState({
    modalTitle: "Confirmation",
    fixedNextStage: null,
    successMessage: "Success",
  });

  return (
    <>
      <ActionButton
        actions={{
          viewSummary: {
            isEligible: () => true,
            icon: <FaEye />,
            label: "View Summary",
            onClick: onViewSummaryOpen,
          },
          requestNoteFeedback: {
            isEligible: () =>
              record?.stage === STAGES.InProgress &&
              sectionType === SECTION_TYPES.Note,
            icon: <FaPaperPlane />,
            label: "Request Feedback",
            onClick: () => {
              setModalActionProps({
                modalTitle: `Request Feedback`,
                modalMessage:
                  "Do you wish to proceed with requesting feedback for the following Lab Note?",
                successMessage: "Feedback request succeeded",
                failMessage: `Feedback request failed`,
              });
              onCreateNoteFeedbackOpen();
            },
          },
          viewFeedback: {
            isEligible: () =>
              sectionType === SECTION_TYPES.Note &&
              (record?.stage === STAGES.InProgressPostFeedback ||
                record?.stage === STAGES.Locked),
            icon: <FaRegComment />,
            label: "View Feedback",
            onClick: onViewNoteFeedbackOpen,
          },
        }}
        size="sm"
        variant="outline"
        colorScheme={
          STATUS_ICON_COMPONENTS[record.stage]?.color.split(".")[0] || "gray"
        }
        label={record.stage}
        LeftIcon={STATUS_ICON_COMPONENTS[record.stage]?.icon}
      />
      {isViewSummaryOpen && (
        <OverviewModal
          isOpen={isViewSummaryOpen}
          onClose={onViewSummaryClose}
          sections={sections}
          record={record}
          sectionType={sectionType}
        />
      )}
      {isCreateNoteFeedbackOpen && (
        <CreateNoteFeedbackModal
          isModalOpen={isCreateNoteFeedbackOpen}
          onModalClose={onCreateNoteFeedbackClose}
          record={record}
          sectionType={sectionType}
          mutate={record.mutate}
          {...modalActionProps}
        />
      )}
      {isViewNoteFeedbackOpen && (
        <ViewNoteFeedbackModal
          isModalOpen={isViewNoteFeedbackOpen}
          onModalClose={onViewNoteFeedbackClose}
          note={record}
        />
      )}
    </>
  );
};
