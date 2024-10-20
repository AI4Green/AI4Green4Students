import { HStack } from "@chakra-ui/react";
import { useState } from "react";
import { DataTable, DataTableGlobalFilter } from "components/core/data-table";
import { AdminHeading } from "components/admin/AdminUI";
import { FaUserCog } from "react-icons/fa";
import { useUserList } from "api/user";
import { useUserManagementTableData } from "components/admin/userManagement/userManagementTable/userUserManagementTableData";
import { userManagementColumns } from "components/admin/userManagement/userManagementTable/userManagementColumns";
import { NewUser } from "components/admin/userManagement/NewUser";
import { DefaultContentLayout } from "layouts/DefaultLayout";

export const UserManagement = () => {
  const { data: users } = useUserList();
  const [searchValue, setSearchValue] = useState("");
  const { tableData } = useUserManagementTableData(users);

  return (
    <DefaultContentLayout>
      <AdminHeading title="User Management" icon={FaUserCog} />
      <DataTable
        data={tableData}
        columns={userManagementColumns}
        globalFilter={searchValue}
      >
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
