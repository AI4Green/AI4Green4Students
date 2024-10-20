import { useEffect, useRef, useState } from "react";
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
import { FormikInput, Datepicker } from "components/core/forms";
import { Modal } from "components/Modal";
import { useProjectGroupsList } from "api/projectGroups";
import { useBackendApi } from "contexts/BackendApi";
import { FaProjectDiagram } from "react-icons/fa";
import { GLOBAL_PARAMETERS } from "constants/global-parameters";
import { useProject } from "api/projects";
import { projectGroupNameValidationSchema as validationSchema } from "../validation";

export const CreateOrEditProjectGroupModal = ({
  project,
  projectGroup,
  isModalOpen,
  onModalClose,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { projectGroups: action } = useBackendApi();
  const { mutate: mutateProject } = useProject(project.id);
  const { mutate: mutateProjectGroups } = useProjectGroupsList(project.id);
  const { t } = useTranslation();
  const toast = useToast();

  const initialValues = () => {
    return projectGroup
      ? {
          name: projectGroup.name,
          projectId: project.id,
          startDate: projectGroup.startDate ?? "",
          planningDeadline: projectGroup.planningDeadline ?? "",
          experimentDeadline: projectGroup.experimentDeadline ?? "",
        }
      : {
          name: "",
          projectId: project.id,
          startDate: "",
          planningDeadline: "",
          experimentDeadline: "",
        };
  };

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);
      const response = !projectGroup
        ? await action.create({ values })
        : await action.edit({ values, id: projectGroup.id });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          title: `Project Group ${!projectGroup ? "created" : "updated"}`,
          status: "success",
          duration: GLOBAL_PARAMETERS.ToastDuration,
          isClosable: true,
          position: "top",
        });
        await mutateProjectGroups();
        await mutateProject();
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
      validationSchema={validationSchema(project)}
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
              <HStack spacing={5}>
                <VStack>
                  <Icon
                    as={FaProjectDiagram}
                    color={projectGroup ? "blue.500" : "green.500"}
                    fontSize="5xl"
                  />
                  <Text as="b">
                    {project.name} <Badge colorScheme="green"> Project </Badge>
                  </Text>
                </VStack>
                <VStack spacing={8}>
                  <FormikInput
                    name="name"
                    label="Project Group name"
                    isRequired
                  />
                  <Datepicker name="startDate" label="Start Date" w="full" />
                  <Datepicker
                    name="planningDeadline"
                    label="Planning Deadline"
                    w="full"
                  />
                  <Datepicker
                    name="experimentDeadline"
                    label="Experiment Deadline"
                    w="full"
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
      title={`${!projectGroup ? "Create" : "Edit"} Project Group`}
      actionBtnCaption={!projectGroup ? "Create" : "Update"}
      onAction={() => formRef.current.handleSubmit()}
      actionBtnColorScheme={!projectGroup ? "green" : "blue"}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={() => {
        resetState();
        onModalClose();
      }}
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
