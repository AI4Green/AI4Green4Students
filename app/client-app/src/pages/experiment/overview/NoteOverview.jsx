import { useParams } from "react-router-dom";
import { Overview } from ".";
import { useSectionsListBySectionType } from "api/section";
import { useNote } from "api/notes";
import { NotFound } from "pages/error/NotFound";

export const NoteOverview = () => {
  const { projectId, projectGroupId, noteId, sectionTypeId } = useParams();
  const { data: note } = useNote(noteId);

  const { data: sections } = useSectionsListBySectionType(sectionTypeId);

  const noteSections = sections?.map((section) => ({
    ...section,
    path: `/project/${projectId}/project-group/${projectGroupId}/note/${noteId}/section/${section.id}`,
  }));

  if (!note) return <NotFound />;

  const headerItems = {
    header: `Lab notes (Plan - ${note?.plan?.title ?? note?.plan?.id})`,
    subHeader: note?.projectName,
    owner: note.plan?.ownerName,
    overviewTitle: "Lab Notes Overview",
  };

  return <Overview sections={noteSections} headerItems={headerItems} />;
};
