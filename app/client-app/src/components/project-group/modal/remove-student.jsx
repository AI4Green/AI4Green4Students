import {
  Alert,
  AlertIcon,
  Avatar,
  Badge,
  HStack,
  Text,
  useToast,
  VStack,
} from "@chakra-ui/react";
import { useProjectGroupsList } from "api";
import { Modal, useModalState } from "components/core/modal";
import { GLOBAL_PARAMETERS } from "constants";
import { useBackendApi } from "contexts";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import {
  useLocation,
  useNavigate,
  useParams,
  useSearchParams,
} from "react-router-dom";

export const RemoveStudentModal = () => {
  const [searchParams] = useSearchParams();
  const id = searchParams.get("id");
  const studentId = searchParams.get("studentId");
  const isRemoveStudentOpen = searchParams.get("action") === "remove-student";
  const { projectId } = useParams();

  const navigate = useNavigate();
  const location = useLocation();

  const { projectGroups: projectGroupAction } = useBackendApi();
  const { data: projectGroups, mutate } = useProjectGroupsList(projectId);

  const { t } = useTranslation();
  const toast = useToast();

  const {
    isModalOpen,
    setIsModalOpen,
    isLoading,
    setIsLoading,
    feedback,
    setFeedback,
    handleReset,
  } = useModalState(location, navigate);

  const projectGroup = isRemoveStudentOpen
    ? projectGroups?.find((projectGroup) => projectGroup.id === Number(id))
    : null;

  const student = projectGroup?.students.find(
    (student) => student.id === studentId
  );

  useEffect(() => {
    setIsModalOpen(true);
  }, [id, isRemoveStudentOpen, projectId, setIsModalOpen, studentId]);

  const handleStudentRemoval = async () => {
    try {
      setIsLoading(true);
      const response = await projectGroupAction.removeStudent({
        id: projectGroup.id,
        values: { studentId: student.id },
      });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          title: `Student ${
            student.name || student.email
          } removed from Project group ${projectGroup.name}`,
          status: "success",
          duration: GLOBAL_PARAMETERS.ToastDuration,
          position: "top",
          isClosable: true,
        });
        handleReset();
        await mutate();
      }
    } catch {
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
        {student?.name && <Avatar name={student.name} size="lg" />}
        <VStack align="stretch" spacing={0}>
          <Text fontWeight="bold">{student?.name}</Text>
          <Text>{student.email}</Text>
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

  if (!projectGroup || !student) {
    navigate(location.pathname, { replace: true });
    return null;
  }

  return (
    <Modal
      body={modalBody}
      title="Delete Confirmation"
      actionBtnCaption="Delete"
      actionBtnColorScheme="red"
      onAction={handleStudentRemoval}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={handleReset}
    />
  );
};
