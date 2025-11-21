import { useToast } from "@chakra-ui/react";
import { useProject, useProjectInstructors } from "api";
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

export const RemoveInstructorModal = () => {
  const [searchParams] = useSearchParams();
  const instructorId = searchParams.get("instructorId");
  const isRemoveInstructorOpen =
    searchParams.get("action") === "remove-instructor";
  const { projectId } = useParams();

  const navigate = useNavigate();
  const location = useLocation();

  const { projects: projectAction } = useBackendApi();
  const { data: project } = useProject(projectId);
  const { data: instructors, mutate } = useProjectInstructors(projectId);

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

  const instructor = instructors?.find(
    (instructor) => instructor.id === instructorId
  );

  useEffect(() => {
    setIsModalOpen(true);
  }, [projectId, isRemoveInstructorOpen, setIsModalOpen, instructorId]);

  const handleInstructorRemoval = async () => {
    try {
      setIsLoading(true);
      const response = await projectAction.removeInstructor(
        projectId,
        instructor.id
      );
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          title: `Instructor ${
            instructor.name || instructor.email
          } removed from Project ${project.name}`,
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

  if (!project || !instructor) {
    navigate(location.pathname, { replace: true });
    return null;
  }

  return (
    <Modal
      body={
        <RemoveModal
          title="Please confirm the removal of the following instructor:"
          remove={instructor.name || instructor.email}
          tags={[
            {
              label: project.name,
              colorScheme: "green",
              leftIcon: TITLE_ICON_COMPONENTS.Project,
            },
          ]}
          feedback={feedback}
        />
      }
      title="Instructor removal confirmation"
      actionBtnCaption="Remove"
      actionBtnColorScheme="red"
      onAction={handleInstructorRemoval}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={handleReset}
    />
  );
};
