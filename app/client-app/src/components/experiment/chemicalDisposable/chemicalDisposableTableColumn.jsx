import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";
import {
  TableCellCheckBox,
  TableCellDeleteRowButton,
  TableCellOther,
  TableCellTextInput,
} from "./ChemicalDiposableTableCell";

export const chemicalDisposableTableColumns = () => [
  {
    accessorKey: "serialNumber",
    header: () => <DataTableColumnHeader sorting={false} title="No." />,
    enableHiding: false,
  },
  {
    accessorKey: "chemical",
    header: () => <DataTableColumnHeader sorting={false} title="Chemical" />,
    cell: TableCellTextInput,
  },
  {
    accessorKey: "halogenatedSolvent",
    header: () => (
      <DataTableColumnHeader sorting={false} title="Halogenated Solvent" />
    ),
    cell: TableCellCheckBox,
  },
  {
    accessorKey: "nonHalogenatedSolvent",
    header: () => (
      <DataTableColumnHeader sorting={false} title="Non-Halogenated Solvent" />
    ),
    cell: TableCellCheckBox,
  },
  {
    accessorKey: "wasteStore",
    header: () => <DataTableColumnHeader sorting={false} title="Waste Store" />,
    cell: TableCellCheckBox,
  },
  {
    accessorKey: "aqueousNonHazardous",
    header: () => (
      <DataTableColumnHeader sorting={false} title="Aqueous (Non Hazardous)" />
    ),
    cell: TableCellCheckBox,
  },
  {
    accessorKey: "others",
    header: () => (
      <DataTableColumnHeader sorting={false} title="Other (specify)" />
    ),
    cell: TableCellOther,
  },
  {
    id: "actions",
    header: () => <DataTableColumnHeader sorting={false} title="Actions" />,
    cell: TableCellDeleteRowButton,
  },
];
