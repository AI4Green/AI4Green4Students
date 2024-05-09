import { useParams } from "react-router-dom";
import { Section } from ".";
import { useNote, useNoteSection } from "api/notes";
import { SECTION_TYPES } from "constants/section-types";
import { useBackendApi } from "contexts/BackendApi";

export const NoteSection = () => {
  const { noteId, sectionId } = useParams();
  const { data: note } = useNote(noteId);
  const { data: noteSection, mutate } = useNoteSection(noteId, sectionId);
  const { notes } = useBackendApi();

  const headerItems = {
    header: `Lab notes (Plan - ${note?.plan?.title ?? note?.plan?.id})`,
    subHeader: note?.projectName,
    overviewTitle: noteSection?.name,
  };

  return (
    <Section
      record={note}
      section={noteSection}
      mutate={mutate}
      sectionType={SECTION_TYPES.Note}
      headerItems={headerItems}
      save={notes.saveFieldResponses}
    />
  );
};
