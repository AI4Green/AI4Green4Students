import {
  HStack,
  Icon,
  Text,
  useDisclosure,
  VStack,
  useBreakpointValue,
  useToast,
} from "@chakra-ui/react";
import {
  FaEye,
  FaFileExport,
  FaLink,
  FaPaperPlane,
  FaTrash,
} from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import { ActionButton } from "components/core/ActionButton";
import { DeleteModal, MoveStageModal, CreateOrEditModal } from "./modal";
import { STAGES_PERMISSIONS } from "constants/site-permissions";
import { useState } from "react";
import { useProjectSummaryByStudent } from "api/projects";
import { useBackendApi } from "contexts/BackendApi";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { STAGES } from "constants/stages";
import { SECTION_TYPES } from "constants/section-types";
import { STATUS_ICON_COMPONENTS } from "constants/experiment-ui";
import { useUser } from "contexts/User";
import { GLOBAL_PARAMETERS } from "constants/global-parameters";

export const LiteratureReviewAction = ({
  literatureReview,
  project,
  label = SECTION_TYPE_LABELS.LiteratureReview,
  LeftIcon = TITLE_ICON_COMPONENTS[SECTION_TYPES.LiteratureReview],
  studentId,
}) => {
  return (
    <Action
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
  project,
  label = SECTION_TYPE_LABELS.Report,
  LeftIcon = TITLE_ICON_COMPONENTS[SECTION_TYPES.Report],
  studentId,
}) => {
  return (
    <Action
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
  project,
  sectionType,
  label,
  LeftIcon,
  studentId,
}) => {
  const { user } = useUser();
  const { reports: reportAction } = useBackendApi();

  const toast = useToast();
  const displayToast = (title, status) => {
    toast({
      title,
      status,
      duration: GLOBAL_PARAMETERS.ToastDuration,
      isClosable: true,
      position: "top",
    });
  };

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

  const canManage =
    (record && record.ownerId === user.userId) ||
    (studentId && studentId === user.userId) ||
    !studentId; // Check if the user can manage the record

  const actions = createActions({
    record,
    canManage,
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
        try {
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
            ? filenamePart.split("=")[1].trim().replace(/^"|"$/g, "")
            : "download";

          const link = Object.assign(document.createElement("a"), {
            href: url,
            download: filename,
          });

          document.body.appendChild(link).click();
          URL.revokeObjectURL(url);

          displayToast("Export successful!", "success");
        } catch (e) {
          displayToast("Export failed!", "error");
        }
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
      {canManage && isOpenDelete && (
        <DeleteModal
          isModalOpen={isOpenDelete}
          onModalClose={onCloseDelete}
          record={record}
          sectionType={sectionType}
        />
      )}
      {canManage && isOpenAdvanceStage && (
        <StageAdvanceModal
          projectId={project.id}
          record={record}
          sectionType={sectionType}
          isOpenAdvanceStage={isOpenAdvanceStage}
          onCloseAdvanceStage={onCloseAdvanceStage}
          modalActionProps={modalActionProps}
        />
      )}
      {canManage && isOpenNew && (
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

const StageAdvanceModal = ({
  projectId,
  record,
  sectionType,
  isOpenAdvanceStage,
  onCloseAdvanceStage,
  modalActionProps,
}) => {
  const { mutate } = useProjectSummaryByStudent(projectId);

  return (
    <MoveStageModal
      isModalOpen={isOpenAdvanceStage}
      onModalClose={onCloseAdvanceStage}
      record={record}
      sectionType={sectionType}
      mutate={mutate}
      {...modalActionProps}
    />
  );
};

const createActions = ({
  record,
  canManage,
  onOpenDelete,
  onOpenNew,
  onOpenAdvanceStage,
  setModalActionProps,
  navigate,
  msgLabel,
}) => {
  return {
    new: {
      isEligible: () => !record && canManage,
      icon: <FaLink />,
      label: "New",
      onClick: onOpenNew,
    },
    view: {
      isEligible: () => record,
      icon: <FaEye />,
      label: STUDENT_ACTIONS.View,
      onClick: () => navigate(record.overviewPath),
    },
    delete: {
      isEligible: () =>
        record &&
        canManage &&
        record?.permissions?.includes(STAGES_PERMISSIONS.OwnerCanEdit),
      icon: <FaTrash />,
      label: STUDENT_ACTIONS.Delete,
      onClick: onOpenDelete,
    },
    submit: {
      isEligible: () =>
        record &&
        canManage &&
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
