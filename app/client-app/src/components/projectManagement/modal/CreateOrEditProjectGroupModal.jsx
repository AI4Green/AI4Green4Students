import { useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { Alert, AlertIcon, VStack, useToast } from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { TextField } from "components/forms/TextField";
import { BasicModal } from "components/BasicModal";
import { MultiSelectField } from "components/forms/MultiSelectField";
import { useProjectGroupsList } from "api/projectGroups";
import { useProjectsList } from "api/projects";
import { useBackendApi } from "contexts/BackendApi";
import { projectGroupNameValidationSchema as validationSchema } from "../validation";

export const CreateOrEditProjectGroupModal = ({
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
          projectId: [projectGroup.projectId], // multi select requires an array
        }
      : {
          name: "",
          projectId: "",
        };
  };

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

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);
      // as multi select returns an array, thus selecting the first element
      const response = !projectGroup
        ? await action.create({
            values: { ...values, projectId: values.projectId[0] },
          })
        : await action.edit({
            values: { ...values, projectId: values.projectId[0] },
            id: projectGroup.id,
          });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        displayToast({
          title: `Project Group ${!projectGroup ? "created" : "updated"}`,
          status: "success",
          duration: 1500,
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
  const Modal = (
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
          <MultiSelectField
            label="Project"
            placeholder="Select a Project"
            name="projectId"
            options={projects.map((project) => ({
              label: project.name,
              value: project.id,
            }))}
            isDisabled={projectGroup}
          />
          <TextField name="name" label="Project Group name" isRequired />
        </VStack>
      </Form>
    </Formik>
  );
  return (
    <BasicModal
      body={Modal}
      title={`${!projectGroup ? "Create" : "Edit"} Project Group`}
      actionBtnCaption={!projectGroup ? "Create" : "Update"}
      onAction={() => formRef.current.handleSubmit()}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};
