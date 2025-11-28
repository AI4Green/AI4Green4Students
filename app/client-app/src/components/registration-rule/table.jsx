import { HStack } from "@chakra-ui/react";
import { useRegistrationRulesList } from "api";
import { DataTable, DataTableGlobalFilter } from "components/core/data-table";
import { REGISTRATION_RULES_PERMISSIONS } from "constants";
import { useUser } from "contexts";
import { NewButton } from "layouts/default";
import { useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";

import { columns } from "./columns";
import { DeleteModal } from "./modal-delete";
import { CreateOrEditModal } from "./modal-form";

export const Table = () => {
  const { user } = useUser();
  const { tableData, list, mutate } = useTableData();
  const [searchValue, setSearchValue] = useState("");
  const [searchParams, setSearchParams] = useSearchParams();
  const action = searchParams.get("action");
  return (
    <>
      <DataTable data={tableData} columns={columns} globalFilter={searchValue}>
        <HStack flex={1} justifyContent="flex-start">
          <DataTableGlobalFilter
            searchValue={searchValue}
            setSearchValue={setSearchValue}
            placeholder="Search"
          />
          {user.permissions?.includes(
            REGISTRATION_RULES_PERMISSIONS.CreateRegistrationRules
          ) && <NewButton onClick={() => setSearchParams({ action: "new" })} />}
        </HStack>
      </DataTable>
      {action === "new" && <CreateOrEditModal list={list} mutate={mutate} />}
      {action === "delete" && <DeleteModal list={list} mutate={mutate} />}
      {action === "edit" && <CreateOrEditModal list={list} mutate={mutate} />}
    </>
  );
};

const useTableData = () => {
  const { data: list, mutate } = useRegistrationRulesList();
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
  return { list, mutate, tableData: tableData ?? [] };
};
