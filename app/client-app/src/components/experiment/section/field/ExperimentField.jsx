import { useIsInstructor } from "../../useIsInstructor";
import { SectionField } from "./SectionField";
import { TriggerField } from "./TriggerField";

/**
 * Creates a fields for the section form.
 * Mainly used to create a input field (formik field).
 * Also, renders a feedback component next to the input field.
 * Feedback component allows the instructor to approve or leave a comment on the field response.
 * Similarly, it allows the student view the feedback.
 * @param {*}
 * field:
 *  - field object mainly used for creating the field.
 *  - contains properties like id, name, fieldType, mandatory, trigger, etc. used to create the field.
 * fieldValues: collection of field values, which can be accessed by using the field.id as key
 * recordId: could be planId or reportId
 * sectionFields: collection of fields in the section
 * @returns
 */
export const ExperimentField = ({
  field,
  fieldValues, // collection of field values, which can be accessed by using the field.id as key
  recordId, // could be planId or reportId
  sectionFields, // collection of fields in the section
}) => {
  const isInstructor = useIsInstructor();
  return (
    <>
      <SectionField field={field} isInstructor={isInstructor} />
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
