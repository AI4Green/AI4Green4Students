import { Alert, AlertIcon, VStack } from "@chakra-ui/react";
import { Form, Formik } from "formik";
import { FormikInput } from "components/forms/FormikInput";
import { BasicModal } from "components/BasicModal";

// Modal for displaying link (for e.g. activation link or password reset link) depending on backend config
export const ModalDisplayLink = ({
  displayLink, // could be activation link, password reset link
  isLoading,
  isModalOpen,
  onModalClose,
  actionSelected,
}) => {
  return (
    <BasicModal
      body={
        <Formik
          enableReinitialize
          initialValues={{
            [actionSelected.name]: displayLink,
          }}
        >
          <Form noValidate>
            <VStack align="stretch" spacing={4}>
              <FormikInput
                label={`${actionSelected.title} Link`}
                name={actionSelected.name}
                type="readOnly"
              />
              <Alert status="info">
                <AlertIcon />
                Please copy the {actionSelected.title} link and pass it to the
                user to complete the {actionSelected.title} process.
              </Alert>
            </VStack>
          </Form>
        </Formik>
      }
      title={actionSelected.title}
      actionBtnCaption="Ok"
      actionBtnColorScheme="blue"
      isLoading={isLoading}
      onAction={onModalClose}
      isOpen={isModalOpen}
      onClose={onModalClose}
      closeOnOverlayClick={!!isLoading}
      cancelBtnEnable={!!isLoading}
    />
  );
};
