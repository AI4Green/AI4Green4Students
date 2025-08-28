import { Flex, Icon, Text, Tooltip, useDisclosure } from "@chakra-ui/react";
import { ActionButton } from "components/core/action-button";
import { DataTableColumnHeader } from "components/core/data-table";
import { STATUS_ICON_COMPONENTS, TITLE_ICON_COMPONENTS } from "constants";
import { FaLink, FaTrash } from "react-icons/fa";

import { CreateOrEditProjectModal, DeleteModal } from "../modal";

/**
 * Columns for the project table.
 * @param {boolean} canManageProjects - Whether the user can manage projects or not
 */
export const columns = (canManageProjects) => [
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
  ...((canManageProjects && [
    {
      id: "actions",
      header: ({ column }) => (
        <DataTableColumnHeader column={column} title="Actions" />
      ),
      cell: ({ row }) => <ProjectAction project={row.original} />,
      maxSize: 5,
    },
  ]) ||
    []),
];

const ProjectAction = ({ project }) => {
  const {
    isOpen: isEditOpen,
    onOpen: onEditOpen,
    onClose: onEditClose,
  } = useDisclosure();

  const {
    isOpen: isDeleteOpen,
    onOpen: onDeleteOpen,
    onClose: onDeleteClose,
  } = useDisclosure();

  const actions = {
    edit: {
      isEligible: () => true,
      icon: <FaLink />,
      label: "Edit",
      onClick: onEditOpen,
    },
    delete: {
      isEligible: () => true,
      icon: <FaTrash />,
      label: "Delete",
      onClick: onDeleteOpen,
    },
  };
  return (
    <>
      <ActionButton actions={actions} size="xs" />
      {isEditOpen && (
        <CreateOrEditProjectModal
          isModalOpen={isEditOpen}
          onModalClose={onEditClose}
          project={project}
        />
      )}
      {isDeleteOpen && (
        <DeleteModal
          isModalOpen={isDeleteOpen}
          onModalClose={onDeleteClose}
          project={project}
          isDeleteProject
        />
      )}
    </>
  );
};
