import { useDisclosure } from "@chakra-ui/react";
import { FaFileExport, FaLink, FaPaperPlane, FaTrash } from "react-icons/fa";
import { GiMaterialsScience } from "react-icons/gi";
import { useNavigate } from "react-router-dom";
import { ActionButton } from "components/ActionButton";
import { DeleteModal } from "components/experiment/modal/DeleteModal";
import { STAGES_PERMISSIONS } from "constants/site-permissions";
import { MoveStageModal } from "components/experiment/modal/MoveStageModal";
import { useState } from "react";
import {
  useProjectSummaryByProjectGroup,
  useProjectSummaryByStudent,
} from "api/projects";
import { STAGES } from "constants/stages";
import { useBackendApi } from "contexts/BackendApi";

export const PlanOverviewAction = ({ plan, isInstructor }) => {
  return (
    <Action
      isInstructor={isInstructor}
      record={plan}
      recordType={RECORD_TYPES.Plan}
      isPlan
    />
  );
};

export const LiteratureReviewAction = ({ literatureReview, isInstructor }) => {
  return (
    <Action
      isInstructor={isInstructor}
      record={literatureReview}
      recordType={RECORD_TYPES.LiteratureReview}
      isLiteratureReview
    />
  );
};

export const ReportAction = ({ report, isInstructor }) => {
  return (
    <Action
      isInstructor={isInstructor}
      record={report}
      recordType={RECORD_TYPES.Report}
      isReport
    />
  );
};

const Action = ({
  record,
  isInstructor,
  recordType,
  isPlan,
  isLiteratureReview,
  isReport,
}) => {
  const mutate = useConditionalProjectSummary(isInstructor, record);
  const { reports: reportAction } = useBackendApi();

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

  const actions = createActions({
    record,
    isInstructor,
    onOpenDelete,
    onOpenAdvanceStage,
    setModalActionProps,
    navigate,
    recordType,
  });

  if (isPlan) {
    actions.labNote = {
      isEligible: () => record.status === STAGES.Approved,
      icon: <GiMaterialsScience />,
      label: "Lab note",
      onClick: () => navigate(record.note?.overviewPath),
    };
  }

  if (isReport) {
    actions.export = {
      isEligible: () => true,
      icon: <FaFileExport />,
      label: STUDENT_ACTIONS.Export,
      onClick: async () => {
        const response = await reportAction.downloadReportExport(record.id);
        const blob = await response.blob();
        const url = URL.createObjectURL(blob);

        const contentDisposition = await response.headers.get(
          "content-disposition"
        );
        const filenamePart = contentDisposition
          ?.split(";")
          .find((n) => n.includes("filename="));
        const filename = filenamePart
          ? filenamePart.replace("filename=", "").trim()
          : "download";

        const link = Object.assign(document.createElement("a"), {
          href: url,
          download: filename,
        });

        document.body.appendChild(link).click();
        URL.revokeObjectURL(url);
      },
    };
  }

  return (
    <>
      <ActionButton actions={actions} size="xs" variant="outline" />
      {isOpenDelete && (
        <DeleteModal
          isModalOpen={isOpenDelete}
          onModalClose={onCloseDelete}
          record={record}
          isPlan={isPlan}
          isLiteratureReview={isLiteratureReview}
          isReport={isReport}
        />
      )}
      {isOpenAdvanceStage && (
        <MoveStageModal
          isModalOpen={isOpenAdvanceStage}
          onModalClose={onCloseAdvanceStage}
          record={record}
          isPlan={isPlan}
          isLiteratureReview={isLiteratureReview}
          isReport={isReport}
          projectId={record.project?.id}
          projectGroupId={record.projectGroup?.id}
          mutate={mutate}
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
      label: STUDENT_ACTIONS.View,
      onClick: () => navigate(record.overviewPath),
    },
    delete: {
      isEligible: () =>
        !isInstructor &&
        record.stagePermissions.includes(STAGES_PERMISSIONS.OwnerCanEdit),
      icon: <FaTrash />,
      label: STUDENT_ACTIONS.Delete,
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
        ? STUDENT_ACTIONS.SubmitChanges
        : STUDENT_ACTIONS.Submit,
      onClick: () => {
        setModalActionProps(getSubmitModalActionProps(recordType));
        onOpenAdvanceStage();
      },
    },
  };
};

const useConditionalProjectSummary = (isInstructor, record) => {
  const { mutate } = isInstructor
    ? useProjectSummaryByProjectGroup(record?.projectGroup?.id)
    : useProjectSummaryByStudent(record?.project?.id);
  return mutate;
};

const RECORD_TYPES = {
  Plan: "Plan",
  LiteratureReview: "Literature Review",
  Report: "Report",
};

const STUDENT_ACTIONS = {
  View: "View",
  Delete: "Delete",
  Submit: "Submit",
  SubmitChanges: "Submit Changes",
  Export: "Export",
};

const getSubmitModalActionProps = (recordType) => ({
  modalTitle: `Submit ${recordType}`,
  modalMessage: `Do you wish to proceed with submission of the following?`,
  successMessage: `${recordType} submission succeeded`,
  failMessage: `${recordType} submission failed`,
  fixedNextStage: recordType === RECORD_TYPES.Report ? STAGES.Submitted : null,
});
