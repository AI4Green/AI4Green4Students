import { HStack, Stack, useDisclosure } from "@chakra-ui/react";
import { Breadcrumbs } from "components/core/breadcrumbs";
import { DataTable, DataTableGlobalFilter } from "components/core/data-table";
import {
  LiteratureReviewActions,
  ReportActions,
  summaryColumns,
} from "components/experiment-summary";
import { CreateOrEditModal } from "components/experiment-summary/modal";
import { ProjectGroup } from "components/project-group/project-group";
import { ExperimentHeading } from "components/section-header/header";
import { SECTION_TYPES, SITE_ROLES, TITLE_ICON_COMPONENTS } from "constants";
import { useUser } from "contexts";
import { DefaultContentLayout, NewButton } from "layouts/default";
import { useState } from "react";
import {
  buildProjectPath,
  buildStudentsProjectGroupPath,
} from "routes/project";

export const Summary = ({ projectSummary, tableData, studentId }) => {
  const { user } = useUser();
  const { project, plans, literatureReviews, reports, projectGroup, author } =
    projectSummary;
  const [searchValue, setSearchValue] = useState("");

  const isInstructor = user?.roles?.includes(SITE_ROLES.Instructor);
  const isAuthor = author.id === user.userId;

  const breadcrumbItems = [
    { label: "Projects", href: "/projects" },
    {
      label: project.name,
      href: !isAuthor && buildProjectPath(project.id),
    },
    ...(!isAuthor
      ? [
          {
            label: projectGroup.name,
            href:
              !user?.roles?.includes(SITE_ROLES.Instructor) &&
              buildStudentsProjectGroupPath(project.id, projectGroup.id),
          },
          {
            label: author.name,
          },
        ]
      : []),
  ];

  return (
    <DefaultContentLayout>
      <Breadcrumbs items={breadcrumbItems} />
      <Stack
        my={2}
        w="full"
        justify="space-between"
        direction={{ base: "column", lg: "row" }}
        gap={{ base: 2, md: 4 }}
      >
        <ExperimentHeading
          projectName={project.name}
          projectGroupName={projectGroup.name}
          owner={author}
        />
        <HStack
          gap={{ base: 1, sm: 3, md: 6, lg: 8 }}
          justify="end"
          align="end"
        >
          <ProjectGroup
            projectId={project.id}
            projectGroupId={projectGroup.id}
            isViewingActivities={isInstructor}
          />

          <LiteratureReviewActions
            literatureReview={literatureReviews[0]}
            project={project}
            studentId={studentId}
          />

          {(isInstructor || isAuthor) && (
            <ReportActions
              report={reports[0]}
              project={project}
              studentId={studentId}
            />
          )}
        </HStack>
      </Stack>
      <DataTable
        data={tableData}
        globalFilter={searchValue}
        columns={summaryColumns(isAuthor)}
      >
        <HStack flex={1} justifyContent="flex-start">
          <DataTableGlobalFilter
            searchValue={searchValue}
            setSearchValue={setSearchValue}
            placeholder="Search"
          />
          {isAuthor && <NewPlan project={project} plansCount={plans?.length} />}
        </HStack>
      </DataTable>
    </DefaultContentLayout>
  );
};

const NewPlan = ({ project, plansCount }) => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  return (
    <>
      <NewButton
        onClick={onOpen}
        icon={TITLE_ICON_COMPONENTS[SECTION_TYPES.Plan]}
        label={plansCount === 0 ? "Start planning" : "New plan"}
      />
      {isOpen && (
        <CreateOrEditModal
          isModalOpen={isOpen}
          onModalClose={onClose}
          project={project}
          sectionType={SECTION_TYPES.Plan}
        />
      )}
    </>
  );
};
