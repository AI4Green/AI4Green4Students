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
import { FaExclamationTriangle } from "react-icons/fa";
import {
  useLocation,
  useNavigate,
  useParams,
  useSearchParams,
} from "react-router-dom";

export const DeleteModal = () => {
  const [searchParams] = useSearchParams();
  const id = searchParams.get("id");
  const { projectId } = useParams();

  const navigate = useNavigate();
  const location = useLocation();

  const { projectGroups: action } = useBackendApi();
  const { data: list, mutate } = useProjectGroupsList(projectId);

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

  const projectGroup = list?.find(
    (projectGroup) => projectGroup.id === Number(id)
  );

  useEffect(() => {
    setIsModalOpen(true);
  }, [id, projectId, setIsModalOpen]);

  const handleDelete = async () => {
    try {
      setIsLoading(true);
      const response = await action.delete({ id });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          title: "Project group deleted",
          status: "success",
          duration: GLOBAL_PARAMETERS.ToastDuration,
          isClosable: true,
          position: "top",
        });
        handleReset();
        await mutate();
      }
    } catch (e) {
      const error = await e.response.text();
      switch (e.response.status) {
        case 400: {
          setFeedback({
            status: "error",
            message: error ?? t("feedback.error_400"),
          });
          break;
        }
        case 404: {
          setFeedback({
            status: "error",
            message: t("feedback.error_404"),
          });
          break;
        }
        default: {
          setFeedback({
            status: "error",
            message: t("feedback.error_title"),
          });
        }
      }
    }
  };

  const modalBody = (
    <HStack>
      <Icon as={FaExclamationTriangle} color="red.500" fontSize="5xl" />
      <VStack align="flex-end" flex={1}>
        {feedback && (
          <Alert status={feedback.status}>
            <AlertIcon />
            {feedback.message}
          </Alert>
        )}
        <Text>Are you sure you want to delete this project group?</Text>

        <VStack
          align="flex-start"
          borderWidth={1}
          borderRadius={7}
          p={2}
          w="full"
          spacing={1}
        >
          <Text as="b">
            <Badge colorScheme="blue">Project group</Badge>
            {projectGroup.name}
          </Text>
          <Text as="b" fontSize="sm">
            <Badge colorScheme="green"> Project </Badge>
            {projectGroup.project.name}
          </Text>
        </VStack>
      </VStack>
    </HStack>
  );

  if (!projectGroup) {
    navigate(location.pathname, { replace: true });
    return null;
  }

  return (
    <Modal
      body={modalBody}
      title="Delete Confirmation"
      actionBtnCaption="Delete"
      actionBtnColorScheme="red"
      onAction={handleDelete}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={handleReset}
    />
  );
};
