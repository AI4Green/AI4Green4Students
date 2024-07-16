import { useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { Alert, AlertIcon, HStack, Icon, Text, VStack } from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { TextField } from "components/forms/TextField";
import { Modal } from "components/Modal";
import { useBackendApi } from "contexts/BackendApi";
import { object, string, number } from "yup";
import { useNavigate } from "react-router-dom";
import { FaBook, FaChartLine, FaTasks } from "react-icons/fa";
import { GLOBAL_PARAMETERS } from "constants/global-parameters";

export const CreateOrEditModal = ({
  existingRecordId,
  isModalOpen,
  onModalClose,
  project,
  isLiteratureReview,
  isPlan,
  isReport,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { t } = useTranslation();
  const navigate = useNavigate();

  const { projectGroup } = project;

  const { action, label, pathLabel, icon } = getCreateOrEditItenms(
    isPlan,
    isReport,
    isLiteratureReview
  );

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);
      const response = !existingRecordId
        ? await action.create({ ...values })
        : await action.edit({ ...values, id: existingRecordId });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        const res = await response.json();
        const overviewPath = getOverviewPath(project.id, pathLabel, res.id);

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
          <HStack spacing={5}>
            <Icon
              as={icon}
              color={existingRecordId ? "blue.500" : "green.500"}
              fontSize="5xl"
            />

            <TextField
              name="title"
              label={`${label} title`}
              isDisabled={!isPlan && !isReport} // enable only for plan & report
              flex={1}
              fieldHelp={
                isPlan &&
                "Format: [student initials]_[experiment name]_[experiment condition]_[date]"
              }
            />
          </HStack>
        </VStack>
      </Form>
    </Formik>
  );
  return (
    <Modal
      body={modalBody}
      title={`${!existingRecordId ? "Create" : "Edit"} ${label}`}
      actionBtnCaption={!existingRecordId ? "Create" : "Update"}
      onAction={() => formRef.current.handleSubmit()}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};

/**
 * Get the relevant items based on the section type.
 * @param {bool} isPlan - is the section type a plan
 * @param {bool} isReport - is the section type a report
 * @param {bool} isLiteratureReview - is the section type a literature review
 * @returns - object relevant to the section type.
 * for e.g. if 'isPlan' is true, it will return
 * - planAction, used to create or edit a plan
 * - label, used to display the title of the modal
 * - pathLabel used for settng the path.
 */
const getCreateOrEditItenms = (isPlan, isReport, isLiteratureReview) => {
  const {
    literatureReviews: lrAction,
    plans: planAction,
    reports: reportAction,
  } = useBackendApi();

  let items;
  switch (true) {
    case isPlan:
      items = {
        action: planAction,
        label: "Plan",
        pathLabel: "plans",
        icon: FaTasks,
      };
      break;
    case isReport:
      items = {
        action: reportAction,
        label: "Report",
        pathLabel: "reports",
        icon: FaChartLine,
      };
      break;
    case isLiteratureReview:
      items = {
        action: lrAction,
        label: "Literature review",
        pathLabel: "literature-reviews",
        icon: FaBook,
      };
      break;
  }
  return items;
};

/**
 * Get the navigation path for the overview page.
 */
const getOverviewPath = (projectId, recordType, recordId) =>
  `/projects/${projectId}/${recordType}/${recordId}/overview`;
