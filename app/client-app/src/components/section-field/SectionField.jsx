import { useSectionForm } from "contexts";
import { useIsInstructor } from "helpers/hooks";
import { Field, TriggerField } from ".";
import { STAGES_PERMISSIONS, SECTION_TYPES } from "constants";

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
export const SectionField = ({
  field,
  fieldValues, // collection of field values, which can be accessed by using the field.id as key
  recordId, // could be planId or reportId
  sectionFields, // collection of fields in the section
}) => {
  const isInstructor = useIsInstructor();
  const { stagePermissions, sectionType, isRecordOwner } = useSectionForm();
  const { OwnerCanEdit, OwnerCanEditCommented } = STAGES_PERMISSIONS;
  const { ProjectGroup } = SECTION_TYPES;

  const hasRequiredPermissions = [OwnerCanEdit, OwnerCanEditCommented].some(
    (permission) => stagePermissions.includes(permission) && !field.isApproved
  );

  /**
   * user is eligible to edit the field if:
   * - not an instructor
   * - section type is project group or
   * - record owner and section type is ignored (ensures only owner can edit ignored form) or
   * - has required permissions
   */
  const isEligibleToEdit =
    !isInstructor &&
    (sectionType.toUpperCase() === ProjectGroup.toUpperCase() ||
      (isRecordOwner && hasRequiredPermissions));

  return (
    <>
      <Field field={field} isDisabled={!isEligibleToEdit} />
      {field.trigger && (
        <TriggerField
          field={field}
          fieldValues={fieldValues}
          recordId={recordId}
          isInstructor={isInstructor}
          sectionFields={sectionFields}
          isDisabled={!isEligibleToEdit}
        />
      )}
    </>
  );
};
