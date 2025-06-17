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
import { ActionButton } from "components/core/ActionButton";
import { FaRegCommentAlt, FaCheck } from "react-icons/fa";
import { Comment } from "./comment";
import { LoadingIndicator } from "components/core/LoadingIndicator";
import { CreateOrEditCommentModal } from "./comment/modal";
import { useIsInstructor } from "helpers/hooks";
import { useBackendApi, useSectionForm } from "contexts";
import { useTranslation } from "react-i18next";
import {
  STAGES,
  STAGES_PERMISSIONS,
  SECTION_TYPES,
  GLOBAL_PARAMETERS,
} from "constants";

/**
 * This component is responsible for rendering the feedback section of a field.
 * The component need to be wrapped by the SectionFormProvider.
 */
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
            isApproved ? "success" : "warning"
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
      onClick: async () => await handleApproval(true),
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
