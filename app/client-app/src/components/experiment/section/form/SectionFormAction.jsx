import { Avatar, Button, HStack, Text } from "@chakra-ui/react";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { SECTION_TYPES } from "constants/section-types";
import { STAGES_PERMISSIONS } from "constants/site-permissions";
import { useSectionForm } from "contexts/SectionForm";
import { FaPlus } from "react-icons/fa";

export const SectionFormAction = ({ record, isLoading, formRef }) => {
  const isInstructor = useIsInstructor();
  const { stagePermissions, sectionType } = useSectionForm();
  const { OwnerCanEdit, OwnerCanEditCommented } = STAGES_PERMISSIONS;
  const { ProjectGroup, Note } = SECTION_TYPES;

  const isSectionTypeIgnored = [
    ProjectGroup.toUpperCase(),
    Note.toUpperCase(),
  ].includes(sectionType.toUpperCase()); // sections that are ignored from the permission check
  const hasRequiredPermissions = [OwnerCanEdit, OwnerCanEditCommented].some(
    (permission) => stagePermissions.includes(permission)
  );

  const canUserSave =
    !isInstructor && (isSectionTypeIgnored || hasRequiredPermissions);

  return (
    <HStack pb={1}>
      {record.ownerName && (
        <>
          <Avatar name={record.ownerName} size="sm" />
          <Text fontSize="md" color="gray.600">
            {record.ownerName}
          </Text>
        </>
      )}

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
