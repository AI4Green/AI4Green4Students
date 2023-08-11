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
  // TODO: get project groups from api

  const initialValues = () => {
    return user && manageRoles
      ? {
          email: user?.email,
          roles: user?.roles.map((role) => role.toUpperCase()),
          projectGroups: [], // TODO: get user project group
        }
      : { email: "", roles: [], projectGroups: [] };
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
      projectGroups: array().when("roles", {
        is: (roles) =>
          roles.some((role) => role.toUpperCase().includes("STUDENT")),
        then: array()
          .min(1, "Please select a project group")
          // TODO: only allow to select valid project group
          .required("Project group required"),
      }),
    });

  return (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={initialValues()}
      onSubmit={handleSubmit}
      validationSchema={validationSchema()}
    >
      {({ values }) => (
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
              label="Role"
              placeholder="Select a role"
              name="roles"
              options={roles.map((role) => ({
                label: role.name,
                value: role.name,
              }))}
            />
            {values.roles.some((role) =>
              role.toUpperCase().includes("STUDENT")
            ) && (
              <FormikMultiSelect
                label="Project group"
                placeholder="Select a project group"
                name="projectGroups"
                options={[]} // TODO: add project group retrieved from api as options
              />
            )}
          </VStack>
        </Form>
      )}
    </Formik>
  );
};
