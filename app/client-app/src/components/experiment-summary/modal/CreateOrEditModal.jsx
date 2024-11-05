import { useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { Alert, AlertIcon, HStack, Icon, VStack } from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { FormikInput } from "components/core/forms";
import { Modal } from "components/core/Modal";
import { useBackendApi } from "contexts";
import { object, string, number } from "yup";
import { useNavigate } from "react-router-dom";
import { FaBook, FaChartLine, FaTasks } from "react-icons/fa";
import { GLOBAL_PARAMETERS, SECTION_TYPES } from "constants";
import { buildOverviewPath } from "routes/Project";

export const CreateOrEditModal = ({
  existingRecordId,
  isModalOpen,
  onModalClose,
  project,
  sectionType,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { t } = useTranslation();
  const navigate = useNavigate();

  const { projectGroup } = project;

  const { action, label, icon } = getCreateOrEditItenms(sectionType);

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);
      const response = !existingRecordId
        ? await action.create({ ...values })
        : await action.edit({ ...values, id: existingRecordId });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        const res = await response.json();
        const overviewPath = buildOverviewPath(
          sectionType,
          project.id,
          projectGroup.id,
          res.id
        );

        navigate(overviewPath, {
          state: {
            toast: {
              position: "top",
              title: `${label} ${
                !existingRecordId ? "initialised" : "updated"
              }`,
              status: "success",
              duration: GLOBAL_PARAMETERS.ToastDuration,
              isClosable: true,
            },
          },
        });
        window.history.replaceState(null, "");
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
      projectGroupId: number().required("Project id is required"),
    });

  const formRef = useRef();
  const modalBody = (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={{
        title: `Sample ${label} title`,
        projectGroupId: projectGroup.id,
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
          {sectionType !== SECTION_TYPES.LiteratureReview && (
            <HStack spacing={5}>
              <Icon
                as={icon}
                color={existingRecordId ? "blue.500" : "green.500"}
                fontSize="5xl"
              />

              <FormikInput
                name="title"
                label={`${label} title`}
                flex={1}
                fieldHelp={
                  sectionType === SECTION_TYPES.Plan &&
                  "Format: [student initials]_[experiment name]_[experiment condition]_[date]"
                }
              />
            </HStack>
          )}
        </VStack>
      </Form>
    </Formik>
  );

  const resetState = () => {
    setFeedback();
    setIsLoading(false);
  };

  return (
    <Modal
      body={modalBody}
      title={`${!existingRecordId ? "Create" : "Edit"} ${label}`}
      actionBtnCaption={!existingRecordId ? "Create" : "Update"}
      onAction={() => formRef.current.handleSubmit()}
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
 * Get the relevant items based on the section type.
 * @param {string} sectionType - section type
 * @returns - object relevant to the section type.
 * for e.g. if section type is 'Plan', it will return
 * - planAction, used to create or edit a plan
 * - label, used to display the title of the modal
 */
const getCreateOrEditItenms = (sectionType) => {
  const {
    literatureReviews: lrAction,
    plans: planAction,
    reports: reportAction,
  } = useBackendApi();

  let items;
  switch (sectionType) {
    case SECTION_TYPES.Plan:
      items = {
        action: planAction,
        label: "Plan",
        icon: FaTasks,
      };
      break;
    case SECTION_TYPES.Report:
      items = {
        action: reportAction,
        label: "Report",
        icon: FaChartLine,
      };
      break;
    case SECTION_TYPES.LiteratureReview:
      items = {
        action: lrAction,
        label: "Literature review",
        icon: FaBook,
      };
      break;
  }
  return items;
};
