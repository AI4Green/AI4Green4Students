export const fetchKeys = {
  predictProducts: (smiles) => `prediction/products?smiles=${smiles}`,
};

export const getPredictionsApi = ({ apiFetcher }) => ({
  /**
   *
   * @param {*} smiles - SMILES
   * @returns - Predicted products
   */
  predictProducts: async (smiles) =>
    await apiFetcher(fetchKeys.predictProducts(smiles)),
});
