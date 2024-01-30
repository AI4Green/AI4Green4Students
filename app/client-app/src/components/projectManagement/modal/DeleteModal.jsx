import { useState } from "react";
import { useTranslation } from "react-i18next";
import {
  Alert,
  AlertIcon,
  VStack,
  Text,
  useToast,
  Badge,
  HStack,
  Icon,
} from "@chakra-ui/react";
import { useProjectsList } from "api/projects";
import { useProjectGroupsList } from "api/projectGroups";
import { BasicModal } from "components/BasicModal";
import { useBackendApi } from "contexts/BackendApi";
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
  const { mutate: mutateProjectGroups } = useProjectGroupsList();
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
          duration: 1500,
          isClosable: true,
          position: "top",
        });
        mutateProjects();
        mutateProjectGroups();
        onModalClose();
      }
    } catch (e) {
      setFeedback({
        status: "error",
        message: t("feedback.error_title"),
      });
    }
  };

  const Modal = (
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
  return (
    <BasicModal
      body={Modal}
      title="Delete Confirmation"
      actionBtnCaption="Delete"
      actionBtnColorScheme="red"
      onAction={handleDelete}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};
