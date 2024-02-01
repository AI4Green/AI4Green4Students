import { useMemo } from "react";

export const SUBSTANCE_TYPE = {
  Reactant: "Reactant",
  Product: "Product",
  Reagent: "Reagent",
  Solvent: "Solvent",
};

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
        substanceType: SUBSTANCE_TYPE.Reactant,
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
        substanceType: SUBSTANCE_TYPE.Product,
        substancesUsed: product,
        molWeight: product_mol_weights[index],
        hazards: product_hazards[index],
      }),
      [products]
    )
  );
  return { productsData: productsData ?? [] };
};
