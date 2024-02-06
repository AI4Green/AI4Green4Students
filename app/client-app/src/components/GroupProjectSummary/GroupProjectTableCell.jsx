import { Checkbox, HStack, IconButton, Input } from "@chakra-ui/react";
import { FaRegTimesCircle } from "react-icons/fa";
import { useEffect, useState } from "react";
import { FormHelpError } from "./FormHelpError";
import { Datepicker } from "components/forms/Datepicker";
import { format } from "date-fns";

export const TableCellOther = ({ getValue, row, column, table }) => {
  const initialValue = getValue();
  const [status, setStatus] = useState(initialValue?.status);
  const [value, setValue] = useState(initialValue?.value);

  const onBlur = () => {
    table.options.meta?.updateData(row.index, column.id, {
      status,
      value,
    });
  };

  useEffect(() => {
    setStatus(initialValue?.status);
    setValue(initialValue?.value);
  }, [initialValue]);

  useEffect(() => {
    if (!status) {
      setValue("");
    }
  }, [status]);

  return (
    <HStack align="center">
      <Checkbox
        isChecked={status}
        onChange={() => setStatus(!status)}
        onBlur={onBlur}
      />

      {status && (
        <Input
          size="sm"
          value={value}
          onChange={(e) => setValue(e.target.value)}
          onBlur={onBlur}
          placeholder="Other specify"
        />
      )}
    </HStack>
  );
};

export const TableCellTextInput = ({ getValue, row, column, table }) => {
  const initialValue = getValue();
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
      placeholder={column.id === "weekDate" ? "Input date" : ""}
    />
  );
};

export const TableCellDateInput = ({ getValue, row, column, table }) => {
  const initialValue = getValue();
  const [value, setValue] = useState(initialValue);

  const onBlur = () => {
    table.options.meta?.updateData(row.index, column.id, value);
  };

  useEffect(() => {
    setValue(initialValue);
  }, [initialValue]);

  return (
    <Input
      placeholder="Input Date"
      size="md"
      type="date"
      format="dd/mm/yyyy"
      value={value}
      onChange={(e) => helpers.setValue(e.target.value)}
      onBlur={onBlur}
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
