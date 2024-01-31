import { useMemo } from "react";

export const useInitialReactionTableData = (reactionData) => {
  const { reactantsData } = useInitialRecatantsData(reactionData);
  const { productsData } = useInitialProductsData(reactionData);

  const initialTableData = useMemo(
    () => [...reactantsData, ...productsData],
    [reactionData]
  );

  return { initialTableData: initialTableData ?? [] };
};

const useInitialRecatantsData = (data) => {
  const {
    reactants,
    reactant_mol_weights,
    reactant_densities,
    reactant_hazards,
  } = data;

  const reactantsData = useMemo(() =>
    reactants?.map(
      (reactant, index) => ({
        substancesUsed: reactant,
        molWeight: reactant_mol_weights[index],
        density: reactant_densities[index],
        hazards: reactant_hazards[index],
      }),
      [reactants]
    )
  );
  return { reactantsData: reactantsData ?? [] };
};

const useInitialProductsData = (data) => {
  const { products, product_mol_weights, product_hazards } = data;

  const productsData = useMemo(() =>
    products?.map(
      (product, index) => ({
        substancesUsed: product,
        molWeight: product_mol_weights[index],
        hazards: product_hazards[index],
      }),
      [products]
    )
  );
  return { productsData: productsData ?? [] };
};
