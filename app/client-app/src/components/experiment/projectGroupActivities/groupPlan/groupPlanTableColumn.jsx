import { TableCellTextAreaInput } from "components/dataTable/DataTableCellItems";
import {
  TableCellDateInput,
  TableCellDeleteRowButton,
} from "components/dataTable/DataTableCellItems";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";

export const groupPlanTableColumn = (students, userLoggedIn, isDisabled) => [
  {
    accessorKey: "serialNumber",
    header: () => <DataTableColumnHeader sorting={false} title="No." />,
    enableHiding: false,
  },
  {
    accessorKey: "weekDate",
    header: () => <DataTableColumnHeader sorting={false} title="Week date" />,
    cell: ({ getValue, row, column, table }) => (
      <TableCellDateInput
        {...{ getValue, row, column, table }}
        placeholder="Week date"
      />
    ),
  },
  {
    accessorKey: "allPlan",
    header: () => <DataTableColumnHeader sorting={false} title="Group Plan" />,
    cell: ({ getValue, row, column, table }) => (
      <TableCellTextAreaInput
        {...{ getValue, row, column, table }}
        placeholder="Group Plan"
        wordLimit={0}
      />
    ),
  },

  ...students.map((student) => ({
    accessorKey: student.id,
    header: () => (
      <DataTableColumnHeader sorting={false} title={student.name} />
    ),
    cell: ({ getValue, row, column, table }) => (
      <TableCellTextAreaInput
        {...{ getValue, row, column, table }}
        isDisabled={userLoggedIn?.email !== student.email}
      />
    ),
  })),

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
