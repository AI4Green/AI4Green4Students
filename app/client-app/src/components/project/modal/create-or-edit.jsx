import {
  Alert,
  AlertIcon,
  HStack,
  Icon,
  useToast,
  VStack,
} from "@chakra-ui/react";
import { useProjectsList, useProjectTypesList } from "api";
import { FormikInput, MultiSelectField } from "components/core/forms";
import { Modal } from "components/core/modal";
import { GLOBAL_PARAMETERS, STAGES } from "constants";
import { useBackendApi } from "contexts";
import { Form, Formik } from "formik";
import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { FaLayerGroup } from "react-icons/fa";

import { validationSchema } from "./validation";

export const CreateOrEditProjectModal = ({
  project,
  isModalOpen,
  onModalClose,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { projects: action } = useBackendApi();
  const { data: projects, mutate } = useProjectsList();
  const { data: projectTypes } = useProjectTypesList();

  const { t } = useTranslation();
  const toast = useToast();

  const initialValues = () => {
    return project
      ? {
          name: project.name,
          projectTypeId: [String(project.projectType.id)],
        }
      : {
          name: "",
          projectTypeId: [],
        };
  };

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);
      const model = {
        name: values.name,
        projectTypeId: Number(values.projectTypeId[0]),
      };
      const response = !project
        ? await action.create({ values: model })
        : await action.edit({ values: model, id: project.id });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          title: `Project ${!project ? "created" : "updated"}`,
          status: "success",
          duration: GLOBAL_PARAMETERS.ToastDuration,
          isClosable: true,
          position: "top",
        });
        mutate();
        onModalClose();
      }
    } catch (e) {
      switch (e?.response?.status) {
        case 400: {
          setFeedback({
            status: "error",
            message: t("feedback.error_400"),
          });
          break;
        }
        default: {
          setFeedback({
            status: "error",
            message: t("feedback.error"),
          });
          break;
        }
      }
    }
  };

  const formRef = useRef();
  const modalBody = (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={initialValues()}
      onSubmit={handleSubmit}
      validationSchema={validationSchema(projects, projectTypes)}
    >
      {({ values, setFieldValue }) => {
        return (
          <Form noValidate>
            <DeadlinesManager values={values} setFieldValue={setFieldValue} />
            <VStack align="stretch" spacing={4}>
              {feedback && (
                <Alert status={feedback.status}>
                  <AlertIcon />
                  {feedback.message}
                </Alert>
              )}
              <HStack spacing={5} align="start">
                <Icon
                  as={FaLayerGroup}
                  color={project ? "blue.500" : "green.500"}
                  fontSize="5xl"
                />
                <VStack w="full">
                  <FormikInput name="name" label="Project name" isRequired />
                  <MultiSelectField
                    name="projectTypeId"
                    label="Project type"
                    options={projectTypes
                      .filter(
                        (projectType) => projectType.stage === STAGES.Ready
                      )
                      .map((projectType) => ({
                        label: projectType.name,
                        value: String(projectType.id),
                        description: projectType.description,
                      }))}
                    isDisabled={!!project}
                  />
                </VStack>
              </HStack>
            </VStack>
          </Form>
        );
      }}
    </Formik>
  );

  const resetState = () => {
    setFeedback();
    setIsLoading(false);
  };

  return (
    <Modal
      body={modalBody}
      title={`${!project ? "Create" : "Edit"} Project`}
      actionBtnCaption={!project ? "Create" : "Update"}
      onAction={() => formRef.current.handleSubmit()}
      actionBtnColorScheme={!project ? "green" : "blue"}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={() => {
        resetState();
        onModalClose();
      }}
    />
  );
};

const DeadlinesManager = ({ values, setFieldValue }) => {
  useDeadline("planningDeadline", "startDate", 14, values, setFieldValue);
  useDeadline(
    "experimentDeadline",
    "planningDeadline",
    28,
    values,
    setFieldValue
  );
  return null;
};

/**
 * Hook to set the deadline field value based on the baseField value
 * @param {*} field - field to be set
 * @param {*} baseField - field to be used as base for calculation
 * @param {*} daysToAdd - number of days to add to the baseField
 * @param {*} values - formik values
 * @param {*} setFieldValue - formik setFieldValue
 */
const useDeadline = (field, baseField, daysToAdd, values, setFieldValue) => {
  useEffect(() => {
    const deadline = values[baseField]
      ? calculateDeadline(values[baseField], daysToAdd)
      : "";
    setFieldValue(field, deadline);
  }, [baseField, daysToAdd, field, setFieldValue, values]);
};

const calculateDeadline = (startdate, daysToAdd) => {
  const deadline = new Date(startdate);
  deadline.setDate(deadline.getDate() + daysToAdd);

  /**
   * isoString is in the format of yyyy-mm-ddThh:mm:ss.sssZ
   * split string by 'T' and get the first element, which is the date
   */
  return deadline.toISOString().split("T")[0];
};
