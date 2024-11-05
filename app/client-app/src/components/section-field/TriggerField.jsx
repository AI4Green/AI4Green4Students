import { useMemo } from "react";
import { SectionField, isFieldTriggered } from ".";

export const TriggerField = ({
  field: {
    id,
    fieldType,
    trigger: { value: triggerValue, target: triggerTargetId },
  },
  fieldValues,
  recordId,
  isInstructor,
  sectionFields,
  isDisabled,
}) => {
  const isFieldTriggeringChild = isFieldTriggered(
    fieldType,
    triggerValue,
    fieldValues[id]
  );

  // get the trigger target field from the section fields
  const triggerTargetField = useMemo(
    () => sectionFields.find((x) => x.id === triggerTargetId),
    [sectionFields, triggerTargetId]
  );

  // render the trigger target field if the trigger field is triggered
  if (isFieldTriggeringChild && triggerTargetField) {
    return (
      <>
        <SectionField
          field={triggerTargetField}
          recordId={recordId}
          sectionFields={sectionFields}
          fieldValues={fieldValues}
          isDisabled={isDisabled}
        />
        {triggerTargetField?.trigger && (
          <TriggerField
            field={triggerTargetField.trigger}
            recordId={recordId}
            isInstructor={isInstructor}
            isDisabled={isDisabled}
          />
        )}
      </>
    );
  }

  return null;
};
