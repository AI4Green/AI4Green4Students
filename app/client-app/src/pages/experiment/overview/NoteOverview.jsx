import { useParams } from "react-router-dom";
import { Overview } from ".";
import { useSectionsListBySectionType } from "api/section";
import { useNote } from "api/notes";
import { NotFound } from "pages/error/NotFound";
import { SECTION_TYPES } from "constants/section-types";
import { Breadcrumbs } from "components/Breadcrumbs";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { useIsInstructor } from "components/experiment/useIsInstructor";

export const NoteOverview = () => {
  const { projectId, noteId } = useParams();
  const { data: note } = useNote(noteId);

  const { data: sections } = useSectionsListBySectionType(
    projectId,
    SECTION_TYPES.Note
  );
  const noteSections = sections?.map((section) => ({
    ...section,
    path: `/projects/${projectId}/notes/${noteId}/sections/${section.id}`,
  }));

  const isInstructor = useIsInstructor();

  if (!note) return <NotFound />;

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Note,
    header: `${note?.reactionName || noteId}`,
    projectName: note?.plan?.projectName,
    owner: note.plan?.ownerName,
    overviewTitle: "Lab Notes Overview",
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
    },
  ];

  return (
    <Overview
      sections={noteSections}
      headerItems={headerItems}
      breadcrumbs={<Breadcrumbs items={breadcrumbItems} />}
    />
  );
};
