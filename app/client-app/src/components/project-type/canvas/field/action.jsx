import {
  HStack,
  Icon,
  IconButton,
  useDisclosure,
  VStack,
} from "@chakra-ui/react";
import { FormikInput, Switch } from "components/core/forms";
import { Modal } from "components/core/modal";
import { BASE_PATH } from "components/project-type/canvas/area";
import { INPUT_TYPES_MAP } from "components/project-type/canvas/field/input-type-palette";
import { Form, Formik } from "formik";
import { useRef } from "react";
import { FaEllipsisH, FaPencilAlt, FaSave, FaTrash } from "react-icons/fa";
import { TbCancel } from "react-icons/tb";
import { useNavigate, useParams } from "react-router-dom";
import { object, string } from "yup";

export const FieldActions = ({ field, fields, setFields }) => {
  const { isOpen, onOpen, onClose } = useDisclosure();

  const handleDelete = () => {
    setFields(fields.filter((x) => x.id !== field.id));
  };

  const handleEditSubmit = (values) => {
    const model = fields.map((x) =>
      x.id === field.id
        ? {
            ...x,
            ...values,
            sortOrder: values.hidden ? 0 : x.sortOrder,
          }
        : x
    );
    setFields(model);
    onClose();
  };

  return (
    <HStack justify="end">
      <IconButton
        size="xs"
        icon={<FaTrash />}
        colorScheme="red"
        variant="ghost"
        onClick={handleDelete}
        aria-label="Delete field"
      />
      <IconButton
        size="xs"
        icon={<Icon as={FaEllipsisH} fontSize="lg" color="gray.500" />}
        variant="ghost"
        onClick={onOpen}
        aria-label="Edit field"
      />
      {isOpen && (
        <FieldEditModal
          isOpen={isOpen}
          onClose={onClose}
          field={field}
          handleEditSubmit={handleEditSubmit}
        />
      )}
    </HStack>
  );
};

const FieldEditModal = ({ isOpen, onClose, field, handleEditSubmit }) => {
  const initialValues = {
    name: field.name,
    mandatory: field.mandatory,
    hidden: field.hidden,
  };

  const validationSchema = object({
    name: string().required("Field name is required."),
  });

  const formRef = useRef();
  const modalBody = (
    <Formik
      enableReinitialize
      innerRef={formRef}
      initialValues={initialValues}
      onSubmit={(values, { resetForm }) => {
        handleEditSubmit(values);
        resetForm();
      }}
      validationSchema={validationSchema}
    >
      <Form noValidate>
        <VStack spacing={4}>
          <FormikInput name="name" label="Name" isRequired />
          <Switch name="mandatory" label="Mandatory" colorScheme="orange" />
          <Switch name="hidden" label="Hidden" colorScheme="blue" />
        </VStack>
      </Form>
    </Formik>
  );

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title="Edit"
      body={modalBody}
      onAction={() => formRef.current.handleSubmit()}
      actionBtnCaption="Update"
    />
  );
};

export const FormActions = ({
  fields,
  setFields,
  isEditing,
  isLoading,
  data,
  mutate,
}) => {
  const { projectTypeId, sectionTypeId, sectionId } = useParams();
  const navigate = useNavigate();

  const handleSubmit = async () => {
    const createdModel = fields.map((field) => ({
      id: typeof field.id === "number" ? field.id : null,
      name: field.name,
      inputType: field.inputType.id,
      mandatory: field.mandatory,
      hidden: field.hidden,
      sortOrder: field.sortOrder,
      defaultValue: INPUT_TYPES_MAP[field.inputType.name].defaultResponse,
    }));
  };

  const handleCancel = () => {
    setFields(data);
    navigate(
      `${BASE_PATH}/${projectTypeId}/section-types/${sectionTypeId}/sections/${sectionId}`,
      { replace: true }
    );
  };

  if (isEditing) {
    return (
      <HStack spacing={4}>
        <IconButton
          size="sm"
          icon={<FaSave />}
          aria-label="Save"
          variant="ghost"
          colorScheme="green"
          onClick={handleSubmit}
          fontSize="lg"
          isLoading={isLoading}
        />
        <IconButton
          size="sm"
          fontSize="lg"
          icon={<TbCancel />}
          aria-label="Cancel"
          variant="ghost"
          colorScheme="yellow"
          onClick={handleCancel}
          isLoading={isLoading}
        />
      </HStack>
    );
  }

  return (
    <IconButton
      size="xs"
      icon={<FaPencilAlt />}
      aria-label="Edit"
      variant="ghost"
      colorScheme="blue"
      onClick={() => {
        navigate(
          `${BASE_PATH}/${projectTypeId}/section-types/${sectionTypeId}/sections/${sectionId}?action=edit&type=section-fields`,
          {
            replace: true,
          }
        );
      }}
    />
  );
};
