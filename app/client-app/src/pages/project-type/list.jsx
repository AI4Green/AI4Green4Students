import { AdminHeading } from "components/admin/admin-ui";
import { ProjectTypeTable } from "components/project-type/table";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import { DefaultContentLayout } from "layouts/default";

export const ProjectTypeList = () => {
  return (
    <DefaultContentLayout>
      <AdminHeading
        title="Project Type Management"
        icon={TITLE_ICON_COMPONENTS.ProjectType}
      />
      <ProjectTypeTable />
    </DefaultContentLayout>
  );
};
