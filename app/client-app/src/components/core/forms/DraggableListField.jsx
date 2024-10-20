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
import { FaBook, FaEdit, FaRegTimesCircle } from "react-icons/fa";
import { Modal } from "components/Modal";
import { object, string } from "yup";
import { TextAreaField } from ".";
import { DragDropContext, Droppable, Draggable } from "react-beautiful-dnd";

const validationSchema = () =>
  object().shape({
    itemInput: string().required("Input required"),
  });

const ModalBody = ({ initialContent = "", onSubmit, modalFormRef }) => (
  <Formik
    enableReinitialize
    innerRef={modalFormRef}
    initialValues={{ itemInput: initialContent }}
    onSubmit={onSubmit}
    validationSchema={validationSchema()}
  >
    <Form noValidate>
      <TextAreaField name="itemInput" placeholder="Enter here" />
    </Form>
  </Formik>
);

const Item = ({ item, handleDelete, handleEdit, modalFormRef, isDisabled }) => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  return (
    <Box borderBottomWidth={1} px={2} my={1} py={2}>
      <Flex color="blackAlpha.800" align="center">
        <Text fontWeight="semibold" mr={2} fontSize="xs">
          {item.order}.
        </Text>
        <Text fontSize="sm">{item.content}</Text>
        <IconButton
          aria-label="Edit item"
          variant="ghost"
          colorScheme="gray"
          icon={<FaEdit />}
          ml="auto"
          size="sm"
          onClick={onOpen}
          disabled={isDisabled}
        />
        <IconButton
          aria-label="Remove item"
          variant="ghost"
          colorScheme="red"
          icon={<FaRegTimesCircle />}
          size="sm"
          onClick={() => handleDelete(item.id)}
          disabled={isDisabled}
        />
        {isOpen && (
          <Modal
            body={
              <ModalBody
                initialContent={item.content}
                modalFormRef={modalFormRef}
                onSubmit={(values) => handleEdit(values, item)}
              />
            }
            title="📖 Edit"
            onAction={() => {
              modalFormRef.current.handleSubmit();
              onClose();
            }}
            isOpen={isOpen}
            onClose={onClose}
          />
        )}
      </Flex>
    </Box>
  );
};

export const DraggableListField = ({ label, name, isDisabled }) => {
  const AddItemState = useDisclosure();
  const [fieldItem, metaItem, helpersItem] = useField(name);
  const modalFormRef = useRef();

  // handles adding a new item
  const handleAddItem = (values) => {
    const idGenerator = () =>
      `temp-${Math.random().toString(36).substring(2, 11)}`;

    let newId = idGenerator();
    while (
      fieldItem.value.some(
        (ref) => ((newId = idGenerator()), ref.id === newId) // generate new id if it already exists
      )
    );

    helpersItem.setValue([
      ...fieldItem.value,
      {
        id: newId,
        order: fieldItem.value.length + 1,
        content: values.itemInput.trim(), // value of the input field in the modal, whose name is 'itemInput'
      },
    ]);
    AddItemState.onClose();
  };

  // handles editing an item
  const handleEditItem = (values, editItem) => {
    const itemIndex = fieldItem.value.findIndex((r) => r.id === editItem.id);
    if (itemIndex !== -1) {
      // if item exists
      const updatedRef = {
        ...editItem,
        content: values.itemInput.trim(),
      };
      const newRefs = [...fieldItem.value];
      newRefs.splice(itemIndex, 1, updatedRef);
      helpersItem.setValue(newRefs);
    }
  };

  const handleDeleteItem = (id) => {
    // filter out the item with the given id
    const updatedItems = fieldItem.value
      .filter((ref) => ref.id !== id)
      .map((ref, index) => ({ ...ref, order: index + 1 }));

    helpersItem.setValue(updatedItems);
  };

  const handleDragEnd = (result) => {
    if (!result.destination) return;

    const items = Array.from(fieldItem.value);
    const [reorderedItem] = items.splice(result.source.index, 1);
    items.splice(result.destination.index, 0, reorderedItem);

    const reorderedItems = items.map((item, index) => ({
      ...item,
      order: index + 1,
    }));

    helpersItem.setValue(reorderedItems);
  };

  return (
    <Field name={name}>
      {({ field }) => (
        <Stack align="start" w="100%" spacing={4} py={2}>
          <Text as="b">{label}</Text>
          {field.value?.length > 0 && (
            <DragDropContext onDragEnd={handleDragEnd}>
              <VStack
                align="start"
                w="100%"
                spacing={2}
                p={4}
                borderRadius={4}
                borderWidth={1}
              >
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
                            isDragDisabled={isDisabled}
                          >
                            {(provided) => (
                              <Box
                                ref={provided.innerRef}
                                {...provided.draggableProps}
                                {...provided.dragHandleProps}
                              >
                                <Item
                                  key={item.index}
                                  item={item}
                                  handleDelete={handleDeleteItem}
                                  handleEdit={handleEditItem}
                                  modalFormRef={modalFormRef}
                                  isDisabled={isDisabled}
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
            onClick={AddItemState.onOpen}
            disabled={isDisabled}
          >
            Add new item
          </Button>
          {AddItemState.isOpen && (
            <Modal
              body={
                <ModalBody
                  onSubmit={handleAddItem}
                  modalFormRef={modalFormRef}
                />
              }
              title="📖 Add a new item"
              onAction={() => modalFormRef.current.handleSubmit()}
              isOpen={AddItemState.isOpen}
              onClose={AddItemState.onClose}
            />
          )}
        </Stack>
      )}
    </Field>
  );
};
