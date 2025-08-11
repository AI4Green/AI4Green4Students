import { HStack } from "@chakra-ui/react";
import { useUserList } from "api";
import { AdminHeading } from "components/admin/admin-ui";
import { NewUser } from "components/admin/user-management/new-user";
import { columns, useTableData } from "components/admin/user-management/table";
import { DataTable, DataTableGlobalFilter } from "components/core/data-table";
import { DefaultContentLayout } from "layouts/default";
import { useState } from "react";
import { FaUserCog } from "react-icons/fa";

export const UserManagement = () => {
  const { data: users } = useUserList();
  const [searchValue, setSearchValue] = useState("");
  const { tableData } = useTableData(users);

  return (
    <DefaultContentLayout>
      <AdminHeading title="User Management" icon={FaUserCog} />
      <DataTable data={tableData} columns={columns} globalFilter={searchValue}>
        <HStack flex={1} justify="flex-start">
          <DataTableGlobalFilter
            searchValue={searchValue}
            setSearchValue={setSearchValue}
            placeholder="Search"
          />
          <NewUser />
        </HStack>
      </DataTable>
    </DefaultContentLayout>
  );
};
