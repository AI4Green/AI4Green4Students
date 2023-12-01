import { useState } from "react";
import { useTranslation } from "react-i18next";
import { Alert, AlertIcon, VStack, Text, useToast } from "@chakra-ui/react";
import { useProjectsList } from "api/projects";
import { useProjectGroupsList } from "api/projectGroups";
import { BasicModal } from "components/BasicModal";
import { useBackendApi } from "contexts/BackendApi";

export const DeleteModal = ({
  project,
  projectGroup,
  isModalOpen,
  onModalClose,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { projects: projectAction, projectGroups: projectGroupAction } =
    useBackendApi();
  const { mutate: mutateProjects } = useProjectsList();
  const { mutate: mutateProjectGroups } = useProjectGroupsList();
  const { t } = useTranslation();
  const toast = useToast();

  // toast config
  const displayToast = ({
    position = "top",
    title,
    status,
    duration = "900",
    isClosable = true,
  }) =>
    toast({
      position,
      title,
      status,
      duration,
      isClosable,
    });

  const handleDelete = async () => {
    try {
      setIsLoading(true);
      const response = project
        ? await projectAction.delete({
            id: project.id,
          })
        : await projectGroupAction.delete({ id: projectGroup.id });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        displayToast({
          title: `Project ${projectGroup ? "Group" : ""} deleted`,
          status: "success",
          duration: 1500,
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
    <VStack>
      {feedback && (
        <Alert status={feedback.status}>
          <AlertIcon />
          {feedback.message}
        </Alert>
      )}
      <Text>
        Are you sure you want to delete this project {projectGroup && "group"}:
      </Text>
      <Text fontWeight="bold">
        {project ? project.name : projectGroup.name}
      </Text>
    </VStack>
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
