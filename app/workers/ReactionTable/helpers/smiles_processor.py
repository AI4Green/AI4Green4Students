import re
from typing import List, Tuple
from .ions import (
    reactants_and_products_from_ionic_cx_smiles,
    reactants_and_products_from_ionic_smiles,
)


def list_reactants_and_products(
    reactants: str, products: str, reaction_smiles: str
) -> Tuple[List[str], List[str]]:
    """
    Process reactants and products strings to obtain lists of reactant and product SMILES.
    Converts words into SMILES symbols and identifies the format as either CXSMILES or SMILES.
    If ions are present, it processes the ionic SMILES accordingly.

    Args:
        reactants (str): A string representing the reactants in the reaction.
        products (str): A string representing the products in the reaction.
        reaction_smiles (str): The full reaction SMILES from the sketcher.

    Returns:
        Tuple[List[str], List[str]]: A tuple containing lists of reactant and product SMILES.
    """
    reactants_smiles = smiles_symbols(reactants)
    products_smiles = smiles_symbols(products)

    # Form the reaction SMILES from reactants and products
    full_reaction_smiles = (reactants_smiles + ">>" + products_smiles).replace(",", ".")

    # Get SMILES from sketcher to check format
    sketcher_smiles = smiles_symbols(reaction_smiles)

    # Process based on format
    if "|" in sketcher_smiles:
        # CXSMILES format
        cx_fragment = re.search(r" \|[^\|]*\|$", sketcher_smiles)
        if cx_fragment:
            full_reaction_smiles += cx_fragment.group()
        reactants_smiles_list, products_smiles_list = (
            reactants_and_products_from_ionic_cx_smiles(full_reaction_smiles)
        )
    elif "+" in full_reaction_smiles or "-" in full_reaction_smiles:
        # Ionic SMILES format
        reactants_smiles_list, products_smiles_list = (
            reactants_and_products_from_ionic_smiles(full_reaction_smiles)
        )
    else:
        # Standard SMILES format
        reactants_smiles_list, products_smiles_list = reactants_smiles.split(
            ","
        ), products_smiles.split(",")

    return reactants_smiles_list, products_smiles_list


def smiles_symbols(compound: str) -> str:
    """
    Restore chemical symbols from MarvinJS format.

    Args:
        compound (str): Compound string with encoded symbols

    Returns:
        str: Compound string with restored symbols
    """
    return compound.replace("minus", "-").replace("plus", "+").replace("sharp", "#")
