import { Text, Icon, Flex } from "@chakra-ui/react";
import { FaBook, FaChartLine, FaTasks } from "react-icons/fa";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";
import { DataTableRowExpander } from "components/dataTable/DataTableRowExpander";
import { LiteratureReviewAction, PlanOverviewAction } from "./tableActions";
import { EXPERIMENT_DATA_TYPES } from "./experiment-data-types";

export const experimentColumns = (isInstructor) => [
  {
    id: "expander",
    cell: ({ row }) => <DataTableRowExpander row={row} />,
    enableHiding: false,
  },
  {
    accessorKey: "id",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="ID" />
    ),
    enableHiding: false,
  },
  ...(isInstructor
    ? [
        {
          accessorKey: "studentName",
          header: ({ column }) => (
            <DataTableColumnHeader column={column} title="Student name" />
          ),
          enableHiding: false,
        },
      ]
    : []),
  {
    accessorKey: "title",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Title" />
    ),
    cell: ({ row, cell }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 2}>
        <Icon
          as={TITLE_ICON_COMPONENTS[row.original.dataType]}
          color="green.600"
        />

        <Text
          fontWeight={(row.getCanExpand() || row.depth === 0) && "semibold"}
          fontSize={row.depth === 0 && "md"}
        >
          {cell.getValue()}
        </Text>
      </Flex>
    ),
  },
  {
    accessorKey: "status",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Status" />
    ),
  },
  {
    accessorKey: isInstructor ? "projectGroups" : "project",
    header: ({ column }) => (
      <DataTableColumnHeader
        column={column}
        title={isInstructor ? "Project group" : "Project"}
      />
    ),
    cell: ({ row, cell }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 5}>
        <Text>{cell.getValue()?.name} </Text>
      </Flex>
    ),
  },
  {
    id: "actions",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Action" />
    ),
    cell: ({ row }) => {
      const action = ACTION_COMPONENTS[row.original.dataType];
      if (action) {
        const { Component, getProps } = action;
        return <Component {...getProps(row)} />;
      }
      return null;
    },
  },
];

const ACTION_COMPONENTS = {
  [EXPERIMENT_DATA_TYPES.Plan]: {
    Component: PlanOverviewAction,
    getProps: (row) => ({ plan: row.original }),
  },
  [EXPERIMENT_DATA_TYPES.LiteratureReview]: {
    Component: LiteratureReviewAction,
    getProps: (row) => ({ literatureReview: row.original }),
  },
};

const TITLE_ICON_COMPONENTS = {
  [EXPERIMENT_DATA_TYPES.LiteratureReview]: FaBook,
  [EXPERIMENT_DATA_TYPES.Plan]: FaTasks,
  [EXPERIMENT_DATA_TYPES.Report]: FaChartLine,
};
