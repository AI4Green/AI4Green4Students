import { FormikInput } from "./FormikInput";
import { VStack } from "@chakra-ui/react";

export const TextField = ({
  name,
  label,
  placeholder,
  fieldTip,
  fieldHelp,
  collapseError,
  isDisable,
  ...p
}) => (
  <VStack align="start" w="100%" spacing={2}>
    <FormikInput
      name={name}
      placeholder={placeholder}
      label={label}
      fieldTip={fieldTip}
      fieldHelp={fieldHelp}
      isDisable={isDisable}
      collapseError={collapseError}
      {...p}
    />
  </VStack>
);
