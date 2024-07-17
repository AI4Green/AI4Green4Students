import {
  Avatar,
  Button,
  HStack,
  Heading,
  Icon,
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

export const Summary = ({ projectSummary, tableData, studentId }) => {
  const { project, plans, literatureReviews, reports, projectGroup, author } =
    projectSummary;
  const [searchValue, setSearchValue] = useState("");

  const isInstructor = useIsInstructor();

  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: project.name,
    },
  ];

  return (
    <DefaultContentLayout>
      <Breadcrumbs items={breadcrumbItems} />
      <HStack my={2} w="full" justify="space-between">
        <ExperimentHeading
          isInstructor={isInstructor}
          projectName={project.name}
          author={author.name}
        />
        <HStack gap={8} align="end">
          <ProjectGroupActivities
            projectId={project.id}
            projectGroupId={projectGroup.id}
          />

          <LiteratureReviewAction
            literatureReview={literatureReviews[0]}
            isInstructor={isInstructor}
            project={project}
            studentId={studentId}
          />

          <ReportAction
            report={reports[0]}
            isInstructor={isInstructor}
            project={project}
            studentId={studentId}
          />
        </HStack>
      </HStack>
      <DataTable
        data={tableData}
        globalFilter={searchValue}
        columns={summaryColumns(isInstructor)}
      >
        <HStack flex={1} justifyContent="flex-start">
          <DataTableSearchBar
            searchValue={searchValue}
            setSearchValue={setSearchValue}
            placeholder="Search"
          />
          {!isInstructor && (
            <NewPlan project={project} plansCount={plans?.length} />
          )}
        </HStack>
      </DataTable>
    </DefaultContentLayout>
  );
};

const ExperimentHeading = ({ isInstructor, projectName, author }) => (
  <HStack align="center" gap={2}>
    {isInstructor && (
      <HStack>
        <Avatar name={author} size="xs" />
        <Text fontSize="sm" fontWeight="semibold" color="gray.700">
          {author}
        </Text>
      </HStack>
    )}

    <Heading as="h1" size="sm" fontWeight="semibold" color="blue.600">
      <Icon as={TITLE_ICON_COMPONENTS.Project} /> Project - {projectName}
    </Heading>
  </HStack>
);

const NewPlan = ({ project, plansCount }) => (
  <NewItemButton
    project={project}
    buttonText={plansCount === 0 ? "Start planning" : "New plan"}
    leftIcon={<FaTasks />}
    modalProp={{ isPlan: true }}
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

const ProjectGroupActivities = ({ projectGroupId, projectId }) => {
  const navigate = useNavigate();

  return (
    <Button
      onClick={() =>
        navigate(
          `/projects/${projectId}/project-groups/${projectGroupId}/activities`
        )
      }
      colorScheme="gray"
      leftIcon={<FaUsers />}
      size="sm"
      variant="outline"
    >
      <Text fontSize="sm" fontWeight="semibold">
        Project Group Activities
      </Text>
    </Button>
  );
};
