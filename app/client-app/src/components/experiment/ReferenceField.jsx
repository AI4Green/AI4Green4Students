import { Field, Formik, useField, Form } from "formik";
import {
  Box,
  Stack,
  VStack,
  Text,
  Button,
  Flex,
  IconButton,
  useDisclosure,
} from "@chakra-ui/react";
import { useRef } from "react";
import { FaBook, FaRegTimesCircle } from "react-icons/fa";
import { BasicModal } from "components/BasicModal";
import { object, string } from "yup";
import { DescriptionTextArea } from "components/forms/DescriptionTextArea";
import { DragDropContext, Droppable, Draggable } from "react-beautiful-dnd";

const validationSchema = () =>
  object().shape({
    referenceInput: string().required("Reference required"),
  });

const Modal = ({ initialContent = "", onSubmit, modalFormRef }) => (
  <Formik
    enableReinitialize
    innerRef={modalFormRef}
    initialValues={{ referenceInput: initialContent }}
    onSubmit={onSubmit}
    validationSchema={validationSchema()}
  >
    <Form noValidate>
      <DescriptionTextArea
        name="referenceInput"
        placeholder="Enter reference here"
      />
    </Form>
  </Formik>
);

const ReferenceItem = ({ item, handleDelete, handleEdit, modalFormRef }) => {
  const UpdateReferenceState = useDisclosure();
  return (
    <Box borderBottomWidth={1} px={2} my={1} py={2}>
      <Flex color={"blackAlpha.700"} align="center">
        <Text fontWeight="semibold" mr={2} fontSize="xs">
          {item.order}.
        </Text>
        <Text fontSize="sm">{item.content}</Text>
        <IconButton
          aria-label="Edit reference item"
          variant="ghost"
          colorScheme="blue"
          icon={<FaBook />}
          ml="auto"
          size="sm"
          onClick={UpdateReferenceState.onOpen}
        />
        <IconButton
          aria-label="Remove reference item"
          variant="ghost"
          colorScheme="red"
          icon={<FaRegTimesCircle />}
          size="sm"
          onClick={() => handleDelete(item.id)}
        />
        {UpdateReferenceState.isOpen && (
          <BasicModal
            body={
              <Modal
                initialContent={item.content}
                modalFormRef={modalFormRef}
                onSubmit={(values) => handleEdit(values, item)}
              />
            }
            title="📖 Edit reference"
            onAction={() => {
              modalFormRef.current.handleSubmit();
              UpdateReferenceState.onClose();
            }}
            isOpen={UpdateReferenceState.isOpen}
            onClose={UpdateReferenceState.onClose}
          />
        )}
      </Flex>
    </Box>
  );
};

export const ReferenceField = ({ label, name }) => {
  const AddReferenceState = useDisclosure();
  const [fieldReference, metaReference, helpersReference] = useField(name);
  const modalFormRef = useRef();

  // handles adding a new reference item
  const handleAddReference = (values) => {
    const idGenerator = () =>
      `temp-${Math.random().toString(36).substring(2, 11)}`;

    let newId = idGenerator();
    while (
      fieldReference.value.some(
        (ref) => ((newId = idGenerator()), ref.id === newId) // generate new id if it already exists
      )
    );

    helpersReference.setValue([
      ...fieldReference.value,
      {
        id: newId,
        order: fieldReference.value.length + 1,
        content: values.referenceInput,
      },
    ]);
    AddReferenceState.onClose();
  };

  // handles editing a reference item
  const handleEditReference = (values, editItem) => {
    const itemIndex = fieldReference.value.findIndex(
      (r) => r.id === editItem.id
    );
    if (itemIndex !== -1) {
      // if item exists
      const updatedRef = {
        ...editItem,
        content: values.referenceInput.trim(),
      };
      const newRefs = [...fieldReference.value];
      newRefs.splice(itemIndex, 1, updatedRef);
      helpersReference.setValue(newRefs);
    }
  };

  const handleDeleteReference = (id) => {
    // filter out the item with the given id
    const updatedReferences = fieldReference.value
      .filter((ref) => ref.id !== id)
      .map((ref, index) => ({ ...ref, order: index + 1 }));

    helpersReference.setValue(updatedReferences);
  };

  const handleDragEnd = (result) => {
    if (!result.destination) return;

    const items = Array.from(fieldReference.value);
    const [reorderedItem] = items.splice(result.source.index, 1);
    items.splice(result.destination.index, 0, reorderedItem);

    const reorderedItems = items.map((item, index) => ({
      ...item,
      order: index + 1,
    }));

    helpersReference.setValue(reorderedItems);
  };

  return (
    <Field name={name}>
      {({ field }) => (
        <Stack align="start" w="100%" spacing={2} py={2}>
          <Text as="b">{label}</Text>
          {field.value?.length > 0 && (
            <DragDropContext onDragEnd={handleDragEnd}>
              <VStack align="start" w="100%" spacing={2} pb={2}>
                <Droppable droppableId="droppable">
                  {(provided) => (
                    <Box
                      ref={provided.innerRef}
                      {...provided.droppableProps}
                      w="100%"
                    >
                      {field.value
                        ?.sort((a, b) => a.order - b.order)
                        .map((item, index) => (
                          <Draggable
                            key={item.id}
                            draggableId={item.id.toString()}
                            index={index}
                          >
                            {(provided) => (
                              <Box
                                ref={provided.innerRef}
                                {...provided.draggableProps}
                                {...provided.dragHandleProps}
                              >
                                <ReferenceItem
                                  key={item.index}
                                  item={item}
                                  handleDelete={handleDeleteReference}
                                  handleEdit={handleEditReference}
                                  modalFormRef={modalFormRef}
                                />
                              </Box>
                            )}
                          </Draggable>
                        ))}
                      {provided.placeholder}
                    </Box>
                  )}
                </Droppable>
              </VStack>
            </DragDropContext>
          )}

          <Button
            colorScheme="blue"
            variant="outline"
            size="sm"
            leftIcon={<FaBook />}
            onClick={AddReferenceState.onOpen}
          >
            Add new reference
          </Button>
          {AddReferenceState.isOpen && (
            <BasicModal
              body={
                <Modal
                  onSubmit={handleAddReference}
                  modalFormRef={modalFormRef}
                />
              }
              title="📖 Add a new reference"
              onAction={() => modalFormRef.current.handleSubmit()}
              isOpen={AddReferenceState.isOpen}
              onClose={AddReferenceState.onClose}
            />
          )}
        </Stack>
      )}
    </Field>
  );
};
