import { useNote, useNoteSection, useProjectGroup } from "api";
import { SectionForm } from "components/section-form";
import { SECTION_TYPES, SITE_ROLES, TITLE_ICON_COMPONENTS } from "constants";
import { useBackendApi, useUser } from "contexts";
import { NotFound } from "pages/error";
import { useParams } from "react-router-dom";
import {
  buildOverviewPath,
  buildProjectPath,
  buildStudentsProjectGroupPath,
} from "routes/project";

export const NoteSection = () => {
  const { noteId, sectionId, projectId, projectGroupId } = useParams();

  const { user } = useUser();
  const { data: note } = useNote(noteId);

  const isInstructor = user?.roles?.includes(SITE_ROLES.Instructor);
  const isOwner = note?.plan?.owner.id === user.userId;

  const { data: form, mutate } = useNoteSection(noteId, sectionId);
  const { data: projectGroup } = useProjectGroup(
    isInstructor ? projectGroupId : null
  );
  const { notes } = useBackendApi();

  if (!note) return <NotFound />;

  const headerItems = {
    header: {
      type: "Plan",
      icon: TITLE_ICON_COMPONENTS.Plan,
      title: note?.reactionName,
    },
    project: note?.plan?.project,
    owner: note?.plan?.owner,
  };

  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: note?.plan?.project.name,
      href: buildProjectPath(projectId),
    },
    ...(!isOwner
      ? [
          {
            label: projectGroup.name,
            href:
              !isInstructor &&
              buildStudentsProjectGroupPath(projectId, projectGroup?.id),
          },
          {
            label: note?.plan?.ownerName,
            href: buildProjectPath(
              projectId,
              projectGroup?.id,
              note?.plan?.ownerId
            ),
          },
        ]
      : []),
    {
      label: note?.reactionName || noteId,
      href: buildOverviewPath(
        SECTION_TYPES.Note,
        projectId,
        projectGroupId,
        note?.id
      ),
    },
    {
      label: form?.name,
    },
  ];

  const item = {
    id: note.id,
    type: SECTION_TYPES.Note,
    stage: {
      name: note?.stage,
      permissions: note?.permissions,
    },
    isOwner,
    action: {
      mutate,
      save: notes.saveFieldResponses,
    },
  };

  return (
    <SectionForm
      item={item}
      form={form}
      headerItems={headerItems}
      breadcrumbItems={breadcrumbItems}
      isInstructor={isInstructor}
    />
  );
};
