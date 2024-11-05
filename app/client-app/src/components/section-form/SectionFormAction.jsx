import { Button, HStack, Text } from "@chakra-ui/react";
import { useIsInstructor } from "helpers/hooks";
import { SECTION_TYPES, STAGES_PERMISSIONS } from "constants";
import { useSectionForm } from "contexts";
import { FaSave } from "react-icons/fa";

export const SectionFormAction = ({ isLoading, formRef }) => {
  const isInstructor = useIsInstructor();
  const { stagePermissions, sectionType, isRecordOwner } = useSectionForm();
  const { OwnerCanEdit, OwnerCanEditCommented } = STAGES_PERMISSIONS;
  const { ProjectGroup } = SECTION_TYPES;

  const hasRequiredPermissions = [OwnerCanEdit, OwnerCanEditCommented].some(
    (permission) => stagePermissions.includes(permission)
  );

  const canUserSave =
    !isInstructor &&
    (sectionType.toUpperCase() === ProjectGroup.toUpperCase() ||
      (isRecordOwner && hasRequiredPermissions));

  return (
    <HStack pb={1}>
      {canUserSave && (
        <Button
          colorScheme="green"
          leftIcon={<FaSave />}
          size="sm"
          isLoading={isLoading}
          onClick={() => formRef.current.handleSubmit()}
        >
          <Text fontSize="sm" fontWeight="medium">
            Save
          </Text>
        </Button>
      )}
    </HStack>
  );
};
