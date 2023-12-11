import { Overview } from ".";
import { useParams } from "react-router-dom";

export const ReactionOverview = () => {
  const { experimentId } = useParams();

  // TODO:
  // Get experiment reaction data. ensure experiment name/title is included in the response
  // Get reaction sections, which would be rection scheme and coshh assessment

  return (
    <Overview
      // sections={sections}
      overview="Reaction Plan Overview"
      // header={reaction?.title}
      // subHeader={reaction?.experimentName}
    />
  );
};
