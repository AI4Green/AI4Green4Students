import { useMemo } from "react";

export const useUserManagementTableData = (appUsers) => {
  const tableData = useMemo(
    () =>
      appUsers?.map((appUser) => ({
        id: appUser.id,
        fullName: appUser.fullName,
        email: appUser.email,
        roles: appUser.roles,
        emailConfirmed: appUser.emailConfirmed,
      })),
    [appUsers]
  );

  return { tableData: tableData ?? [] };
};
