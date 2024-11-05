import { useBackendApi } from "contexts";
import useSWR from "swr";

export const fetchKeys = {
  registrationRulesList: "registrationRules/",
  minimalRegistrationRulesList: "registrationRules/minimal",
  registrationRule: (registrationRuleId) =>
    `registrationRule/${registrationRuleId}`,
};

export const getRegistrationRulesApi = ({ api }) => ({
  /**
   * Create new registration rule
   * @param {*} body
   */

  create: ({ values }) =>
    api.post("registrationRules/", {
      json: values,
    }),

  edit: ({ values, id }) =>
    api.put(`registrationRules/${id}`, {
      json: values,
    }),

  delete: ({ id }) => api.delete(`registrationRules/${id}`),

  /**
   * Validate the email against registration rules
   * @param {*} email
   * @returns
   */
  validate: (email) =>
    api.post(`registrationRules/validate`, {
      json: email,
    }),
});

export const useRegistrationRulesList = () => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    fetchKeys.registrationRulesList,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
