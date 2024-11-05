import { useState } from "react";
import { useParams } from "react-router-dom";
import { Overview } from "./Overview";
import { useSectionsListBySectionType, useNote, useProjectGroup } from "api";
import { NotFound } from "pages/error";
import { SECTION_TYPES, TITLE_ICON_COMPONENTS, STAGES } from "constants";
import { Breadcrumbs } from "components/core/Breadcrumbs";
import { useIsInstructor } from "helpers/hooks";
import {
  buildSectionFormPath,
  buildProjectPath,
  buildStudentsProjectGroupPath,
} from "routes/Project";
import { useUser, useBackendApi } from "contexts";
import { Button, Box, useToast } from "@chakra-ui/react";
import { InstructorAction } from "components/experiment-summary";

export const NoteOverview = () => {
  const { user } = useUser();
  const { projectId, projectGroupId, noteId } = useParams();
  const { data: note, mutate } = useNote(noteId);
  const { data: projectGroup } = useProjectGroup(projectGroupId);
  const { notes } = useBackendApi();

  const [isLoading, setIsLoading] = useState(false);

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

  const toast = useToast();

  const handleRequestFeedback = async () => {
    // Check if feedback has already been requested
    if (note?.feedbackRequested) {
      toast({
        title: "Feedback Already Requested",
        description: "You have already requested feedback for this note.",
        status: "info",
        duration: 5000,
        isClosable: true,
      });
      return;
    }

    setIsLoading(true);

    try {
      const response = await notes.requestFeedback(noteId);

      if (response.ok) {
        toast({
          title: "Feedback Requested",
          description: "Your feedback request has been sent successfully.",
          status: "success",
          duration: 5000,
          isClosable: true,
        });

        // Update the note state to reflect that feedback has been requested
        mutate({ ...note, feedbackRequested: true }, false);
      }
    } catch (error) {
      toast({
        title: "Error",
        description: "An unexpected error occurred.",
        status: "error",
        duration: 5000,
        isClosable: true,
      });
    } finally {
      setIsLoading(false);
    }
  };
  const handleCompleteFeedback = async () => {
    setIsLoading(true);

    try {
      const response = await notes.completeFeedback(noteId);
      const responseData = await response.json();

      if (response.ok) {
        toast({
          title: "Feedback Completed",
          description:
            responseData.message ||
            "Your feedback has been completed successfully.",
          status: "success",
          duration: 5000,
          isClosable: true,
        });

        // Update the note state to reflect that feedback has been completed
        mutate({ ...note, feedbackRequested: false }, false);
      } else {
        // Handle non-OK responses
        toast({
          title: "Error",
          description:
            responseData.message ||
            "Failed to complete feedback. Please try again.",
          status: "error",
          duration: 5000,
          isClosable: true,
        });
      }
    } catch (error) {
      toast({
        title: "Error",
        description: "An unexpected error occurred.",
        status: "error",
        duration: 5000,
        isClosable: true,
      });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <>
      <Overview
        sections={noteSections}
        headerItems={headerItems}
        breadcrumbs={<Breadcrumbs items={breadcrumbItems} />}
        InstructorAction={
          note?.stage === STAGES.Locked && (
            <InstructorAction
              record={{ ...note, mutate }}
              sectionType={SECTION_TYPES.Note}
            />
          )
        }
      />

      {isAuthor && (
        <Box display="flex" justifyContent="center" mb={36}>
          <Button
            colorScheme="green"
            size="md"
            onClick={handleRequestFeedback}
            isDisabled={
              note?.feedbackRequested ||
              note?.stage === STAGES.Locked ||
              isLoading
            }
            isLoading={isLoading}
          >
            Request Feedback
          </Button>
        </Box>
      )}
      {isInstructor && (
        <Box display="flex" justifyContent="center" mb={36}>
          <Button
            colorScheme="green"
            size="md"
            onClick={handleCompleteFeedback}
            isDisabled={
              !note?.feedbackRequested ||
              note?.stage === STAGES.Locked ||
              isLoading
            }
            isLoading={isLoading}
          >
            Complete Feedback
          </Button>
        </Box>
      )}
    </>
  );
};
