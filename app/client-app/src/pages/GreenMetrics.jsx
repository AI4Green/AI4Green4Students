import { Stack, VStack } from "@chakra-ui/react";
import { EFactor } from "components/greenMetrics/EFactor";
import { ReactionMassEfficiency } from "components/greenMetrics/ReactionMassEfficiency";
import { WasteIntensity } from "components/greenMetrics/WasteIntensity";

export default function GreenMetrics() {
  return (
    <Stack align="stretch" w="100%" alignItems="center">
      <VStack
        m={4}
        p={4}
        align="stretch"
        minW={{ base: "85%", md: "70%", lg: "60%", xl: "50%" }}
        spacing={4}
      >
        <VStack
          align="flex-start"
          borderWidth={1}
          px={5}
          py={2}
          borderRadius={7}
          spacing={4}
        >
          <WasteIntensity />
          <EFactor />
          <ReactionMassEfficiency />
        </VStack>
      </VStack>
    </Stack>
  );
}
