import { Button, HStack, Text } from "@chakra-ui/react";
import { useProjectTypesList } from "api/project-type";
import { DataTable, DataTableGlobalFilter } from "components/core/data-table";
import { columns } from "components/project-type/columns";
import { PROJECT_TYPE_MANAGEMENT_PERMISSIONS } from "constants";
import { useUser } from "contexts";
import { useMemo, useState } from "react";
import { FaPlus } from "react-icons/fa";
import { useSearchParams } from "react-router-dom";

export const ProjectTypeTable = () => {
  const { user } = useUser();
  const { data } = useTableData();
  const [searchValue, setSearchValue] = useState("");
  return (
    <DataTable data={data} columns={columns} globalFilter={searchValue}>
      <HStack flex={1} justifyContent="flex-start">
        <DataTableGlobalFilter
          searchValue={searchValue}
          setSearchValue={setSearchValue}
          placeholder="Search"
        />
        {user.permissions?.includes(
          PROJECT_TYPE_MANAGEMENT_PERMISSIONS.CreateProjectTypes
        ) && <New />}
      </HStack>
    </DataTable>
  );
};

const New = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const action = searchParams.get("action");
  return (
    <>
      <Button
        onClick={() => setSearchParams({ action: "new" })}
        colorScheme="green"
        leftIcon={<FaPlus />}
        size="sm"
      >
        <Text fontSize="sm" fontWeight="semibold">
          New project type
        </Text>
      </Button>
      {action === "new" && <Text> Render new project type modal</Text>}
    </>
  );
};

const useTableData = () => {
  const { data: projectTypes } = useProjectTypesList();

  const tableData = useMemo(
    () =>
      projectTypes?.map((projectType) => ({
        id: projectType.id,
        name: projectType.name,
        description: projectType.description,
        stage: projectType.stage,
        targetPath: `/project-types/${projectType.id}`,
      })),
    [projectTypes]
  );
  return { data: tableData ?? [] };
};
