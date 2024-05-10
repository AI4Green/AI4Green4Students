import { Text, Icon, Flex } from "@chakra-ui/react";
import {
  FaBook,
  FaChartBar,
  FaCheckCircle,
  FaClock,
  FaPencilAlt,
  FaSearch,
  FaTasks,
} from "react-icons/fa";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";
import {
  LiteratureReviewAction,
  PlanOverviewAction,
  ReportAction,
} from "./tableActions";
import { EXPERIMENT_DATA_TYPES } from "./useExperimentTableData";
import { STAGES } from "constants/stages";

export const experimentColumns = (isInstructor) => [
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
    cell: ({ row, cell }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 2}>
        <Icon
          as={STATUS_ICON_COMPONENTS[row.original.status].icon}
          color={STATUS_ICON_COMPONENTS[row.original.status].color}
        />

        {cell.getValue()}
      </Flex>
    ),
  },
  {
    accessorKey: isInstructor ? "projectGroup" : "project",
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
        return <Component {...getProps(row, isInstructor)} />;
      }
      return null;
    },
  },
];

const ACTION_COMPONENTS = {
  [EXPERIMENT_DATA_TYPES.Plan]: {
    Component: PlanOverviewAction,
    getProps: (row, isInstructor) => ({ plan: row.original, isInstructor }),
  },
  [EXPERIMENT_DATA_TYPES.LiteratureReview]: {
    Component: LiteratureReviewAction,
    getProps: (row, isInstructor) => ({
      literatureReview: row.original,
      isInstructor,
    }),
  },
  [EXPERIMENT_DATA_TYPES.Report]: {
    Component: ReportAction,
    getProps: (row, isInstructor) => ({
      report: row.original,
      isInstructor,
    }),
  },
};

const TITLE_ICON_COMPONENTS = {
  [EXPERIMENT_DATA_TYPES.LiteratureReview]: FaBook,
  [EXPERIMENT_DATA_TYPES.Plan]: FaTasks,
  [EXPERIMENT_DATA_TYPES.Report]: FaChartBar,
};

const STATUS_ICON_COMPONENTS = {
  [STAGES.Draft]: { icon: FaPencilAlt, color: "blue" },
  [STAGES.InReview]: { icon: FaSearch, color: "purple" },
  [STAGES.AwaitingChanges]: { icon: FaClock, color: "orange" },
  [STAGES.Approved]: { icon: FaCheckCircle, color: "green" },
};
