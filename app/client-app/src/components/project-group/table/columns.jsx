import { Avatar, Flex, Icon, Text } from "@chakra-ui/react";
import { ActionButton } from "components/core/action-button";
import {
  DataTableColumnHeader,
  DataTableRowExpander,
} from "components/core/data-table";
import { DeleteModal } from "components/project-group/modal";
import {
  PROJECTMANAGEMENT_PERMISSIONS,
  USERMANAGEMENT_PERMISSIONS,
} from "constants";
import { useUser } from "contexts";
import {
  FaLink,
  FaLock,
  FaProjectDiagram,
  FaRegUser,
  FaTrash,
  FaUsers,
} from "react-icons/fa";
import { useNavigate, useSearchParams } from "react-router-dom";

import {
  CreateOrEditProjectGroupModal,
  LockProjectGroupNotesModal,
  RemoveStudentModal,
  StudentInviteModal,
} from "../modal";

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
    accessorKey: "email",
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
  const { user } = useUser();
  const [searchParams, setSearchParams] = useSearchParams();
  const action = searchParams.get("action");

  const actions = {
    remove: {
      isEligible: () =>
        user?.permissions?.includes(PROJECTMANAGEMENT_PERMISSIONS.EditProjects),
      icon: <FaTrash />,
      label: "Remove",
      onClick: () =>
        setSearchParams({
          action: "remove-student",
          id: projectGroup.id,
          studentId: student.id,
        }),
      colorScheme: "red",
    },
  };
  return (
    <>
      <ActionButton actions={actions} size="xs" />
      {action === "remove-student" && <RemoveStudentModal />}
    </>
  );
};

const ProjectGroupAction = ({ projectGroup }) => {
  const { user } = useUser();
  const { project } = projectGroup;
  const activitiesPath = `/projects/${project.id}/project-groups/${projectGroup.id}/activities`;
  const navigate = useNavigate();

  const [searchParams, setSearchParams] = useSearchParams();
  const action = searchParams.get("action");

  const pgActions = {
    edit: {
      isEligible: () =>
        user?.permissions?.includes(PROJECTMANAGEMENT_PERMISSIONS.EditProjects),
      icon: <FaLink />,
      label: "Edit",
      onClick: () =>
        setSearchParams({
          action: "edit",
          id: projectGroup.id,
        }),
    },
    lockNotes: {
      isEligible: () =>
        user?.permissions?.includes(
          PROJECTMANAGEMENT_PERMISSIONS.LockProjectGroupNotes
        ),
      icon: <FaLock />,
      label: "Lock notes",
      onClick: () =>
        setSearchParams({
          action: "lock-notes",
          id: projectGroup.id,
        }),
    },
    delete: {
      isEligible: () =>
        user?.permissions?.includes(
          PROJECTMANAGEMENT_PERMISSIONS.DeleteProjects
        ),
      icon: <FaTrash />,
      label: "Delete project group",
      onClick: () =>
        setSearchParams({
          action: "delete",
          id: projectGroup.id,
        }),
    },
    inviteStudents: {
      isEligible: () =>
        user?.permissions?.includes(USERMANAGEMENT_PERMISSIONS.InviteStudents),
      icon: <FaRegUser />,
      label: "Invite students",
      onClick: () =>
        setSearchParams({
          action: "invite-students",
          id: projectGroup.id,
        }),
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
      {action === "edit" && <CreateOrEditProjectGroupModal />}
      {action === "delete" && <DeleteModal />}
      {action === "invite-students" && <StudentInviteModal />}
      {action === "lock-notes" && <LockProjectGroupNotesModal />}
    </>
  );
};
