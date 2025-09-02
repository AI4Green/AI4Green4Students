import { HStack, Stack } from "@chakra-ui/react";
import { Areas, Sections } from "components/project-type/canvas";

export const ProjectTypeCanvas = () => {
  return (
    <Stack align="center" minW="full" pb={8}>
      <HStack
        align="start"
        justify="center"
        spacing={6}
        w={{ base: "full", xl: "90%", "2xl": "70%" }}
      >
        <Areas />
        <Sections />
      </HStack>
    </Stack>
  );
};
