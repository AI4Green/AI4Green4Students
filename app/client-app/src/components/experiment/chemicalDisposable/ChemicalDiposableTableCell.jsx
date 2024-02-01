import { Checkbox, HStack, Input } from "@chakra-ui/react";
import { useEffect, useState } from "react";

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
