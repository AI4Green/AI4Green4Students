import { Box, Divider, Flex, Heading, VStack } from "@chakra-ui/react";
import { CardListLayout } from "components/project/CardListLayout";
import { Card } from "components/project/Card";
import { useProjectsList } from "api/projects";

export const ProjectList = ({ isInstructor }) => {
  const { data: projects } = useProjectsList();
  return (
    <VStack w="100%" spacing={4} alignItems="center">
      <CardListLayout>
        <Box w="full">
          <Heading size="md">Available Projects</Heading>
          <Divider />
        </Box>
        <Flex direction="row" wrap="wrap" spacing={0}>
          {projects?.map((project) => (
            <Card
              isProject
              key={project.id}
              title={project.name}
              href={
                isInstructor
                  ? `/project/${project.id}/project-groups`
                  : `/project/${project.id}`
              }
            />
          ))}
        </Flex>
      </CardListLayout>
    </VStack>
  );
};
