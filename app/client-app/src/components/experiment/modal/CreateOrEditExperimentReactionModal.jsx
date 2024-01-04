import { useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { Alert, AlertIcon, VStack } from "@chakra-ui/react";
import { useExperimentsList } from "api/experiments";
import { Form, Formik } from "formik";
import { TextField } from "components/forms/TextField";
import { BasicModal } from "components/BasicModal";
import { useBackendApi } from "contexts/BackendApi";
import { object, string } from "yup";
import { useNavigate } from "react-router-dom";

export const CreateOrEditExperimentReactionModal = ({
  reaction,
  experiment,
  isModalOpen,
  onModalClose,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { experimentReactions: action } = useBackendApi();
  const { mutate: mutateExperiment } = useExperimentsList(reaction?.projectId);
  const { t } = useTranslation();
  const navigate = useNavigate();

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);
      const response = !reaction
        ? await action.create({
            values: {
              ...values,
              experimentId: experiment.id,
            },
          })
        : await action.edit({
            values: {
              ...values,
              experimentId: reaction.experimentId,
            },
            id: reaction.id,
          });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        const res = await response.json();
        navigate(
          `/experiments/${res.experimentId}/reaction-overview/${res.id}`,
          {
            state: {
              toast: {
                position: "top",
                title: `Reaction ${
                  !experiment ? "initialised" : "title updated"
                }`,
                status: "success",
                duration: 1500,
                isClosable: true,
              },
            },
          }
        );
        mutateExperiment();
        onModalClose();
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
    });

  const formRef = useRef();
  const Modal = (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={{
        title: reaction?.title || "",
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
          <TextField name="title" label="Reaction title" isRequired />
        </VStack>
      </Form>
    </Formik>
  );
  return (
    <BasicModal
      body={Modal}
      title={`${!reaction ? "Create" : "Edit"} Reaction`}
      actionBtnCaption={!reaction ? "Create" : "Update"}
      onAction={() => formRef.current.handleSubmit()}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};
