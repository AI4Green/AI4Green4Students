import { FormLabel, Text, VStack } from "@chakra-ui/react";
import { useField } from "formik";
import {
  EFactorCalculator,
  RMECalculator,
  WasteIntensityCalculator,
} from "./calculator";

/**
 * Green Metrics Calculator.
 * It consists of Waste Intensity, E-Factor, and RME calculators
 */

export const GreenMetricsCalculator = ({ name, isDisabled }) => {
  const [field, meta, helpers] = useField(name);

  return (
    <VStack w="full" align="flex-start" p={6} spacing={8}>
      <FormLabel>
        <Text as="b">Calculate Green Metrics</Text>
      </FormLabel>
      <WasteIntensityCalculator
        name={`${name}.wasteIntensityCalculation`}
        isDisabled={isDisabled}
      />

      <EFactorCalculator
        name={`${name}.efactorCalculation`}
        isDisabled={isDisabled}
      />

      <RMECalculator name={`${name}.rmeCalculation`} isDisabled={isDisabled} />
    </VStack>
  );
};
