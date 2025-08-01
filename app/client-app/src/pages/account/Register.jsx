import {
  Alert,
  AlertIcon,
  Button,
  Container,
  Heading,
  HStack,
  Link,
  VStack,
} from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { Link as RouterLink, useLocation } from "react-router-dom";
import { FaUserPlus } from "react-icons/fa";
import { useTranslation } from "react-i18next";
import { object, string } from "yup";
import { useResetState, useScrollIntoView } from "helpers/hooks";
import {
  FormikInput,
  ScrollToError,
  EmailField,
  PasswordField,
  passwordSchema,
  emailSchemaRegistrationRules,
} from "components/core/forms";
import { TitledAlert } from "components/core/TitledAlert";
import { useBackendApi } from "contexts";

export const validationSchema = ({ t }) =>
  object().shape({
    fullname: string().required(t("validation.fullname_required")),
    ...emailSchemaRegistrationRules({ t }),
    ...passwordSchema(t),
  });

export const Register = () => {
  const { key } = useLocation();
  const { t } = useTranslation();
  const {
    account: { register },
  } = useBackendApi();

  // ajax submissions may cause feedback to display
  // but we reset feedback if the page should remount
  const [feedback, setFeedback] = useResetState([key]);

  const [scrollTarget, scrollTargetIntoView] = useScrollIntoView({
    behavior: "smooth",
  });

  const handleSubmit = async (values, actions) => {
    // If submission was triggered by hitting Enter,
    // we should blur the focused input
    // so we don't mess up validation when we reset after submission
    if (document?.activeElement) document.activeElement.blur();

    try {
      await register(values);

      setFeedback({
        status: "success",
        message: t("register.feedback.success"),
      });

      // when we reset, untouch fields so that clicking away from an input
      // that we are emptying doesn't (in)validate that now empty input
      actions.resetForm({ touched: [] });
    } catch (e) {
      switch (e?.response?.status) {
        case 400: {
          const result = await e.response.json();

          let message = t("feedback.invalidForm");
          if (result.isExistingUser)
            message = t("register.feedback.existingUser");
          else if (result.isNotAllowlisted)
            message = t("register.feedback.emailNotAllowlisted");

          setFeedback({ status: "error", message });

          break;
        }

        default:
          setFeedback({
            status: "error",
            message: t("feedback.error"),
          });
      }
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
          {t("register.heading")}
        </Heading>

        {feedback?.status && (
          <Alert status={feedback.status}>
            <AlertIcon />
            {feedback.message}
          </Alert>
        )}
        {feedback?.status === "success" && (
          <TitledAlert title={t("register.feedback.confirm_title")}>
            {t("register.feedback.confirm_message")}
          </TitledAlert>
        )}

        <Formik
          initialValues={{
            fullname: "",
            email: "",
            password: "",
          }}
          onSubmit={handleSubmit}
          validationSchema={validationSchema({ t })}
        >
          {({ isSubmitting, isValid }) => (
            <Form noValidate>
              <ScrollToError />
              <VStack align="stretch" spacing={8}>
                <EmailField hasCheckReminder autoFocus />

                <FormikInput
                  name="fullname"
                  label={t("register.fields.fullname")}
                  placeholder={t("register.fields.fullname_placeholder")}
                  isRequired
                />

                <PasswordField />

                <HStack justify="space-between">
                  <Button
                    w="200px"
                    variant="outline"
                    leftIcon={<FaUserPlus />}
                    type="submit"
                    disabled={isSubmitting || !isValid} // disable if form is not valid
                    isLoading={isSubmitting}
                  >
                    {t("buttons.register")}
                  </Button>

                  <Link as={RouterLink} to="/account/login">
                    {t("register.links.login")}
                  </Link>
                </HStack>
              </VStack>
            </Form>
          )}
        </Formik>
      </VStack>
    </Container>
  );
};
