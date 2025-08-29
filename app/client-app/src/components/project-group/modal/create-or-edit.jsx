import {
  Alert,
  AlertIcon,
  Badge,
  HStack,
  Icon,
  Text,
  useToast,
  VStack,
} from "@chakra-ui/react";
import { useProject, useProjectGroupsList } from "api";
import { Datepicker, FormikInput } from "components/core/forms";
import { Modal } from "components/core/modal";
import { GLOBAL_PARAMETERS } from "constants";
import { useBackendApi } from "contexts";
import { Form, Formik } from "formik";
import { useEffect, useRef } from "react";
import { useTranslation } from "react-i18next";
import { FaProjectDiagram } from "react-icons/fa";
import {
  useLocation,
  useNavigate,
  useParams,
  useSearchParams,
} from "react-router-dom";

import { useModalState } from "./useModalState";
import { validationSchema } from "./validation";

export const CreateOrEditProjectGroupModal = () => {
  const [searchParams] = useSearchParams();
  const id = searchParams.get("id");
  const isEditAction = searchParams.get("action") === "edit";
  const { projectId } = useParams();

  const navigate = useNavigate();
  const location = useLocation();

  const { projectGroups: action } = useBackendApi();
  const { data: project } = useProject(projectId);
  const { data: projectGroups, mutate } = useProjectGroupsList(projectId);

  const { t } = useTranslation();
  const toast = useToast();

  const formRef = useRef();

  const {
    isModalOpen,
    setIsModalOpen,
    isLoading,
    setIsLoading,
    feedback,
    setFeedback,
    handleReset,
  } = useModalState(location, navigate, formRef);

  const projectGroup = isEditAction
    ? projectGroups?.find((projectGroup) => projectGroup.id === Number(id))
    : undefined;

  useEffect(() => {
    setIsModalOpen(true);
  }, [id, isEditAction, projectId, setIsModalOpen]);

  const initialValues = () => {
    return projectGroup
      ? {
          name: projectGroup.name,
          projectId: projectGroup.project.id,
          startDate: projectGroup.startDate ?? "",
          planningDeadline: projectGroup.planningDeadline ?? "",
          experimentDeadline: projectGroup.experimentDeadline ?? "",
        }
      : {
          name: "",
          projectId: projectId,
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
        handleReset();
        await mutate();
      }
    } catch {
      setFeedback({
        status: "error",
        message: t("feedback.error_title"),
      });
    }
  };

  const modalBody = (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={initialValues()}
      onSubmit={handleSubmit}
      validationSchema={validationSchema(projectGroups)}
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
              <HStack spacing={5}>
                <VStack>
                  <Icon
                    as={FaProjectDiagram}
                    color={projectGroup ? "blue.500" : "green.500"}
                    fontSize="5xl"
                  />
                  <Text as="b">
                    {project?.name}
                    <Badge colorScheme="green"> Project </Badge>
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

  if (isEditAction && !projectGroup) {
    navigate(location.pathname, { replace: true });
    return null;
  }

  return (
    <Modal
      body={modalBody}
      title={`${!projectGroup ? "Create" : "Edit"} Project Group`}
      actionBtnCaption={!projectGroup ? "Create" : "Update"}
      onAction={() => formRef.current.handleSubmit()}
      actionBtnColorScheme={!projectGroup ? "green" : "blue"}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={handleReset}
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
