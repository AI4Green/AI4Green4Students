import { Avatar, Flex, Icon, Text } from "@chakra-ui/react";
import { ActionButton } from "components/core/action-button";
import {
  DataTableColumnHeader,
  DataTableRowExpander,
} from "components/core/data-table";
import { PROJECTMANAGEMENT_PERMISSIONS } from "constants";
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
        <Actions projectGroup={row.original} />
      ) : (
        <StudentActions
          studentId={row.original.id}
          projectGroupId={parentRow.original.id}
        />
      );
    },
    maxSize: 5,
  },
];

const StudentActions = ({ studentId, projectGroupId }) => {
  const { user } = useUser();
  const [, setSearchParams] = useSearchParams();

  const actions = {
    remove: {
      isEligible: () =>
        user?.permissions?.includes(
          PROJECTMANAGEMENT_PERMISSIONS.InviteStudents
        ),
      icon: <FaTrash />,
      label: "Remove",
      onClick: () =>
        setSearchParams({
          action: "remove-student",
          projectGroupId,
          studentId,
        }),
      colorScheme: "red",
    },
  };
  return <ActionButton actions={actions} size="xs" />;
};

const Actions = ({ projectGroup }) => {
  const { user } = useUser();
  const { project } = projectGroup;
  const activitiesPath = `/projects/${project.id}/project-groups/${projectGroup.id}/activities`;
  const navigate = useNavigate();

  const [, setSearchParams] = useSearchParams();

  const pgActions = {
    edit: {
      isEligible: () =>
        user?.permissions?.includes(
          PROJECTMANAGEMENT_PERMISSIONS.EditProjectGroups
        ),
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
          PROJECTMANAGEMENT_PERMISSIONS.DeleteProjectGroups
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
        user?.permissions?.includes(
          PROJECTMANAGEMENT_PERMISSIONS.InviteStudents
        ),
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
  return <ActionButton actions={pgActions} size="xs" variant="outline" />;
};
