import { useDisclosure } from "@chakra-ui/react";
import { FaTrash, FaLink, FaPaperPlane, FaExchangeAlt } from "react-icons/fa";
import { GiMaterialsScience } from "react-icons/gi";
import { useNavigate } from "react-router-dom";
import { ActionButton } from "components/ActionButton";
import { STAGES } from "constants/stages";
import { DeleteModal } from "../modal/DeleteModal";
import { STAGES_PERMISSIONS } from "constants/site-permissions";
import { MoveStageModal } from "../modal/MoveStageModal";
import { useState } from "react";

export const PlanOverviewAction = ({ plan, isInstructor }) => {
  const [modalActionProps, setModalActionProps] = useState({
    modalTitle: "Confirmation",
    fixedNextStage: null,
    successMessage: "Success",
  });

  const {
    isOpen: isOpenDelete,
    onOpen: onOpenDelete,
    onClose: onCloseDelete,
  } = useDisclosure();
  const {
    isOpen: isOpenAdvanceStage,
    onOpen: onOpenAdvanceStage,
    onClose: onCloseAdvanceStage,
  } = useDisclosure();
  const navigate = useNavigate();

  const planOverviewActions = createActions({
    record: plan,
    isInstructor,
    onOpenDelete,
    onOpenAdvanceStage,
    setModalActionProps,
    navigate,
    recordType: "plan",
  });

  planOverviewActions.labNote = {
    isEligible: () => true,
    icon: <GiMaterialsScience />,
    label: "Lab note",
    onClick: () => navigate(plan.note.overviewPath),
  };

  return (
    <>
      <ActionButton actions={planOverviewActions} size="xs" variant="outline" />
      {isOpenDelete && (
        <DeleteModal
          isModalOpen={isOpenDelete}
          onModalClose={onCloseDelete}
          record={plan}
          isPlan
        />
      )}
      {isOpenAdvanceStage && (
        <MoveStageModal
          isModalOpen={isOpenAdvanceStage}
          onModalClose={onCloseAdvanceStage}
          record={plan}
          isPlan
          {...modalActionProps}
        />
      )}
    </>
  );
};

export const LiteratureReviewAction = ({ literatureReview, isInstructor }) => {
  const [modalActionProps, setModalActionProps] = useState({
    modalTitle: "Confirmation",
    fixedNextStage: null,
    successMessage: "Success",
  });
  const {
    isOpen: isOpenDelete,
    onOpen: onOpenDelete,
    onClose: onCloseDelete,
  } = useDisclosure();
  const {
    isOpen: isOpenAdvanceStage,
    onOpen: onOpenAdvanceStage,
    onClose: onCloseAdvanceStage,
  } = useDisclosure();
  const navigate = useNavigate();

  const literatureReviewActions = createActions({
    record: literatureReview,
    isInstructor,
    onOpenDelete,
    onOpenAdvanceStage,
    setModalActionProps,
    navigate,
    recordType: "literatureReview",
  });

  return (
    <>
      <ActionButton
        actions={literatureReviewActions}
        size="xs"
        variant="outline"
      />
      {isOpenDelete && (
        <DeleteModal
          isModalOpen={isOpenDelete}
          onModalClose={onCloseDelete}
          record={literatureReview}
          isLiteratureReview
        />
      )}
      {isOpenAdvanceStage && (
        <MoveStageModal
          isModalOpen={isOpenAdvanceStage}
          onModalClose={onCloseAdvanceStage}
          record={literatureReview}
          isLiteratureReview
          {...modalActionProps}
        />
      )}
    </>
  );
};

const createActions = ({
  record,
  isInstructor,
  onOpenDelete,
  onOpenAdvanceStage,
  setModalActionProps,
  navigate,
  recordType,
}) => {
  return {
    view: {
      isEligible: () => true,
      icon: <FaLink />,
      label: "View",
      onClick: () => navigate(record.overviewPath),
    },
    delete: {
      isEligible: () =>
        !isInstructor &&
        record.stagePermissions.includes(STAGES_PERMISSIONS.OwnerCanEdit),
      icon: <FaTrash />,
      label: "Delete",
      onClick: onOpenDelete,
    },
    submit: {
      isEligible: () =>
        !isInstructor &&
        [
          STAGES_PERMISSIONS.OwnerCanEdit,
          STAGES_PERMISSIONS.OwnerCanEditCommented,
        ].some((permission) => record.stagePermissions.includes(permission)),
      icon: <FaPaperPlane />,
      label: record.stagePermissions.includes(
        STAGES_PERMISSIONS.OwnerCanEditCommented
      )
        ? "Submit changes"
        : "Submit",
      onClick: () => {
        setModalActionProps({
          modalTitle: `Submit ${recordType}`,
          modalMessage:
            "Do you wish to proceed with submission of the following?",
          successMessage: `${recordType} submission succeeded`,
          failMessage: `${recordType} submission failed`,
        });
        onOpenAdvanceStage();
      },
    },
    requestChanges: {
      isEligible: () =>
        isInstructor &&
        record.stagePermissions.includes(
          STAGES_PERMISSIONS.InstructorCanComment
        ),
      icon: <FaExchangeAlt />,
      label: "Request changes",
      onClick: () => {
        setModalActionProps({
          modalTitle: "Request changes",
          modalMessage:
            "Do you wish to proceed with requesting changes for the following?",
          fixedNextStage: STAGES.AwaitingChanges,
          successMessage: "Request changes succeeded",
          failMessage: "Request changes failed",
          isInstructor,
        });
        onOpenAdvanceStage();
      },
    },
    cancelRequestChanges: {
      isEligible: () =>
        isInstructor && record.status === STAGES.AwaitingChanges,
      icon: <FaExchangeAlt />,
      label: "Cancel request changes",
      onClick: () => {
        setModalActionProps({
          modalTitle: "Cancel Request changes",
          modalMessage:
            "Do you wish to proceed with cancelling the request changes for the following?",
          fixedNextStage: STAGES.InReview,
          successMessage: "Request changes cancellation succeeded",
          failMessage: "Request changes cancellation failed",
          isInstructor,
        });
        onOpenAdvanceStage();
      },
    },
  };
};
