import base64
import logging
import os
from io import BytesIO
from typing import List

import requests
from PredictProducts.helpers.reaction_drawer import draw_labeled_reaction_image
from PredictProducts.models.dto import PredictedProduct, PredictedProductResponse
from rdkit import Chem

from .pubchem_api import PubchemAPI


class PredictionService:
    def __init__(self, first_n: int = 5):
        self.first_n = first_n
        self.pubchem_api = PubchemAPI()
        self.api_url = os.getenv("PRODUCT_PREDICTION_API_URL")

    def predict(self, smiles: str) -> List[PredictedProduct]:
        """
        Takes a SMILES string and returns the first N predictions (unsorted).
        """
        request_body = {"smiles": [smiles]}

        try:
            response = requests.post(
                self.api_url,
                json=request_body,
                headers={"Content-Type": "application/json"},
            )
            response.raise_for_status()

            raw_results = response.json()

            results: List[PredictedProductResponse] = [
                PredictedProductResponse.model_validate(result)
                for result in raw_results
            ]

        except Exception as e:
            logging.error(f"Error during prediction: {e}")
            return []

        if not results:
            return []

        first_result = results[0]
        products = first_result.products[: self.first_n]
        scores = [round(score, 2) for score in first_result.scores[: self.first_n]]

        reactants = self._parse_smiles(smiles)

        data: List[PredictedProduct] = []

        for product, score in zip(products, scores):
            mol = Chem.MolFromSmiles(product)
            name = self.pubchem_api.get_name_from_pubchem(product)
            iupac = name.iupac_name
            synonyms = name.synonyms
            label = iupac if iupac else product
            img = draw_labeled_reaction_image(reactants, mol, label)
            buffer = BytesIO()
            img.save(buffer, format="PNG")
            image_b64 = base64.b64encode(buffer.getvalue()).decode("utf-8")
            data.append(
                PredictedProduct(
                    product=product,
                    score=score,
                    reaction_image=image_b64,
                    iupac_name=iupac,
                    synonyms=synonyms,
                )
            )

        return data

    def _parse_smiles(self, smi_str: str):
        """Parses a SMILES string and returns a list of RDKit Mol objects"""
        return [Chem.MolFromSmiles(smi) for smi in smi_str.split(".") if smi]
