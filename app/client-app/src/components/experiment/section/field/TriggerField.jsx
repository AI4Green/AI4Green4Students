import { useMemo } from "react";
import { ExperimentField } from "./ExperimentField";
import { isTriggered } from "../form/fieldEvaluation";

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
}) => {
  const isFieldTriggeringChild = isTriggered(
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
