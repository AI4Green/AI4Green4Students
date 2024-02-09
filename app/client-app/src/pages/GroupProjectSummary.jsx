import { HazardSummaryTable } from "components/groupProjectActivities/HazardSummaryTable";
import { LiteratureSummaryTable } from "components/groupProjectActivities/LiteratureSummary";
import { GroupProjectTable } from "components/groupProjectActivities/GroupProjectTable";

export const GroupProjectSummary = () => {
  return (
    <>
      <LiteratureSummaryTable />
      <GroupProjectTable />
      {/* <HazardSummaryTable /> */}
    </>
  );
};
