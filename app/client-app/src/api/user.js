import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

const fetchKeys = {
  me: "user/me",
  userList: "users",
  user: (id) => `users/${id}`,
};

/**
 * Get an authenticated user's profile
 * @returns
 */
export const useProfile = () => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    fetchKeys.me,
    async (url) => {
      try {
        return await apiFetcher(url);
      } catch (e) {
        if (e?.response?.status === 401) return null;
        throw e;
      }
    },
    { suspense: true, refreshInterval: 600000 } // re-check profile every 10mins in case of cookie expiry
  );
};

export const getUserApi = ({ api }) => ({
  /**
   * Save the User's UI Culture
   * @param {*} culture a culture/locale/language code
   * @returns
   */
  setUICulture: (culture) =>
    api.put("user/uiCulture", {
      json: culture,
    }),

  // Update user roles
  setUserRoles: ({ id, values }) =>
    api.put(`user/userRoles/${id}`, {
      json: values,
    }),

  // Update user email
  setUserEmail: ({ id, values }) =>
    api.put(`user/userEmail/${id}`, {
      json: values,
    }),

  // Delete user and email delete update if true
  delete: (user) =>
    api.delete(`users/${user.id}`, {
      json: user,
    }),
});

/**
 * Get a list of users
 * @returns
 */
export const useUserList = () => {
  const { apiFetcher } = useBackendApi();
  return useSWR(
    fetchKeys.userList,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};

/**
 * Get a user based on their user id
 * @returns
 */
export const useUserSelected = (id) => {
  const { apiFetcher } = useBackendApi();
  return useSWR(
    fetchKeys.user(id),
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
