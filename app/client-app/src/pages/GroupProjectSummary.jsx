import { HazardSummaryTable } from "components/GroupProjectSummary/HazardSummaryTable";
import { LiteratureSummaryTable } from "components/GroupProjectSummary/LiteratureSummary";
import { GroupProjectTable } from "components/GroupProjectSummary/GroupProjectTable";

export const GroupProjectSummary = () => {
  return (
    <>
      <LiteratureSummaryTable />
      <GroupProjectTable />
      <HazardSummaryTable />
    </>
  );
};
