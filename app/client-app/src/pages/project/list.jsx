import { HStack } from "@chakra-ui/react";
import { useProjectsList } from "api";
import { DataTable, DataTableGlobalFilter } from "components/core/data-table";
import { CreateOrEditProjectModal } from "components/project/modal";
import { columns } from "components/project/table";
import {
  PROJECTMANAGEMENT_PERMISSIONS,
  SITE_ROLES,
  STAGES,
  TITLE_ICON_COMPONENTS,
} from "constants";
import { useUser } from "contexts";
import {
  DefaultContentHeader,
  DefaultContentLayout,
  NewButton,
} from "layouts/default";
import { useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";

export const ProjectList = () => {
  const { user } = useUser();
  const { tableData } = useProjectTableData();
  const [searchValue, setSearchValue] = useState("");

  return (
    <DefaultContentLayout>
      <HStack>
        <DefaultContentHeader
          header="Projects"
          icon={TITLE_ICON_COMPONENTS.Project}
        />
      </HStack>
      <DataTable data={tableData} globalFilter={searchValue} columns={columns}>
        <HStack flex={1} spacing={4} justify="flex-start">
          <DataTableGlobalFilter
            searchValue={searchValue}
            setSearchValue={setSearchValue}
            placeholder="Search"
          />
          {user.permissions?.includes(
            PROJECTMANAGEMENT_PERMISSIONS.CreateProjects
          ) && <NewProject />}
        </HStack>
      </DataTable>
    </DefaultContentLayout>
  );
};

const NewProject = () => {
  const [searchParams] = useSearchParams();
  const action = searchParams.get("action");

  return (
    <>
      <NewButton to="/projects?action=new" />

      {action === "new" && <CreateOrEditProjectModal />}
    </>
  );
};

/**
 * Hook to get the table data for listing projects.
 * @returns {Object} - Object containing the table data
 */
const useProjectTableData = () => {
  const { data: projects } = useProjectsList();
  const { user } = useUser();
  const isInstructor = user?.roles?.includes(SITE_ROLES.Instructor);
  const tableData = useMemo(
    () =>
      projects?.map((project) => ({
        id: project.id,
        name: project.name,
        status: project.status || STAGES.OnGoing,
        projectType: project.projectType,
        targetPath: isInstructor
          ? `/projects/${project.id}/project-groups` // for instructors
          : `/projects/${project.id}`, // for students
      })),
    [isInstructor, projects]
  );
  return { tableData: tableData ?? [] };
};
