import { Heading, Stack, VStack } from "@chakra-ui/react";
import {
  PMICalculator,
  EFactorCalculator,
  RMECalculator,
  WasteIntensityCalculator,
} from "components/experiment-forms/green-metrics/calculator";
import { Formik } from "formik";

export default function GreenMetrics() {
  return (
    <Stack w="full" alignItems="center">
      <VStack
        m={4}
        p={4}
        align="stretch"
        minW={{ base: "85%", md: "70%", lg: "60%", xl: "50%" }}
        spacing={4}
      >
        <Heading fontSize="xl">Calculate Sustainable Metrics</Heading>
        <Formik
          enableReinitialize
          initialValues={{
            wasteIntensityCalculation: {
              waste: 0,
              output: 0,
              wasteIntensity: 0,
            },
            efactorCalculation: {
              wasteMass: 0,
              productMass: 0,
              eFactor: 0,
            },
            rmeCalculation: {
              mass: 0,
              energy: 0,
              rme: 0,
            },
          }}
        >
          <VStack align="flex-start" spacing={8}>
            <WasteIntensityCalculator name="wasteIntensity" />
            <EFactorCalculator name="efactorCalculation" />
            <RMECalculator name="rmeCalculation" />
            <PMICalculator name="pmiCalculation" />
          </VStack>
        </Formik>
      </VStack>
    </Stack>
  );
}
