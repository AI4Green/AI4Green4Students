import { useParams } from "react-router-dom";
import { Overview } from "./Overview";
import { useSectionsListBySectionType, useNote, useProjectGroup } from "api";
import { NotFound } from "pages/error";
import { SECTION_TYPES, TITLE_ICON_COMPONENTS } from "constants";
import { Breadcrumbs } from "components/core/Breadcrumbs";
import { useIsInstructor } from "helpers/hooks";
import {
  buildSectionFormPath,
  buildProjectPath,
  buildStudentsProjectGroupPath,
} from "routes/Project";
import { useUser } from "contexts";
import { InstructorAction, StudentAction } from "components/experiment-summary";

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

  const isInstructor = useIsInstructor();
  const isAuthor = note?.plan?.ownerId === user.userId;

  if (!note) return <NotFound />;

  const headerItems = {
    icon: TITLE_ICON_COMPONENTS.Note,
    header: note?.reactionName,
    projectName: note?.plan?.projectName,
    owner: note.plan?.ownerName,
    ownerId: note.plan?.ownerId,
    overviewTitle: "Lab Notes Overview",
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
    },
  ];

  return (
    <>
      <Overview
        sections={noteSections}
        headerItems={headerItems}
        breadcrumbs={<Breadcrumbs items={breadcrumbItems} />}
        InstructorAction={
          <InstructorAction
            record={{ ...note, mutate }}
            sectionType={SECTION_TYPES.Note}
            sections={noteSections}
          />
        }
        StudentAction={
          <StudentAction
            record={{ ...note, mutate }}
            sectionType={SECTION_TYPES.Note}
            sections={noteSections}
          />
        }
      />
    </>
  );
};
