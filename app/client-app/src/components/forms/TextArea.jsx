import {
  Textarea,
  Badge,
  VStack,
  FormControl,
  Text,
  FormLabel,
} from "@chakra-ui/react";
import { countWords } from "helpers/strings";
import { FormHelpError } from "./FormHelpError";
import { useField } from "formik";
import { useDebounce } from "helpers/hooks/useDebounce";
import { useEffect, useState } from "react";

const WordCountBadge = ({ value, limit }) => {
  const count = countWords(value);
  return limit ? (
    <Badge colorScheme={count > limit ? "red" : undefined}>
      Word Count: {count} / {limit}
    </Badge>
  ) : null;
};

export const TextArea = ({
  name,
  title,
  wordLimit,
  placeholder,
  collapseError,
  fieldTip,
  fieldHelp,
  isRequired,
  ...p
}) => {
  const [field, meta, helpers] = useField({ name, type: "text" });

  const [value, setValue] = useState(field.value);
  const debouncedValue = useDebounce(value, 150);

  const handleChange = ({ target: { value } }) => {
    setValue(value);
  };

  useEffect(() => {
    helpers.setValue(debouncedValue);
  }, [debouncedValue]);

  useEffect(() => {
    setValue(field.value);
  }, [field.value]);

  return (
    <VStack w="100%" align="start">
      <FormControl
        id={field.name}
        isRequired={isRequired}
        isInvalid={meta.error && meta.touched}
      >
        <FormLabel>
          <Text as="b">{title}</Text>
        </FormLabel>

        <Textarea
          value={value}
          placeholder={placeholder}
          onChange={handleChange}
          rows="6"
          {...p}
        />

        <WordCountBadge value={value} limit={wordLimit} />

        {fieldTip}

        <FormHelpError
          isInvalid={meta.error && meta.touched}
          error={meta.error}
          help={fieldHelp}
          collapseEmpty={collapseError}
          replaceHelpWithError
        />
      </FormControl>
    </VStack>
  );
};
