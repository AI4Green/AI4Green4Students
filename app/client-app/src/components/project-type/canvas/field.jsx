import { dropTargetForElements } from "@atlaskit/pragmatic-drag-and-drop/element/adapter";
import { Box, Divider, HStack, Text, useToast, VStack } from "@chakra-ui/react";
import { useSectionFields } from "api";
import { Badge } from "components/core/Badge";
import {
  INPUT_TYPES_MAP,
  InputTypePalette,
} from "components/project-type/canvas/field/input-type-palette";
import { INPUT_TYPES_MAP as FIELD_TYPES_MAP } from "components/section-field";
import { TOAST_DEFAULTS } from "constants";
import { Form, Formik } from "formik";
import { useCallback, useEffect, useRef, useState } from "react";
import { useTranslation } from "react-i18next";
import { useSearchParams } from "react-router-dom";
import { array, object, string } from "yup";

import { FormActions } from "./field/action";
import { FieldManager } from "./field/manager";

export const Field = ({ section }) => {
  const [searchParams] = useSearchParams();

  const isEditing =
    searchParams.get("action") === "edit" &&
    searchParams.get("type") === "section-fields";

  const [feedback, setFeedback] = useState();
  const [isLoading, setIsLoading] = useState(false);
  const [isDragOver, setIsDragOver] = useState(false);

  const [fields, setFields] = useState([]);

  const { data, mutate } = useSectionFields(section.id);

  useEffect(() => {
    if (data) {
      setFields(data);
    }
  }, [data]);

  const { t } = useTranslation();
  const toast = useToast();
  const dropRef = useRef(null);

  const handleDrop = useCallback(
    ({ source }) => {
      setIsDragOver(false);
      if (!isEditing) {
        toast({
          ...TOAST_DEFAULTS,
          title: "Please enable edit mode first",
          status: "warning",
        });
        return;
      }

      const { inputType } = source.data;

      if (!inputType || !inputType.name) {
        toast({
          ...TOAST_DEFAULTS,
          title: "Invalid input type",
          status: "error",
        });
        return;
      }

      const newField = {
        id: `temp-${Date.now()}`,
        name: `${inputType.name} ${fields.length + 1}`,
        mandatory: false,
        hidden: false,
        inputType,
        sortOrder: fields.length + 1,
      };

      setFields([...fields, newField]);

      toast({
        ...TOAST_DEFAULTS,
        title: `Added ${inputType.name}`,
        status: "success",
      });
    },
    [fields, toast, isEditing]
  );

  useEffect(() => {
    if (!dropRef.current) return;

    const cleanup = dropTargetForElements({
      element: dropRef.current,
      canDrop: ({ source }) => source.data.type === DRAG_TYPES.INPUT_TYPE,
      onDragEnter: () => isEditing && setIsDragOver(true),
      onDragLeave: () => setIsDragOver(false),
      onDrop: handleDrop,
    });

    return cleanup;
  }, [isEditing, handleDrop]);

  const initialValues = fields.reduce((acc, field) => {
    acc[field.id] = INPUT_TYPES_MAP[field.inputType.name].defaultResponse;
    return acc;
  }, {});

  return (
    <HStack align="start" w="full">
      {isEditing && (
        <InputTypePalette
          onAdd={(inputType) => handleDrop({ source: { data: { inputType } } })}
        />
      )}
      <VStack
        ref={dropRef}
        w="full"
        p={4}
        align="stretch"
        spacing={4}
        borderWidth={1}
        borderRadius={4}
        borderColor={isDragOver ? "green.300" : "gray.200"}
        transition="all 0.2s"
        minH="400px"
        bg={isDragOver ? "green.50" : "white"}
      >
        <HStack justify="space-between" w="full">
          <Badge label="Section fields" colorScheme="orange" />
          <FormActions
            fields={fields}
            setFields={setFields}
            isEditing={isEditing}
            isLoading={isLoading}
            data={data}
            mutate={mutate}
          />
        </HStack>
        <Divider />

        {fields.length === 0 && <NoFieldsAlert />}

        {Object.keys(initialValues).length > 0 && (
          <Formik
            enableReinitialize
            initialValues={initialValues}
            validationSchema={validationSchema}
          >
            <Form>
              <VStack spacing={4} align="stretch" w="full">
                {!isEditing ? (
                  sortFields(fields).map((field) => {
                    const [, Component] =
                      Object.entries(FIELD_TYPES_MAP).find(
                        ([key]) =>
                          key.toUpperCase() ===
                          field.inputType.name.toUpperCase()
                      ) || [];

                    return (
                      Component && (
                        <HStack key={field.id} align="start">
                          {field.sortOrder != 0 && (
                            <Text
                              fontWeight="light"
                              fontSize="xxs"
                              color="gray.500"
                            >
                              {field.sortOrder}.
                            </Text>
                          )}
                          <VStack spacing={2} align="start" w="full">
                            <Component field={field} isDisabled />
                            <Divider />
                          </VStack>
                        </HStack>
                      )
                    );
                  })
                ) : (
                  <FieldManager fields={fields} setFields={setFields} />
                )}
              </VStack>
            </Form>
          </Formik>
        )}
      </VStack>
    </HStack>
  );
};

const NoFieldsAlert = () => (
  <VStack spacing={4} py={8} align="center" w="full">
    <Box p={6} borderWidth={1} borderStyle="dashed" textAlign="center">
      <Text color="gray.500" fontSize="sm" mb={2}>
        No fields added yet
      </Text>
      <Text color="gray.400" fontSize="xs">
        Enable edit mode to add fields. Drag input types from the palette to add
        them
      </Text>
    </Box>
  </VStack>
);

const sortFields = (fields) => {
  const fieldMap = new Map(fields.map((f) => [f.id, f]));
  const parentToChildren = new Map();
  const childIds = new Set();

  for (const field of fields) {
    const { triggerField } = field;
    if (triggerField) {
      const child = fieldMap.get(triggerField.id);
      if (child) {
        if (!parentToChildren.has(field.id)) {
          parentToChildren.set(field.id, []);
        }
        parentToChildren.get(field.id).push(child);
        childIds.add(child.id);
      }
    }
  }

  const topLevelFields = fields
    .filter((f) => !childIds.has(f.id))
    .sort((a, b) => a.sortOrder - b.sortOrder);

  const result = [];

  const addFieldAndChildren = (field) => {
    result.push(field);
    if (parentToChildren.has(field.id)) {
      const children = parentToChildren.get(field.id);
      children
        .sort((a, b) => a.sortOrder - b.sortOrder)
        .forEach(addFieldAndChildren);
    }
  };

  topLevelFields.forEach(addFieldAndChildren);

  return result;
};

const validationSchema = object({
  fields: array().of(
    object().shape({
      id: string().required(),
      name: string().required("Field name is required"),
      type: string().required(),
      order: string().required(),
    })
  ),
});

export const DRAG_TYPES = {
  INPUT_TYPE: "input-type",
  FIELD_ITEM: "field-item",
};
