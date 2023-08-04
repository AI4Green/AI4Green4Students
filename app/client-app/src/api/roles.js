import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  rolesList: "roles",
  role: (roleId) => `roles/${roleId}`,
};

export const useRolesList = () => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    fetchKeys.rolesList,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

/**
 * Get an role based on roleId
 */
export const useRole = (roleId) => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    roleId ? fetchKeys.role(roleId) : null,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
