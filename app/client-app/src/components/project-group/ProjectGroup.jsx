import { Button, Text, useBreakpointValue } from "@chakra-ui/react";
import { FaUsers } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import {
  buildStudentsProjectGroupPath,
  buildActivitiesPath,
} from "routes/Project";

export const ProjectGroup = ({
  projectGroupId,
  projectId,
  isViewingActivities,
}) => {
  const navigate = useNavigate();
  const buttonSize = useBreakpointValue({ base: "xs", md: "sm" });
  const path = isViewingActivities
    ? buildActivitiesPath(projectId, projectGroupId)
    : buildStudentsProjectGroupPath(projectId, projectGroupId);

  return (
    <Button
      onClick={() => navigate(path, { replace: true })}
      colorScheme="gray"
      leftIcon={<FaUsers />}
      size={buttonSize}
      variant="outline"
      py={{ base: 3, md: 4 }}
    >
      <Text fontSize="xs" fontWeight="medium">
        {isViewingActivities ? "Project Group Activities" : "Project Group"}
      </Text>
    </Button>
  );
};
