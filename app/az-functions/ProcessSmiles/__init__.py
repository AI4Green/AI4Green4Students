import logging
import azure.functions as func

import json
import re
from typing import Dict, List, Tuple

from rdkit import Chem

from . import models
from .helpers.compound import get_compound_from_inchi
from .helpers.auxiliary import smiles_symbols
from .helpers.ions import (
    reactants_and_products_from_ionic_cx_smiles,
    reactants_and_products_from_ionic_smiles,
)


# Processess data from the Ketcher or MarvinJS editor and generates data for the reaction table.
def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info("Python HTTP trigger function processed a request.")

    # get the SMILES string of reactants and products from the args and then replace the symbols
    reactants0 = req.params.get("reactants")
    products0 = req.params.get("products")
    reactionSmiles = req.params.get("reactionSmiles")

    if not reactants0 or not products0 or not reactionSmiles:
        logging.error("Missing required parameter")
        return error_response("Missing required parameter")

    reactants_smiles_list, products_smiles_list = get_reactants_and_products_list(
        reactants0, products0, reactionSmiles
    )
    logging.info(f"Reactants SMILES: {reactants_smiles_list}")
    logging.info(f"Products SMILES: {products_smiles_list}")

    reactant_data = {
        "molecular_weight_list": [],
        "name_list": [],
        "hazard_list": [],
        "density_list": [],
        "primary_key_list": [],
    }
    product_data = {
        "molecular_weight_list": [],
        "name_list": [],
        "hazard_list": [],
        "density_list": [],
        "primary_key_list": [],
        "table_numbers": [],
    }

    # Find reactants in database then add data to the dictionary
    for idx, reactant_smiles in enumerate(reactants_smiles_list):
        novel_compound = False  # false but change later if true
        mol = Chem.MolFromSmiles(reactant_smiles)
        if mol is None:
            logging.error(f"Cannot process Reactant {idx + 1} structure")
            return error_response(f"Cannot process Reactant {idx + 1} structure")

        inchi = Chem.MolToInchi(mol)
        reactant = get_compound_from_inchi(inchi)

        if reactant is None:
            # currently, not searching for novel compounds in the database
            logging.error(f"Reactant {idx + 1} not found in database")
            return error_response(f"Reactant {idx + 1} not found in database")

        # now we have the compound/novel_compound object, we can get all the data and add to reactant_data dict
        get_compound_data(reactant_data, reactant, novel_compound)
    number_of_reactants = len(reactant_data["name_list"])
    logging.info(f"Number of reactants: {number_of_reactants}")

    # Find products in database then add data to the dictionary
    for idx, product_smiles in enumerate(products_smiles_list):
        novel_compound = False  # false but change later if true
        mol = Chem.MolFromSmiles(product_smiles)
        if mol is None:
            logging.error(f"Cannot process product {idx + 1} structure")
            return error_response(f"Cannot process product {idx + 1} structure")
        inchi = Chem.MolToInchi(mol)
        product = get_compound_from_inchi(inchi)

        if product is None:
            # currently, not searching for novel compounds in the database
            logging.error(f"Product {idx + 1} not found in database")
            return error_response(f"Product {idx + 1} not found in database")

        # now we have the compound/novel_compound object, we can get all the data and add to product_data dict
        get_compound_data(product_data, product, novel_compound)
        product_data["table_numbers"].append(number_of_reactants + idx)
    number_of_products = len(product_data["name_list"])
    logging.info(f"Number of products: {number_of_products}")

    # Reagents - There are too many for a unselected dropdown. Could do recently used within workbook
    # identifiers = reagent_name + reagent_cas
    identifiers = []

    # Now it renders the reaction table template
    reaction_table_data = {
        "reactants": reactant_data["name_list"],
        "reactant_mol_weights": reactant_data["molecular_weight_list"],
        "reactant_densities": reactant_data["density_list"],
        "reactant_hazards": reactant_data["hazard_list"],
        "reactant_primary_keys": reactant_data["primary_key_list"],
        "number_of_reactants": number_of_reactants,
        "number_of_products": number_of_products,
        "identifiers": identifiers,
        "reactant_table_numbers": [],
        "products": product_data["name_list"],
        "product_mol_weights": product_data["molecular_weight_list"],
        "product_densities": product_data["density_list"],
        "product_hazards": product_data["hazard_list"],
        "product_primary_keys": product_data["primary_key_list"],
        "product_table_numbers": product_data["table_numbers"],
        "reagent_table_numbers": [],
        "reaction_table_data": "",
        "summary_table_data": "",
    }

    return func.HttpResponse(
        body=json.dumps({"data": reaction_table_data}),
        mimetype="application/json",
        status_code=200,
    )


def get_reactants_and_products_list(
    reactants: str, products: str, reactionSmiles: str
) -> Tuple[List[str], List[str]]:
    """
    Process reactants and products strings to obtain lists of reactant and product SMILES.
    Converts words into SMILES symbols and identifies the format as either CXSMILES or SMILES.
    If ions are present, it processes the ionic SMILES accordingly.

    Args:
        reactants (str): A string representing the reactants in the reaction.
        products (str): A string representing the products in the reaction.

    Returns:
        Tuple[List[str], List[str]]: A tuple containing lists of reactant and product SMILES.

    """
    reactants_smiles = smiles_symbols(reactants)
    products_smiles = smiles_symbols(products)

    # form the reaction_smiles. Just from reactands and products to exclude reagents/other data
    reaction_smiles = (reactants_smiles + ">>" + products_smiles).replace(",", ".")

    # we get the smiles straight from the sketcher to see if we have CXSmiles
    sketcher_smiles = smiles_symbols(reactionSmiles)

    # [OH-].[Na+]>>[Cl-].[Cl-].[Zn++] |f:0.1,2.3.4|
    if "|" in sketcher_smiles:
        reaction_smiles += re.search(r" \|[^\|]*\|$", sketcher_smiles).group()
        (
            reactants_smiles_list,
            products_smiles_list,
        ) = reactants_and_products_from_ionic_cx_smiles(reaction_smiles)
    elif "+" in reaction_smiles or "-" in reaction_smiles:
        (
            reactants_smiles_list,
            products_smiles_list,
        ) = reactants_and_products_from_ionic_smiles(reaction_smiles)
        # reactions with no ions - make rxn object directly from string
    else:
        reactants_smiles_list, products_smiles_list = reactants_smiles.split(
            ","
        ), products_smiles.split(",")
    return reactants_smiles_list, products_smiles_list


def get_compound_data(
    compound_data: Dict,
    compound: models.Compound,
    novel_compound: bool,
):
    """
    Update compound data dictionary with information from the given compound object.

    Args:
        compound_data (Dict): A dictionary containing lists to store compound data.
        compound (Union[models.Compound, models.NovelCompound]): The compound or novel compound object.
        novel_compound (bool): A boolean flag indicating whether the compound is a novel compound.
    """

    # now we have the compound/novel_compound object, we can get all the data
    molecular_weight = (
        float(compound.molec_weight) if compound.molec_weight != "" else 0
    )
    compound_data["molecular_weight_list"].append(molecular_weight)

    compound_name = compound.name if compound.name != "" else "Not found"
    compound_data["name_list"].append(compound_name)

    compound_hazard = (
        compound.hphrase if compound.hphrase != "No hazard codes found" else "Unknown"
    )
    compound_data["hazard_list"].append(compound_hazard)

    compound_density = compound.density if compound.density != "" else "-"
    compound_data["density_list"].append(compound_density)

    compound_data["primary_key_list"].append(compound.id)


def error_response(error: str) -> func.HttpResponse:
    return func.HttpResponse(
        body=json.dumps({"error": error}),
        mimetype="application/json",
        status_code=400,
    )
