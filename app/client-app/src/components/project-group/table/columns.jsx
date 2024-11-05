import { Text, Icon, Flex, Avatar, useDisclosure } from "@chakra-ui/react";
import {
  FaLink,
  FaLock,
  FaProjectDiagram,
  FaRegUser,
  FaTrash,
  FaUsers,
} from "react-icons/fa";
import {
  DataTableColumnHeader,
  DataTableRowExpander,
} from "components/core/data-table";
import { ActionButton } from "components/core/ActionButton";
import {
  RemoveStudentModal,
  CreateOrEditProjectGroupModal,
  StudentInviteModal,
  LockProjectGroupNotesModal,
} from "../modal";
import { useNavigate } from "react-router-dom";
import { useUser } from "contexts";
import { PROJECTMANAGEMENT_PERMISSIONS } from "constants";
import { DeleteModal } from "components/project/modal";

/**
 * Columns for the project group table.
 */
export const columns = [
  {
    id: "expander",
    cell: ({ row }) => <DataTableRowExpander row={row} />,
    enableHiding: false,
    maxSize: 5,
  },
  {
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Name" />
    ),
    accessorKey: "name",
    cell: ({ row, cell }) => (
      <Flex alignItems="center" gap={2} paddingLeft={row.depth * 2}>
        {row.depth === 0 ? (
          <Icon as={FaProjectDiagram} color="green.600" />
        ) : (
          <Avatar size="xs" name={cell.getValue()} />
        )}

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

  {
    id: "actions",
    header: ({ column }) => (
      <DataTableColumnHeader column={column} title="Actions" />
    ),
    cell: ({ row }) => {
      const parentRowId = row.id.split(".").slice(0, -1).join(".");
      const parentRow = row.getParentRow(parentRowId);
      return row.depth === 0 ? (
        <ProjectGroupAction projectGroup={row.original} />
      ) : (
        <PGStudentAction
          student={row.original}
          projectGroup={parentRow.original}
        />
      );
    },
    maxSize: 5,
  },
];

const PGStudentAction = ({ student, projectGroup }) => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  const actions = {
    remove: {
      isEligible: () => true,
      icon: <FaTrash />,
      label: "Remove",
      onClick: onOpen,
      colorScheme: "red",
    },
  };
  return (
    <>
      <ActionButton actions={actions} size="xs" />
      {isOpen && (
        <RemoveStudentModal
          isModalOpen={isOpen}
          onModalClose={onClose}
          student={student}
          projectGroup={projectGroup}
        />
      )}
    </>
  );
};

const ProjectGroupAction = ({ projectGroup }) => {
  const { user } = useUser();
  const { project } = projectGroup;
  const activitiesPath = `/projects/${project.id}/project-groups/${projectGroup.id}/activities`;
  const navigate = useNavigate();

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

  const {
    isOpen: isInviteOpen,
    onOpen: onInviteOpen,
    onClose: onInviteClose,
  } = useDisclosure();

  const {
    isOpen: isLockNotesOpen,
    onOpen: onLockNotesOpen,
    onClose: onLockNotesClose,
  } = useDisclosure();

  const pgActions = {
    edit: {
      isEligible: () => true,
      icon: <FaLink />,
      label: "Edit",
      onClick: onEditOpen,
    },
    lockNotes: {
      isEligible: () =>
        user?.permissions?.includes(
          PROJECTMANAGEMENT_PERMISSIONS.LockProjectGroupNotes
        ),
      icon: <FaLock />,
      label: "Lock notes",
      onClick: onLockNotesOpen,
    },
    delete: {
      isEligible: () => true,
      icon: <FaTrash />,
      label: "Delete project group",
      onClick: onDeleteOpen,
    },
    inviteStudents: {
      isEligible: () => true,
      icon: <FaRegUser />,
      label: "Invite students",
      onClick: onInviteOpen,
    },
    viewActivities: {
      isEligible: () => true,
      icon: <FaUsers />,
      label: "View project group activities",
      onClick: () => navigate(activitiesPath),
    },
  };
  return (
    <>
      <ActionButton actions={pgActions} size="xs" variant="outline" />
      {isEditOpen && (
        <CreateOrEditProjectGroupModal
          isModalOpen={isEditOpen}
          onModalClose={onEditClose}
          projectGroup={projectGroup}
          project={project}
        />
      )}

      {isDeleteOpen && (
        <DeleteModal
          isModalOpen={isDeleteOpen}
          onModalClose={onDeleteClose}
          projectGroup={projectGroup}
          project={project}
        />
      )}
      {isInviteOpen && (
        <StudentInviteModal
          isModalOpen={isInviteOpen}
          onModalClose={onInviteClose}
          projectGroup={projectGroup}
          project={project}
        />
      )}
      {isLockNotesOpen && (
        <LockProjectGroupNotesModal
          isModalOpen={isLockNotesOpen}
          onModalClose={onLockNotesClose}
          projectGroup={projectGroup}
        />
      )}
    </>
  );
};
