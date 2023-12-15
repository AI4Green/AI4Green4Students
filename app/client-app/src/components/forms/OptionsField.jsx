/*
  can be used as either checkbox or radio group component
  - isMultiple: checkbox group if true else radio group
  - options: array of options obj (require name, id)
*/

import {
  Checkbox,
  CheckboxGroup,
  FormControl,
  FormLabel,
  Stack,
  Text,
  Alert,
  AlertIcon,
  RadioGroup,
  Radio,
} from "@chakra-ui/react";
import { useField } from "formik";

export const OptionsField = ({
  name,
  label,
  isRequired,
  options = [],
  isMultiple = false,
  ...p
}) => {
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
        <OptionGroup
          isMultiple={isMultiple}
          options={options}
          field={field}
          onChange={onChange}
          {...p}
        />
      ) : (
        <Alert maxH="35px" borderRadius={8} colorScheme="gray" variant="subtle">
          <AlertIcon color="gray.400" />
          <Text fontSize="sm" color="gray.600">
            No options available
          </Text>
        </Alert>
      )}
    </FormControl>
  );
};

const OptionGroup = ({ isMultiple, options, field, onChange, ...p }) => {
  const Component = isMultiple ? CheckboxGroup : RadioGroup;
  const Item = isMultiple ? Checkbox : Radio;

  return (
    <Component
      colorScheme="green"
      defaultValue={
        isMultiple
          ? field.value.map((value) => value.name)
          : field.value[0].name
      }
      onChange={onChange}
      {...p}
    >
      <Stack spacing={2} direction="column">
        {options.map((option) => (
          <Item key={option.id} value={option.name}>
            {option.name}
          </Item>
        ))}
      </Stack>
    </Component>
  );
};
