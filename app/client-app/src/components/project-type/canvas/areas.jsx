import { Badge, Button, Divider, Icon, VStack } from "@chakra-ui/react";
import { useSectionTypesList } from "api/project-type";
import { SECTION_TYPES, TITLE_ICON_COMPONENTS } from "constants";
import { useNavigate, useParams } from "react-router-dom";

export const BASE_PATH = "/admin/project-type-management";

export const Areas = () => {
  const navigate = useNavigate();
  const { projectTypeId, sectionTypeId } = useParams();
  const { data: sectionTypes } = useSectionTypesList();
  return (
    <VStack
      px={2}
      py={4}
      spacing={4}
      align="start"
      borderWidth={1}
      borderRadius={7}
      borderColor="blue.100"
    >
      <SimpleBadge label="Areas" colorScheme="blue" />
      <Divider />
      {sectionTypes.map((sectionType) => (
        <Button
          borderRadius="xl"
          key={sectionType.id}
          justifyContent="flex-start"
          w="full"
          leftIcon={<Icon as={TITLE_ICON_COMPONENTS[sectionType.name]} />}
          variant={Number(sectionTypeId) === sectionType.id ? "solid" : "ghost"}
          size="xs"
          onClick={() => {
            navigate(
              `${BASE_PATH}/${projectTypeId}/section-types/${sectionType.id}/sections`,
              {
                replace: true,
              }
            );
          }}
        >
          {sectionTypeLabels[sectionType.name]}
        </Button>
      ))}
    </VStack>
  );
};

const sectionTypeLabels = {
  [SECTION_TYPES.LiteratureReview]: "Literature Review",
  [SECTION_TYPES.Plan]: "Plan",
  [SECTION_TYPES.Note]: "Note",
  [SECTION_TYPES.Report]: "Report",
  [SECTION_TYPES.ProjectGroup]: "Project Group Activities",
};

export const SimpleBadge = ({ label, colorScheme, ...p }) => {
  return (
    <Badge
      colorScheme={colorScheme}
      px={4}
      py={0.5}
      borderRadius="xl"
      fontSize="xs"
      fontWeight="medium"
      textTransform="capitalize"
      {...p}
    >
      {label}
    </Badge>
  );
};
