import { useParams } from "react-router-dom";
import { Section } from "./Section";
import { useNote, useNoteSection, useProjectGroup } from "api";
import { SECTION_TYPES, TITLE_ICON_COMPONENTS } from "constants";
import { useBackendApi, useUser } from "contexts";
import { useIsInstructor } from "helpers/hooks";
import {
  buildOverviewPath,
  buildProjectPath,
  buildStudentsProjectGroupPath,
} from "routes/Project";

export const NoteSection = () => {
  const { user } = useUser();
  const { noteId, sectionId, projectId, projectGroupId } = useParams();
  const { data: note } = useNote(noteId);
  const { data: noteSection, mutate } = useNoteSection(noteId, sectionId);
  const { data: projectGroup } = useProjectGroup(projectGroupId);
  const { notes } = useBackendApi();

  const isInstructor = useIsInstructor();
  const isAuthor = note?.plan?.ownerId === user.userId;

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Note,
    header: note?.reactionName,
    projectName: note?.plan?.projectName,
    owner: note?.plan?.ownerName,
    overviewTitle: `${noteSection?.name} Form`,
  };
  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: note?.plan?.projectName,
      href: buildProjectPath(projectId),
    },
    ...(!isAuthor
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
        projectGroup?.id,
        note?.id
      ),
    },
    {
      label: noteSection?.name,
    },
  ];

  return (
    <Section
      record={note}
      section={noteSection}
      mutate={mutate}
      sectionType={SECTION_TYPES.Note}
      headerItems={headerItems}
      save={notes.saveFieldResponses}
      breadcrumbItems={breadcrumbItems}
    />
  );
};
