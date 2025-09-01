import { Flex, Icon, Text } from "@chakra-ui/react";
import { ActionButton } from "components/core/action-button";
import { DataTableColumnHeader } from "components/core/data-table";
import { CreateOrEditProjectTypeModal } from "components/project-type/modal-form";
import {
  PROJECT_TYPE_MANAGEMENT_PERMISSIONS,
  STATUS_ICON_COMPONENTS,
  TITLE_ICON_COMPONENTS,
} from "constants";
import { useUser } from "contexts";
import { FaCheck, FaLink, FaTimes, FaTrash } from "react-icons/fa";
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
      width: "sm",
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
    accessorKey: "inUseCount",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="In Use Count" />
    ),
    meta: {
      width: 1,
    },
  },
  {
    id: "inUse",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="In Use" />
    ),
    cell: ({ row }) => {
      const count = row.original.inUseCount;
      return (
        <Flex alignItems="center" gap={2}>
          <Icon
            as={count > 0 ? FaCheck : FaTimes}
            color={count > 0 ? "green.600" : "red.600"}
            fontSize="lg"
          />
          {count > 0 ? "Yes" : "No"}
        </Flex>
      );
    },

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
  const id = searchParams.get("id");

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
      {action === "edit" && id && <CreateOrEditProjectTypeModal />}
      {action === "delete" && id && (
        <Text> Render delete project type modal</Text>
      )}
    </>
  );
};
