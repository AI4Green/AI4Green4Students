import {
  Alert,
  AlertIcon,
  FormLabel,
  HStack,
  Switch,
  useToast,
  VStack,
} from "@chakra-ui/react";
import { FormikInput } from "components/core/forms";
import { Modal, useModalState } from "components/core/modal";
import { GLOBAL_PARAMETERS } from "constants";
import { useBackendApi } from "contexts";
import { Field, Form, Formik } from "formik";
import { useEffect, useRef } from "react";
import { useTranslation } from "react-i18next";
import { useLocation, useNavigate, useSearchParams } from "react-router-dom";

import { validation } from "./validation";

export const CreateOrEditModal = ({ list, mutate }) => {
  const [searchParams] = useSearchParams();
  const id = searchParams.get("id");
  const isEditAction = searchParams.get("action") === "edit";

  const navigate = useNavigate();
  const location = useLocation();

  const { registrationRules: action } = useBackendApi();

  const { t } = useTranslation();
  const toast = useToast();

  const formRef = useRef();

  const {
    isModalOpen,
    setIsModalOpen,
    isLoading,
    setIsLoading,
    feedback,
    setFeedback,
    handleReset,
  } = useModalState(location, navigate, formRef);

  const registrationRule = isEditAction
    ? list?.find((x) => x.id === Number(id))
    : null;

  useEffect(() => {
    setIsModalOpen(true);
  }, [id, isEditAction, setIsModalOpen]);

  const initialValues = () => {
    return registrationRule
      ? {
          value: registrationRule?.value,
          isBlocked: registrationRule?.isBlocked,
        }
      : {
          value: "",
          isBlocked: true,
        };
  };

  const handleSubmit = async (values) => {
    try {
      setIsLoading(true);

      const model = {
        value: values.value,
        isBlocked: values.isBlocked,
      };

      const response = !registrationRule
        ? await action.create(model)
        : await action.edit(registrationRule.id, model);

      setIsLoading(false);

      if (response && (response.status === 204 || response.status === 200)) {
        toast({
          title: `Registration rule ${
            !registrationRule ? "created" : "updated"
          }`,
          status: "success",
          duration: GLOBAL_PARAMETERS.ToastDuration,
          isClosable: true,
          position: "top",
        });
        handleReset();
        await mutate();
      }
    } catch {
      setFeedback({
        status: "error",
        message: t("feedback.error_title"),
      });
    }
  };

  const modalBody = (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={initialValues()}
      onSubmit={handleSubmit}
      validationSchema={!registrationRule && validation(list)}
    >
      {({ values, setFieldValue }) => (
        <Form noValidate>
          <VStack align="stretch" spacing={4}>
            {feedback && (
              <Alert status={feedback.status}>
                <AlertIcon />
                {feedback.message}
              </Alert>
            )}

            {!registrationRule ? (
              <FormikInput name="value" label="Value" isRequired />
            ) : (
              <FormikInput name="value" label="Value" isDisabled />
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
    <Modal
      body={modalBody}
      title={`${!registrationRule ? "Create" : "Edit"} Registration Rule`}
      actionBtnCaption={!registrationRule ? "Create" : "Update"}
      onAction={() => formRef.current.handleSubmit()}
      actionBtnColorScheme={!registrationRule ? "green" : "blue"}
      isLoading={isLoading}
      isOpen={isModalOpen}
      onClose={handleReset}
    />
  );
};
