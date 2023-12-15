import { VStack } from "@chakra-ui/react";
import { useRef } from "react";
import { object, string } from "yup";
import { Formik, Form } from "formik";
import { BasicModal } from "components/BasicModal";
import { TextAreaField } from "components/forms/TextAreaField";

export const CreateCommentModal = ({ field, isModalOpen, onModalClose }) => {
  const formRef = useRef();

  const handleSubmit = async () => {
    // TODO: make api call to add a comment for field response
    // also set the field-response as reviewed
  };

  const validationSchema = () =>
    object().shape({
      value: string().required("Comment value is required"),
    });

  const modal = (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={{
        value: "",
      }}
      onSubmit={handleSubmit}
      validationSchema={validationSchema()}
    >
      <Form noValidate>
        <VStack align="stretch" spacing={4}>
          <TextAreaField
            name="value"
            title="Comment"
            placeholder="Enter your comment here"
            isRequired
          />
        </VStack>
      </Form>
    </Formik>
  );
  return (
    <BasicModal
      body={modal}
      title="Add Comment"
      actionBtnCaption="Comment"
      onAction={() => formRef.current.handleSubmit()}
      isOpen={isModalOpen}
      onClose={onModalClose}
    />
  );
};
