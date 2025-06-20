import {
  Alert,
  AlertIcon,
  HStack,
  Icon,
  VStack,
  useToast,
} from "@chakra-ui/react";
import { Form, Formik } from "formik";
import {
  EmailField,
  MultiSelectField,
  FormikInput,
  emailSchema,
} from "components/core/forms";
import { useTranslation } from "react-i18next";
import { object, string, array } from "yup";
import { useBackendApi } from "contexts";
import { useRef, useState } from "react";
import { useUserList, useRolesList } from "api";
import { Modal } from "components/core/Modal";
import { FaUserCog } from "react-icons/fa";
import { errorMessage } from "./error-message";
import { GLOBAL_PARAMETERS } from "constants";

export const ManageRolesOrInviteModal = ({
  user,
  manageRoles,
  isModalOpen,
  onModalClose,
}) => {
  const { users } = useBackendApi();
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();
  const toast = useToast();

  const { data: roles } = useRolesList();
  const { mutate } = useUserList();

  const initialValues = () => {
    return user && manageRoles
      ? {
          email: user?.email,
          roles: user?.roles.map((role) => role.toUpperCase()),
        }
      : { email: "", roles: [] };
  };

  const { t } = useTranslation();

  const validationSchema = () =>
    object().shape({
      ...emailSchema(t),
      roles: array()
        .min(1, "Please select a role")
        .of(
          string().oneOf(
            roles.map((role) => role.name),
            "Invalid roles"
          )
        )
        .required("Valid roles required"),
    });

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);

      const response = !manageRoles
        ? await users.invite(values)
        : await users.setUserRoles({ id: user.id, values: values });

      setIsLoading(false);

      if (response && response.status === 204) {
        toast({
          title: `User ${!manageRoles ? "invited" : "roles updated"}`,
          status: "success",
          duration: GLOBAL_PARAMETERS.ToastDuration,
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
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={initialValues()}
      onSubmit={handleSubmit}
      validationSchema={validationSchema()}
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
            <Icon
              as={FaUserCog}
              color={manageRoles ? "blue.500" : "green.500"}
              fontSize="5xl"
            />
            <VStack w="full">
              {!manageRoles ? (
                <EmailField hasCheckReminder autoFocus />
              ) : (
                <FormikInput name="email" label="Email" isDisabled />
              )}
              <MultiSelectField
                label="Role"
                placeholder="Select a role"
                name="roles"
                options={roles.map((role) => ({
                  label: role.name,
                  value: role.name,
                }))}
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
      title={`${!manageRoles ? "Invite" : "Update"} user`}
      actionBtnCaption={!manageRoles ? "Invite" : "Update"}
      onAction={() => formRef.current.handleSubmit()}
      actionBtnColorScheme={!manageRoles ? "green" : "blue"}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={() => {
        resetState();
        onModalClose();
      }}
    />
  );
};
