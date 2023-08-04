import { FormikInput } from "./FormikInput";
import { Text, VStack } from "@chakra-ui/react";
import { useField } from "formik";
import { useEffect, useState } from "react";
import { useDebounce } from "../../helpers/hooks/useDebounce";

export const TextField = ({ name, header, ...p }) => {
  const [field, meta, helpers] = useField({ name, type: "text" });

  const [value, setValue] = useState(field.value);
  const debouncedValue = useDebounce(value, 150);

  useEffect(() => {
    helpers.setValue(debouncedValue);
  }, [debouncedValue]);

  return (
    <VStack align="start" w="100%" spacing={2}>
      <Text as="b">{header}</Text>
      <FormikInput
        name={name}
        placeholder="Name"
        size="md"
        onChange={setValue}
        value={value}
        collapseError
        isInvalid={meta.error && meta.touched}
        error={meta.error}
        {...p}
      />
    </VStack>
  );
};
