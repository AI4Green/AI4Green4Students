import {
  draggable,
  dropTargetForElements,
  monitorForElements,
} from "@atlaskit/pragmatic-drag-and-drop/element/adapter";
import { reorder } from "@atlaskit/pragmatic-drag-and-drop/reorder";
import { HStack, Icon, Text, VStack } from "@chakra-ui/react";
import { Badge } from "components/core/Badge";
import { INPUT_TYPES_MAP as FIELD_TYPES_MAP } from "components/section-field";
import { useCallback, useEffect, useRef, useState } from "react";
import { FaCheckCircle, FaEyeSlash, FaGripVertical } from "react-icons/fa";

import { FieldActions } from "./action";
import { INPUT_TYPES_MAP } from "./input-type-palette";

export const FieldManager = ({ fields, setFields }) => {
  const handleReorder = useCallback(
    (from, to) => {
      if (from === to) return;
      const reordered = reorder({
        list: [...fields].sort((a, b) => a.order - b.order),
        startIndex: from,
        finishIndex: to,
      }).map((item, idx) => ({ ...item, order: idx + 1 }));
      setFields(reordered);
    },
    [fields, setFields]
  );

  useEffect(() => {
    return monitorForElements({
      onDrop: ({ source, location }) => {
        const dest = location.current.dropTargets[0];
        if (!dest) return;
        const { index: from } = source.data;
        const { index: to } = dest.data;
        if (typeof from === "number" && typeof to === "number") {
          handleReorder(from, to);
        }
      },
    });
  }, [handleReorder]);

  return (
    <>
      {fields.map((field, index) => (
        <FieldItem
          key={field.id}
          field={field}
          index={index}
          fields={fields}
          setFields={setFields}
        />
      ))}
    </>
  );
};

const FieldItem = ({ field, index, fields, setFields }) => {
  const [isDragging, setIsDragging] = useState(false);
  const [isOver, setIsOver] = useState(false);
  const dragRef = useRef(null);
  const dropRef = useRef(null);

  useEffect(() => {
    const cleanupDrag = draggable({
      element: dragRef.current,
      getInitialData: () => ({ itemId: field.id, index, type: "field-item" }),
      onDragStart: () => setIsDragging(true),
      onDrop: () => setIsDragging(false),
    });

    const cleanupDrop = dropTargetForElements({
      element: dropRef.current,
      getData: () => ({ index, type: "field-item" }),
      canDrop: ({ source }) => source.data.type === "field-item",
      onDragEnter: () => setIsOver(true),
      onDragLeave: () => setIsOver(false),
      onDrop: () => setIsOver(false),
    });

    return () => {
      cleanupDrag();
      cleanupDrop();
    };
  }, [index, field.id]);

  const [, Component] =
    Object.entries(FIELD_TYPES_MAP).find(
      ([key]) => key.toUpperCase() === field.inputType.name.toUpperCase()
    ) || [];

  return (
    <VStack
      ref={dropRef}
      p={3}
      borderWidth={1}
      borderRadius="md"
      borderColor={isOver ? "blue.300" : "gray.200"}
      bg={isOver ? "blue.50" : isDragging ? "gray.100" : "white"}
      align="stretch"
      transition="all 0.2s"
      w="full"
    >
      <HStack justify="space-between">
        <Text fontWeight="light" fontSize="xs" color="gray.500">
          {field.sortOrder}.
        </Text>
        <Info field={field} />
      </HStack>
      <HStack>
        <VStack ref={dragRef} cursor="grab" _active={{ cursor: "grabbing" }}>
          <Icon as={FaGripVertical} color="gray.400" fontSize="xl" />
        </VStack>
        <Component field={field} isDisabled />
      </HStack>
      <FieldActions field={field} fields={fields} setFields={setFields} />
    </VStack>
  );
};

const Info = ({ field }) => (
  <HStack>
    {field.mandatory && (
      <Badge
        label="Mandatory"
        leftIcon={FaCheckCircle}
        colorScheme="orange"
        variant="outline"
        fontSize="xxs"
      />
    )}
    {field.hidden && (
      <Badge
        label="Hidden"
        leftIcon={FaEyeSlash}
        colorScheme="blue"
        variant="outline"
        fontSize="xxs"
      />
    )}

    <Badge
      label={INPUT_TYPES_MAP[field.inputType.name].label}
      leftIcon={INPUT_TYPES_MAP[field.inputType.name].icon}
      colorScheme="gray"
      fontSize="xxs"
    />
  </HStack>
);
