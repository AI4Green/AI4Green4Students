import { Box, Select, Text } from "@chakra-ui/react";

export const SelectFieldErrorText = ({ name }) => {
  return <Text color="red">{name}</Text>;
};

export const SelectField = ({
  name,
  value,
  onChange,
  onBlur,
  isInvalid,
  placeholder,
  dataList,
  mt,
  children,
}) => {
  return (
    <Box mt={mt}>
      <Text as="b">{placeholder}</Text>
      <Select
        name={name}
        value={value}
        onChange={onChange}
        onBlur={onBlur}
        placeholder="Select an option"
        isInvalid={isInvalid}
        w="185px"
        mt={1}
      >
        {Object.values(dataList)
          .sort((a, b) => a.sortOrder - b.sortOrder)
          .map((data, index) => (
            <option key={index} value={data.value}>
              {data.value}
            </option>
          ))}
      </Select>
      {children}
    </Box>
  );
};
