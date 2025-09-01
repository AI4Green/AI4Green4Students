import { Flex, Icon, Text } from "@chakra-ui/react";
import { ActionButton } from "components/core/action-button";
import { DataTableColumnHeader } from "components/core/data-table";
import {
  PROJECT_TYPE_MANAGEMENT_PERMISSIONS,
  STATUS_ICON_COMPONENTS,
  TITLE_ICON_COMPONENTS,
} from "constants";
import { useUser } from "contexts";
import { FaLink, FaTrash } from "react-icons/fa";
import { useSearchParams } from "react-router-dom";

export const columns = [
  {
    accessorKey: "name",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Name" />
    ),
    cell: ({ cell }) => (
      <Flex alignItems="center" gap={2}>
        <Icon
          as={TITLE_ICON_COMPONENTS.ProjectType}
          color="green.600"
          fontSize="lg"
        />
        <Text>{cell.getValue()}</Text>
      </Flex>
    ),
    meta: {
      width: 1,
    },
  },
  {
    accessorKey: "description",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Description" />
    ),
    meta: {
      width: "xs",
    },
  },
  {
    accessorKey: "stage",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Status" />
    ),
    cell: ({ row, cell }) => (
      <Flex alignItems="center" gap={2}>
        <Icon
          as={STATUS_ICON_COMPONENTS[row.original.stage].icon}
          color={STATUS_ICON_COMPONENTS[row.original.stage].color}
        />
        {cell.getValue()}
      </Flex>
    ),
    meta: {
      width: 1,
    },
  },
  {
    id: "actions",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Actions" />
    ),
    cell: ({ row }) => <Action projectTypeId={row.original.id} />,
    meta: {
      width: 1,
    },
  },
];

const Action = ({ projectTypeId }) => {
  const { user } = useUser();
  const [searchParams, setSearchParams] = useSearchParams();
  const action = searchParams.get("action");

  const actions = {
    edit: {
      isEligible: () =>
        user.permissions.includes(
          PROJECT_TYPE_MANAGEMENT_PERMISSIONS.EditProjectTypes
        ),
      icon: <FaLink />,
      label: "Edit",
      onClick: () => setSearchParams({ action: "edit", id: projectTypeId }),
    },
    delete: {
      isEligible: () =>
        user.permissions.includes(
          PROJECT_TYPE_MANAGEMENT_PERMISSIONS.DeleteProjectTypes
        ),
      icon: <FaTrash />,
      label: "Delete",
      onClick: () => setSearchParams({ action: "delete", id: projectTypeId }),
    },
  };
  return (
    <>
      <ActionButton actions={actions} size="xs" />
      {action === "edit" && <Text> Render edit project type modal</Text>}
      {action === "delete" && <Text> Render delete project type modal</Text>}
    </>
  );
};
