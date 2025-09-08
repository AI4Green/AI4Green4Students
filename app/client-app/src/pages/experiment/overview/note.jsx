import { useNote, useProjectGroup, useSectionsListBySectionType } from "api";
import { Breadcrumbs } from "components/core/breadcrumbs";
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
  const { user } = useUser();
  const { projectId, projectGroupId, noteId } = useParams();
  const { data: note, mutate } = useNote(noteId);
  const { data: projectGroup } = useProjectGroup(projectGroupId);

  const { data: sections } = useSectionsListBySectionType(
    projectId,
    SECTION_TYPES.Note
  );
  const noteSections = sections?.map((section) => ({
    ...section,
    stage: note?.stage,
    path: buildSectionFormPath(
      SECTION_TYPES.Note,
      projectId,
      projectGroup?.id,
      noteId,
      section.id
    ),
  }));

  const isInstructor = user?.roles?.includes(SITE_ROLES.Instructor);
  const isOwner = note?.plan?.ownerId === user.userId;

  if (!note) return <NotFound />;

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

  return (
    <>
      <Overview
        stage={note?.stage}
        sections={noteSections}
        headerItems={headerItems}
        breadcrumbs={<Breadcrumbs items={breadcrumbItems} />}
        isOwner={isOwner}
        isInstructor={isInstructor}
      />
    </>
  );
};
