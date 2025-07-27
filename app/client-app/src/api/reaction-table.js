import { useBackendApi } from "contexts";
import useSWR from "swr";

export const fetchKeys = {
  reactiontTable: (reactants, products, smiles) =>
    `reactionTable/data?reactants=${reactants}&products=${products}&smiles=${smiles}`,
  solvents: "reactionTable/solvents",
  compounds: (query) => `reactionTable/compounds?query=${query}`,
  reagent: (reagent) => `reactionTable/reagent?name=${reagent}`,
  solvent: (solvent) => `reactionTable/solvent?name=${solvent}`,
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
      fetchKeys.reactiontTable(reactants, products, reactionSmiles)
    ),

  /**
   *
   * @param {*} query - Query for getting compounds.(starts with query)
   * @returns - Compounds list starting with query
   */
  getCompounds: async (query) => await apiFetcher(fetchKeys.compounds(query)),

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
    fetchKeys.solvents,
    async (url) => {
      const data = await apiFetcher(url);
      return data;
    },
    { suspense: true }
  );
};
