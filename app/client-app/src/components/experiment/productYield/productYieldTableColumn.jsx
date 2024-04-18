import {
  TableCellTextInput,
  TableCellNumberInput,
  TableCellDeleteRowButton,
  DataTableColumnHeader,
} from "components/dataTable/DataTableCellItems";

export const productYieldTableColumn = (isDisabled) => [
  {
    accessorKey: "serialNumber",
    header: () => <DataTableColumnHeader sorting={false} title="No." />,
    enableHiding: false,
  },
  {
    accessorKey: "product",
    header: () => (
      <DataTableColumnHeader sorting={false} title="Yield for Product" />
    ),
    cell: ({ getValue, row, column, table }) => (
      <TableCellTextInput
        {...{ getValue, row, column, table }}
        placeholder="Product"
        isDisabled={isDisabled}
      />
    ),
  },
  {
    accessorKey: "expectedMass",
    header: () => (
      <DataTableColumnHeader sorting={false} title="Expected mass=" />
    ),
    cell: ({ getValue, row, column, table }) => (
      <TableCellNumberInput
        {...{ getValue, row, column, table }}
        placeholder="Expected mass"
        isDisabled={isDisabled}
      />
    ),
  },
  {
    accessorKey: "actualMass",
    header: () => (
      <DataTableColumnHeader sorting={false} title="Actual mass=" />
    ),
    cell: ({ getValue, row, column, table }) => (
      <TableCellNumberInput
        {...{ getValue, row, column, table }}
        placeholder="Actual mass"
        isDisabled={isDisabled}
      />
    ),
  },
  {
    accessorKey: "moles",
    header: () => (
      <DataTableColumnHeader sorting={false} title="No. of moles=" />
    ),
    cell: ({ getValue, row, column, table }) => (
      <TableCellNumberInput
        {...{ getValue, row, column, table }}
        placeholder="Moles"
        isDisabled={isDisabled}
      />
    ),
  },
  {
    accessorKey: "yield",
    header: () => <DataTableColumnHeader sorting={false} title="%Yield" />,
    cell: ({ getValue, row, column, table }) => (
      <TableCellNumberInput
        {...{ getValue, row, column, table }}
        placeholder="%Yield"
        isDisabled={isDisabled}
      />
    ),
  },

  ...(!isDisabled
    ? [
        {
          id: "actions",
          header: () => (
            <DataTableColumnHeader sorting={false} title="Actions" />
          ),
          cell: TableCellDeleteRowButton,
        },
      ]
    : []),
];
