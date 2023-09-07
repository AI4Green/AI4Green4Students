import { useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { Alert, AlertIcon, VStack, useToast } from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { FormikInput } from "components/forms/FormikInput";
import { BasicModal } from "components/BasicModal";
import { useProjectsList } from "api/projects";
import { useBackendApi } from "contexts/BackendApi";
import { projectNameValidationSchema as validationSchema } from "../validation";

export const CreateOrEditProjectModal = ({
  project,
  isModalOpen,
  onModalClose,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { projects: action } = useBackendApi();
  const { data: projects, mutate } = useProjectsList();
  const { t } = useTranslation();
  const toast = useToast();

  const initialValues = () => {
    return project ? { name: project.name } : { name: "" };
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
      const response = !project
        ? await action.create({ values })
        : await action.edit({ values, id: project.id });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        displayToast({
          title: `Project ${!project ? "created" : "updated"}`,
          status: "success",
          duration: 1500,
        });
        mutate();
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
            <Alert status="error">
              <AlertIcon />
              {feedback}
            </Alert>
          )}
          <FormikInput name="name" label="Project name" isRequired />
        </VStack>
      </Form>
    </Formik>
  );
  return (
    <BasicModal
      body={Modal}
      title={`${!project ? "Create" : "Edit"} Project`}
      actionBtnCaption={!project ? "Create" : "Update"}
      onAction={() => formRef.current.handleSubmit()}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};
