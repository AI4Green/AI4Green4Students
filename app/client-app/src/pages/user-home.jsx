import { VStack } from "@chakra-ui/react";
import { ProjectList } from "./project/list";

export const UserHome = () => {
  return (
    <VStack w="100%" spacing={4} alignItems="center">
      <ProjectList />
    </VStack>
  );
};
