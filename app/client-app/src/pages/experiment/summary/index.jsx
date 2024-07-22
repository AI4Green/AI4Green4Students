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

export const Summary = ({ projectSummary, tableData, studentId }) => {
  const { project, plans, literatureReviews, reports, projectGroup, author } =
    projectSummary;
  const [searchValue, setSearchValue] = useState("");

  const isInstructor = useIsInstructor();

  const breadcrumbItems = [
    { label: "Home", href: "/" },
    {
      label: project.name,
      href: `/projects/${project.id}`,
    },
    ...(isInstructor
      ? [
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
          isInstructor={isInstructor}
          projectName={project.name}
          author={author.name}
        />
        <HStack
          gap={{ base: 1, sm: 3, md: 6, lg: 8 }}
          justify="end"
          align="end"
        >
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
      </Stack>
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
      color="blue.600"
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
  const buttonSize = useBreakpointValue({ base: "xs", md: "sm" });
  return (
    <Button
      onClick={() =>
        navigate(
          `/projects/${projectId}/project-groups/${projectGroupId}/activities`
        )
      }
      colorScheme="gray"
      leftIcon={<FaUsers />}
      size={buttonSize}
      variant="outline"
      py={{ base: 3, md: 4 }}
    >
      Project Group Activities
    </Button>
  );
};
