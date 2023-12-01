import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Alert, AlertIcon, VStack, Text, useToast } from "@chakra-ui/react";
import { useProjectGroupsList } from "api/projectGroups";
import { BasicModal } from "components/BasicModal";
import { useBackendApi } from "contexts/BackendApi";

export const RemoveStudentModal = ({
  student,
  projectGroup,
  isModalOpen,
  onModalClose,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { projectGroups: projectGroupAction } = useBackendApi();
  const { mutate: mutateProjectGroups } = useProjectGroupsList();
  const { t } = useTranslation();
  const toast = useToast();

  // toast config
  const displayToast = ({
    position = "top",
    title,
    status,
    duration = "900",
    isClosable = true,
  }) =>
    toast({
      position,
      title,
      status,
      duration,
      isClosable,
    });

  const handleStudentRemoval = async () => {
    try {
      setIsLoading(true);
      const response = await projectGroupAction.removeStudent({
        id: projectGroup.id,
        values: { studentId: student.studentId },
      });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        displayToast({
          title: `Student ${
            student.name ?? student.studentEmail
          } removed from Project group ${projectGroup.name}`,
          status: "success",
          duration: 1500,
        });
        mutateProjectGroups();
        onModalClose();
      }
    } catch (e) {
      setFeedback({
        status: "error",
        message: t("feedback.error_title"),
      });
    }
  };

  const Modal = (
    <VStack>
      {feedback && (
        <Alert status={feedback.status}>
          <AlertIcon />
          {feedback.message}
        </Alert>
      )}
      <Text>
        Are you sure you want to remove the student
        <Text as="span" fontWeight="bold">
          {` ${student.name ?? student.studentEmail} `}
        </Text>
        from the
        <Text as="span" fontWeight="bold">
          {` ${projectGroup?.name} `}
        </Text>
        Project group ?
      </Text>
    </VStack>
  );
  return (
    <BasicModal
      body={Modal}
      title="Delete Confirmation"
      actionBtnCaption="Delete"
      actionBtnColorScheme="red"
      onAction={handleStudentRemoval}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};
