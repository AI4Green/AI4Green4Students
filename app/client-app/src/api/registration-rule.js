import { useBackendApi } from "contexts";
import useSWR from "swr";

export const fetchKeys = {
  list: "registrationRules/",
  get: (id) => `registrationRules/${id}`,
};

export const getRegistrationRulesApi = ({ api }) => ({
  create: (values) =>
    api.post("registrationRules/", {
      json: values,
    }),

  edit: (id, values) =>
    api.put(`registrationRules/${id}`, {
      json: values,
    }),

  delete: (id) => api.delete(`registrationRules/${id}`),

  validate: (email) =>
    api.post(`registrationRules/validate`, {
      json: email,
    }),
});

export const useRegistrationRulesList = () => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    fetchKeys.list,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
