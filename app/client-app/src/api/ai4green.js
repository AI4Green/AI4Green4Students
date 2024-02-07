import { useBackendApi } from "contexts/BackendApi";
import useSWR from "swr";

export const fetchKeys = {
  processSmiles: (reactants, products, reactionSmiles) =>
    `ai4green/_Process?reactants=${reactants}&products=${products}&reactionSmiles=${reactionSmiles}`,
  solventsList: "ai4green/ListSolvents",
  compoundsList: (queryName) => `ai4green/ListCompounds?queryName=${queryName}`,
  reagent: (reagent) => `ai4green/Reagent?reagentName=${reagent}`,
  solvent: (solvent) => `ai4green/Solvent?solventName=${solvent}`,
};

export const getAi4GreenApi = ({ apiFetcher }) => ({
  /**
   *
   * @param {*} reactants - Reactants SMILES
   * @param {*} products - Products SMILES
   * @param {*} reactionSmiles - Reaction SMILES (no any diff. from sketcher SMILES)
   * @returns - Reaction data
   */
  process: async (reactants, products, reactionSmiles) =>
    await apiFetcher(
      fetchKeys.processSmiles(reactants, products, reactionSmiles)
    ),

  /**
   *
   * @param {*} queryName - Query for getting compounds.(starts with queryName)
   * @returns - Compounds list starting with queryName
   */
  getCompounds: async (queryName) =>
    await apiFetcher(fetchKeys.compoundsList(queryName)),

  /**
   *
   * @param {*} reagent - Reagent name
   * @returns - Reagent data
   */
  getReagent: async (reagent) => await apiFetcher(fetchKeys.reagent(reagent)),

  /**
   *
   * @param {*} solvent - Solvent name
   * @returns - Solvent data
   */
  getSolvent: async (solvent) => await apiFetcher(fetchKeys.solvent(solvent)),
});

export const useSolventsList = () => {
  const { apiFetcher } = useBackendApi();

  return useSWR(
    fetchKeys.solventsList,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
