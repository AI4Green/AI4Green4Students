import { Alert, AlertIcon, VStack } from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { FormikInput } from "components/forms/FormikInput";
import { EmailField } from "components/forms/EmailField";
import { validationSchemaExistingEmail as emailSchema } from "components/forms/EmailField";
import { useTranslation } from "react-i18next";
import { object } from "yup";

export const UpdateUserEmailModal = ({
  user,
  feedback,
  handleSubmit,
  formRef,
}) => {
  const { t } = useTranslation();

  const validationSchema = (existingEmail) =>
    object().shape({
      ...emailSchema({ t, existingEmail }),
    });

  return (
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
            <Alert status="error">
              <AlertIcon />
              {feedback}
            </Alert>
          )}
          <FormikInput
            name="currentEmail"
            label="Current Email Address"
            isDisable
          />
          <EmailField hasCheckReminder autoFocus label="New Email Address" />
        </VStack>
      </Form>
    </Formik>
  );
};
