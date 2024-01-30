import { useDisclosure } from "@chakra-ui/react";
import { FaLink, FaTrash, FaProjectDiagram, FaRegUser } from "react-icons/fa";
import { CreateOrEditProjectGroupModal } from "../modal/CreateOrEditProjectGroupModal";
import { CreateOrEditProjectModal } from "../modal/CreateOrEditProjectModal";
import { DeleteModal } from "../modal/DeleteModal";
import { RemoveStudentModal } from "../modal/RemoveStudentModal";
import { ActionButton } from "components/ActionButton";
import { StudentInviteModal } from "../modal/StudentInviteModal";

export const ProjectAction = ({ project }) => {
  // const EditProjectState = useDisclosure();
  // const DeleteProjectState = useDisclosure();
  const NewProjectGroupState = useDisclosure();

  /**
   * TODO: Figure out how do we want to handle multiple projects.
   * For now, we will only have one default project, which is seeded in the db.
   * Creation, deletion, and editing of projects is disabled.
   * Currently, project is tied to sections, which are also seeded in the db.
   * So, when we create a new project, we need to plan out how we want to handle the
   * sections association with the new project.
   */
  const projectManagementActions = {
    // edit: {
    //   isEligible: () => true,
    //   icon: <FaLink />,
    //   label: "Edit",
    //   onClick: EditProjectState.onOpen,
    // },
    // delete: {
    //   isEligible: () => true,
    //   icon: <FaTrash />,
    //   label: "Delete",
    //   onClick: DeleteProjectState.onOpen,
    // },
    newProjectGroup: {
      isEligible: () => true,
      icon: <FaProjectDiagram />,
      label: "Create Project Group",
      onClick: NewProjectGroupState.onOpen,
    },
  };
  return (
    <>
      <ActionButton
        actions={projectManagementActions}
        size="xs"
        colorScheme="green"
      />
      {NewProjectGroupState.isOpen && (
        <CreateOrEditProjectGroupModal
          isModalOpen={NewProjectGroupState.isOpen}
          onModalClose={NewProjectGroupState.onClose}
          project={project}
        />
      )}
      {/* {EditProjectState.isOpen && (
        <CreateOrEditProjectModal
          isModalOpen={EditProjectState.isOpen}
          onModalClose={EditProjectState.onClose}
          project={project}
        />
      )}
      {DeleteProjectState.isOpen && (
        <DeleteModal
          isModalOpen={DeleteProjectState.isOpen}
          onModalClose={DeleteProjectState.onClose}
          project={project}
          isDeleteProject
        />
      )} */}
    </>
  );
};

export const ProjectGroupStudentAction = ({ student, projectGroup }) => {
  const RemoveStudentState = useDisclosure();
  const projectGroupStudentActions = {
    remove: {
      isEligible: () => true,
      icon: <FaTrash />,
      label: "Remove",
      onClick: RemoveStudentState.onOpen,
      colorScheme: "red",
    },
  };
  return (
    <>
      <ActionButton
        actions={projectGroupStudentActions}
        size="xs"
        variant="ghost"
      />
      {RemoveStudentState.isOpen && (
        <RemoveStudentModal
          isModalOpen={RemoveStudentState.isOpen}
          onModalClose={RemoveStudentState.onClose}
          student={student}
          projectGroup={projectGroup}
        />
      )}
    </>
  );
};

export const ProjectGroupAction = ({ projectGroup, project }) => {
  const EditProjectGroupState = useDisclosure();
  const DeleteProjectGroupState = useDisclosure();
  const InviteStudentsState = useDisclosure();

  const projectGroupActions = {
    edit: {
      isEligible: () => true,
      icon: <FaLink />,
      label: "Edit",
      onClick: EditProjectGroupState.onOpen,
    },
    delete: {
      isEligible: () => true,
      icon: <FaTrash />,
      label: "Delete project group",
      onClick: DeleteProjectGroupState.onOpen,
    },
    inviteStudents: {
      isEligible: () => true,
      icon: <FaRegUser />,
      label: "Invite students",
      onClick: InviteStudentsState.onOpen,
    },
  };
  return (
    <>
      <ActionButton actions={projectGroupActions} size="xs" variant="outline" />
      {EditProjectGroupState.isOpen && (
        <CreateOrEditProjectGroupModal
          isModalOpen={EditProjectGroupState.isOpen}
          onModalClose={EditProjectGroupState.onClose}
          projectGroup={projectGroup}
          project={project}
        />
      )}

      {DeleteProjectGroupState.isOpen && (
        <DeleteModal
          isModalOpen={DeleteProjectGroupState.isOpen}
          onModalClose={DeleteProjectGroupState.onClose}
          projectGroup={projectGroup}
          project={project}
        />
      )}
      {InviteStudentsState.isOpen && (
        <StudentInviteModal
          isModalOpen={InviteStudentsState.isOpen}
          onModalClose={InviteStudentsState.onClose}
          projectGroup={projectGroup}
          project={project}
        />
      )}
    </>
  );
};
