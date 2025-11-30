import { useToast } from "@chakra-ui/react";
import { useProjectGroupsList } from "api";
import { Modal, useModalState } from "components/core/modal";
import { InviteModal } from "components/project/modal";
import { GLOBAL_PARAMETERS, TITLE_ICON_COMPONENTS } from "constants";
import { useBackendApi } from "contexts";
import { useEffect, useRef } from "react";
import { useTranslation } from "react-i18next";
import {
  useLocation,
  useNavigate,
  useParams,
  useSearchParams,
} from "react-router-dom";

export const StudentInviteModal = ({ list, mutate }) => {
  const [searchParams] = useSearchParams();
  const id = searchParams.get("id");
  const isInviteAction = searchParams.get("action") === "invite-students";
  const { projectId } = useParams();

  const navigate = useNavigate();
  const location = useLocation();

  const { projectGroups: action } = useBackendApi();

  const { t } = useTranslation();
  const toast = useToast();

  const formRef = useRef();

  const {
    isModalOpen,
    setIsModalOpen,
    isLoading,
    setIsLoading,
    feedback,
    setFeedback,
    handleReset,
  } = useModalState(location, navigate, formRef);

  const projectGroup = isInviteAction
    ? list?.find((projectGroup) => projectGroup.id === Number(id))
    : null;

  useEffect(() => {
    setIsModalOpen(true);
  }, [id, isInviteAction, projectId, setIsModalOpen]);

  const handleSubmit = async ({ emails }) => {
    try {
      setIsLoading(true);
      const response = await action.inviteStudents(projectGroup.id, {
        emails,
        projectId,
      });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          title: `Students invited successfully`,
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

  if (!projectGroup) {
    navigate(location.pathname, { replace: true });
    return null;
  }

  return (
    <Modal
      body={
        <InviteModal
          ref={formRef}
          handleSubmit={handleSubmit}
          feedback={feedback}
          title="Invite students to the project group."
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
      title="Project group invitation"
      actionBtnCaption="Invite"
      onAction={() => formRef.current.handleSubmit()}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={handleReset}
    />
  );
};
