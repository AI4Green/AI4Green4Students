import {
  Tag,
  TagLabel,
  TagCloseButton,
  useDisclosure,
  TagLeftIcon,
  useToast,
  HStack,
} from "@chakra-ui/react";
import { useState } from "react";
import { ActionButton } from "components/ActionButton";
import { FaRegCommentAlt, FaCheck } from "react-icons/fa";
import { Comment } from "./comment/Comment";
import { LoadingIndicator } from "components/LoadingIndicator";
import { CreateOrEditCommentModal } from "./comment/modal/CreateOrEditCommentModal";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { useBackendApi } from "contexts/BackendApi";
import { useTranslation } from "react-i18next";
import { useSectionForm } from "contexts/SectionForm";
import { STAGES } from "constants/stages";
import { STAGES_PERMISSIONS } from "constants/site-permissions";
import { SECTION_TYPES } from "constants/section-types";
import { GLOBAL_PARAMETERS } from "constants/global-parameters";

export const Feedback = ({ field }) => {
  const { sectionType, stage, stagePermissions, mutate, isRecordOwner } =
    useSectionForm();
  const isInstructor = useIsInstructor();
  const { comments: action } = useBackendApi();
  const [isLoading, setIsLoading] = useState(false);
  const toast = useToast();
  const { t } = useTranslation();
  const { isOpen, onOpen, onClose } = useDisclosure();

  const handleApproval = async (isApproved) => {
    try {
      setIsLoading(true);
      const response = await action.setApprovalStatus(
        field.fieldResponseId,
        isApproved
      );

      if (response && (response.status === 204 || response.status === 200)) {
        toast(
          toastOptions(
            isApproved ? "Approved" : "Approval cancelled",
            "success"
          )
        );
        await mutate();
      }
    } catch (e) {
      toast(toastOptions(t("feedback.error_title"), "error"));
    } finally {
      setIsLoading(false);
    }
  };

  const canInstructorComment =
    isInstructor &&
    stagePermissions.includes(STAGES_PERMISSIONS.InstructorCanComment);

  const canViewComment =
    (isInstructor || isRecordOwner) &&
    stage !== STAGES.Draft &&
    sectionType !== SECTION_TYPES.ProjectGroup &&
    sectionType !== SECTION_TYPES.Note &&
    field?.comments >= 1;

  const actions = {
    approve: {
      isEligible: () => canInstructorComment,
      label: "Approve",
      onClick: () => handleApproval(true),
      icon: <FaCheck />,
    },
    comment: {
      isEligible: () => canInstructorComment,
      label: "Add comment",
      onClick: onOpen,
      icon: <FaRegCommentAlt />,
    },
  };

  return (
    <HStack align="flex-start">
      {isLoading ? (
        <LoadingIndicator />
      ) : field.isApproved ? (
        <Tag colorScheme="green" borderRadius="full" variant="outline">
          <TagLeftIcon as={FaCheck} />
          <TagLabel>Approved</TagLabel>
          {canInstructorComment && (
            <TagCloseButton onClick={() => handleApproval(false)} />
          )}
        </Tag>
      ) : (
        canInstructorComment && (
          <>
            <ActionButton actions={actions} size="xs" variant="outline" />
            {isOpen && (
              <CreateOrEditCommentModal
                fieldResponseId={field.fieldResponseId}
                isModalOpen={isOpen}
                onModalClose={onClose}
              />
            )}
          </>
        )
      )}
      {canViewComment && <Comment field={field} />}
    </HStack>
  );
};

const toastOptions = (title, status) => ({
  position: "top",
  title,
  status,
  duration: GLOBAL_PARAMETERS.ToastDuration,
  isClosable: true,
});
