import {
  Button,
  HStack,
  Heading,
  Icon,
  Text,
  VStack,
  useDisclosure,
} from "@chakra-ui/react";
import { useProjectsList } from "api/projects";
import { DataTable } from "components/dataTable/DataTable";
import { DataTableSearchBar } from "components/dataTable/DataTableSearchBar";
import { ExperimentLayout } from "components/experiment/ExperimentLayout";
import { CreateOrEditModal } from "components/experiment/modal/CreateOrEditModal";
import { experimentColumns } from "components/experiment/summary/table/experimentColumns";
import { useExperimentTableData } from "components/experiment/summary/table/useExperimentTableData";
import { useIsInstructor } from "components/experiment/useIsInstructor";
import { NotFound } from "pages/error/NotFound";
import { useState } from "react";
import {
  FaBook,
  FaChartBar,
  FaLayerGroup,
  FaTasks,
  FaUsers,
} from "react-icons/fa";
import { useNavigate } from "react-router-dom";

export const Summary = ({ projectId, projectSummary }) => {
  const { data: projects } = useProjectsList();
  const [searchValue, setSearchValue] = useState("");
  const project = projects.find((x) => x.id.toString() === projectId);
  const isInstructor = useIsInstructor();
  const { tableData } = useExperimentTableData(projectSummary, project);

  const { plans, literatureReviews, reports, projectGroup } = projectSummary;

  return (
    <WithValidProjectId projectId={projectId} projects={projects}>
      <ExperimentLayout>
        <HStack my={2} w="100%" justifyContent="space-between">
          <ExperimentHeading project={project} />
          <ProjectGroupActivities
            projectGroupId={projectGroup.id}
            project={project}
          />
        </HStack>
        <DataTable
          data={tableData}
          globalFilter={searchValue}
          columns={experimentColumns(isInstructor)}
        >
          <HStack flex={1} justifyContent="flex-start">
            <DataTableSearchBar
              searchValue={searchValue}
              setSearchValue={setSearchValue}
              placeholder="Search"
            />
            {!isInstructor && <NewPlan project={project} plans={plans} />}
            {!isInstructor && literatureReviews.length === 0 && (
              <NewLiteratureReview project={project} />
            )}
            {!isInstructor && reports.length === 0 && (
              <NewReport project={project} />
            )}
          </HStack>
        </DataTable>
      </ExperimentLayout>
    </WithValidProjectId>
  );
};

const WithValidProjectId = ({ projectId, projects, children }) => {
  const isValidProject = projects.some(
    (project) => project.id.toString() === projectId
  );

  return isValidProject ? children : <NotFound />;
};

const ExperimentHeading = ({ project }) => (
  <VStack align="start">
    <Heading as="h2" size="md" fontWeight="semibold" color="blue.600">
      <Icon as={FaLayerGroup} /> {project?.name}
    </Heading>
  </VStack>
);

const NewPlan = ({ project, plans }) => (
  <NewItemButton
    project={project}
    plans={plans}
    buttonText={plans?.length === 0 ? "Start planning" : "New plan"}
    leftIcon={<FaTasks />}
    modalProp={{ isPlan: true }}
  />
);

const NewLiteratureReview = ({ project }) => (
  <NewItemButton
    project={project}
    buttonText="Create Literature review"
    leftIcon={<FaBook />}
    modalProp={{ isLiteratureReview: true }}
  />
);

const NewReport = ({ project }) => (
  <NewItemButton
    project={project}
    buttonText="Create Report"
    leftIcon={<FaChartBar />}
    modalProp={{ isReport: true }}
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

const ProjectGroupActivities = ({ projectGroupId, project }) => {
  const navigate = useNavigate();
  return (
    <Button
      onClick={() =>
        navigate(
          `/projects/${project.id}/project-groups/${projectGroupId}/activities`
        )
      }
      colorScheme="green"
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
