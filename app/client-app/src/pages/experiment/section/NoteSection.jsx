import { useParams } from "react-router-dom";
import { Section } from ".";
import { useNote, useNoteSection } from "api/notes";
import { SECTION_TYPES } from "constants/section-types";
import { useBackendApi } from "contexts/BackendApi";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { buildOverviewPath, buildProjectPath } from "routes/Project";

export const NoteSection = () => {
  const { noteId, sectionId, projectId } = useParams();
  const { data: note } = useNote(noteId);
  const { data: noteSection, mutate } = useNoteSection(noteId, sectionId);
  const { notes } = useBackendApi();

  const isInstructor = useIsInstructor();

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Note,
    header: `${note?.reactionName || noteId}`,
    projectName: note?.plan?.projectName,
    owner: note.plan?.ownerName,
    overviewTitle: `${noteSection?.name} Form`,
  };
  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: note?.plan?.projectName,
      href: buildProjectPath(projectId),
    },
    ...(isInstructor
      ? [
          {
            label: note.plan?.ownerName,
            href: buildProjectPath(projectId, isInstructor, note.plan?.ownerId),
          },
        ]
      : []),
    {
      label: note?.reactionName || noteId,
      href: buildOverviewPath(SECTION_TYPES.Note, projectId, noteId),
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
