import { Text } from "@chakra-ui/react";
import { ActionButton } from "components/core/action-button";
import { Badge } from "components/core/Badge";
import { DataTableColumnHeader } from "components/core/data-table";
import { REGISTRATION_RULES_PERMISSIONS } from "constants";
import { useUser } from "contexts";
import { FaLink, FaTrash } from "react-icons/fa";
import { useSearchParams } from "react-router-dom";

import { DeleteModal } from "./modal-delete";
import { CreateOrEditModal } from "./modal-form";

export const columns = [
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Value" />
    ),
    accessorKey: "value",
    cell: ({ row, cell }) => {
      const isBlocked = row.original.isBlocked;
      return (
        <Text fontWeight="semibold" color={isBlocked ? "red.500" : "green.600"}>
          {cell.getValue()}
        </Text>
      );
    },
    enableHiding: false,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Type" />
    ),
    accessorKey: "type",
    cell: ({ cell, row }) => {
      return (
        <Badge
          label={cell.getValue()}
          colorScheme={row.original.isBlocked ? "red" : "green"}
          fontSize="xxs"
        />
      );
    },
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Modified" />
    ),
    accessorKey: "modified",
    cell: ({ cell }) => {
      return <Text fontSize="xxs">{cell.getValue()}</Text>;
    },
  },
  {
    id: "actions",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Actions" />
    ),
    cell: ({ row }) => <Actions rule={row.original} />,
  },
];

export const Actions = ({ rule }) => {
  const { user } = useUser();
  const [searchParams, setSearchParams] = useSearchParams();
  const action = searchParams.get("action");

  const actions = {
    delete: {
      isEligible: () =>
        user.permissions?.includes(
          REGISTRATION_RULES_PERMISSIONS.DeleteRegistrationRules
        ),
      icon: <FaTrash />,
      label: "Remove",
      onClick: () => setSearchParams({ action: "delete", id: rule.id }),
      colorScheme: "red",
    },
    edit: {
      isEligible: () =>
        user.permissions?.includes(
          REGISTRATION_RULES_PERMISSIONS.EditRegistrationRules
        ),
      icon: <FaLink />,
      label: "Edit",
      onClick: () => setSearchParams({ action: "edit", id: rule.id }),
      colorScheme: "blue",
    },
  };
  return (
    <>
      <ActionButton actions={actions} size="xs" />
      {action === "delete" && <DeleteModal />}
      {action === "edit" && <CreateOrEditModal />}
    </>
  );
};
