import { useParams } from "react-router-dom";
import { Section } from ".";
import { useNote, useNoteSection } from "api/notes";
import { SECTION_TYPES } from "constants/section-types";
import { useBackendApi } from "contexts/BackendApi";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { useIsInstructor } from "components/experiment/useIsInstructor";

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
      href: `/projects/${projectId}`,
    },
    ...(isInstructor
      ? [
          {
            label: note.plan?.ownerName,
            href: `/projects/${projectId}/students/${note.plan?.ownerId}`,
          },
        ]
      : []),
    {
      label: note?.reactionName || noteId,
      href: `/projects/${projectId}/notes/${noteId}/overview`,
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
