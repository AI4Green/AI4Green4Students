import {
  Badge,
  Box,
  Checkbox,
  IconButton,
  Input,
  NumberDecrementStepper,
  NumberIncrementStepper,
  NumberInput,
  NumberInputField,
  NumberInputStepper,
  Select,
  Textarea,
  VStack,
} from "@chakra-ui/react";
import { countWords } from "helpers/strings";
import { useEffect, useState } from "react";
import { FaRegTimesCircle } from "react-icons/fa";

const withTableCell =
  (Component) =>
  ({ getValue, row, column, table, ...props }) => {
    const initialValue = getValue() || "";
    const [value, setValue] = useState(initialValue);

    const onBlur = () => {
      table.options.meta?.updateData(row.index, column.id, value);
    };

    useEffect(() => {
      setValue(initialValue);
    }, [initialValue]);

    return (
      <Component
        value={value}
        onChange={(e) => setValue(e.target.value)}
        onBlur={onBlur}
        {...props}
      />
    );
  };

export const TableCellTextInput = withTableCell(Input);

export const TableCellTextAreaInput = withTableCell(
  ({ wordLimit, ...props }) => (
    <VStack align="flex-e">
      <Textarea size="sm" rows="6" {...props} />
      <WordCountBadge {...props} limit={wordLimit} />
    </VStack>
  )
);

export const TableCellDateInput = withTableCell(({ ...props }) => (
  <Input size="sm" type="date" {...props} />
));

export const TableCellDropdown = withTableCell(({ options, ...props }) => (
  <Box maxW="130px">
    <Select size="sm" placeholder="Select option" {...props}>
      {options.map((option, index) => (
        <option key={index} value={option.value}>
          {option.label}
        </option>
      ))}
    </Select>
  </Box>
));

export const TableCellNumberInput = withTableCell(({ ...props }) => (
  <Box maxW="100px">
    <NumberInput size="sm" step={0.2} {...props}>
      <NumberInputField />
      <NumberInputStepper>
        <NumberIncrementStepper />
        <NumberDecrementStepper />
      </NumberInputStepper>
    </NumberInput>
  </Box>
));

export const TableCellCheckBox = withTableCell(({ ...props }) => (
  <Checkbox {...props} />
));

export const TableCellDeleteRowButton = ({ row, table }) => (
  <IconButton
    onClick={() => table.options.meta?.removeRow(row.index)}
    aria-label="Delete Row"
    icon={<FaRegTimesCircle />}
    variant="ghost"
    fontSize="md"
    colorScheme="red"
  />
);

const WordCountBadge = ({ value, limit }) => {
  const count = countWords(value);
  return limit ? (
    <Badge colorScheme={count > limit ? "red" : undefined}>
      Word Count: {count} / {limit}
    </Badge>
  ) : null;
};
