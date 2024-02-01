export const getAi4GreenApi = ({ api }) => ({
  /**
   *
   * @param {*} reactants - Reactants SMILES
   * @param {*} products - Products SMILES
   * @param {*} reactionSmiles - Reaction SMILES (no any diff. from sketcher SMILES)
   * @returns - Reaction data
   */
  process: (reactants, products, reactionSmiles) =>
    api.get(
      `ai4green/_process?reactants=${reactants}&products=${products}&reactionSmiles=${reactionSmiles}`
    ),

  getPartialReagents: (reagent) =>
    api.get(`ai4green/partialReagents?queryName=${reagent}`),

  getReagent: (reagent) => api.get(`ai4green/reagent?reagentName=${reagent}`),
});
