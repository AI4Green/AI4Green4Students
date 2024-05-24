import {
  Alert,
  AlertIcon,
  HStack,
  Icon,
  VStack,
  useToast,
} from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { MultiSelectField } from "components/forms/MultiSelectField";
import { TextField } from "components/forms/TextField";
import { EmailField } from "components/forms/EmailField";
import { useTranslation } from "react-i18next";
import { useRolesList } from "api/roles";
import { object, string, array } from "yup";
import { validationSchemaRegRules as emailSchema } from "components/forms/EmailField";
import { useBackendApi } from "contexts/BackendApi";
import { useRef, useState } from "react";
import { useUserList } from "api/user";
import { BasicModal } from "components/BasicModal";
import { DisplayLink } from "../DisplayLink";
import { FaUserCog } from "react-icons/fa";
import { errorMessage } from "../error-message";

export const ManageRolesOrInviteModal = ({
  user,
  manageRoles,
  isModalOpen,
  onModalClose,
}) => {
  const activationLinkKey = "activationLink";
  const { users, account } = useBackendApi();
  const [isLoading, setIsLoading] = useState();
  const [feedback, setFeedback] = useState();
  const [generatedLink, setGeneratedLink] = useState();
  const toast = useToast();

  // Inviting student to project group will be via ProjectManagement page
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
      ...emailSchema({ t }),
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
        ? await account.invite(values)
        : await users.setUserRoles({ id: user.id, values: values });

      setIsLoading(false);

      if (manageRoles && response && response.status === 200) {
        const parsed = await response?.json(); // parse response body

        parsed[activationLinkKey] &&
          setGeneratedLink(parsed[activationLinkKey] ?? ""); // update the state if link is available

        toast({
          title: "User invite link generated",
          status: "success",
          duration: 1500,
          isClosable: true,
          position: "top",
        });
        return;
      }

      if (response && response.status === 204) {
        toast({
          title: `User ${!manageRoles ? "invited" : "roles updated"}`,
          status: "success",
          duration: 1500,
        });
        mutate();
        onModalClose();
      }
    } catch (e) {
      const { messageError } = errorMessage(e, t);
      setFeedback({
        status: "error",
        message: messageError,
      });
    }
  };
  const formRef = useRef();

  const Modal = (
    <>
      {generatedLink ? (
        <DisplayLink displayLink={generatedLink} linkType="activation" />
      ) : (
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
                    <TextField name="email" label="Email" isDisabled />
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
      )}
    </>
  );
  return (
    <BasicModal
      body={Modal}
      title={`${!manageRoles ? "Invite" : "Update"} user`}
      actionBtnCaption={
        !manageRoles ? "Invite" : generatedLink ? "Ok" : "Update"
      }
      onAction={
        !generatedLink ? () => formRef.current.handleSubmit() : onModalClose
      }
      actionBtnColorScheme={!manageRoles || generatedLink ? "green" : "blue"}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={onModalClose}
      cancelBtnEnable={!generatedLink}
    />
  );
};
