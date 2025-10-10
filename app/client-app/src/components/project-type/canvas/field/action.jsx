import {
  Box,
  HStack,
  Icon,
  IconButton,
  Text,
  useDisclosure,
  VStack,
} from "@chakra-ui/react";
import { FormikInput, Switch } from "components/core/forms";
import { Modal } from "components/core/modal";
import { BASE_PATH } from "components/project-type/canvas/area";
import { INPUT_TYPES } from "constants";
import { Form, Formik } from "formik";
import { useRef, useState } from "react";
import { FaEllipsisH, FaPencilAlt, FaSave, FaTrash } from "react-icons/fa";
import { TbCancel } from "react-icons/tb";
import { useNavigate, useParams } from "react-router-dom";
import CreatableSelect from "react-select/creatable";
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
  const selectFieldOptions = field.selectFieldOptions?.map((option) => ({
    id: option.id,
    label: option.name,
    value: option.name,
  }));
  const [selectedOptions, setSelectedOptions] = useState(selectFieldOptions);

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
        values.selectFieldOptions = selectedOptions?.map((option) => ({
          id: option.id || `new-${option.value}`,
          name: option.value,
        }));
        handleEditSubmit(values);
        resetForm();
      }}
      validationSchema={validationSchema}
    >
      <Form noValidate>
        <VStack spacing={4} align="stretch">
          <FormikInput name="name" label="Name" isRequired />
          <Switch name="mandatory" label="Mandatory" colorScheme="orange" />
          <Switch name="hidden" label="Hidden" colorScheme="blue" />
          {field.inputType.name === INPUT_TYPES.Multiple ||
          field.inputType.name === INPUT_TYPES.Radio ? (
            <HStack>
              <Text fontSize="sm">Options</Text>
              <Box flex={1}>
                <CreatableSelect
                  isCreatable
                  isMulti
                  options={selectFieldOptions}
                  value={selectedOptions}
                  onChange={(value) => {
                    setSelectedOptions(value);
                  }}
                  placeholder="Type to add new option"
                />
              </Box>
            </HStack>
          ) : null}
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
  isEditing,
  isLoading,
  handleSubmit,
  handleCancel,
}) => {
  const { projectTypeId, sectionTypeId, sectionId } = useParams();
  const navigate = useNavigate();

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
