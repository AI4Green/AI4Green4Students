import { Heading, VStack, Box, Text, HStack } from "@chakra-ui/react";
import { FaAngleDoubleRight } from "react-icons/fa";
import { NavButton } from "components/NavButton";
import { useUser } from "contexts/User";
import { useTranslation } from "react-i18next";

export const UserHome = () => {
  const { user } = useUser();
  const { t } = useTranslation();
  return (
    <VStack align="stretch" w="100%" spacing={4}>
      <HStack minH={64} spacing={0}>
        <VStack p={4} w="65%" bg="gray.100" h="100%" justifyContent="center">
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
    </VStack>
  );
};
