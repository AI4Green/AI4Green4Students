import { Text, Icon, Flex } from "@chakra-ui/react";
import { FaFlask, FaLayerGroup } from "react-icons/fa";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";
import { DataTableRowExpander } from "components/dataTable/DataTableRowExpander";
import {
  ExperimentAction,
  OverviewAction,
  ReactionOverviewAction,
} from "./tableActions";

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
        {(row.original.isOverview || row.original.isReaction) && (
          <Icon
            as={row.original.isOverview ? FaLayerGroup : FaFlask}
            color="green.600"
          />
        )}

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
      const isParent = row.depth === 0; // if the depth is 0, it means it's a parent row
      return isParent ? (
        !isInstructor && <ExperimentAction row={row} /> // show action buttons for student
      ) : row.original.isOverview ? (
        <OverviewAction experiment={row.original} />
      ) : (
        <ReactionOverviewAction
          reaction={row.original}
          isInstructor={isInstructor}
        />
      );
    },
  },
];
