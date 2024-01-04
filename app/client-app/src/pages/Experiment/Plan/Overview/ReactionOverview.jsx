import { Overview } from ".";
import { useParams } from "react-router-dom";
import { useSectionsList } from "api/section";
import { useExperimentReaction } from "api/experimentReactions";

export const ReactionOverview = () => {
  const { experimentId, reactionId } = useParams();
  const { data: reaction } = useExperimentReaction(reactionId);

  // TODO: currently, all sections are returned.
  // Get reaction sections, which would be rection scheme and coshh assessment
  const { data: sections } = useSectionsList(experimentId);

  return (
    <Overview
      sections={sections}
      overviewTitle="Reaction Plan Overview"
      header={reaction?.title}
      subHeader={reaction?.experimentTitle}
    />
  );
};
