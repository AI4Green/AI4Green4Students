import { Heading, VStack } from "@chakra-ui/react";
import { useUser } from "contexts/User";
import { useTranslation } from "react-i18next";

export const UserHome = () => {
  const { user } = useUser();
  const { t } = useTranslation();
  return (
    <VStack align="stretch" px={8} w="100%" spacing={4} p={4}>
      <Heading as="h2" size="lg">
        {t("home.heading", { name: user?.fullName })}
      </Heading>
    </VStack>
  );
};
