import {
  HStack,
  Icon,
  Text,
  useDisclosure,
  VStack,
  useBreakpointValue,
} from "@chakra-ui/react";
import { FaFileExport, FaLink, FaPaperPlane, FaTrash } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import { ActionButton } from "components/ActionButton";
import { DeleteModal } from "components/experiment/modal/DeleteModal";
import { STAGES_PERMISSIONS } from "constants/site-permissions";
import { MoveStageModal } from "components/experiment/modal/MoveStageModal";
import { useState } from "react";
import { useProjectSummaryByStudent } from "api/projects";
import { useBackendApi } from "contexts/BackendApi";
import { CreateOrEditModal } from "components/experiment/modal/CreateOrEditModal";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { STAGES } from "constants/stages";
import { SECTION_TYPES } from "constants/section-types";
import { STATUS_ICON_COMPONENTS } from "constants/experiment-ui";

export const LiteratureReviewAction = ({
  literatureReview,
  isInstructor,
  project,
  label = SECTION_TYPE_LABELS.LiteratureReview,
  LeftIcon = TITLE_ICON_COMPONENTS[SECTION_TYPES.LiteratureReview],
  studentId,
}) => {
  return (
    <Action
      isInstructor={isInstructor}
      record={literatureReview || null}
      sectionType={SECTION_TYPES.LiteratureReview}
      project={project}
      label={label}
      LeftIcon={LeftIcon}
      studentId={studentId}
    />
  );
};

export const ReportAction = ({
  report,
  isInstructor,
  project,
  label = SECTION_TYPE_LABELS.Report,
  LeftIcon = TITLE_ICON_COMPONENTS[SECTION_TYPES.Report],
  studentId,
}) => {
  return (
    <Action
      isInstructor={isInstructor}
      record={report || null}
      sectionType={SECTION_TYPES.Report}
      project={project}
      label={label}
      LeftIcon={LeftIcon}
      studentId={studentId}
    />
  );
};

const Action = ({
  record,
  isInstructor,
  project,
  sectionType,
  label,
  LeftIcon,
  studentId,
}) => {
  const { mutate } = useProjectSummaryByStudent(project.id, studentId);
  const { reports: reportAction } = useBackendApi();

  const [modalActionProps, setModalActionProps] = useState({
    modalTitle: "Confirmation",
    fixedNextStage: null,
    successMessage: "Success",
  });

  const {
    isOpen: isOpenNew,
    onOpen: onOpenNew,
    onClose: onCloseNew,
  } = useDisclosure();

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
    onOpenNew,
    onOpenDelete,
    onOpenAdvanceStage,
    setModalActionProps,
    navigate,
    label,
  });

  if (sectionType === SECTION_TYPES.Report) {
    actions.export = {
      isEligible: () => record,
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

  const displayStatus = useBreakpointValue({ base: false, md: true });
  const StatusIcon = STATUS_ICON_COMPONENTS[record?.stage]?.icon || null;

  return (
    <>
      <VStack align="end">
        {record && displayStatus && (
          <HStack>
            <Icon
              as={STATUS_ICON_COMPONENTS[record.stage].icon}
              color={STATUS_ICON_COMPONENTS[record.stage].color}
            />

            <Text fontSize="xs" color="gray.700">
              {record?.stage}
            </Text>
          </HStack>
        )}
        <ActionButton
          actions={{
            ...actions,
            ...(displayStatus ||
              (record && {
                status: {
                  isEligible: () => record,
                  icon: <StatusIcon />,
                  label: `Status - ${record?.stage}`,
                  isDisabled: !displayStatus,
                },
              })),
          }}
          variant={record ? "outline" : "solid"}
          label={label}
          LeftIcon={LeftIcon}
          colorScheme={
            displayStatus
              ? record
                ? "gray"
                : "green"
              : STATUS_ICON_COMPONENTS[record?.stage]?.color || "green"
          }
          py={{ base: 3, md: 4 }}
        />
      </VStack>
      {isOpenDelete && (
        <DeleteModal
          isModalOpen={isOpenDelete}
          onModalClose={onCloseDelete}
          record={record}
          sectionType={sectionType}
        />
      )}
      {isOpenAdvanceStage && (
        <MoveStageModal
          isModalOpen={isOpenAdvanceStage}
          onModalClose={onCloseAdvanceStage}
          record={record}
          sectionType={sectionType}
          mutate={mutate}
          {...modalActionProps}
        />
      )}
      {isOpenNew && (
        <CreateOrEditModal
          isModalOpen={isOpenNew}
          onModalClose={onCloseNew}
          project={project}
          sectionType={sectionType}
        />
      )}
    </>
  );
};

const createActions = ({
  record,
  isInstructor,
  onOpenDelete,
  onOpenNew,
  onOpenAdvanceStage,
  setModalActionProps,
  navigate,
  msgLabel,
}) => {
  return {
    new: {
      isEligible: () => !record && !isInstructor,
      icon: <FaLink />,
      label: "New",
      onClick: onOpenNew,
    },
    view: {
      isEligible: () => record,
      icon: <FaLink />,
      label: STUDENT_ACTIONS.View,
      onClick: () => navigate(record.overviewPath),
    },
    delete: {
      isEligible: () =>
        record &&
        !isInstructor &&
        record?.permissions?.includes(STAGES_PERMISSIONS.OwnerCanEdit),
      icon: <FaTrash />,
      label: STUDENT_ACTIONS.Delete,
      onClick: onOpenDelete,
    },
    submit: {
      isEligible: () =>
        record &&
        !isInstructor &&
        [
          STAGES_PERMISSIONS.OwnerCanEdit,
          STAGES_PERMISSIONS.OwnerCanEditCommented,
        ].some((permission) => record?.permissions?.includes(permission)),
      icon: <FaPaperPlane />,
      label: record?.permissions?.includes(
        STAGES_PERMISSIONS.OwnerCanEditCommented
      )
        ? STUDENT_ACTIONS.SubmitChanges
        : STUDENT_ACTIONS.Submit,
      onClick: () => {
        setModalActionProps(getSubmitModalActionProps(msgLabel));
        onOpenAdvanceStage();
      },
    },
  };
};

const SECTION_TYPE_LABELS = {
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

const getSubmitModalActionProps = (msgLabel) => ({
  modalTitle: `Submit ${msgLabel}`,
  modalMessage: `Do you wish to proceed with submission of the following?`,
  successMessage: `${msgLabel} submission succeeded`,
  failMessage: `${msgLabel} submission failed`,
  fixedNextStage:
    msgLabel === SECTION_TYPE_LABELS.Report ? STAGES.Submitted : null,
});
