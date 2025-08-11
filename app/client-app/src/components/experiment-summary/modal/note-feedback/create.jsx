import {
  Alert,
  AlertIcon,
  Box,
  HStack,
  Icon,
  Text,
  useToast,
  VStack,
} from "@chakra-ui/react";
import { FormattedTextInput } from "components/core/forms";
import { Modal } from "components/core/modal";
import { GLOBAL_PARAMETERS, TITLE_ICON_COMPONENTS } from "constants";
import { useBackendApi } from "contexts";
import { Form, Formik } from "formik";
import { useRef, useState } from "react";
import { useTranslation } from "react-i18next";

export const CreateModal = ({
  modalTitle,
  modalMessage,
  successMessage,
  isModalOpen,
  onModalClose,
  record,
  mutate,
  isComplete,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();

  const { t } = useTranslation();
  const toast = useToast();

  const { notes: action } = useBackendApi();

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);
      let response;
      if (isComplete) {
        response = await action.completeFeedback(record.id, values.feedback);
      } else {
        response = await action.requestFeedback(record.id);
      }
      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          position: "top",
          title: successMessage,
          status: "success",
          duration: GLOBAL_PARAMETERS.ToastDuration,
          isClosable: true,
        });
        await mutate();
        onModalClose();
      }
    } catch {
      setFeedback({
        status: "error",
        message: t("feedback.error_title"),
      });
    }
  };

  const formRef = useRef();
  const modalBody = (
    <VStack>
      {feedback && (
        <Alert status={feedback.status}>
          <AlertIcon />
          {feedback.message}
        </Alert>
      )}
      <HStack spacing={5} w="full">
        <Icon
          as={TITLE_ICON_COMPONENTS.Note}
          color={"green.500"}
          fontSize="5xl"
        />
        <VStack align="flex-start" flex={1}>
          <Text align="left">{modalMessage}</Text>
          <Text fontWeight="medium">{record.reactionName}</Text>
        </VStack>
      </HStack>
      <Box w="full">
        {isComplete && (
          <Formik
            innerRef={formRef}
            initialValues={{ feedback: record.feedback || "" }}
            onSubmit={handleSubmit}
          >
            <Form>
              <FormattedTextInput
                name="feedback"
                label=""
                placeholder="Enter your feedback here"
                isRequired
              />
            </Form>
          </Formik>
        )}
      </Box>
    </VStack>
  );

  const resetState = () => {
    setFeedback();
    setIsLoading(false);
  };

  return (
    <Modal
      body={modalBody}
      title={modalTitle}
      actionBtnColorScheme="green"
      onAction={
        isComplete ? () => formRef.current.handleSubmit() : handleSubmit
      }
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={() => {
        resetState();
        onModalClose();
      }}
    />
  );
};
