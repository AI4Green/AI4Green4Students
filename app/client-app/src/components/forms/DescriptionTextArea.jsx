import { TextArea } from "./TextArea";
import { VStack } from "@chakra-ui/react";

export const DescriptionTextArea = ({
  name,
  title,
  wordLimit,
  placeholder,
  fieldTip,
  fieldHelp,
  collapseError,
  isDisabled,
  isRequired,
  ...p
}) => (
  <VStack align="start" w="100%" spacing={2}>
    <TextArea
      name={name}
      title={title}
      wordLimit={wordLimit}
      placeholder={placeholder}
      fieldTip={fieldTip}
      fieldHelp={fieldHelp}
      isDisabled={isDisabled}
      collapseError={collapseError}
      isRequired={isRequired}
      {...p}
    />
  </VStack>
);
