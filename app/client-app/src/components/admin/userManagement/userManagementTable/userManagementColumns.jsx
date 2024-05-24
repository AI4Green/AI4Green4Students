import { Avatar, Badge, HStack, Text } from "@chakra-ui/react";
import { DataTableColumnHeader } from "components/dataTable/DataTableColumnHeader";
import { UserManagementTableActions } from "./userManagementTableActions";

export const userManagementColumns = [
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Name" />
    ),
    accessorKey: "fullName",
    cell: ({ cell }) => (
      <HStack>
        <Avatar size="sm" name={cell.getValue() || "⚠️"} />
        <Text as="b">{cell.getValue()}</Text>,
      </HStack>
    ),
    enableHiding: false,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Email" />
    ),
    accessorKey: "email",
    cell: ({ cell, row }) => (
      <Text
        as="b"
        color={row.original.emailConfirmed ? "green.500" : "orange.500"}
      >
        {cell.getValue()}
      </Text>
    ),
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Roles" />
    ),
    accessorKey: "roles",
    cell: ({ cell }) => {
      const roles = cell.getValue();
      return roles.map((role) => {
        return (
          <Badge px={2} py={1} borderRadius={5} colorScheme="blue" key={role}>
            {role}
          </Badge>
        );
      });
    },
  },
  {
    id: "actions",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Actions" />
    ),
    cell: ({ row }) => <UserManagementTableActions user={row.original} />,
  },
];
