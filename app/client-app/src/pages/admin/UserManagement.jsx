import { HStack } from "@chakra-ui/react";
import { useState } from "react";
import { DataTable, DataTableGlobalFilter } from "components/core/data-table";
import { AdminHeading } from "components/admin/AdminUI";
import { FaUserCog } from "react-icons/fa";
import { useUserList } from "api";
import { useTableData, columns } from "components/admin/user-management/table";
import { NewUser } from "components/admin/user-management/NewUser";
import { DefaultContentLayout } from "layouts/DefaultLayout";

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
