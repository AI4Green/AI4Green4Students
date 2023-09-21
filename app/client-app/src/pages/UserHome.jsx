import {
  Heading,
  VStack,
  Box,
  Text,
  Stack,
  HStack,
  Divider,
  Flex,
} from "@chakra-ui/react";
import { FaAngleDoubleRight } from "react-icons/fa";
import { NavButton } from "components/NavButton";
import { ProjectSummaryCard } from "components/projectManagement/ProjectSummaryCard";
import { useUser } from "contexts/User";
import { useTranslation } from "react-i18next";
import { useProjectsList } from "api/projects";

export const UserHome = () => {
  const { user } = useUser();
  const { t } = useTranslation();
  const { data: projects } = useProjectsList();

  return (
    <VStack w="100%" spacing={4} alignItems="center">
      <HStack minH={64} spacing={0} w="100%">
        <VStack p={4} minW="55%" bg="gray.100" h="100%" justifyContent="center">
          <Box>
            <Heading as="h2" size="lg" fontWeight="medium">
              {t("home.heading")}
            </Heading>
            <Heading as="h2" size="lg" fontWeight="bold">
              {user?.fullName} !
            </Heading>
            <Text mt={2}>{t("home.subheading")}</Text>
          </Box>
        </VStack>
        <VStack
          p={4}
          spacing={2}
          align="start"
          bgGradient="radial(circle 1500px at top left, green.50, blue.100)"
          h="100%"
          justifyContent="center"
        >
          <Heading as="h3" size="lg" fontWeight="bold">
            Green Chemistry
          </Heading>
          <Text w="65%">
            This is the chemistry that considers the design of chemical products
            and processes to reduce the use or generation of hazardous
            substances.
          </Text>
          <NavButton
            colorScheme="green"
            rightIcon={<FaAngleDoubleRight />}
            to="/greenchemistry"
          >
            Learn more
          </NavButton>
        </VStack>
      </HStack>
      <Stack
        alignItems="flex-start"
        w={{ base: "95%", md: "85%", lg: "75%", xl: "60%" }}
        align="stretch"
        p={4}
        spacing={3}
      >
        <Box w="full">
          <Heading size="md">Available Projects</Heading>
          <Divider />
        </Box>
        <Flex direction="row" wrap="wrap" spacing={0}>
          {projects?.map(
            (project) =>
              project.projectGroups.length > 0 &&
              project.projectGroups.map((group) => (
                <ProjectSummaryCard
                  key={group.id}
                  title={project.name}
                  subtitle={group.name}
                  isProject
                  href={`/experiments/project/${project.id}`}
                />
              ))
          )}
        </Flex>
      </Stack>
    </VStack>
  );
};
