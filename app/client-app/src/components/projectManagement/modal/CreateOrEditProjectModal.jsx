import { useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import {
  Alert,
  AlertIcon,
  HStack,
  Icon,
  VStack,
  useToast,
} from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { TextField } from "components/forms/TextField";
import { Modal } from "components/Modal";
import { useProjectsList } from "api/projects";
import { useBackendApi } from "contexts/BackendApi";
import { projectValidationSchema as validationSchema } from "../validation";
import { FaLayerGroup } from "react-icons/fa";
import { Datepicker } from "components/forms/Datepicker";

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
    return project
      ? {
          name: project.name,
          startDate: project.startDate ?? "",
          planningDeadline: project.planningDeadline ?? "",
          experimentDeadline: project.experimentDeadline ?? "",
        }
      : {
          name: "",
          startDate: "",
          planningDeadline: "",
          experimentDeadline: "",
        };
  };

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);
      const response = !project
        ? await action.create({ values })
        : await action.edit({ values, id: project.id });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          title: `Project ${!project ? "created" : "updated"}`,
          status: "success",
          duration: 1500,
          isClosable: true,
          position: "top",
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
  const modalBody = (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={initialValues()}
      onSubmit={handleSubmit}
      validationSchema={validationSchema(projects)}
    >
      {({ values, setFieldValue }) => {
        useDeadline("planningDeadline", "startDate", 14, values, setFieldValue);
        useDeadline(
          "experimentDeadline",
          "planningDeadline",
          28,
          values,
          setFieldValue
        );

        return (
          <Form noValidate>
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
                  <TextField name="name" label="Project name" isRequired />
                  <Datepicker name="startDate" label="Start date" isRequired />
                  <Datepicker
                    name="planningDeadline"
                    label="Planning deadline"
                    isRequired
                  />
                  <Datepicker
                    name="experimentDeadline"
                    label="Experiment deadline"
                    isRequired
                  />
                </VStack>
              </HStack>
            </VStack>
          </Form>
        );
      }}
    </Formik>
  );
  return (
    <Modal
      body={modalBody}
      title={`${!project ? "Create" : "Edit"} Project`}
      actionBtnCaption={!project ? "Create" : "Update"}
      onAction={() => formRef.current.handleSubmit()}
      actionBtnColorScheme={!project ? "green" : "blue"}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
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
  }, [values[baseField], setFieldValue]);
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
