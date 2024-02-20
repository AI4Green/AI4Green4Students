import {
  Tag,
  TagLabel,
  TagCloseButton,
  useDisclosure,
  TagLeftIcon,
  useToast,
  VStack,
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

export const Feedback = ({ field }) => {
  const { stage, stagePermissions } = useSectionForm();
  const isInstructor = useIsInstructor();
  const { mutate } = useSectionForm();
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

  const actions = {
    approve: {
      isEligible: () => {
        return stagePermissions.includes(
          STAGES_PERMISSIONS.InstructorCanComment
        );
      },
      label: "Approve",
      onClick: () => handleApproval(true),
      icon: <FaCheck />,
    },
    comment: {
      isEligible: () => {
        return stagePermissions.includes(
          STAGES_PERMISSIONS.InstructorCanComment
        );
      },
      label: "Add comment",
      onClick: onOpen,
      icon: <FaRegCommentAlt />,
    },
  };

  return (
    <VStack align="flex-start">
      {isLoading ? (
        <LoadingIndicator />
      ) : field.isApproved ? (
        <Tag colorScheme="green">
          <TagLeftIcon as={FaCheck} />
          <TagLabel>Approved</TagLabel>
          {isInstructor && (
            <TagCloseButton onClick={() => handleApproval(false)} />
          )}
        </Tag>
      ) : (
        isInstructor && (
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
      {
        // hide Comment in draft stage
        stage !== STAGES.Draft && <Comment field={field} />
      }
    </VStack>
  );
};

const toastOptions = (title, status) => ({
  position: "top",
  title,
  status,
  duration: 1500,
  isClosable: true,
});
