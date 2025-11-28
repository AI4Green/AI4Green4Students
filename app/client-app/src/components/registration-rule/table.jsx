import { HStack } from "@chakra-ui/react";
import { useRegistrationRulesList } from "api";
import { DataTable, DataTableGlobalFilter } from "components/core/data-table";
import { REGISTRATION_RULES_PERMISSIONS } from "constants";
import { useUser } from "contexts";
import { NewButton } from "layouts/default";
import { useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";

import { columns } from "./columns";
import { CreateOrEditModal } from "./modal-form";

export const Table = () => {
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
          REGISTRATION_RULES_PERMISSIONS.CreateRegistrationRules
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
      <NewButton onClick={() => setSearchParams({ action: "new" })} />
      {action === "new" && <CreateOrEditModal />}
    </>
  );
};

export const useTableData = () => {
  const { data: list } = useRegistrationRulesList();
  const tableData = useMemo(
    () =>
      list?.map((x) => ({
        id: x.id,
        value: x.value,
        type: x.isBlocked ? "Blocked" : "Allowed",
        isBlocked: x.isBlocked,
        modified: new Date(x.modified).toLocaleString(),
      })),
    [list]
  );
  return { data: tableData ?? [] };
};
