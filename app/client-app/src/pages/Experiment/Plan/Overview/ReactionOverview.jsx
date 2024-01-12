import { Overview } from ".";
import { useParams } from "react-router-dom";
import { useSectionsList } from "api/section";
import { useExperimentReaction } from "api/experimentReactions";
import { NotFound } from "pages/error/NotFound";

export const ReactionOverview = () => {
  const { experimentId, reactionId } = useParams();
  const { data: reaction } = useExperimentReaction(reactionId);

  // TODO: currently, all sections are returned.
  // Get experiment reaction data. ensure experiment name/title is included in the response
  // Get reaction sections, which would be rection scheme and coshh assessment
  const { data: sections } = useSectionsList(experimentId);

  if (!reaction) return <NotFound />;

  const headerItems = {
    header: reaction?.title,
    subHeader: reaction?.projectName,
    owner: reaction?.ownerName,
    overviewTitle: "Reaction Overview",
  };

  return (
    <Overview
      sections={sections}
      experimentId={reaction?.experimentId}
      headerItems={headerItems}
    />
  );
};
