import { useNote, useProjectGroup, useSectionsListBySectionType } from "api";
import {
  InstructorActions,
  StudentActions,
} from "components/experiment-summary";
import { SECTION_TYPES, SITE_ROLES, TITLE_ICON_COMPONENTS } from "constants";
import { useUser } from "contexts";
import { NotFound } from "pages/error";
import { useParams } from "react-router-dom";
import {
  buildProjectPath,
  buildSectionFormPath,
  buildStudentsProjectGroupPath,
} from "routes/project";

import { Overview } from "./overview";

export const NoteOverview = () => {
  const { projectId, projectGroupId, noteId } = useParams();

  const { user } = useUser();
  const { data: note, mutate } = useNote(noteId);

  const isInstructor = user?.roles?.includes(SITE_ROLES.Instructor);
  const isOwner = note?.plan?.owner.id === user.userId;

  const { data: sections } = useSectionsListBySectionType(
    projectId,
    SECTION_TYPES.Note
  );
  const { data: projectGroup } = useProjectGroup(
    isInstructor ? projectGroupId : null
  );

  if (!note) return <NotFound />;

  const noteSections = sections?.map((section) => ({
    ...section,
    stage: note?.stage,
    path: buildSectionFormPath(
      SECTION_TYPES.Note,
      projectId,
      projectGroupId,
      noteId,
      section.id
    ),
  }));

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Note,
    header: {
      type: "Note",
      icon: TITLE_ICON_COMPONENTS.Note,
      title: note?.reactionName,
    },
    project: note?.plan?.project,
    owner: note.plan?.owner,
    action: isInstructor ? (
      <InstructorActions
        record={{ ...note, mutate }}
        sectionType={SECTION_TYPES.Note}
        sections={noteSections}
      />
    ) : (
      <StudentActions
        record={{ ...note, mutate }}
        sectionType={SECTION_TYPES.Note}
        sections={noteSections}
      />
    ),
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
            label: note?.plan?.owner.name,
            href: buildProjectPath(
              projectId,
              projectGroup?.id,
              note?.plan?.owner.id
            ),
          },
        ]
      : []),
    {
      label: note?.reactionName || noteId,
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
  };

  return (
    <>
      <Overview
        item={item}
        sections={noteSections}
        headerItems={headerItems}
        breadcrumbs={breadcrumbItems}
        isInstructor={isInstructor}
      />
    </>
  );
};
