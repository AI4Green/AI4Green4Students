import { useDisclosure } from "@chakra-ui/react";
import { ActionButton } from "components/core/ActionButton";
import {
  STATUS_ICON_COMPONENTS,
  STAGES,
  SECTION_TYPES,
  STAGES_PERMISSIONS,
} from "constants";
import { useState } from "react";
import { FaCheckCircle, FaExchangeAlt, FaEye, FaLock } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import {
  MoveStageModal,
  OverviewModal,
  CreateNoteFeedbackModal,
  ViewNoteFeedbackModal,
} from "./modal";

export const InstructorAction = ({
  record,
  isEverySectionApproved,
  sectionType,
  sections,
}) => {
  const [modalActionProps, setModalActionProps] = useState({
    modalTitle: "Confirmation",
    fixedNextStage: null,
    successMessage: "Success",
  });

  const {
    isOpen: isAdvanceStageOpen,
    onOpen: onAdvanceStageOpen,
    onClose: onAdvanceStageClose,
  } = useDisclosure();

  const {
    isOpen: isSummaryOpen,
    onOpen: onSummaryOpen,
    onClose: onSummaryClose,
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

  const navigate = useNavigate();

  const actions = createInstructorActions({
    record,
    sectionType,
    isEverySectionApproved,
    onAdvanceStageOpen,
    setModalActionProps,
    navigate,
  });

  // not included in createInstructorActions as this is only specific to Overview page
  actions.viewSummary = {
    isEligible: () => true,
    icon: <FaEye />,
    label: "View Summary",
    onClick: onSummaryOpen,
  };

  actions.completeFeedback = {
    isEligible: () =>
      record?.stage === STAGES.FeedbackRequested &&
      sectionType === SECTION_TYPES.Note,
    icon: <FaCheckCircle />,
    label: "Complete Feedback",
    onClick: () => {
      setModalActionProps({
        modalTitle: `Complete Feedback`,
        modalMessage:
          "Do you wish to proceed with completing feedback for the following Lab Note?",
        successMessage: "Feedback completion succeeded",
        failMessage: `Feedback completion failed`,
        isComplete: true,
      });
      onCreateNoteFeedbackOpen();
    },
  };

  actions.viewFeedback = {
    isEligible: () =>
      sectionType === SECTION_TYPES.Note &&
      (record?.stage === STAGES.InProgressPostFeedback ||
        record?.stage === STAGES.Locked),
    icon: <FaEye />,
    label: "View Feedback",
    onClick: onViewNoteFeedbackOpen,
  };

  return (
    <>
      <ActionButton
        actions={actions}
        size="sm"
        variant="outline"
        colorScheme={
          STATUS_ICON_COMPONENTS[record.stage]?.color.split(".")[0] || // can extract base color. e.g. "green.500" -> "green"
          "gray"
        }
        label={record.stage}
        LeftIcon={STATUS_ICON_COMPONENTS[record.stage]?.icon}
      />
      {isAdvanceStageOpen && (
        <MoveStageModal
          isModalOpen={isAdvanceStageOpen}
          onModalClose={onAdvanceStageClose}
          record={record}
          sectionType={sectionType}
          mutate={record.mutate}
          {...modalActionProps}
        />
      )}
      {isSummaryOpen && (
        <OverviewModal
          isOpen={isSummaryOpen}
          onClose={onSummaryClose}
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

export const createInstructorActions = ({
  record,
  sectionType,
  isEverySectionApproved,
  onAdvanceStageOpen,
  setModalActionProps,
}) => {
  return {
    requestChanges: {
      isEligible: () =>
        record.permissions.includes(STAGES_PERMISSIONS.InstructorCanComment),
      icon: <FaExchangeAlt />,
      label: "Request Changes",
      onClick: () => {
        setModalActionProps({
          modalTitle: "Request Changes",
          modalMessage:
            "Do you wish to proceed with requesting changes for the following?",
          fixedNextStage: STAGES.AwaitingChanges,
          successMessage: "Request changes succeeded",
          failMessage: "Request changes failed",
        });
        onAdvanceStageOpen();
      },
    },
    cancelRequestChanges: {
      isEligible: () => record.stage === STAGES.AwaitingChanges,
      icon: <FaExchangeAlt />,
      label: "Cancel Request Changes",
      onClick: () => {
        setModalActionProps({
          modalTitle: "Cancel Request Changes",
          modalMessage:
            "Do you wish to proceed with cancelling the request changes for the following?",
          fixedNextStage: STAGES.InReview,
          successMessage: "Request changes cancellation succeeded",
          failMessage: "Request changes cancellation failed",
        });
        onAdvanceStageOpen();
      },
    },
    markAsApproved: {
      isEligible: () =>
        record.permissions.includes(STAGES_PERMISSIONS.InstructorCanComment) &&
        isEverySectionApproved,
      icon: <FaCheckCircle />,
      label: "Mark as approved",
      onClick: () => {
        setModalActionProps({
          modalTitle: "Mark as approved",
          modalMessage:
            "Do you wish to proceed with marking the following as approved?",
          fixedNextStage: STAGES.Approved,
          successMessage: "Mark as approved succeeded",
          failMessage: "Mark as approved failed",
        });
        onAdvanceStageOpen();
      },
    },
    cancelApproval: {
      isEligible: () => record.stage === STAGES.Approved,
      icon: <FaCheckCircle />,
      label: "Cancel Approval",
      onClick: () => {
        setModalActionProps({
          modalTitle: "Cancel Approval",
          modalMessage:
            "Do you wish to proceed with cancelling the approval for the following?",
          fixedNextStage: STAGES.InReview,
          successMessage: "Approval cancellation succeeded",
          failMessage: "Approval cancellation  failed",
        });
        onAdvanceStageOpen();
      },
    },
    cancelNotesLock: {
      isEligible: () =>
        record.stage === STAGES.Locked && sectionType === SECTION_TYPES.Note,
      icon: <FaLock />,
      label: "Cancel Note Lock",
      onClick: () => {
        setModalActionProps({
          modalTitle: "Cancel Note Lock",
          modalMessage:
            "Do you wish to proceed with cancelling the note lock for the following?",
          fixedNextStage: record.feedbackRequested
            ? STAGES.InProgressPostFeedback
            : STAGES.InProgress,
          successMessage: "Note lock cancellation succeeded",
          failMessage: "Note lock cancellation  failed",
        });
        onAdvanceStageOpen();
      },
    },
  };
};
