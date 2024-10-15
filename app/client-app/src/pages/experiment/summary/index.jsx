import {
  Avatar,
  Button,
  HStack,
  useBreakpointValue,
  Icon,
  Stack,
  Text,
  useDisclosure,
} from "@chakra-ui/react";
import { DataTable } from "components/dataTable/DataTable";
import { DataTableSearchBar } from "components/dataTable/DataTableSearchBar";
import { DefaultContentLayout } from "layouts/DefaultLayout";
import { CreateOrEditModal } from "components/experiment/modal/CreateOrEditModal";
import { summaryColumns } from "components/experiment/summary/table/summaryColumns";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { useState } from "react";
import { FaTasks, FaUsers } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import { Breadcrumbs } from "components/Breadcrumbs";
import { TITLE_ICON_COMPONENTS } from "constants/experiment-ui";
import {
  LiteratureReviewAction,
  ReportAction,
} from "components/experiment/summary/summaryActions";
import { SECTION_TYPES } from "constants/section-types";
import {
  buildProjectPath,
  buildActivitiesPath,
  buildStudentsProjectGroupPath,
} from "routes/Project";
import { useUser } from "contexts/User";

export const Summary = ({ projectSummary, tableData, studentId }) => {
  const { user } = useUser();
  const { project, plans, literatureReviews, reports, projectGroup, author } =
    projectSummary;
  const [searchValue, setSearchValue] = useState("");

  const isInstructor = useIsInstructor();
  const isAuthor = author.id === user.userId;

  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: project.name,
      href: !isAuthor && buildProjectPath(project.id),
    },
    ...(!isAuthor
      ? [
          {
            label: projectGroup.name,
            href:
              !isInstructor &&
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
          isAuthor={isAuthor}
          projectName={project.name}
          author={author.name}
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

          <LiteratureReviewAction
            literatureReview={literatureReviews[0]}
            project={project}
            studentId={studentId}
          />

          {(isInstructor || isAuthor) && (
            <ReportAction
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
          <DataTableSearchBar
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

const ExperimentHeading = ({ isAuthor, projectName, author }) => (
  <HStack align="center" gap={2}>
    {!isAuthor && (
      <HStack>
        <Avatar name={author} size="xs" />
        <Text
          fontSize={{ base: "xs", md: "sm" }}
          fontWeight="semibold"
          color="gray.700"
        >
          {author}
        </Text>
      </HStack>
    )}

    <Text
      fontSize={{ base: "xs", md: "sm" }}
      fontWeight="semibold"
      color="brand.500"
    >
      <Icon as={TITLE_ICON_COMPONENTS.Project} /> Project - {projectName}
    </Text>
  </HStack>
);

const NewPlan = ({ project, plansCount }) => (
  <NewItemButton
    project={project}
    buttonText={plansCount === 0 ? "Start planning" : "New plan"}
    leftIcon={<FaTasks />}
    modalProp={{ sectionType: SECTION_TYPES.Plan }}
  />
);

const NewItemButton = ({ project, buttonText, leftIcon, modalProp }) => {
  const { isOpen, onOpen, onClose } = useDisclosure();
  return (
    <>
      <Button
        onClick={onOpen}
        colorScheme="green"
        leftIcon={leftIcon}
        size="sm"
      >
        <Text fontSize="sm" fontWeight="semibold">
          {buttonText}
        </Text>
      </Button>

      <CreateOrEditModal
        isModalOpen={isOpen}
        onModalClose={onClose}
        project={project}
        {...modalProp}
      />
    </>
  );
};

export const ProjectGroup = ({
  projectGroupId,
  projectId,
  isViewingActivities,
}) => {
  const navigate = useNavigate();
  const buttonSize = useBreakpointValue({ base: "xs", md: "sm" });
  const path = isViewingActivities
    ? buildActivitiesPath(projectId, projectGroupId)
    : buildStudentsProjectGroupPath(projectId, projectGroupId);

  return (
    <Button
      onClick={() => navigate(path, { replace: true })}
      colorScheme="gray"
      leftIcon={<FaUsers />}
      size={buttonSize}
      variant="outline"
      py={{ base: 3, md: 4 }}
    >
      <Text fontSize="xs" fontWeight="medium">
        {isViewingActivities ? "Project Group Activities" : "Project Group"}
      </Text>
    </Button>
  );
};
