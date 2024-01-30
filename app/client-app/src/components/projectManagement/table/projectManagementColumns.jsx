import { Text, Icon, Flex, Avatar } from "@chakra-ui/react";
import { FaLayerGroup, FaProjectDiagram } from "react-icons/fa";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";
import { DataTableRowExpander } from "components/dataTable/DataTableRowExpander";
import {
  ProjectAction,
  ProjectGroupAction,
  ProjectGroupStudentAction,
} from "./tableActions";

export const projectManagementColumns = [
  {
    id: "expander",
    cell: ({ row }) => <DataTableRowExpander row={row} />,
    enableHiding: false,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="ID" />
    ),
    accessorKey: "id",
    enableHiding: false,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Name" />
    ),
    accessorKey: "name",
    cell: ({ row, cell }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 5}>
        {row.depth === 0 ? (
          <Icon as={FaLayerGroup} color="green.600" fontSize="xl" />
        ) : row.depth === 1 ? (
          <Icon as={FaProjectDiagram} color="green.600" fontSize="sm" />
        ) : (
          <Avatar size="xs" name={cell.getValue()} />
        )}

        <Text
          fontSize={row.depth === 0 && "lg"}
          fontWeight={row.depth != 2 && "semibold"}
        >
          {cell.getValue()}
        </Text>
      </Flex>
    ),
    enableHiding: false,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Start date" />
    ),
    accessorKey: "startDate",
    enableHiding: false,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Planning deadline" />
    ),
    accessorKey: "planningDeadline",
    enableHiding: false,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Experiment deadline" />
    ),
    accessorKey: "experimentDeadline",
    enableHiding: false,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="No. of Project Groups" />
    ),
    accessorKey: "projectGroupNumber",
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="No. of Students" />
    ),
    accessorKey: "studentNumber",
    cell: ({ row, cell }) => {
      const isParent = row.depth === 0; // if the depth is 0, it means it's a parent row
      const totalStudents =
        isParent && row.subRows.length > 0
          ? row.original.subRows.reduce(
              (total, subRow) => total + subRow.studentNumber,
              0
            )
          : cell.getValue();

      return (
        <Text fontWeight={(isParent || row.depth === 0) && "semibold"}>
          {totalStudents}
        </Text>
      );
    },
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Student email" />
    ),
    accessorKey: "studentEmail",
  },

  {
    id: "actions",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Actions" />
    ),
    cell: ({ row }) => {
      const parentRowId = row.id.split(".").slice(0, -1).join(".");
      const parentRow = row.getParentRow(parentRowId);

      return row.depth === 0 ? (
        <ProjectAction project={row.original} />
      ) : row.depth === 1 ? (
        <ProjectGroupAction
          projectGroup={row.original}
          project={parentRow.original}
        />
      ) : (
        <ProjectGroupStudentAction
          student={row.original}
          projectGroup={parentRow.original}
        />
      );
    },
  },
];
