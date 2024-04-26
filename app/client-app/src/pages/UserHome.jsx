import { Heading, VStack, Box, Text, HStack, Image } from "@chakra-ui/react";
import { FaAngleDoubleRight } from "react-icons/fa";
import { NavButton } from "components/NavButton";
import { useUser } from "contexts/User";
import { useTranslation } from "react-i18next";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { ProjectList } from "./project/ProjectList";

export const UserHome = () => {
  const isInstructor = useIsInstructor();
  return (
    <VStack w="100%" spacing={4} alignItems="center">
      {!isInstructor && <IntroSection />}
      <ProjectList />
    </VStack>
  );
};

const IntroSection = () => {
  const { user } = useUser();
  const { t } = useTranslation();

  return (
    <HStack minH={64} spacing={0} w="100%">
      <VStack
        p={4}
        minW={{ base: "sm", sm: "20%", md: "28%", lg: "35%", xl: "40%" }}
        bg="gray.100"
        h="100%"
        justifyContent="center"
      >
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
      <HStack
        p={4}
        spacing={2}
        align="start"
        bgGradient="radial(circle 1500px at top left, green.50, blue.100)"
        h="100%"
        justifyContent="center"
      >
        <VStack h="full" justify="space-around" spacing={8}>
          <VStack align="flex-start" spacing={2}>
            <Heading as="h3" size="lg" fontWeight="bold">
              Green Chemistry
            </Heading>
            <Text w="95%">
              This is the chemistry that considers the design of chemical
              products and processes to reduce the use or generation of
              hazardous substances.
            </Text>
            <NavButton
              colorScheme="green"
              rightIcon={<FaAngleDoubleRight />}
              to="/greenchemistry"
              size="sm"
            >
              Learn more
            </NavButton>
          </VStack>
          <VStack align="flex-start" spacing={2}>
            <Heading as="h3" size="lg" fontWeight="bold">
              Sustainability Metrics
            </Heading>
            <Text w="95%">
              Sustainable Chemistry Metrics enable quantitiative evaluation of
              chemical reactions. You can learn and practise calculating some of
              the metrics by clicking the link below.
            </Text>
            <NavButton
              colorScheme="green"
              rightIcon={<FaAngleDoubleRight />}
              to="/metrics"
              size="sm"
            >
              Calculate Sustainable Metrics
            </NavButton>
          </VStack>
        </VStack>
        <Image
          boxSize={{ base: "xs", lg: "sm", xl: "md" }}
          objectFit="cover"
          src="/assets/green_chemistry.jpeg"
          alt="Green Chemistry image"
        />
      </HStack>
    </HStack>
  );
};
