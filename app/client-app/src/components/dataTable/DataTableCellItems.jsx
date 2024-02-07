import {
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
} from "@chakra-ui/react";
import { useEffect, useState } from "react";
import { FaRegTimesCircle } from "react-icons/fa";

export const TableCellTextInput = ({ getValue, row, column, table, ...p }) => {
  const initialValue = getValue() || "";
  const [value, setValue] = useState(initialValue);

  const onBlur = () => {
    table.options.meta?.updateData(row.index, column.id, value);
  };

  useEffect(() => {
    setValue(initialValue);
  }, [initialValue]);

  return (
    <Input
      size="sm"
      value={value}
      onChange={(e) => setValue(e.target.value)}
      onBlur={onBlur}
      {...p}
    />
  );
};

export const TableCellDropdown = ({
  getValue,
  row,
  column,
  table,
  options,
  ...p
}) => {
  const initialValue = getValue() || "";
  const [value, setValue] = useState(initialValue);

  useEffect(() => {
    setValue(initialValue);
  }, [initialValue]);

  const onBlur = () => {
    table.options.meta?.updateData(row.index, column.id, value);
  };

  return (
    <Box maxW="130px">
      <Select
        size="sm"
        value={value}
        onChange={(e) => setValue(e.target.value)}
        onBlur={onBlur}
        placeholder="Select option"
        {...p}
      >
        {options.map((option, index) => (
          <option key={index} value={option.value}>
            {option.label}
          </option>
        ))}
      </Select>
    </Box>
  );
};

export const TableCellNumberInput = ({
  getValue,
  row,
  column,
  table,
  ...p
}) => {
  const initialValue = getValue() || "";
  const [value, setValue] = useState(initialValue);

  const onBlur = () => {
    table.options.meta?.updateData(row.index, column.id, value);
  };

  useEffect(() => {
    setValue(initialValue);
  }, [initialValue]);

  return (
    <Box maxW="100px">
      <NumberInput
        size="sm"
        value={value}
        onChange={(e) => setValue(e)}
        onBlur={onBlur}
        step={0.2}
        {...p}
      >
        <NumberInputField />
        <NumberInputStepper>
          <NumberIncrementStepper />
          <NumberDecrementStepper />
        </NumberInputStepper>
      </NumberInput>
    </Box>
  );
};

export const TableCellCheckBox = ({ getValue, row, column, table, ...p }) => {
  const initialValue = getValue() || false;
  const [value, setValue] = useState(initialValue);

  const onBlur = () => {
    table.options.meta?.updateData(row.index, column.id, value);
  };

  useEffect(() => {
    setValue(initialValue);
  }, [initialValue]);

  return (
    <Checkbox
      isChecked={value}
      onChange={() => setValue(!value)}
      onBlur={onBlur}
      {...p}
    />
  );
};

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
