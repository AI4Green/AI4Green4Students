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
import {
  STAGES_PERMISSIONS,
  TITLE_ICON_COMPONENTS,
  STAGES,
  SECTION_TYPES,
  STATUS_ICON_COMPONENTS,
  GLOBAL_PARAMETERS,
} from "constants";
import { useState } from "react";
import {
  useProjectSummaryByStudent,
  useLiteratureReviewSectionsList,
  useReportSectionsList,
} from "api";
import { useBackendApi, useUser } from "contexts";
import { useIsInstructor } from "helpers/hooks";
import { createInstructorActions } from ".";

export const LiteratureReviewAction = ({
  literatureReview,
  project,
  label = SECTION_TYPE_LABELS.LiteratureReview,
  LeftIcon = TITLE_ICON_COMPONENTS[SECTION_TYPES.LiteratureReview],
  studentId,
}) => {
  const { data: sections } = literatureReview?.id
    ? useLiteratureReviewSectionsList(literatureReview.id)
    : { data: null };

  return (
    <Action
      record={literatureReview || null}
      sectionType={SECTION_TYPES.LiteratureReview}
      project={project}
      label={label}
      LeftIcon={LeftIcon}
      studentId={studentId}
      isEverySectionApproved={sections?.every((section) => section.approved)}
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
  const { data: sections } = report?.id
    ? useReportSectionsList(report.id)
    : { data: null };

  return (
    <Action
      record={report || null}
      sectionType={SECTION_TYPES.Report}
      project={project}
      label={label}
      LeftIcon={LeftIcon}
      studentId={studentId}
      isEverySectionApproved={sections?.every((section) => section.approved)}
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
  isEverySectionApproved,
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

  const isInstructor = useIsInstructor();

  const {
    isOpen: isNewOpen,
    onOpen: onNewOpen,
    onClose: onNewClose,
  } = useDisclosure();

  const {
    isOpen: isDeleteOpen,
    onOpen: onDeleteOpen,
    onClose: onDeleteClose,
  } = useDisclosure();

  const {
    isOpen: isAdvanceStageOpen,
    onOpen: onAdvanceStageOpen,
    onClose: onAdvanceStageClose,
  } = useDisclosure();

  const navigate = useNavigate();

  const canManage =
    (record && record.ownerId === user.userId) ||
    (studentId && studentId === user.userId) ||
    !studentId; // Check if the user can manage the record

  const actions = {
    ...createActions({
      record,
      canManage,
      onOpenNew: onNewOpen,
      onDeleteOpen,
      onAdvanceStageOpen,
      setModalActionProps,
      navigate,
      label,
    }),
    ...(isInstructor && record
      ? createInstructorActions({
          // append any instructor actions
          record,
          sectionType,
          isEverySectionApproved,
          onAdvanceStageOpen,
          setModalActionProps,
        })
      : {}),
  };

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
      {canManage && isDeleteOpen && (
        <DeleteModal
          isModalOpen={isDeleteOpen}
          onModalClose={onDeleteClose}
          record={record}
          sectionType={sectionType}
        />
      )}
      {isAdvanceStageOpen && (
        <StageAdvanceModal
          projectId={project.id}
          record={record}
          sectionType={sectionType}
          isOpenAdvanceStage={isAdvanceStageOpen}
          onCloseAdvanceStage={onAdvanceStageClose}
          modalActionProps={modalActionProps}
          studentId={studentId}
        />
      )}
      {canManage && isNewOpen && (
        <CreateOrEditModal
          isModalOpen={isNewOpen}
          onModalClose={onNewClose}
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
  studentId,
}) => {
  const { mutate } = studentId
    ? useProjectSummaryByStudent(projectId, studentId)
    : useProjectSummaryByStudent(projectId);

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
  onDeleteOpen,
  onOpenNew,
  onAdvanceStageOpen,
  setModalActionProps,
  navigate,
  label,
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
      onClick: onDeleteOpen,
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
        setModalActionProps(getSubmitModalActionProps(label));
        onAdvanceStageOpen();
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
  successMessage: `${msgLabel} submitted successfully`,
  failMessage: `${msgLabel} submission failed`,
  fixedNextStage:
    msgLabel === SECTION_TYPE_LABELS.Report ? STAGES.Submitted : null,
});
