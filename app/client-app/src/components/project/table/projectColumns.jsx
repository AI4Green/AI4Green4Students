import { Flex, Icon, Text, useDisclosure } from "@chakra-ui/react";
import { ActionButton } from "components/core/ActionButton";
import { DataTableColumnHeader } from "components/core/data-table";
import { STATUS_ICON_COMPONENTS } from "constants/experiment-ui";
import { FaLayerGroup, FaLink, FaTrash } from "react-icons/fa";
import { CreateOrEditProjectModal } from "../modal/CreateOrEditProjectModal";
import { DeleteModal } from "../modal/DeleteModal";

/**
 * Columns for the project table.
 * @param {boolean} canManageProjects - Whether the user can manage projects or not
 */
export const projectColumns = (canManageProjects) => [
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Name" />
    ),
    accessorKey: "name",
    cell: ({ cell }) => (
      <Flex alignItems="center" gap={2}>
        <Icon as={FaLayerGroup} color="green.600" />
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
  // const {
  //   isOpen: isEditOpen,
  //   onOpen: onEditOpen,
  //   onClose: onEditClose,
  // } = useDisclosure();

  // const {
  //   isOpen: isDeleteOpen,
  //   onOpen: onDeleteOpen,
  //   onClose: onDeleteClose,
  // } = useDisclosure();

  /**
   * TODO: Figure out how do we want to handle multiple projects.
   * For now, we will only have one default project, which is seeded in the db.
   * Creation, deletion, and editing of projects is disabled.
   * Currently, project is tied to sections, which are also seeded in the db.
   * So, when we create a new project, we need to plan out how we want to handle the
   * sections association with the new project.
   */
  const actions = {
    // edit: {
    //   isEligible: () => true,
    //   icon: <FaLink />,
    //   label: "Edit",
    //   onClick: onEditOpen,
    // },
    // delete: {
    //   isEligible: () => true,
    //   icon: <FaTrash />,
    //   label: "Delete",
    //   onClick: onDeleteOpen,
    // },
  };
  return (
    <>
      <ActionButton actions={actions} size="xs" />
      {/* {isEditOpen && (
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
      )} */}
    </>
  );
};
