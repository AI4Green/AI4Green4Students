/*
  Select component, which can be used as either single select or multi select
*/

import { Field } from "formik";
import Select from "react-select";
import { FormLabel, theme, FormControl } from "@chakra-ui/react";
import { FormHelpError } from "./FormHelpError";

const CustomSelect = ({
  field,
  form,
  isMulti = false,
  options,
  label,
  placeholder,
  isDisabled,
}) => {
  const onChange = (option) => {
    const selectedValue = isMulti
      ? option.map((item) => item.value)
      : [option?.value || ""];

    form.setFieldValue(field.name, selectedValue);
  };

  const selected = () => {
    return options
      ? options.filter((option) => field.value.indexOf(option.value) >= 0)
      : [];
  };

  const styles = {
    control: (baseStyles, state) => ({
      ...baseStyles,
      border: state.isFocused
        ? `1px Solid ${theme.colors.blue[800]}`
        : selected().length < 1 &&
          !state.isFocused &&
          `2px Solid ${theme.colors.red[500]}`,
      "&:hover": {
        backgroundColor: theme.colors.gray[100],
      },
    }),
  };

  return (
    <FormControl isRequired isInvalid={form.errors[field.name]}>
      <FormLabel>{label}</FormLabel>
      <Select
        name={field.name}
        value={selected()}
        onChange={onChange}
        placeholder={placeholder}
        options={options}
        isMulti={isMulti}
        styles={styles}
        isDisabled={isDisabled}
      />
      <FormHelpError
        isInvalid={form.errors[field.name] && form.touched[field.name]}
        error={form.errors[field.name]}
        collapseEmpty
      />
    </FormControl>
  );
};

export const MultiSelectField = ({
  options,
  name,
  placeholder,
  label, // label
  isMulti,
  initialValue, // initially selected values
  isDisabled,
}) => {
  return (
    <Field name={name} initialValue={initialValue}>
      {({ field, form }) => (
        <CustomSelect
          field={field}
          form={form}
          isMulti={isMulti}
          options={options}
          label={label}
          placeholder={placeholder}
          isDisabled={isDisabled}
        />
      )}
    </Field>
  );
};
