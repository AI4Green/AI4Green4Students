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
import { useRef, useEffect, useState, useCallback } from "react";
import { FaBook, FaEdit, FaRegTimesCircle } from "react-icons/fa";
import { Modal } from "components/core/Modal";
import { object, string } from "yup";
import { TextAreaField } from ".";
import {
  draggable,
  dropTargetForElements,
  monitorForElements,
} from "@atlaskit/pragmatic-drag-and-drop/element/adapter";
import { reorder } from "@atlaskit/pragmatic-drag-and-drop/reorder";

const DRAG_TYPE = "draggable-list-item";

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

const Item = ({
  item,
  handleDelete,
  handleEdit,
  modalFormRef,
  isDisabled,
  onDragStart,
  index,
}) => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const dragRef = useRef(null);
  const dropRef = useRef(null);
  const [isDragging, setIsDragging] = useState(false);
  const [isOver, setIsOver] = useState(false);

  useEffect(() => {
    const dragElement = dragRef.current;
    const dropElement = dropRef.current;

    if (!dragElement || !dropElement || isDisabled) return;

    const cleanupDrag = draggable({
      element: dragElement,
      getInitialData: () => ({
        itemId: item.id,
        index,
        type: DRAG_TYPE,
      }),
      onDragStart: () => {
        setIsDragging(true);
        onDragStart?.(item.id);
      },
      onDrop: () => setIsDragging(false),
    });

    const cleanupDrop = dropTargetForElements({
      element: dropElement,
      getData: () => ({
        index,
        type: DRAG_TYPE,
      }),
      canDrop: ({ source }) => source.data.type === DRAG_TYPE,
      onDragEnter: ({ source }) =>
        source.data.type === DRAG_TYPE && setIsOver(true),
      onDragLeave: () => setIsOver(false),
      onDrop: () => setIsOver(false),
    });

    return () => {
      cleanupDrag();
      cleanupDrop();
    };
  }, [item.id, index, isDisabled, onDragStart]);

  const handleModalSubmit = (values) => {
    handleEdit(values, item);
    onClose();
  };

  return (
    <Box
      ref={dropRef}
      w="100%"
      borderRadius="md"
      transition="all 0.2s"
      border="2px solid transparent"
      borderColor={isOver ? "blue.300" : "transparent"}
      bg={isOver ? "blue.50" : "transparent"}
    >
      <Box
        ref={dragRef}
        borderBottomWidth={1}
        px={2}
        my={1}
        py={2}
        opacity={isDragging ? 0.5 : 1}
        transition="opacity 0.2s"
        cursor={isDisabled ? "default" : "grab"}
        _active={{ cursor: isDisabled ? "default" : "grabbing" }}
      >
        <Flex color="blackAlpha.800" align="center">
          <Text fontWeight="semibold" mr={2} fontSize="xs">
            {item.order}.
          </Text>
          <Text fontSize="sm" flex="1">
            {item.content}
          </Text>
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
                  onSubmit={handleModalSubmit}
                />
              }
              title="📖 Edit"
              onAction={() => {
                modalFormRef.current.handleSubmit();
              }}
              isOpen={isOpen}
              onClose={onClose}
            />
          )}
        </Flex>
      </Box>
    </Box>
  );
};

export const DraggableListField = ({ label, name, isDisabled }) => {
  const AddItemState = useDisclosure();
  const [fieldItem, metaItem, helpersItem] = useField(name);
  const modalFormRef = useRef();
  const [draggedItemId, setDraggedItemId] = useState(null);

  // handles adding a new item
  const handleAddItem = useCallback(
    (values) => {
      const newId = generateUniqueId(fieldItem.value.map((item) => item.id));

      helpersItem.setValue([
        ...fieldItem.value,
        {
          id: newId,
          order: fieldItem.value.length + 1,
          content: values.itemInput.trim(), // value of the input field in the modal, whose name is 'itemInput'
        },
      ]);
      AddItemState.onClose();
    },
    [fieldItem.value, helpersItem, AddItemState],
  );

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

  const handleDragStart = useCallback((itemId) => {
    setDraggedItemId(itemId);
  }, []);

  const handleReorder = useCallback(
    (sourceIndex, destinationIndex) => {
      if (sourceIndex === destinationIndex) return;

      // sort items by order
      const sortedItems = [...fieldItem.value].sort(
        (a, b) => a.order - b.order,
      );

      // reorder items
      const reorderedItems = reorder({
        list: sortedItems,
        startIndex: sourceIndex,
        finishIndex: destinationIndex,
      });

      // only update the order property, preserve rest
      const updatedItems = reorderedItems.map((item, index) => ({
        ...item,
        order: index + 1,
      }));

      helpersItem.setValue(updatedItems);
      setDraggedItemId(null);
    },
    [fieldItem.value, helpersItem],
  );

  useDropMonitor(isDisabled, handleReorder);

  return (
    <Field name={name}>
      {({ field }) => (
        <Stack align="start" w="100%" spacing={4} py={2}>
          <Text as="b">{label}</Text>
          {field.value?.length > 0 && (
            <VStack
              align="start"
              w="100%"
              spacing={2}
              p={4}
              borderRadius={4}
              borderWidth={1}
              position="relative"
            >
              {field.value
                ?.sort((a, b) => a.order - b.order)
                .map((item, index) => (
                  <Item
                    key={item.id}
                    item={item}
                    index={index}
                    handleDelete={handleDeleteItem}
                    handleEdit={handleEditItem}
                    modalFormRef={modalFormRef}
                    isDisabled={isDisabled}
                    onDragStart={handleDragStart}
                  />
                ))}
            </VStack>
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

// generate unique id when adding a new item
const generateUniqueId = (existingIds) => {
  const idGenerator = () =>
    `temp-${Math.random().toString(36).substring(2, 11)}`;
  let newId = idGenerator();

  while (existingIds.some((id) => id === newId)) {
    newId = idGenerator();
  }

  return newId;
};

// validate drop
const useDropMonitor = (isDisabled, onReorder) => {
  useEffect(() => {
    if (isDisabled) return;

    return monitorForElements({
      onDrop: ({ source, location }) => {
        try {
          const destination = location.current.dropTargets[0];
          if (!destination) return;

          const sourceIndex = source.data.index;
          const destinationIndex = destination.data.index;

          if (
            typeof sourceIndex !== "number" ||
            typeof destinationIndex !== "number"
          ) {
            console.warn("indexes not numbers", {
              sourceIndex,
              destinationIndex,
            });
            return;
          }

          onReorder(sourceIndex, destinationIndex);
        } catch (error) {
          console.error("drop error", error);
        }
      },
    });
  }, [isDisabled, onReorder]);
};
