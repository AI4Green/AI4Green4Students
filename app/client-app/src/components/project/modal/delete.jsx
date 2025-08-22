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
import { useProjectGroupsList, useProjectsList } from "api";
import { Modal } from "components/core/modal";
import { GLOBAL_PARAMETERS } from "constants";
import { useBackendApi } from "contexts";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import { FaExclamationTriangle } from "react-icons/fa";

export const DeleteModal = ({
  project,
  projectGroup,
  isModalOpen,
  onModalClose,
  isDeleteProject,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { projects: projectAction, projectGroups: projectGroupAction } =
    useBackendApi();
  const { mutate: mutateProjects } = useProjectsList();
  const { mutate: mutateProjectGroups } = useProjectGroupsList(project.id);
  const { t } = useTranslation();
  const toast = useToast();

  const handleDelete = async () => {
    try {
      setIsLoading(true);
      const response = isDeleteProject
        ? await projectAction.delete({
            id: project.id,
          })
        : await projectGroupAction.delete({ id: projectGroup.id });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          title: `Project ${projectGroup ? "Group" : ""} deleted`,
          status: "success",
          duration: GLOBAL_PARAMETERS.ToastDuration,
          isClosable: true,
          position: "top",
        });
        await mutateProjects();
        await mutateProjectGroups();
        onModalClose();
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
        <Text>
          {`Are you sure you want to delete this project${
            !isDeleteProject ? " group" : ""
          }?`}
        </Text>

        {isDeleteProject ? (
          <Text as="b">
            {project.name} <Badge colorScheme="green"> Project </Badge>
          </Text>
        ) : (
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
              {project.name}
            </Text>
          </VStack>
        )}
      </VStack>
    </HStack>
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
      onAction={handleDelete}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={() => {
        resetState();
        onModalClose();
      }}
    />
  );
};
