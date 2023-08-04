import {
  Alert,
  AlertIcon,
  VStack,
  Switch,
  FormLabel,
  HStack,
  useToast,
} from "@chakra-ui/react";
import { Form, Formik, Field } from "formik";
import { FormikInput } from "components/forms/FormikInput";
import { regRuleValueValidationSchema } from "components/registrationRule/validation";
import { useRegistrationRulesList } from "api/registrationRules";
import { BasicModal } from "components/BasicModal";
import { useRef, useState } from "react";
import { useBackendApi } from "contexts/BackendApi";
import { useTranslation } from "react-i18next";

export const ModalCreateOrEditRegistrationRule = ({
  registrationRule, // Only available in edit
  isModalOpen,
  onModalClose,
}) => {
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();
  const { registrationRules: action } = useBackendApi();
  const { data: registrationRulesList, mutate } = useRegistrationRulesList();
  const { t } = useTranslation();

  const initialValues = () => {
    return registrationRule
      ? {
          value: registrationRule?.value,
          isBlocked: registrationRule?.isBlocked,
        }
      : {
          value: "",
          isBlocked: true, // by default set to true
        };
  };

  const toast = useToast();
  // toast configured for the User management page
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

      const response = !registrationRule
        ? await action.create({ values })
        : await action.edit({ values, id: registrationRule.id });

      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        displayToast({
          title: `Registration rule ${
            !registrationRule ? "created" : "updated"
          }`,
          status: "success",
          duration: 1500,
        });
        mutate(); // refresh the user list
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
  const Modal = (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={initialValues()}
      onSubmit={handleSubmit}
      validationSchema={
        // Only apply validation when creating new rule
        !registrationRule && regRuleValueValidationSchema(registrationRulesList)
      }
    >
      {({ values, setFieldValue }) => (
        <Form noValidate>
          <VStack align="stretch" spacing={4}>
            {feedback && (
              <Alert status="error">
                <AlertIcon />
                {feedback}
              </Alert>
            )}

            {!registrationRule ? (
              <FormikInput name="value" label="Value" isRequired />
            ) : (
              <FormikInput name="value" label="Value" isDisable />
            )}
            <HStack>
              <FormLabel htmlFor="registration-block-" mb="0">
                Add to blocklist ?
              </FormLabel>
              <Field name="isBlocked">
                {({ field }) => (
                  <Switch
                    id="isBlocked"
                    {...field}
                    isChecked={values.isBlocked}
                    onChange={(e) =>
                      setFieldValue("isBlocked", e.target.checked)
                    }
                    colorScheme="teal"
                  />
                )}
              </Field>
            </HStack>
          </VStack>
        </Form>
      )}
    </Formik>
  );
  return (
    <BasicModal
      body={Modal} // render modal as per the selected action
      title={`${!registrationRule ? "Create" : "Edit"} registration rule`}
      actionBtnCaption={!registrationRule ? "Create" : "Update"}
      onAction={() => formRef.current.handleSubmit()}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};
