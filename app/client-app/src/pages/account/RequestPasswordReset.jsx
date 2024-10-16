import {
  Alert,
  AlertIcon,
  Button,
  Container,
  Heading,
  VStack,
} from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { useLocation } from "react-router-dom";
import { BiMailSend } from "react-icons/bi";
import { useTranslation } from "react-i18next";
import { object } from "yup";
import { useResetState } from "helpers/hooks/useResetState";
import {
  EmailField,
  validationSchema as emailSchema,
} from "components/forms/EmailField";
import { useBackendApi } from "contexts/BackendApi";
import { useScrollIntoView } from "helpers/hooks/useScrollIntoView";

const validationSchema = (t) => object().shape(emailSchema(t));

export const RequestPasswordReset = () => {
  const { key } = useLocation();
  const { t } = useTranslation();
  const {
    account: { requestPasswordReset },
  } = useBackendApi();

  // ajax submissions may cause feedback to display
  // but we reset feedback if the page should remount
  const [feedback, setFeedback] = useResetState([key]);

  const [scrollTarget, scrollTargetIntoView] = useScrollIntoView({
    behavior: "smooth",
  });

  const handleSubmit = async ({ email }, actions) => {
    // If submission was triggered by hitting Enter,
    // we should blur the focused input
    // so we don't mess up validation when we reset after submission
    if (document?.activeElement) document.activeElement.blur();

    let success = false;

    try {
      await requestPasswordReset(email);

      success = true;
    } catch (e) {
      switch (e?.response?.status) {
        case 404:
          // We didn't find the user, but we don't want to expose that
          success = true;
          break;
        case 400:
          setFeedback({
            status: "error",
            message: t("feedback.invalidForm"),
          });
          break;

        default:
          setFeedback({
            status: "error",
            message: t("feedback.error"),
          });
      }
    }

    if (success) {
      setFeedback({
        status: "success",
        message: t("requestPasswordReset.feedback.success"),
      });

      // when we reset, untouch fields so that clicking away from an input
      // that we are emptying doesn't (in)validate that now empty input
      actions.resetForm({ touched: [] });
    }

    scrollTargetIntoView();

    actions.setSubmitting(false);
  };

  return (
    <Container
      ref={scrollTarget}
      key={key}
      h="80vh"
      display="flex"
      flexDirection="column"
      justifyContent="center"
    >
      <VStack
        borderWidth={1}
        borderRadius="md"
        spacing={12}
        align="stretch"
        py={12}
        px={8}
      >
        <Heading as="h2" size="lg" fontWeight="light">
          {t("requestPasswordReset.heading")}
        </Heading>

        {feedback?.status && (
          <Alert status={feedback.status}>
            <AlertIcon />
            {feedback.message}
          </Alert>
        )}

        <Formik
          initialValues={{
            email: "",
          }}
          onSubmit={handleSubmit}
          validationSchema={validationSchema(t)}
        >
          {({ isSubmitting }) => (
            <Form noValidate>
              <VStack align="stretch" spacing={8}>
                <EmailField autoFocus />

                <Button
                  w="200px"
                  variant="outline"
                  leftIcon={<BiMailSend />}
                  type="submit"
                  disabled={isSubmitting}
                  isLoading={isSubmitting}
                >
                  {t("requestPasswordReset.buttons.submit")}
                </Button>
              </VStack>
            </Form>
          )}
        </Formik>
      </VStack>
    </Container>
  );
};
