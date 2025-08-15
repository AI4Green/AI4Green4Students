export const getPredictionsApi = ({ api }) => ({
  /**
   *
   * @param {*} smiles - SMILES
   * @returns - Forward prediction results
   */
  forwardPrediction: (smiles) =>
    api.post("prediction/forward", { json: smiles }),
});
