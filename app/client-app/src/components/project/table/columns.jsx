import { Flex, Icon, Text, Tooltip } from "@chakra-ui/react";
import { ActionButton } from "components/core/action-button";
import { DataTableColumnHeader } from "components/core/data-table";
import {
  PROJECTMANAGEMENT_PERMISSIONS,
  STATUS_ICON_COMPONENTS,
  TITLE_ICON_COMPONENTS,
} from "constants";
import { useUser } from "contexts";
import { FaLink, FaTrash } from "react-icons/fa";
import { useSearchParams } from "react-router-dom";

import { CreateOrEditProjectModal, DeleteModal } from "../modal";

/**
 * Columns for the project table.
 */
export const columns = [
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Name" />
    ),
    accessorKey: "name",
    cell: ({ cell }) => (
      <Flex alignItems="center" gap={2}>
        <Icon as={TITLE_ICON_COMPONENTS.Project} color="green.600" />
        <Text fontSize="md" fontWeight="semibold">
          {cell.getValue()}
        </Text>
      </Flex>
    ),
    enableHiding: false,
  },
  {
    accessorKey: "status",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Status" />
    ),
    cell: ({ row, cell }) => (
      <Flex alignItems="center" gap={2}>
        <Icon
          as={STATUS_ICON_COMPONENTS[row.original.status].icon}
          color={STATUS_ICON_COMPONENTS[row.original.status].color}
        />
        {cell.getValue()}
      </Flex>
    ),
  },
  {
    accessorKey: "projectType",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Project Type" />
    ),
    cell: ({ cell }) => (
      <Flex alignItems="center" gap={2}>
        <Icon
          as={TITLE_ICON_COMPONENTS.ProjectType}
          color="green.600"
          fontSize="lg"
        />
        <Tooltip
          label={cell.getValue().description}
          placement="bottom"
          hasArrow
        >
          <Text>{cell.getValue().name}</Text>
        </Tooltip>
      </Flex>
    ),
  },
  {
    id: "actions",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Actions" />
    ),
    cell: ({ row }) => <ProjectAction id={row.original.id} />,
  },
];

const ProjectAction = ({ id }) => {
  const { user } = useUser();
  const [searchParams, setSearchParams] = useSearchParams();
  const action = searchParams.get("action");

  const actions = {
    edit: {
      isEligible: () =>
        user.permissions?.includes(PROJECTMANAGEMENT_PERMISSIONS.EditProjects),
      icon: <FaLink />,
      label: "Edit",
      onClick: () => setSearchParams({ action: "edit", id }),
    },
    delete: {
      isEligible: () =>
        user.permissions?.includes(
          PROJECTMANAGEMENT_PERMISSIONS.DeleteProjects
        ),
      icon: <FaTrash />,
      label: "Delete",
      onClick: () => setSearchParams({ action: "delete", id }),
    },
  };
  return (
    <>
      <ActionButton actions={actions} size="xs" />
      {action === "edit" && <CreateOrEditProjectModal />}
      {action === "delete" && <DeleteModal />}
    </>
  );
};
