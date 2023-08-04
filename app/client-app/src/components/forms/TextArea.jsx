import { Textarea, Badge, VStack, FormControl, Text } from "@chakra-ui/react";
import { countWords } from "helpers/strings";
import { FormHelpError } from "./FormHelpError";

const WordCountBadge = ({ value, limit }) => {
  const count = countWords(value);
  return limit ? (
    <Badge colorScheme={count > limit ? "red" : undefined}>
      Word Count: {count} / {limit}
    </Badge>
  ) : null;
};

export const TextArea = ({
  title,
  wordLimit,
  value = "",
  placeholder = "",
  onChange = () => {},
  isInvalid,
  collapseError,
  error,
}) => {
  const handleChange = ({ target: { value } }) => {
    onChange(value);
  };

  return (
    <VStack w="100%" align="start">
      <FormControl isInvalid={isInvalid}>
        <Text as="b">{title}</Text>
        <Textarea
          value={value}
          placeholder={placeholder}
          onChange={handleChange}
          rows="6"
        />

        <WordCountBadge value={value} limit={wordLimit} />

        <FormHelpError
          isInvalid={isInvalid}
          replaceHelpWithError
          error={error}
          collapseEmpty={collapseError}
        />
      </FormControl>
    </VStack>
  );
};
