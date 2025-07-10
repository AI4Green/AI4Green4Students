import {
  Box,
  Radio as ChakraRadio,
  Checkbox,
  CheckboxGroup,
  HStack,
  RadioGroup,
  Stack,
  Text,
  useTheme,
} from "@chakra-ui/react";
import { defaultRadioOptions } from "components/core/forms";
import { isFieldTriggered } from "components/section-field";
import { INPUT_TYPES } from "constants";
import { useMemo } from "react";
import { ChemicalDisposalTable } from "./chemical-disposal";
import { ReactionSchemeTable } from "./reaction-scheme";
import ReactQuill from "react-quill";

export const FieldResponse = ({
  field,
  sectionId,
  recordId,
  ignoreFieldName,
}) => {
  const theme = useTheme();
  const ignoreFieldTypes = [INPUT_TYPES.Header, INPUT_TYPES.Content];
  const fieldName =
    ignoreFieldName || field.fieldType === INPUT_TYPES.ReactionScheme
      ? null
      : field.name;

  const { fieldResponse, selectFieldOptions, fieldType } = field;

  // Each field type has a different way of rendering the response.
  // TODO: This can be extended to include more field types. For now, it should covers plan section fields or most common field types.
  const inputTypes = {
    [INPUT_TYPES.ReactionScheme]: () => (
      <ReactionSchemeTable
        fieldResponse={fieldResponse}
        sectionId={sectionId}
        recordId={recordId}
      />
    ),

    [INPUT_TYPES.TextFieldType]: () => (
      <Text color="gray.500">{fieldResponse || NO_RESPONSE}</Text>
    ),
    [INPUT_TYPES.Description]: () => (
      <Text
        color="gray.500"
        borderWidth={1}
        minH={20}
        p={2}
        w="full"
        borderRadius={4}
      >
        {fieldResponse || NO_RESPONSE}
      </Text>
    ),
    [INPUT_TYPES.FormattedTextInput]: () => (
      <Box
        w="full"
        borderRadius="4"
        borderWidth={1}
        sx={{
          ".ql-editor": {
            maxHeight: "400px",
            fontFamily: theme.fonts.body,
            fontSize: "xs",
            color: "gray.500",
          },
        }}
      >
        <ReactQuill
          value={fieldResponse || NO_RESPONSE}
          readOnly
          theme="bubble"
          modules={{ toolbar: false }}
        />
      </Box>
    ),
    [INPUT_TYPES.Radio]: () => {
      const options = selectFieldOptions || defaultRadioOptions;
      return (
        <RadioGroup defaultValue={fieldResponse[0]?.name} isDisabled>
          <HStack spacing={4}>
            {options?.map((option, index) => (
              <ChakraRadio key={index} value={option.name}>
                <Text fontSize="xs">{option.name}</Text>
              </ChakraRadio>
            ))}
          </HStack>
        </RadioGroup>
      );
    },
    [INPUT_TYPES.Multiple]: () => {
      const defaultValues = Array.isArray(fieldResponse)
        ? fieldResponse.map((value) => value.name)
        : [];
      return (
        <CheckboxGroup defaultValue={defaultValues} isDisabled>
          <Stack direction="row" wrap="wrap" gap={4}>
            {selectFieldOptions?.map((option, index) => (
              <Checkbox key={index} value={option.name}>
                <Text fontSize="xs">{option.name}</Text>
              </Checkbox>
            ))}
          </Stack>
        </CheckboxGroup>
      );
    },
    [INPUT_TYPES.ChemicalDisposalTable]: () => (
      <ChemicalDisposalTable
        fieldResponse={Array.isArray(fieldResponse) ? fieldResponse : []}
      />
    ),
  };

  if (ignoreFieldTypes.includes(fieldType)) return null;

  return inputTypes[fieldType] ? (
    <Box key={field.id} px={4} py={2} fontSize="xs" w="full">
      {fieldName && (
        <Text fontWeight="normal" mb={1} fontSize="sm">
          {fieldName}
        </Text>
      )}
      {inputTypes[fieldType]()}
    </Box>
  ) : null;
};

// Helper component to render trigger field response.
export const TriggerFieldResponse = ({
  field: {
    id,
    fieldType,
    trigger: { value: triggerValue, target: triggerTargetId },
  },
  fieldValues,
  sectionFields,
  sectionId,
  recordId,
  ignoreFieldName,
}) => {
  const isFieldTriggeringChild = isFieldTriggered(
    fieldType,
    triggerValue,
    fieldValues[id]
  );

  const triggerTargetField = useMemo(
    () => sectionFields.find((x) => x.id === triggerTargetId),
    [sectionFields, triggerTargetId]
  );

  if (isFieldTriggeringChild && triggerTargetField) {
    return (
      <>
        <FieldResponse
          field={triggerTargetField}
          sectionId={sectionId}
          recordId={recordId}
          ignoreFieldName={ignoreFieldName}
        />
        {triggerTargetField?.trigger && (
          <TriggerFieldResponse
            field={triggerTargetField.trigger}
            recordId={recordId}
          />
        )}
      </>
    );
  }
  return null;
};

export const NO_RESPONSE = "No input provided.";
