import { Button, HStack, Text } from "@chakra-ui/react";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { SECTION_TYPES } from "constants/section-types";
import { STAGES_PERMISSIONS } from "constants/site-permissions";
import { useSectionForm } from "contexts/SectionForm";
import { FaPlus } from "react-icons/fa";

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
          leftIcon={<FaPlus />}
          size="xs"
          isLoading={isLoading}
          onClick={() => formRef.current.handleSubmit()}
        >
          <Text fontSize="xs" fontWeight="semibold">
            Save
          </Text>
        </Button>
      )}
    </HStack>
  );
};
