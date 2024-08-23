import { Flex, Text, Avatar } from "@chakra-ui/react";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";

/**
 * Columns for the student table.
 */
export const projectGroupStudentColumns = [
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Name" />
    ),
    accessorKey: "name",
    cell: ({ cell }) => (
      <Flex alignItems="center" gap={2}>
        <Avatar size="xs" name={cell.getValue()} />
        <Text fontSize="md" fontWeight="semibold">
          {cell.getValue()}
        </Text>
      </Flex>
    ),
    enableHiding: false,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Student email" />
    ),
    accessorKey: "studentEmail",
  },
];
