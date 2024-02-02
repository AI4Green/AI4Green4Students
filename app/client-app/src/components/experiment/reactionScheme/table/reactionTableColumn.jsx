import { VStack, Text, HStack, Box } from "@chakra-ui/react";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";
import {
  TableCellTextInput,
  TableCellNumberInput,
  TableCellDropdown,
} from "components/dataTable/DataTableCellItems";
import { TableCellCheckBox } from "components/dataTable/DataTableCellItems";
import { HazardsValidation } from "./ReactionTable";
import { TableCellDeleteRowButton } from "components/dataTable/DataTableCellItems";

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
    accessorKey: "substanceType",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Type" />
    ),
    cell: ({ getValue }) => <Text fontSize="sm">{getValue()}</Text>,
  },
  {
    accessorKey: "substancesUsed",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Substances Used" />
    ),
  },
  {
    accessorKey: "limiting",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Limiting" />
    ),
    cell: TableCellCheckBox,
  },
  {
    accessorKey: "mass",
    header: ({ column }) => {
      const {
        isDisabled,
        massColumnHeaderDropdown: { ColumnUnitHeaderDropdown, props },
      } = config;
      return (
        <VStack spacing={0}>
          <DataTableColumnHeader column={column} title="Mass (Vol)" />
          <ColumnUnitHeaderDropdown {...props} isDisabled={isDisabled} />
        </VStack>
      );
    },
    cell: ({ getValue, row, column, table }) => {
      const { isDisabled } = config;
      return (
        <TableCellNumberInput
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
    accessorKey: "amount",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Amount (mmol)" />
    ),
    cell: ({ getValue, row, column, table }) => {
      const { isDisabled } = config;
      return (
        <TableCellNumberInput
          {...{ getValue, row, column, table }}
          isDisabled={isDisabled}
          placeholder="Mass (Vol)"
        />
      );
    },
  },
  {
    accessorKey: "density",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Density" />
    ),
  },
  {
    accessorKey: "hazardsInput",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Hazards" />
    ),
    cell: ({ getValue, row, column, table }) => {
      const { isDisabled } = config;
      return (
        <HStack>
          <Box minW="60px">
            <TableCellTextInput
              {...{ getValue, row, column, table }}
              isDisabled={isDisabled}
              placeholder="Hazards"
            />
          </Box>
          <HazardsValidation input={getValue()} valid={row.original.hazards} />
        </HStack>
      );
    },
  },
  {
    id: "actions",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Actions" />
    ),
    cell: ({ row, table }) =>
      row.original?.manualEntry ? (
        <TableCellDeleteRowButton {...{ row, table }} /> // only show delete button if the row is manually entered
      ) : null,
  },
];
