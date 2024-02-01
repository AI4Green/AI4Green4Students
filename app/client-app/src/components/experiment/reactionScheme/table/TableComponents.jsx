import {
  Box,
  Icon,
  IconButton,
  Input,
  NumberDecrementStepper,
  NumberIncrementStepper,
  NumberInput,
  NumberInputField,
  NumberInputStepper,
  Popover,
  PopoverArrow,
  PopoverBody,
  PopoverCloseButton,
  PopoverContent,
  PopoverHeader,
  PopoverTrigger,
  Select,
} from "@chakra-ui/react";
import { useEffect, useState } from "react";
import {
  FaCheck,
  FaCheckCircle,
  FaExclamationCircle,
  FaTimes,
} from "react-icons/fa";

export const TableCellTextInput = ({
  getValue,
  row,
  column,
  table,
  placeholder,
  isDisabled,
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
    <Input
      size="sm"
      value={value}
      onChange={(e) => setValue(e.target.value)}
      onBlur={onBlur}
      isDisabled={isDisabled}
      placeholder={placeholder}
    />
  );
};

export const TableCellDropdown = ({
  getValue,
  row,
  column,
  table,
  options,
  isDisabled,
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
        isDisabled={isDisabled}
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
  isDisabled,
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
        isDisabled={isDisabled}
        step={0.2}
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

//TODO: add validation for the hazards against the backend
export const HazardsValidation = ({ input, valid }) => {
  if (!input || !valid) return null;

  return input === valid ? (
    <Icon as={FaCheckCircle} color="green.500" fontSize="lg" />
  ) : (
    <Popover>
      <PopoverTrigger>
        <IconButton
          aria-label="warning"
          icon={
            <Icon as={FaExclamationCircle} color="orange.500" fontSize="lg" />
          }
          size="sm"
          variant="ghost"
        />
      </PopoverTrigger>
      <PopoverContent color="white" bg="orange.500">
        <PopoverArrow />
        <PopoverCloseButton />
        <PopoverBody fontWeight="bold" border="0">
          Incorrect hazard code
        </PopoverBody>
      </PopoverContent>
    </Popover>
  );
};
