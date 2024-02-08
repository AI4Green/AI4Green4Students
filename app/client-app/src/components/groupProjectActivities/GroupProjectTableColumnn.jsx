import {
  TableCellDateInput,
  TableCellOther,
  TableCellTextInput,
} from "./GroupProjectTableCell";

const students = [
  {
    id: 1,
    name: "Apple",
  },
  {
    id: 2,
    name: "ball",
  },
];
export const groupProjectTableColumn = [
  {
    id: "serialNumber",
    Header: "id",
    accessorKey: "id",
  },

  {
    Header: "Weekly Date",
    accessorKey: "weeklyDate",
    cell: TableCellDateInput,
  },
  {
    Header: "Plan for All",
    accessorKey: "planForAll",
    cell: TableCellTextInput,
  },
  {
    Header: "Student A",
    accessorKey: "studentA",
    cell: TableCellTextInput,
  },
  {
    Header: "Student B",
    accessorKey: "studentB",
    cell: TableCellTextInput,
  },
  {
    Header: "Student C",
    accessorKey: "studentC",
    cell: TableCellTextInput,
  },
  {
    Header: "Student D",
    accessorKey: "studentD",
    cell: TableCellTextInput,
  },
  {
    Header: "Student E",
    accessorKey: "studentE",
    cell: TableCellTextInput,
  },
  {
    Header: "Student F",
    accessorKey: "studentF",
    cell: TableCellTextInput,
  },
  {
    Header: "Others",
    accessorKey: "others",
    cell: TableCellOther,
  },
];
