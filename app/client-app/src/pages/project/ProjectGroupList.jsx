import { Box, Divider, Flex, Heading, VStack } from "@chakra-ui/react";
import { useProject } from "api/projects";
import { CardListLayout } from "components/project/CardListLayout";
import { useParams } from "react-router-dom";
import { Card } from "components/project/Card";

export const ProjectGroupList = () => {
  const { projectId } = useParams();

  const { data: project } = useProject(projectId);
  const { projectGroups } = project;

  return (
    <VStack w="100%" spacing={4} alignItems="center">
      <CardListLayout>
        <Box w="full">
          <Heading size="md">Available Project Groups</Heading>
          <Divider />
        </Box>
        <Flex direction="row" wrap="wrap" spacing={0}>
          {projectGroups?.map((projectGroup) => (
            <Card
              key={projectGroup.id}
              title={projectGroup.name}
              href={`/projects/${project.id}/project-group/${projectGroup.id}`}
            />
          ))}
        </Flex>
      </CardListLayout>
    </VStack>
  );
};
