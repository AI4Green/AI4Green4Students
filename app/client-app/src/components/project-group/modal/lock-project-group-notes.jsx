import {
  Alert,
  AlertIcon,
  Badge,
  HStack,
  Icon,
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
import { FaLock } from "react-icons/fa";
import {
  useLocation,
  useNavigate,
  useParams,
  useSearchParams,
} from "react-router-dom";

export const LockProjectGroupNotesModal = () => {
  const [searchParams] = useSearchParams();
  const id = searchParams.get("id");
  const isLockNotesOpen = searchParams.get("action") === "lock-notes";
  const { projectId } = useParams();

  const navigate = useNavigate();
  const location = useLocation();

  const { notes: action } = useBackendApi();
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

  const projectGroup = isLockNotesOpen
    ? projectGroups?.find((projectGroup) => projectGroup.id === Number(id))
    : null;

  useEffect(() => {
    setIsModalOpen(true);
  }, [id, isLockNotesOpen, projectId, setIsModalOpen]);

  const handleNotesLocking = async () => {
    try {
      setIsLoading(true);
      const response = await action.lockProjectGroupNotes(id);

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

  const modalBody = (
    <VStack align="flex-start" spacing={4}>
      {feedback && (
        <Alert status={feedback.status}>
          <AlertIcon />
          {feedback.message}
        </Alert>
      )}
      <Text>
        Are you sure you want to lock all the notes for this project group?
      </Text>

      <HStack borderWidth={1} borderRadius={7} p={2} w="full">
        <Icon as={FaLock} color="yellow.400" fontSize="5xl" />
        <VStack align="stretch" spacing={0}>
          <Text fontWeight="bold" fontSize="xl">
            {projectGroup.name}
          </Text>
          <HStack>
            <Badge colorScheme="green">Project</Badge>
            <Text as="b" fontSize="sm">
              {projectGroup?.project?.name}
            </Text>
          </HStack>
        </VStack>
      </HStack>
    </VStack>
  );

  if (!projectGroup) {
    navigate(location.pathname, { replace: true });
    return null;
  }

  return (
    <Modal
      body={modalBody}
      title="Lock Notes Confirmation"
      actionBtnCaption="Lock"
      actionBtnColorScheme="yellow"
      onAction={handleNotesLocking}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={handleReset}
    />
  );
};
