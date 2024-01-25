import { useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { Alert, AlertIcon, VStack } from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { TextField } from "components/forms/TextField";
import { BasicModal } from "components/BasicModal";
import { usePlansList } from "api/plans";
import { useBackendApi } from "contexts/BackendApi";
import { object, string, array } from "yup";
import { useNavigate } from "react-router-dom";

export const CreateOrEditPlanModal = ({
  existingPlanId,
  isModalOpen,
  onModalClose,
  project,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { plans: action } = useBackendApi();
  const { mutate: mutatePlans } = usePlansList(project?.id);
  const { t } = useTranslation();
  const navigate = useNavigate();

  const {
    sectionTypes: { planSectionTypeId },
    projectGroups,
  } = project;

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);
      const response = !existingPlanId
        ? await action.create({
            values: {
              ...values,
              projectGroupId: values.projectGroupId[0],
            },
          })
        : await action.edit({
            values: {
              ...values,
              projectGroupId: values.projectGroupId[0],
            },
            id: existingPlanId,
          });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        const res = await response.json();
        navigate(`/project/${planSectionTypeId}/plan-overview/${res.id}`, {
          state: {
            toast: {
              position: "top",
              title: `Plan ${!existingPlanId ? "initialised" : "updated"}`,
              status: "success",
              duration: 1500,
              isClosable: true,
            },
          },
        });
        mutatePlans();
      }
    } catch (e) {
      setFeedback({
        status: "error",
        message: t("feedback.error_title"),
      });
    }
  };
  const validationSchema = () =>
    object().shape({
      title: string().required("Title is required"),
      projectGroupId: array().required("Project group is required"),
    });

  const formRef = useRef();
  const Modal = (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={{
        title: "Sample plan title", // TODO: Double check if we want to set title for plans or not. For now, we set it to "Plan title" and disable the field.
        projectGroupId: [projectGroups[0].id], // first project group as student can only be in one project group
      }}
      onSubmit={handleSubmit}
      validationSchema={validationSchema()}
    >
      <Form noValidate>
        <VStack align="stretch" spacing={4}>
          {feedback && (
            <Alert status={feedback.status}>
              <AlertIcon />
              {feedback.message}
            </Alert>
          )}
          <TextField name="title" label="Plan title" isDisabled />
        </VStack>
      </Form>
    </Formik>
  );
  return (
    <BasicModal
      body={Modal}
      title={`${!existingPlanId ? "Create" : "Edit"} Plan`}
      actionBtnCaption={!existingPlanId ? "Create" : "Update"}
      onAction={() => formRef.current.handleSubmit()}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};
