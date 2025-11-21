import { HStack } from "@chakra-ui/react";
import { useProject, useProjectInstructors } from "api";
import { Breadcrumbs } from "components/core/breadcrumbs";
import { DataTable, DataTableGlobalFilter } from "components/core/data-table";
import { InstructorInviteModal } from "components/project/modal";
import { instructorColumns } from "components/project/table";
import {
  PROJECTMANAGEMENT_PERMISSIONS,
  TITLE_ICON_COMPONENTS,
} from "constants";
import { useUser } from "contexts";
import {
  DefaultContentHeader,
  DefaultContentLayout,
  NewButton,
} from "layouts/default";
import { useMemo, useState } from "react";
import { useParams, useSearchParams } from "react-router-dom";

export const ProjectInstructorList = () => {
  const { user } = useUser();
  const { projectId } = useParams();
  const { data: project } = useProject(projectId);
  const { tableData, mutate } = useTableData(projectId);
  const [searchValue, setSearchValue] = useState("");

  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: project?.name,
      href: `/projects/${project?.id}`,
    },
    {
      label: "Project Instructors",
    },
  ];

  return (
    <DefaultContentLayout>
      <Breadcrumbs items={breadcrumbItems} />
      <HStack>
        <DefaultContentHeader
          header="Instructors"
          icon={TITLE_ICON_COMPONENTS.Instructors}
        />
      </HStack>
      <DataTable
        data={tableData}
        globalFilter={searchValue}
        columns={instructorColumns}
      >
        <HStack flex={1} spacing={4} justify="flex-start">
          <DataTableGlobalFilter
            searchValue={searchValue}
            setSearchValue={setSearchValue}
            placeholder="Search"
          />
          {user.permissions?.includes(
            PROJECTMANAGEMENT_PERMISSIONS.CreateProjects
          ) && <Invite mutate={mutate} />}
        </HStack>
      </DataTable>
    </DefaultContentLayout>
  );
};

const Invite = ({ mutate }) => {
  const [searchParams, setSearchParams] = useSearchParams();
  const action = searchParams.get("action");

  return (
    <>
      <NewButton
        label="Invite"
        onClick={() => setSearchParams({ action: "invite-instructors" })}
      />

      {action === "invite-instructors" && (
        <InstructorInviteModal mutate={mutate} />
      )}
    </>
  );
};

const useTableData = (id) => {
  const { data: instructors, mutate } = useProjectInstructors(id);
  const tableData = useMemo(
    () =>
      instructors?.map((instructor) => ({
        id: instructor.id,
        name: instructor.fullName,
        email: instructor.email,
        roles: instructor.roles,
        emailConfirmed: instructor.emailConfirmed,
      })),
    [instructors]
  );
  return { tableData: tableData ?? [], mutate };
};
