import { useToast } from "@chakra-ui/react";
import { Modal, useModalState } from "components/core/modal";
import { GLOBAL_PARAMETERS, TITLE_ICON_COMPONENTS } from "constants";
import { useBackendApi } from "contexts";
import { ConfirmationModal as RemoveModal } from "layouts/default";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate, useSearchParams } from "react-router-dom";

export const RemoveStudentModal = ({ list, mutate }) => {
  const [searchParams] = useSearchParams();
  const projectGroupId = searchParams.get("projectGroupId");
  const studentId = searchParams.get("studentId");
  const isRemoveStudentOpen = searchParams.get("action") === "remove-student";

  const navigate = useNavigate();
  const location = useLocation();

  const { projectGroups: projectGroupAction } = useBackendApi();

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
    ? list?.find((projectGroup) => projectGroup.id === Number(projectGroupId))
    : null;

  const student = projectGroup?.students.find(
    (student) => student.id === studentId
  );

  useEffect(() => {
    setIsModalOpen(true);
  }, [projectGroupId, isRemoveStudentOpen, setIsModalOpen, studentId]);

  const handleStudentRemoval = async () => {
    try {
      setIsLoading(true);
      const response = await projectGroupAction.removeStudent(
        projectGroup.id,
        student.id
      );
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
          content={{
            value: student.name || student.email,
            description: "Are you sure you want to remove this student?",
            tags: [
              {
                label: projectGroup.project.name,
                leftIcon: TITLE_ICON_COMPONENTS.Project,
              },
              {
                label: projectGroup.name,
                leftIcon: TITLE_ICON_COMPONENTS.ProjectGroup,
              },
            ],
          }}
          feedback={feedback}
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
