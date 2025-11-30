import { useToast } from "@chakra-ui/react";
import { Modal, useModalState } from "components/core/modal";
import { GLOBAL_PARAMETERS, TITLE_ICON_COMPONENTS } from "constants";
import { useBackendApi } from "contexts";
import { ConfirmationModal } from "layouts/default";
import { useEffect } from "react";
import { useTranslation } from "react-i18next";
import { FaLock } from "react-icons/fa";
import {
  useLocation,
  useNavigate,
  useParams,
  useSearchParams,
} from "react-router-dom";

export const LockProjectGroupNotesModal = ({ list, mutate }) => {
  const [searchParams] = useSearchParams();
  const id = searchParams.get("id");
  const isLockNotesOpen = searchParams.get("action") === "lock-notes";
  const { projectId } = useParams();

  const navigate = useNavigate();
  const location = useLocation();

  const { notes: action } = useBackendApi();

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

  const projectGroup = isLockNotesOpen
    ? list?.find((projectGroup) => projectGroup.id === Number(id))
    : null;

  useEffect(() => {
    setIsModalOpen(true);
  }, [id, isLockNotesOpen, projectId, setIsModalOpen]);

  const handleNotesLocking = async () => {
    try {
      setIsLoading(true);
      const response = await action.lockProjectGroupNotes(projectGroup?.id);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          title: `All the notes for project group ${projectGroup.name} has now been locked.`,
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
    } finally {
      setIsLoading(false);
    }
  };

  if (!projectGroup) {
    navigate(location.pathname, { replace: true });
    return null;
  }

  return (
    <Modal
      body={
        <ConfirmationModal
          content={{
            description:
              "Are you sure you want to lock all the notes for this project group?",
            value: projectGroup.name,
            tags: [
              {
                label: "Project group",
                leftIcon: TITLE_ICON_COMPONENTS.ProjectGroup,
              },
            ],
          }}
          iconProps={{
            Icon: FaLock,
            color: "yellow.400",
            fontSize: "5xl",
          }}
          feedback={feedback}
        />
      }
      title="Lock Notes Confirmation"
      actionBtnCaption="Lock"
      actionBtnColorScheme="yellow"
      actionBtnLeftIcon={<FaLock />}
      onAction={handleNotesLocking}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={handleReset}
    />
  );
};
