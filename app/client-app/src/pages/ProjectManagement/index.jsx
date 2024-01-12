import {
  Stack,
  HStack,
  Heading,
  VStack,
  Text,
  Input,
  InputGroup,
  InputLeftElement,
  Button,
  useDisclosure,
} from "@chakra-ui/react";
import {
  FaLayerGroup,
  FaPlus,
  FaProjectDiagram,
  FaSearch,
  FaUserFriends,
} from "react-icons/fa";
import { useState, useEffect, useMemo } from "react";
import { useProjectsList } from "api/projects";
import { useProjectGroupsList } from "api/projectGroups";
import { DataTable } from "components/dataTable/DataTable";
import { CreateOrEditProjectModal as NewProjectModal } from "components/projectManagement/modal/CreateOrEditProjectModal";
import { CreateOrEditProjectGroupModal as NewProjectGroupModal } from "components/projectManagement/modal/CreateOrEditProjectGroupModal";
import { StudentInviteModal } from "components/projectManagement/modal/StudentInviteModal";
import { projectColumns } from "components/projectManagement/projectColumns";
import { projectGroupColumns } from "components/projectManagement/projectGroupColumns";

const ProjectManagementHeader = ({
  activeOption,
  searchValue,
  setSearchValue,
  NewProjectState,
  NewProjectGroupState,
}) => {
  const InviteStudentsState = useDisclosure();
  return (
    <HStack my={2} w="100%" justifyContent="space-between">
      <HStack spacing={4}>
        <Heading as="h2" size="xs" fontWeight="semibold">
          {activeOption.label}
        </Heading>
        <Text fontSize="xs" color="gray" fontWeight="semibold">
          ({activeOption.data?.length})
        </Text>
      </HStack>

      <HStack flex={1} justifyContent="flex-end">
        <HStack>
          <InputGroup>
            <InputLeftElement pointerEvents="none" height="100%">
              <FaSearch color="gray" />
            </InputLeftElement>
            <Input
              variant="outline"
              borderColor="gray.400"
              size="sm"
              borderRadius={6}
              placeholder={`Search ${activeOption.label}`}
              _placeholder={{ opacity: 1 }}
              onChange={(e) => setSearchValue(e.target.value)}
              value={searchValue || ""}
            />
          </InputGroup>
        </HStack>
        <Button
          onClick={activeOption.state.onOpen}
          colorScheme="green"
          leftIcon={<FaPlus />}
          size="sm"
        >
          <Text fontSize="sm" fontWeight="semibold">
            {`New ${activeOption.label.slice(0, -1)}`}
          </Text>
        </Button>
        {NewProjectState.isOpen && (
          <NewProjectModal
            isModalOpen={NewProjectState.isOpen}
            onModalClose={NewProjectState.onClose}
          />
        )}
        {NewProjectGroupState.isOpen && (
          <NewProjectGroupModal
            isModalOpen={NewProjectGroupState.isOpen}
            onModalClose={NewProjectGroupState.onClose}
          />
        )}
        <Button
          onClick={InviteStudentsState.onOpen}
          colorScheme="blue"
          leftIcon={<FaUserFriends />}
          size="sm"
        >
          <Text fontSize="sm" fontWeight="semibold">
            Invite Students
          </Text>
        </Button>
        {InviteStudentsState.isOpen && (
          <StudentInviteModal
            isModalOpen={InviteStudentsState.isOpen}
            onModalClose={InviteStudentsState.onClose}
          />
        )}
      </HStack>
    </HStack>
  );
};

export const ProjectManagement = () => {
  const { data: projectGroups } = useProjectGroupsList();
  const { data: projects } = useProjectsList();
  const [searchValue, setSearchValue] = useState("");

  const projectData = useMemo(
    () =>
      projects?.map((project) => {
        const relatedGroups = projectGroups?.filter(
          (relatedGroup) => relatedGroup.projectId === project.id
        ); // get related project groups

        return {
          id: project.id,
          name: project.name,
          projectGroupNumber: relatedGroups.length ?? 0,

          subRows:
            relatedGroups?.map((group) => ({
              id: group.id,
              name: group.name,
              studentNumber: group.students.length,
            })) || [], // return empty if no related groups
        };
      }),
    [projects, projectGroups]
  );

  const projectGroupData = useMemo(
    () =>
      projectGroups?.map((group) => {
        const relatedProject = projects.find(
          (project) => project.id === group.projectId
        ); // get related project

        return {
          id: group.id,
          name: group.name,
          projectId: relatedProject?.id ?? "",
          projectName: relatedProject?.name ?? "",
          studentNumber: group.students.length,

          subRows: group.students.map((student) => ({
            studentId: student.id,
            name: student.name,
            studentEmail: student.email,
          })),
        };
      }),
    [projectGroups, projects]
  );

  const NewProjectState = useDisclosure();
  const NewProjectGroupState = useDisclosure();

  const options = [
    {
      label: "Projects",
      icon: <FaLayerGroup />,
      value: "projects",
      columns: projectColumns,
      data: projectData,
      state: NewProjectState,
    },
    {
      label: "Project Groups",
      icon: <FaProjectDiagram />,
      value: "projectGroups",
      columns: projectGroupColumns,
      data: projectGroupData,
      state: NewProjectGroupState,
    },
  ];

  const [activeOption, setActiveOption] = useState(options[0]);

  useEffect(() => {
    // Update the activeOption state with the updated option
    setActiveOption((prevActiveOption) => {
      const updatedOption = options.find(
        (option) => option.label === prevActiveOption.label
      );
      return updatedOption || prevActiveOption;
    });
  }, [projects, projectGroups]);

  const ToggleButtons = () => (
    <HStack spacing={4}>
      {options.map((option) => (
        <Button
          key={option.value}
          variant={activeOption.label === option.label ? "solid" : "outline"}
          leftIcon={option.icon}
          onClick={() => setActiveOption(option)}
        >
          <Text fontSize="sm" fontWeight="semibold">
            {option.label}
          </Text>
        </Button>
      ))}
    </HStack>
  );

  return (
    <Stack align="stretch" w="100%" alignItems="center">
      <VStack
        m={4}
        p={4}
        align="stretch"
        minW={{ base: "full", md: "95%", lg: "80%", xl: "70%" }}
        spacing={4}
      >
        <ToggleButtons />
        <VStack
          align="flex-start"
          borderWidth={1}
          px={5}
          py={2}
          borderRadius={7}
          spacing={4}
        >
          <ProjectManagementHeader
            searchValue={searchValue}
            setSearchValue={setSearchValue}
            activeOption={activeOption}
            NewProjectState={NewProjectState}
            NewProjectGroupState={NewProjectGroupState}
          />
          <DataTable
            data={activeOption.data}
            columns={activeOption.columns}
            globalFilter={searchValue}
          />
        </VStack>
      </VStack>
    </Stack>
  );
};
