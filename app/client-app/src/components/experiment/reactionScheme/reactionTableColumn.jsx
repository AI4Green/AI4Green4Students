import { Input, Select } from "@chakra-ui/react";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";
import { useEffect, useState } from "react";

export const reactionTableColumns = () => [
  {
    accessorKey: "substancesUsed",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Substances Used" />
    ),
  },
  {
    accessorKey: "mass",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Mass (Vol)" />
    ),
    cell: TableCellTextInput,
  },
  {
    accessorKey: "gls",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="g/l/s (Physical form)" />
    ),
    cell: ({ getValue, row, column, table }) => (
      <TableCellDropdown
        {...{ getValue, row, column, table }}
        options={[
          { value: "g", label: "Gas" },
          { value: "l", label: "Liquid" },
          { value: "s", label: "Solid" },
        ]}
      />
    ),
  },
  {
    accessorKey: "molWeight",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Mol.Wt" />
    ),
  },
  {
    accessorKey: "density",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Density" />
    ),
  },
  {
    accessorKey: "hazards",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Hazards" />
    ),
    cell: TableCellTextInput, // TODO: Check whether this needs validation
  },
  {
    // TODO: Add a button to delete the row only if user mannually added this row.
    // If the row is generated from the sketcher, then it should not be deleted.
    accessorKey: "action",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Actions" />
    ),
  },
];

const TableCellTextInput = ({ getValue, row, column, table }) => {
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
    />
  );
};

const TableCellDropdown = ({ getValue, row, column, table, options }) => {
  const initialValue = getValue() || "";
  const [value, setValue] = useState(initialValue);

  useEffect(() => {
    setValue(initialValue);
  }, [initialValue]);

  const onBlur = () => {
    table.options.meta?.updateData(row.index, column.id, value);
  };

  return (
    <Select
      size="sm"
      value={value}
      onChange={(e) => setValue(e.target.value)}
      onBlur={onBlur}
    >
      {options.map((option, index) => (
        <option key={index} value={option.value}>
          {option.label}
        </option>
      ))}
    </Select>
  );
};
