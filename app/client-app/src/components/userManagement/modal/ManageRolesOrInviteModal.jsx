import { Alert, AlertIcon, VStack } from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { MultiSelectField } from "components/forms/MultiSelectField";
import { TextField } from "components/forms/TextField";
import { EmailField } from "components/forms/EmailField";
import { useTranslation } from "react-i18next";
import { useRolesList } from "api/roles";
import { object, string, array } from "yup";
import { validationSchemaRegRules as emailSchema } from "components/forms/EmailField";

export const ManageRolesOrInviteModal = ({
  user,
  feedback,
  manageRoles,
  handleSubmit,
  formRef,
}) => {
  // Inviting student to project group will be via ProjectManagement page
  const { data: roles } = useRolesList();

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

  return (
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
          {!manageRoles ? (
            <EmailField hasCheckReminder autoFocus />
          ) : (
            <TextField name="email" label="Email" isDisable />
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
      </Form>
    </Formik>
  );
};
