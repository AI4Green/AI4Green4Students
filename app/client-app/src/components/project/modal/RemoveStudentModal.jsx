import { useState } from "react";
import { useTranslation } from "react-i18next";
import {
  Alert,
  AlertIcon,
  VStack,
  Text,
  useToast,
  Avatar,
  HStack,
  Badge,
} from "@chakra-ui/react";
import { useProjectGroupsList } from "api/projectGroups";
import { Modal } from "components/Modal";
import { useBackendApi } from "contexts/BackendApi";
import { GLOBAL_PARAMETERS } from "constants/global-parameters";

export const RemoveStudentModal = ({
  student,
  projectGroup,
  isModalOpen,
  onModalClose,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { projectGroups: projectGroupAction } = useBackendApi();
  const { mutate: mutateProjectGroups } = useProjectGroupsList(
    projectGroup.project.id
  );
  const { t } = useTranslation();
  const toast = useToast();

  const handleStudentRemoval = async () => {
    try {
      setIsLoading(true);
      const response = await projectGroupAction.removeStudent({
        id: projectGroup.id,
        values: { studentId: student.studentId },
      });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          title: `Student ${
            student.name || student.studentEmail
          } removed from Project group ${projectGroup.name}`,
          status: "success",
          duration: GLOBAL_PARAMETERS.ToastDuration,
          position: "top",
          isClosable: true,
        });
        await mutateProjectGroups();
        onModalClose();
      }
    } catch (e) {
      setFeedback({
        status: "error",
        message: t("feedback.error_title"),
      });
    }
  };

  const modalBody = (
    <VStack align="flex-start" spacing={4}>
      {feedback && (
        <Alert status={feedback.status}>
          <AlertIcon />
          {feedback.message}
        </Alert>
      )}
      <Text>Are you sure you want to remove the following student?</Text>

      <HStack borderWidth={1} borderRadius={7} p={2} w="full">
        {student.name && <Avatar name={student.name} size="lg" />}
        <VStack align="stretch" spacing={0}>
          <Text fontWeight="bold">{student.name}</Text>
          <Text>{student.studentEmail}</Text>
          <HStack>
            <Badge colorScheme="green">Project group</Badge>
            <Text as="b" fontSize="sm">
              {projectGroup?.name}
            </Text>
          </HStack>
        </VStack>
      </HStack>
    </VStack>
  );

  const resetState = () => {
    setFeedback();
    setIsLoading(false);
  };

  return (
    <Modal
      body={modalBody}
      title="Delete Confirmation"
      actionBtnCaption="Delete"
      actionBtnColorScheme="red"
      onAction={handleStudentRemoval}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={() => {
        resetState();
        onModalClose();
      }}
    />
  );
};
