import { Input, Select, HStack } from "@chakra-ui/react";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";
import { useEffect, useState } from "react";

/**
 *
 * @param {*} config - contains the following properties:
 * - isDisabled: boolean (whether the table is disabled or not)
 * - massColumnHeaderDropdown: object (contains the following properties)
 *    - ColumnUnitHeaderDropdown: dropdown component
 *    - props: props to pass to the dropdown
 * @returns - an array of objects to be used as columns in the DataTable
 */

export const reactionTableColumns = (config) => [
  {
    accessorKey: "substancesUsed",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Substances Used" />
    ),
  },
  {
    accessorKey: "mass",
    header: ({ column }) => {
      const {
        isDisabled,
        massColumnHeaderDropdown: { ColumnUnitHeaderDropdown, props },
      } = config;
      return (
        <HStack>
          <DataTableColumnHeader column={column} title="Mass (Vol)" />
          <ColumnUnitHeaderDropdown {...props} isDisabled={isDisabled} />
        </HStack>
      );
    },
    cell: ({ getValue, row, column, table }) => {
      const { isDisabled } = config;
      return (
        <TableCellTextInput
          {...{ getValue, row, column, table }}
          isDisabled={isDisabled}
          placeholder="Mass (Vol)"
        />
      );
    },
  },
  {
    accessorKey: "gls",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="g/l/s (Physical form)" />
    ),
    cell: ({ getValue, row, column, table }) => {
      const { isDisabled } = config;
      return (
        <TableCellDropdown
          {...{ getValue, row, column, table }}
          options={[
            { value: "g", label: "Gas" },
            { value: "l", label: "Liquid" },
            { value: "s", label: "Solid" },
          ]}
          isDisabled={isDisabled}
        />
      );
    },
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
    cell: ({ getValue, row, column, table }) => {
      const { isDisabled } = config;
      return (
        <TableCellTextInput
          {...{ getValue, row, column, table }}
          isDisabled={isDisabled}
          placeholder="Hazards"
        />
      );
    },
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

const TableCellTextInput = ({
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

const TableCellDropdown = ({
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
  );
};
