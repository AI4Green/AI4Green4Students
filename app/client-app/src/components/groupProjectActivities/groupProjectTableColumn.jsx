import {
  TableCellTextInput,
  TableCellDateInput,
  TableCellDeleteRowButton,
} from "components/dataTable/DataTableCellItems";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";

export const groupProjectTableColumn = ({ isDisabled }) => [
  {
    accessorKey: "serialNumber",
    header: () => <DataTableColumnHeader sorting={false} title="No." />,
    enableHiding: false,
  },
  {
    accessorKey: "weekDate",
    header: () => <DataTableColumnHeader sorting={false} title="weekDate" />,
    cell: ({ getValue, row, column, table }) => (
      <TableCellDateInput
        {...{ getValue, row, column, table }}
        isDisabled={isDisabled}
        placeholder="weekDate"
      />
    ),
  },
  {
    accessorKey: "allPlan",
    header: () => <DataTableColumnHeader sorting={false} title="Gproup Plan" />,
    cell: ({ getValue, row, column, table }) => (
      <TableCellTextInput
        {...{ getValue, row, column, table }}
        isDisabled={isDisabled}
      />
    ),
  },
  {
    accessorKey: "studenta",
    header: () => <DataTableColumnHeader sorting={false} title="Student A" />,
    cell: ({ getValue, row, column, table }) => (
      <TableCellTextInput
        {...{ getValue, row, column, table }}
        isDisabled={isDisabled}
      />
    ),
  },
  {
    accessorKey: "studentb",
    header: () => <DataTableColumnHeader sorting={false} title="Student B" />,
    cell: ({ getValue, row, column, table }) => (
      <TableCellTextInput
        {...{ getValue, row, column, table }}
        isDisabled={isDisabled}
      />
    ),
  },
  {
    accessorKey: "studentc",
    header: () => <DataTableColumnHeader sorting={false} title="Student C" />,
    cell: ({ getValue, row, column, table }) => (
      <TableCellTextInput
        {...{ getValue, row, column, table }}
        isDisabled={isDisabled}
      />
    ),
  },
  {
    accessorKey: "studentd",
    header: () => <DataTableColumnHeader sorting={false} title="Student D" />,
    cell: ({ getValue, row, column, table }) => (
      <TableCellTextInput
        {...{ getValue, row, column, table }}
        isDisabled={isDisabled}
      />
    ),
  },
  {
    accessorKey: "studente",
    header: () => <DataTableColumnHeader sorting={false} title="Student E" />,
    cell: ({ getValue, row, column, table }) => (
      <TableCellTextInput
        {...{ getValue, row, column, table }}
        isDisabled={isDisabled}
      />
    ),
  },
  {
    accessorKey: "studentf",
    header: () => <DataTableColumnHeader sorting={false} title="Student F" />,
    cell: ({ getValue, row, column, table }) => (
      <TableCellTextInput
        {...{ getValue, row, column, table }}
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
