import { Heading, HStack, Text } from "@chakra-ui/react";
import { TextField } from "components/forms/TextField";
import { TextAreaField } from "components/forms/TextAreaField";
import { FileUploadField } from "components/forms/FileUploadField";
import { DraggableListField } from "components/forms/DraggableListField";
import { INPUT_TYPES } from "constants/input-types";
import { OptionsField } from "components/forms/OptionsField";
import { Feedback } from "./feedback/Feedback";
import { useMemo } from "react";
import { ReactionScheme } from "../reactionScheme/ReactionScheme";
import { ChemicalDisposableTable } from "../chemicalDisposable/ChemicalDisposableTable";
import { useIsInstructor } from "../useIsInstructor";

export const ExperimentField = ({
  field,
  fieldValues, // collection of field values, which can be accessed by using the field.id as key
  recordId, // could be planId or reportId
  sectionFields, // collection of fields in the section
}) => {
  const isInstructor = useIsInstructor();
  return (
    <>
      <Field field={field} isInstructor={isInstructor} />
      {field.trigger && (
        <TriggerField
          field={field}
          fieldValues={fieldValues}
          recordId={recordId}
          isInstructor={isInstructor}
          sectionFields={sectionFields}
        />
      )}
    </>
  );
};

const Field = ({ field, isInstructor }) => {
  switch (field.fieldType.toUpperCase()) {
    case INPUT_TYPES.Header.toUpperCase():
      return (
        <Heading size="sm" as="u">
          {field.name}
        </Heading>
      );
    case INPUT_TYPES.Content.toUpperCase():
      return (
        <HStack>
          <Heading size="xs" fontWeight="semibold">
            {field.name}
          </Heading>
          <Text fontSize="sm">{field.defaultResponse}</Text>
        </HStack>
      );
    case INPUT_TYPES.Text.toUpperCase():
      return (
        <HStack>
          <TextField
            name={field.id}
            label={field.name}
            isRequired={field.mandatory}
            placeholder={field.name}
            isDisabled={isInstructor || field.isApproved}
          />
          <Feedback field={field} />
        </HStack>
      );
    case INPUT_TYPES.Description.toUpperCase():
      return (
        <HStack>
          <TextAreaField
            name={field.id}
            title={field.name}
            placeholder={field.name}
            isRequired={field.mandatory}
            isDisabled={isInstructor || field.isApproved}
          />
          <Feedback field={field} />
        </HStack>
      );
    case INPUT_TYPES.File.toUpperCase():
      return (
        <HStack>
          <FileUploadField
            name={field.id}
            isFilePresentName={`${field.id}_isFilePresent`}
            title={field.name}
            accept={field.fieldResponse?.accept ?? [".pdf", ".docx", ".doc"]} // default accepted file ext. is pdf, docx, doc
            existingFile={field.fieldResponse?.fileName}
            downloadLink={field.fieldResponse?.fileName} // TODO: change this to the actual download link
            isRequired={field.mandatory}
            isDisabled={isInstructor || field.isApproved}
          />
          <Feedback field={field} />
        </HStack>
      );
    case INPUT_TYPES.DraggableList.toUpperCase():
      return (
        <HStack>
          <DraggableListField name={field.id} label={field.name} />
          <Feedback field={field} />
        </HStack>
      );

    case INPUT_TYPES.ReactionScheme.toUpperCase():
      return (
        <HStack>
          <ReactionScheme
            name={field.id}
            isDisabled={isInstructor || field.isApproved}
          />
          <Feedback field={field} />
        </HStack>
      );

    case INPUT_TYPES.ChemicalDisposalTable.toUpperCase():
      return (
        <HStack>
          <ChemicalDisposableTable
            name={field.id}
            label={field.name}
            isDisabled={isInstructor || field.isApproved}
          />
          <Feedback field={field} />
        </HStack>
      );

    case INPUT_TYPES.Multiple.toUpperCase():
      return (
        <HStack>
          <OptionsField
            name={field.id}
            label={field.name}
            options={field?.selectFieldOptions}
            isMultiple
            isDisabled={isInstructor || field.isApproved}
            isRequired={field.mandatory}
          />
          <Feedback field={field} />
        </HStack>
      );

    case INPUT_TYPES.Radio.toUpperCase():
      return (
        <HStack>
          <OptionsField
            name={field.id}
            label={field.name}
            options={field?.selectFieldOptions}
            isDisabled={isInstructor || field.isApproved}
            isRequired={field.mandatory}
          />
          <Feedback field={field} />
        </HStack>
      );

    default:
      return null;
  }
};

const TriggerField = ({
  field: {
    id,
    fieldType,
    trigger: { value: triggerValue, target: triggerTargetId },
  },
  fieldValues,
  recordId,
  isInstructor,
  sectionFields,
}) => {
  // determines whether the trigger field is triggered
  const isTriggered = () => {
    switch (fieldType.toUpperCase()) {
      case INPUT_TYPES.Text.toUpperCase():
      case INPUT_TYPES.Description.toUpperCase():
        return triggerValue === fieldValues[id];

      case INPUT_TYPES.Multiple.toUpperCase():
      case INPUT_TYPES.Radio.toUpperCase():
        return fieldValues[id]?.some((value) => triggerValue === value.name);

      default:
        return false;
    }
  };

  // get the trigger target field from the section fields
  const triggerTargetField = useMemo(
    () => sectionFields.find((x) => x.id === triggerTargetId),
    [sectionFields, triggerTargetId]
  );

  // render the trigger target field if the trigger field is triggered
  if (isTriggered() && triggerTargetField) {
    return (
      <>
        <ExperimentField
          field={triggerTargetField}
          recordId={recordId}
          sectionFields={sectionFields}
          fieldValues={fieldValues}
        />
        {triggerTargetField?.triggerCause && (
          <TriggerField
            field={triggerTargetField.triggerCause}
            recordId={recordId}
            isInstructor={isInstructor}
          />
        )}
      </>
    );
  }

  return null;
};
