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
import { RemoveModal } from "components/project/modal";
import { GLOBAL_PARAMETERS, TITLE_ICON_COMPONENTS } from "constants";
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
        values: { id: student.id },
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

  if (!projectGroup || !student) {
    navigate(location.pathname, { replace: true });
    return null;
  }

  return (
    <Modal
      body={
        <RemoveModal
          title="Please confirm the removal of the following student:"
          remove={student.name || student.email}
          tags={[
            {
              label: projectGroup.project.name,
              colorScheme: "green",
              leftIcon: TITLE_ICON_COMPONENTS.Project,
            },
            {
              label: projectGroup.name,
              colorScheme: "blue",
              leftIcon: TITLE_ICON_COMPONENTS.ProjectGroup,
            },
          ]}
        />
      }
      title="Student removal confirmation"
      actionBtnCaption="Remove"
      actionBtnColorScheme="red"
      onAction={handleStudentRemoval}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={handleReset}
    />
  );
};
