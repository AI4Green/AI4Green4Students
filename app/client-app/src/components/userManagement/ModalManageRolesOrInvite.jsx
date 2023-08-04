import { Alert, AlertIcon, VStack } from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { FormikMultiSelect } from "components/forms/FormikMultiSelect";
import { FormikInput } from "components/forms/FormikInput";
import { EmailField } from "components/forms/EmailField";
import { useTranslation } from "react-i18next";
import { useRolesList } from "api/roles";
import { object, string, array } from "yup";
import { validationSchemaRegRules as emailSchema } from "components/forms/EmailField";

export const ModalManageRolesOrInvite = ({
  user,
  feedback,
  manageRoles,
  handleSubmit,
  formRef,
}) => {
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
        .min(1, "Please select at least one role")
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
            <Alert status="error">
              <AlertIcon />
              {feedback}
            </Alert>
          )}
          {!manageRoles ? (
            <EmailField hasCheckReminder autoFocus />
          ) : (
            <FormikInput name="email" label="Email" isDisable />
          )}
          <FormikMultiSelect
            label="Roles"
            placeholder="Select roles"
            name="roles"
            options={roles.map((role) => ({
              label: role.name,
              value: role.name,
            }))}
            isMulti
          />
        </VStack>
      </Form>
    </Formik>
  );
};
