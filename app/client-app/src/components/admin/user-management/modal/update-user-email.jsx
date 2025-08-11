import {
  Alert,
  AlertIcon,
  HStack,
  Icon,
  useToast,
  VStack,
} from "@chakra-ui/react";
import {
  EmailField,
  emailSchemaExistingEmail,
  FormikInput,
} from "components/core/forms";
import { Modal } from "components/core/modal";
import { useBackendApi } from "contexts";
import { Form, Formik } from "formik";
import { useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { FaUserCog } from "react-icons/fa";
import { object } from "yup";

import { errorMessage } from "./error-message";

export const UpdateUserEmailModal = ({ user, isModalOpen, onModalClose }) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();
  const toast = useToast();
  const {
    users,
    registrationRules: { validate },
  } = useBackendApi();

  const { t } = useTranslation();

  const validationSchema = (existingEmail) =>
    object().shape({
      ...emailSchemaExistingEmail({ t, existingEmail, validate }),
    });

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);
      const response = await users.setUserEmail({ id: user.id, values });
      setIsLoading(false);

      if (response && response.status === 204) {
        toast({
          title: "Email change link sent",
          status: "success",
          duration: 1500,
          isClosable: true,
          position: "top",
        });
        onModalClose();
      }
    } catch (e) {
      const { messageError } = await errorMessage(e, t);
      setFeedback({
        status: "error",
        message: messageError,
      });
    }
  };

  const formRef = useRef();
  const modalBody = (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={{ currentEmail: user?.email, email: "" }}
      onSubmit={handleSubmit}
      validationSchema={validationSchema(user?.email)}
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
            <Icon as={FaUserCog} color="green.500" fontSize="5xl" />
            <VStack w="full">
              <FormikInput
                name="currentEmail"
                label="Current Email Address"
                isDisabled
              />
              <EmailField
                hasCheckReminder
                autoFocus
                label="New Email Address"
              />
            </VStack>
          </HStack>
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
      title="Change User email"
      actionBtnCaption="Update"
      onAction={() => formRef.current.handleSubmit()}
      actionBtnColorScheme="green"
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={() => {
        resetState();
        onModalClose();
      }}
    />
  );
};
