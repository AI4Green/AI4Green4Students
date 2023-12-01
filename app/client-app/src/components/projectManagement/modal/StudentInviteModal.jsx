import { useState, useRef } from "react";
import { useTranslation } from "react-i18next";
import { Alert, AlertIcon, Textarea, VStack, useToast } from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { object, string, array, number } from "yup";
import { FormikMultiSelect } from "components/forms/FormikMultiSelect";
import { BasicModal } from "components/BasicModal";
import { useProjectsList } from "api/projects";
import { useProjectGroupsList } from "api/projectGroups";
import { useBackendApi } from "contexts/BackendApi";

export const StudentInviteModal = ({ isModalOpen, onModalClose }) => {
  const [emailList, setEmailList] = useState([]);
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { projectGroups: action } = useBackendApi();
  const { data: projects } = useProjectsList();
  const { mutate: mutateProjectGroups } = useProjectGroupsList();
  const { t } = useTranslation();
  const toast = useToast();

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
      // as multi select returns an array, thus selecting the first element
      const response = await action.inviteStudents({
        values: { projectId: values.projectId[0], emails: values.emails },
        id: values.projectGroupId[0],
      });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        displayToast({
          title: `Students added successfully`,
          status: "success",
          duration: 1500,
        });
        mutateProjectGroups();
        onModalClose();
      }
      // TODO: handle warnings
      // for example: such informinng the user that
      // some emails were already in the project group.
      // some emails were removed from their existing project group
      // and moved to the proposed project group.
    } catch (e) {
      setFeedback({
        status: "error",
        message: t("feedback.error_title"),
      });
    }
  };

  const handleEmailTextAreaChange = ({ target: { value } }, setFieldValue) => {
    const emailSchema = string().email("Invalid email format");

    const list = value.split(",").map((item) => item.trim());
    const uniqueList = [...new Set(list)]; // remove duplicate item
    const validEmailList = uniqueList.filter((item) => {
      return item !== "" && emailSchema.isValidSync(item);
    });

    setEmailList(validEmailList);
    setFieldValue("emails", validEmailList);
  };

  const filteredProjectGroups = (projectId) => {
    const project = projects.find((project) => project.id === projectId);
    return project?.projectGroups || [];
  };

  const validationSchema = () =>
    object().shape({
      emails: array()
        .required("Emails list required")
        .min(1, "Please select at least one email"),
      projectId: array()
        .required("Project required")
        .of(
          number().oneOf(
            projects.map((project) => project.id),
            "Invalid Project"
          )
        ),
      projectGroupId: array().required("Project Group required"),
    });

  const formRef = useRef();
  const Modal = (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={{
        emails: [],
        projectId: [],
        projectGroupId: [],
      }}
      onSubmit={handleSubmit}
      validationSchema={validationSchema()}
    >
      {({ values, setFieldValue }) => (
        <Form noValidate>
          <VStack align="stretch" spacing={4}>
            {feedback && (
              <Alert status={feedback.status}>
                <AlertIcon />
                {feedback.message}
              </Alert>
            )}

            <Textarea
              placeholder="Enter/Paste emails list here"
              rows="3"
              onChange={(value) =>
                handleEmailTextAreaChange(value, setFieldValue)
              }
            />
            <FormikMultiSelect
              isMulti
              label="Student emails"
              placeholder="Select a email"
              name="emails"
              options={emailList.map((email) => ({
                label: email,
                value: email,
              }))}
            />

            <FormikMultiSelect
              label="Project"
              placeholder="Select a Project"
              name="projectId"
              options={projects.map((project) => ({
                label: project.name,
                value: project.id,
              }))}
            />
            <FormikMultiSelect
              label="Project group"
              placeholder="Select a project group"
              name="projectGroupId"
              options={filteredProjectGroups(values.projectId[0]).map(
                (projectGroup) => ({
                  label: projectGroup.name,
                  value: projectGroup.id,
                })
              )}
            />
          </VStack>
        </Form>
      )}
    </Formik>
  );
  return (
    <BasicModal
      body={Modal}
      title="Invite Students"
      actionBtnCaption="Invite"
      onAction={() => formRef.current.handleSubmit()}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};
