import { useState, useRef } from "react";
import { useTranslation } from "react-i18next";
import {
  Alert,
  AlertIcon,
  Textarea,
  VStack,
  useToast,
  Text,
  Badge,
} from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { object, string, array, number } from "yup";
import { MultiSelectField } from "components/forms/MultiSelectField";
import { Modal } from "components/Modal";
import { useProjectGroupsList } from "api/projectGroups";
import { useBackendApi } from "contexts/BackendApi";
import { GLOBAL_PARAMETERS } from "constants/global-parameters";

export const StudentInviteModal = ({
  isModalOpen,
  onModalClose,
  projectGroup,
  project,
}) => {
  const [emailList, setEmailList] = useState([]);
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { projectGroups: action } = useBackendApi();
  const { mutate: mutateProjectGroups } = useProjectGroupsList();
  const { t } = useTranslation();
  const toast = useToast();

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);
      const response = await action.inviteStudents({
        values: { projectId: values.projectId, emails: values.emails },
        id: values.projectGroupId,
      });
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          title: `Students added successfully`,
          status: "success",
          duration: GLOBAL_PARAMETERS.ToastDuration,
          position: "top",
          isClosable: true,
        });
        mutateProjectGroups();
        onModalClose();
      }
      // TODO: handle warnings
      // for example:
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

  /**
   * TODO: add email validation.
   * In other parts of the app, we validate emails against registration rules on the go
   * but this approach can have performance issues in this case because we
   * may have a large number of emails to validate.
   * Probaly, we can validate in batches or during the submit but would require some work in the backend,
   * especially if we want to provide feedback to the user about which emails are invalid.
   */
  const validationSchema = () =>
    object().shape({
      emails: array()
        .required("Emails list required")
        .min(1, "Please select at least one email"),
      projectGroupId: number().required("Project group required"),
    });

  const formRef = useRef();
  const modalBody = (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={{
        emails: [],
        projectGroupId: projectGroup.id,
        projectId: project.id,
      }}
      onSubmit={handleSubmit}
      validationSchema={validationSchema()}
    >
      {({ setFieldValue }) => (
        <Form noValidate>
          <VStack align="flex-start" spacing={4}>
            {feedback && (
              <Alert status={feedback.status}>
                <AlertIcon />
                {feedback.message}
              </Alert>
            )}

            <Text as="i">Invite students to the following:</Text>
            <VStack
              align="flex-start"
              w="full"
              spacing={1}
              borderWidth={1}
              borderRadius={7}
              p={2}
            >
              <Text as="b" fontSize="sm">
                <Badge colorScheme="green"> Project </Badge>
                {project.name}
              </Text>
              <Text as="b" fontSize="sm">
                <Badge colorScheme="blue"> Project group </Badge>
                {projectGroup.name}
              </Text>
            </VStack>

            <Textarea
              placeholder="Enter/Paste emails list here"
              rows="3"
              onChange={(value) =>
                handleEmailTextAreaChange(value, setFieldValue)
              }
            />
            <MultiSelectField
              isMulti
              label="Student emails"
              placeholder="Select a email"
              name="emails"
              options={emailList.map((email) => ({
                label: email,
                value: email,
              }))}
            />
          </VStack>
        </Form>
      )}
    </Formik>
  );
  return (
    <Modal
      body={modalBody}
      title="Invite Students"
      actionBtnCaption="Invite"
      onAction={() => formRef.current.handleSubmit()}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};
