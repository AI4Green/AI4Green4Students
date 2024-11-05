import { useDisclosure } from "@chakra-ui/react";
import { ActionButton } from "components/core/ActionButton";
import { STATUS_ICON_COMPONENTS } from "constants/experiment-ui";
import { useState } from "react";
import { FaCheckCircle, FaExchangeAlt, FaEye, FaLock } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import { MoveStageModal, OverviewModal } from "./modal";
import { STAGES_PERMISSIONS } from "constants/site-permissions";
import { STAGES } from "constants/stages";
import { SECTION_TYPES } from "constants/section-types";

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
    isOpen: isOpenAdvanceStage,
    onOpen: onOpenAdvanceStage,
    onClose: onCloseAdvanceStage,
  } = useDisclosure();

  const {
    isOpen: isOpenSummary,
    onOpen: onOpenSummary,
    onClose: onCloseSummary,
  } = useDisclosure();

  const navigate = useNavigate();

  const actions = createInstructorActions({
    record,
    sectionType,
    isEverySectionApproved,
    onOpenAdvanceStage,
    setModalActionProps,
    navigate,
  });

  // not included in createInstructorActions as this is only specific to Overview page
  actions.viewSummary = {
    isEligible: () => true,
    icon: <FaEye />,
    label: "View Summary",
    onClick: onOpenSummary,
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
      {isOpenAdvanceStage && (
        <MoveStageModal
          isModalOpen={isOpenAdvanceStage}
          onModalClose={onCloseAdvanceStage}
          record={record}
          sectionType={sectionType}
          mutate={record.mutate}
          {...modalActionProps}
        />
      )}
      {isOpenSummary && (
        <OverviewModal
          isOpen={isOpenSummary}
          onClose={onCloseSummary}
          sections={sections}
          record={record}
          sectionType={sectionType}
        />
      )}
    </>
  );
};

export const createInstructorActions = ({
  record,
  sectionType,
  isEverySectionApproved,
  onOpenAdvanceStage,
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
        onOpenAdvanceStage();
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
        onOpenAdvanceStage();
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
        onOpenAdvanceStage();
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
        onOpenAdvanceStage();
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
          fixedNextStage: STAGES.Draft,
          successMessage: "Note lock cancellation succeeded",
          failMessage: "Note lock cancellation  failed",
        });
        onOpenAdvanceStage();
      },
    },
  };
};
