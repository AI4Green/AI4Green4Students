import {
  Alert,
  AlertIcon,
  Button,
  Container,
  Heading,
  HStack,
  Link,
  VStack,
  Box,
  Text,
  Divider,
} from "@chakra-ui/react";
import { Link as RouterLink, useLocation, useNavigate } from "react-router-dom";
import { FaSignInAlt, FaMicrosoft } from "react-icons/fa";
import { Form, Formik } from "formik";
import { useTranslation } from "react-i18next";
import { object, string } from "yup";
import { useResetState, useScrollIntoView } from "helpers/hooks";
import { useUser, useBackendApi } from "contexts";
import { ResendConfirmAlert } from "components/account/ResendConfirmAlert";
import { EmailField, PasswordField } from "components/core/forms";

const validationSchema = (t) =>
  object().shape({
    username: string()
      .test(
        "valid-username",
        t("validation.email_valid"),
        (v) =>
          // this allows for DECSYS style "@admin" usernames in future
          // for a non-email seeded superuser
          string().email().isValidSync(v) ||
          string().matches(/^@/).isValidSync(v)
      )
      .required(t("validation.email_required")),
    password: string().required(t("validation.password_required")),
  });

export const Login = () => {
  const { t } = useTranslation();
  const { key, state } = useLocation();
  const navigate = useNavigate();
  const { signIn } = useUser();
  const {
    account: { login, oidcLogin },
  } = useBackendApi();

  // ajax submissions may cause feedback to display
  // but we reset feedback if the page should remount
  const [feedback, setFeedback] = useResetState([key]);

  const [scrollTarget, scrollTargetIntoView] = useScrollIntoView({
    behavior: "smooth",
  });

  const handleSubmit = async (values, actions) => {
    try {
      const { user } = await login(values).json();
      signIn(user);

      // redirect back to where we came from; otherwise the app root
      navigate(state?.from ?? "/");
      return;
    } catch (e) {
      switch (e?.response?.status) {
        case 400: {
          const result = await e.response.json();

          setFeedback({
            status: "error",
            message: result.isUnconfirmedAccount
              ? t("feedback.account.unconfirmed")
              : t("login.feedback.loginFailed"),
            resendConfirm: result.isUnconfirmedAccount,
          });

          break;
        }
        default:
          setFeedback({
            status: "error",
            message: t("feedback.error"),
          });
      }

      scrollTargetIntoView();
    }

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
      alignItems="center"
    >
      <VStack
        w={{ base: "90%", md: "500px" }}
        maxW="500px"
        borderWidth={1}
        borderRadius="md"
        spacing={12}
        align="stretch"
        py={12}
        px={8}
      >
        <Heading as="h2" size="lg" fontWeight="light">
          {t("login.heading")}
        </Heading>

        <Formik
          initialValues={{
            username: "",
            password: "",
          }}
          onSubmit={handleSubmit}
          validationSchema={validationSchema(t)}
        >
          {({ isSubmitting, values }) => (
            <Form noValidate>
              <VStack align="center">
                <VStack spacing={8} w="full">
                  {feedback?.status && (
                    <Alert status={feedback.status}>
                      <AlertIcon />
                      {feedback.message}
                    </Alert>
                  )}
                  {feedback?.resendConfirm && (
                    <ResendConfirmAlert userIdOrEmail={values.username} />
                  )}

                  <EmailField name="username" autoFocus />

                  <PasswordField
                    fieldTip={
                      <Box mt={2}>
                        <Link
                          as={RouterLink}
                          to="/account/password/reset"
                          fontSize="xs"
                        >
                          {t("login.links.forgotPassword")}
                        </Link>
                      </Box>
                    }
                  />
                </VStack>

                <Button
                  w="full"
                  variant="outline"
                  leftIcon={<FaSignInAlt />}
                  type="submit"
                  disabled={isSubmitting}
                  isLoading={isSubmitting}
                  colorScheme="green"
                >
                  {t("buttons.login")}
                </Button>
              </VStack>
            </Form>
          )}
        </Formik>

        <VStack spacing={4}>
          <VStack spacing={4}>
            <Text fontSize="sm" color="gray.500" textAlign="center">
              Or continue with
            </Text>
            <Button
              variant="ghost"
              size="sm"
              leftIcon={<FaMicrosoft />}
              onClick={() => oidcLogin({ redirectUri: "/", idp: "Microsoft" })}
            >
              Microsoft
            </Button>
          </VStack>

          <Divider />

          <HStack justify="center">
            <Text fontSize="sm" color="gray.500" textAlign="center">
              Don't have an account?
            </Text>
            <Link as={RouterLink} to="/account/register" fontSize="sm">
              Register
            </Link>
          </HStack>
        </VStack>
      </VStack>
    </Container>
  );
};
