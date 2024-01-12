import { useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { Alert, AlertIcon, VStack } from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { TextField } from "components/forms/TextField";
import { BasicModal } from "components/BasicModal";
import { MultiSelectField } from "components/forms/MultiSelectField";
import { useProjectGroupsList } from "api/projectGroups";
import { useExperimentsList } from "api/experiments";
import { useExperimentTypesList } from "api/experimentTypes";
import { useBackendApi } from "contexts/BackendApi";
import { object, string, array } from "yup";
import { useNavigate } from "react-router-dom";

export const CreateOrEditExperimentModal = ({
  experiment,
  isModalOpen,
  onModalClose,
  projectGroup,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { experiments: action } = useBackendApi();
  const { mutate: mutateExperiments } = useExperimentsList(
    projectGroup?.projectId
  );
  const { data: projectGroups } = useProjectGroupsList();
  const { data: experimentTypes } = useExperimentTypesList();
  const { t } = useTranslation();
  const navigate = useNavigate();

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);
      const response = !experiment
        ? await action.create({
            values: {
              ...values,
              experimentTypeId: values.experimentTypeId[0],
              projectGroupId: values.projectGroupId[0],
            },
          })
        : await action.edit({
            values: {
              ...values,
              experimentTypeId: values.experimentTypeId[0],
              projectGroupId: values.projectGroupId[0],
            },
            id: experiment.id,
          });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        const res = await response.json();
        navigate(`/experiments/${res.id}/plan-overview`, {
          state: {
            toast: {
              position: "top",
              title: `Experiment ${
                !experiment ? "initialised" : "title updated"
              }`,
              status: "success",
              duration: 1500,
              isClosable: true,
            },
          },
        });
        mutateExperiments();
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
        title: experiment?.title || "",
        projectGroupId: [projectGroup.id],
        experimentTypeId: [experimentTypes[0].id], // TODO: default to first experiment type for now,
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
          <MultiSelectField
            label="Project"
            placeholder="Select a Project"
            name="projectGroupId"
            options={projectGroups.map((projectGroup) => ({
              label: projectGroup.projectName,
              value: projectGroup.id,
              // Assigning experiment to a project group not a project
              // In future, this could allow project groups students to access/view the experiment
            }))}
            isDisabled
          />
          <MultiSelectField
            label="Experiment type"
            placeholder="Select an experiment type"
            name="experimentTypeId"
            options={experimentTypes.map((experimentType) => ({
              label: experimentType.name,
              value: experimentType.id,
            }))}
            isDisabled
          />
          <TextField name="title" label="Experiment title" isRequired />
        </VStack>
      </Form>
    </Formik>
  );
  return (
    <BasicModal
      body={Modal}
      title={`${!experiment ? "Create" : "Edit"} Experiment`}
      actionBtnCaption={!experiment ? "Create" : "Update"}
      onAction={() => formRef.current.handleSubmit()}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};
