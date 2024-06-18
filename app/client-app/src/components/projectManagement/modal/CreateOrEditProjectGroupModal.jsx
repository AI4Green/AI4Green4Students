import { useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import {
  Alert,
  AlertIcon,
  VStack,
  useToast,
  Text,
  Badge,
  HStack,
  Icon,
} from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { TextField } from "components/forms/TextField";
import { Modal } from "components/Modal";
import { useProjectGroupsList } from "api/projectGroups";
import { useProjectsList } from "api/projects";
import { useBackendApi } from "contexts/BackendApi";
import { projectGroupNameValidationSchema as validationSchema } from "../validation";
import { FaProjectDiagram } from "react-icons/fa";

export const CreateOrEditProjectGroupModal = ({
  project,
  projectGroup,
  isModalOpen,
  onModalClose,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { projectGroups: action } = useBackendApi();
  const { mutate: mutateProjectGroups } = useProjectGroupsList();
  const { data: projects, mutate: mutateProjects } = useProjectsList();
  const { t } = useTranslation();
  const toast = useToast();

  const initialValues = () => {
    return projectGroup
      ? {
          name: projectGroup.name,
          projectId: project.id,
        }
      : {
          name: "",
          projectId: project.id,
        };
  };

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);
      // as multi select returns an array, thus selecting the first element
      const response = !projectGroup
        ? await action.create({
            values: { ...values },
          })
        : await action.edit({
            values: { ...values },
            id: projectGroup.id,
          });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          title: `Project Group ${!projectGroup ? "created" : "updated"}`,
          status: "success",
          duration: 1500,
          isClosable: true,
          position: "top",
        });
        mutateProjectGroups();
        mutateProjects();
        onModalClose();
      }
    } catch (e) {
      setFeedback({
        status: "error",
        message: t("feedback.error_title"),
      });
    }
  };

  const formRef = useRef();
  const modalBody = (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={initialValues()}
      onSubmit={handleSubmit}
      validationSchema={validationSchema(projects)}
    >
      <Form noValidate>
        <VStack align="stretch" spacing={4}>
          {feedback && (
            <Alert status={feedback.status}>
              <AlertIcon />
              {feedback.message}
            </Alert>
          )}
          <HStack spacing={5}>
            <VStack w="full">
              <Icon
                as={FaProjectDiagram}
                color={projectGroup ? "blue.500" : "green.500"}
                fontSize="5xl"
              />
              <Text as="b">
                {project.name} <Badge colorScheme="green"> Project </Badge>
              </Text>
            </VStack>
            <TextField name="name" label="Project Group name" isRequired />
          </HStack>
        </VStack>
      </Form>
    </Formik>
  );
  return (
    <Modal
      body={modalBody}
      title={`${!projectGroup ? "Create" : "Edit"} Project Group`}
      actionBtnCaption={!projectGroup ? "Create" : "Update"}
      onAction={() => formRef.current.handleSubmit()}
      actionBtnColorScheme={!projectGroup ? "green" : "blue"}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};
