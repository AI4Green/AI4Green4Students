import {
  Checkbox,
  CheckboxGroup,
  FormControl,
  FormLabel,
  Stack,
  Text,
  Alert,
  AlertIcon,
} from "@chakra-ui/react";
import { useField } from "formik";

export const CheckBox = ({ name, label, isRequired, options = [] }) => {
  const [field, meta, helpers] = useField(name);

  const onChange = (values) => {
    const selectedOptions = options.filter((option) =>
      values.includes(option.name)
    );
    helpers.setValue(selectedOptions);
  };

  return (
    <FormControl isRequired={isRequired} id={field.name}>
      <FormLabel>
        <Text as="b">{label}</Text>
      </FormLabel>

      {options.length >= 1 ? (
        <CheckboxGroup
          colorScheme="green"
          defaultValue={field.value.map((value) => value.name)}
          onChange={onChange}
        >
          <Stack spacing={[1, 5]} direction={["column", "row"]}>
            {options.map((option) => (
              <Checkbox key={option.id} value={option.name}>
                {option.name}
              </Checkbox>
            ))}
          </Stack>
        </CheckboxGroup>
      ) : (
        <Alert
          height="35px"
          borderRadius={8}
          colorScheme="gray"
          variant="subtle"
        >
          <AlertIcon color="gray.400" />
          <Text fontSize="sm" color="gray.600">
            No options available
          </Text>
        </Alert>
      )}
    </FormControl>
  );
};
