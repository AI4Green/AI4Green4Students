import {
  Alert,
  AlertIcon,
  HStack,
  Icon,
  VStack,
  useToast,
} from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { FormikInput, EmailField, emailSchema } from "components/core/forms";
import { useTranslation } from "react-i18next";
import { object } from "yup";
import { useRef, useState } from "react";
import { useUserList } from "api";
import { useBackendApi } from "contexts";
import { errorMessage } from "./error-message";
import { DisplayLink } from "./DisplayLink";
import { FaUserCog } from "react-icons/fa";
import { Modal } from "components/core/Modal";

export const UpdateUserEmailModal = ({ user, isModalOpen, onModalClose }) => {
  const emailChangeKey = "emailChangeLink";
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();
  const [generatedLink, setGeneratedLink] = useState();
  const toast = useToast();
  const { mutate } = useUserList();
  const { users } = useBackendApi();

  const { t } = useTranslation();

  const validationSchema = (existingEmail) =>
    object().shape({
      ...emailSchema({ t, existingEmail }),
    });

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);
      const response = await users.setUserEmail({ id: user.id, values });
      setIsLoading(false);

      if (response && response.status === 200) {
        const parsed = await response?.json(); // parse response body

        parsed[emailChangeKey] &&
          setGeneratedLink(parsed[emailChangeKey] ?? ""); // update the state if link is available

        toast({
          title: "Email change link generated",
          status: "success",
          duration: 1500,
          isClosable: true,
          position: "top",
        });
        return;
      }

      if (response && response.status === 204) {
        toast({
          title: "Email change link sent",
          status: "success",
          duration: 1500,
          isClosable: true,
          position: "top",
        });
        mutate();
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
    <>
      {generatedLink ? (
        <DisplayLink
          displayLink={generatedLink}
          linkType="email change request"
        />
      ) : (
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
      )}
    </>
  );

  const resetState = () => {
    setFeedback();
    setIsLoading(false);
  };

  return (
    <Modal
      body={modalBody}
      title="Change User email"
      actionBtnCaption={generatedLink ? "Ok" : "Update"}
      onAction={
        !generatedLink ? () => formRef.current.handleSubmit() : onModalClose
      }
      actionBtnColorScheme="green"
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={() => {
        resetState();
        onModalClose();
      }}
      cancelBtnEnable={!generatedLink}
    />
  );
};
